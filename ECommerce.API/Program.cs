using ECommerce.Infastructure;
using ECommerce.Infastructure.Filters;
using ECommerce.Infastructure.Services.Storage.Azure;
using ECommerce.Persistance;
using ECommerce.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using ECommerce.Application;
using ECommerce.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ECommerce.Infastructure.Services.Storage.Local;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using Serilog.Context;
using ECommerce.API.Configurations.ColumnWriters;
using Microsoft.AspNetCore.HttpLogging;
using ECommerce.API.Extensions;
using ECommerce.SignalR;
using ECommerce.SignalR.Hubs;
using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;
using ECommerce.Infastructure.Services.Storage;
using Amazon;
using ECommerce.Infastructure.Services.Storage.AWS;

namespace ECommerce.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            //Generic store Servis tanýmlama
            //builder.Services.AddStorage<LocalStorage>();
            //builder.Services.AddStorage<AzureStorage>();
            builder.Services.AddStorage<AWSStorage>();

            //Database
            builder.Services.AddDbContext<ECommerceAPIDbContext>(Options => Options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

            //AWS (configure aws s3 client)
            var awsSettings = builder.Configuration.GetSection("Storage:AWS");
            var credential = new BasicAWSCredentials(awsSettings["AWSAccessKey"], awsSettings["AWSSecretAccessKey"]);

            //AWS (configure aws options)
            var awsOptions = builder.Configuration.GetAWSOptions();
            awsOptions.Credentials = credential;
            awsOptions.Region = RegionEndpoint.EUCentral1;
            builder.Services.AddDefaultAWSOptions(awsOptions);

            //AWS S3 servisi
            builder.Services.AddAWSService<IAmazonS3>();

            //Identity
            builder.Services.AddIdentity<AppUser, AppRole>(options => 
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<ECommerceAPIDbContext>();

            //SerieLog
            Logger log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs/log.txt")
                .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "logs", needAutoCreateTable: true,
                columnOptions: new Dictionary<string, ColumnWriterBase> //Eðer custom property yazýlmak istenmiyorsa bunu yazmaya gerek yok
                {
                    {"message", new RenderedMessageColumnWriter()},
                    {"message_template", new MessageTemplateColumnWriter()},
                    {"level", new LevelColumnWriter()},
                    {"time_stamp", new TimestampColumnWriter()},
                    {"exception", new ExceptionColumnWriter()},
                    {"log_event", new LogEventSerializedColumnWriter()},
                    {"user_name", new UsernameColumnWriter() } //custom property
                })
                .WriteTo.Seq(builder.Configuration["Seq:SeqUrl"]) //loglara görsel arayüz kurup ordan bakmak için seq kullanabiliriz bunun için => PowerShell e docker run --name Seq -e ACCEPT_EULA=Y -p 5432:80 datalust/seq:latest
                .Enrich.FromLogContext() //harici property kullanýlmak istenildiðinde kullanýlýr.
                .MinimumLevel.Information()
                .CreateLogger();
            builder.Host.UseSerilog(log);

            builder.Services.AddHttpLogging(logging => //Yapýlan requestler de log mekanizmasý sayesinde yakalanmasýný istiyorum.
            {
                logging.LoggingFields = HttpLoggingFields.All;
                logging.RequestHeaders.Add("sec_ch_ua"); //Kullanýcaya dair bütün bilgileri getirecek key
                logging.MediaTypeOptions.AddText("application/javascript");
                logging.RequestBodyLogLimit = 4096; // Taþýnacak veri limitleri
                logging.ResponseBodyLogLimit = 4096;// Taþýnacak veri limitleri
            });

            //JWT Token
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Admin", options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateAudience = true, // OLuþturulacak token deðerinin hangi origin(sitelerin) kullanacaðýný belirtir
                    ValidateIssuer = true, //Oluþturulacak token deðerini kimin daðýttýðýný ifade edeceðimiz alandýr.
                    ValidateLifetime = true, //Oluþturulan token deðerinin süresini kontrol edecek doðrulama.
                    ValidateIssuerSigningKey = true, //Üretilecek token deðerinin uygulamamýza ait bir deðer olduðunu ifade eden security key verisinin doðrulamasýdýr.

                    ValidAudience = builder.Configuration["Token:Audience"],
                    ValidIssuer = builder.Configuration["Token:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
                    LifetimeValidator = (notBefore, expires, securityToken, validationParameters)=> expires != null ? expires > DateTime.UtcNow : false,
                    NameClaimType = ClaimTypes.Name // user.Identity.Name ile elde edilir.
                };
            });

            //Services
            builder.Services.AddHttpContextAccessor(); //Client dan gelen request neticesinde oluþturulan HttpContext nesnesine katmanlardaki classlar üzerinden eriþebilmemizi saðlayan servistir.
            builder.Services.AddPersistenceServices();
            builder.Services.AddInfrastructureServices();
            builder.Services.AddApplicationServices();
            builder.Services.AddSignalRServices();

            builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
            //policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
             policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials()
            ));

            //Validations
            builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>());
                //.AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<CreateProductValidation>()).ConFiureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.ConfigureExceptionHandler<Program>(app.Services.GetRequiredService<ILogger<Program>>());
            app.UseSerilogRequestLogging(); // Bunu yazdýktan öncekiler loglanmayacak, sonrakiler loglanacak
            app.UseHttpLogging();

            app.UseCors();

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.Use(async (context, next) =>
            {
                var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
                LogContext.PushProperty("user_name", username);
                await next();
            });

            app.MapControllers();

            app.Hubs();

            app.Run();
        }
    }
}