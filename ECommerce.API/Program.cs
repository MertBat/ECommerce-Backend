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

namespace ECommerce.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            //Generic Servis tanýmlama
            //builder.Services.AddStorage<LocalStorage>();
            builder.Services.AddStorage<AzureStorage>();

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"]))
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
            app.UseCors();

            app.UseStaticFiles();

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}