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


            //Generic Servis tanımlama
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
                    ValidateAudience = true, // OLuşturulacak token değerinin hangi origin(sitelerin) kullanacağını belirtir
                    ValidateIssuer = true, //Oluşturulacak token değerini kimin dağıttığını ifade edeceğimiz alandır.
                    ValidateLifetime = true, //Oluşturulan token değerinin süresini kontrol edecek doğrulama.
                    ValidateIssuerSigningKey = true, //Üretilecek token değerinin uygulamamıza ait bir değer olduğunu ifade eden security key verisinin doğrulamasıdır.

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