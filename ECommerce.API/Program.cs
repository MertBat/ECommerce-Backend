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

namespace ECommerce.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            //Generic Servis tan�mlama
            builder.Services.AddStorage<LocalStorage>();
            //builder.Services.AddStorage<AzureStorage>();

            builder.Services.AddDbContext<ECommerceAPIDbContext>(Options => Options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

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
                columnOptions: new Dictionary<string, ColumnWriterBase> //E�er custom property yaz�lmak istenmiyorsa bunu yazmaya gerek yok
                {
                    {"message", new RenderedMessageColumnWriter()},
                    {"message_template", new MessageTemplateColumnWriter()},
                    {"level", new LevelColumnWriter()},
                    {"time_stamp", new TimestampColumnWriter()},
                    {"exception", new ExceptionColumnWriter()},
                    {"log_event", new LogEventSerializedColumnWriter()},
                    {"user_name", new UsernameColumnWriter() } //custom property
                })
                .WriteTo.Seq(builder.Configuration["Seq:SeqUrl"]) //loglara g�rsel aray�z kurup ordan bakmak i�in seq kullanabiliriz bunun i�in => PowerShell e docker run --name Seq -e ACCEPT_EULA=Y -p 5432:80 datalust/seq:latest
                .Enrich.FromLogContext() //harici property kullan�lmak istenildi�inde kullan�l�r.
                .MinimumLevel.Information()
                .CreateLogger();
            builder.Host.UseSerilog(log);

            builder.Services.AddHttpLogging(logging => //Yap�lan requestler de log mekanizmas� sayesinde yakalanmas�n� istiyorum.
            {
                logging.LoggingFields = HttpLoggingFields.All;
                logging.RequestHeaders.Add("sec_ch_ua"); //Kullan�caya dair b�t�n bilgileri getirecek key
                logging.MediaTypeOptions.AddText("application/javascript");
                logging.RequestBodyLogLimit = 4096; // Ta��nacak veri limitleri
                logging.ResponseBodyLogLimit = 4096;// Ta��nacak veri limitleri
            });

            //JWT Token
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer("Admin", options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateAudience = true, // OLu�turulacak token de�erinin hangi origin(sitelerin) kullanaca��n� belirtir
                    ValidateIssuer = true, //Olu�turulacak token de�erini kimin da��tt���n� ifade edece�imiz aland�r.
                    ValidateLifetime = true, //Olu�turulan token de�erinin s�resini kontrol edecek do�rulama.
                    ValidateIssuerSigningKey = true, //�retilecek token de�erinin uygulamam�za ait bir de�er oldu�unu ifade eden security key verisinin do�rulamas�d�r.

                    ValidAudience = builder.Configuration["Token:Audience"],
                    ValidIssuer = builder.Configuration["Token:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
                    LifetimeValidator = (notBefore, expires, securityToken, validationParameters)=> expires != null ? expires > DateTime.UtcNow : false,
                    NameClaimType = ClaimTypes.Name // user.Identity.Name ile elde edilir.
                };
            });

            //Services
            builder.Services.AddPersistenceServices();
            builder.Services.AddInfrastructureServices();
            builder.Services.AddApplicationServices();

            builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
            //policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
             policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod()
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

            app.UseSerilogRequestLogging(); // Bunu yazd�ktan �ncekiler loglanmayacak, sonrakiler loglanacak
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

            app.Run();
        }
    }
}