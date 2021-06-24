using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RemittanceApp.Data.DataContext;
using RemittanceApp.Extensions;
using RemittanceApp.Middlewares;
using RemittanceApp.Services.Interfaces;
using RemittanceApp.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemittanceApp
{
    public class Startup
    {
        #region Properties
        public IConfiguration Configuration { get; }
        #endregion

        #region Constr
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        #endregion

        #region Configuration Methods
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddCors(
                options => options.AddPolicy("AllowCors",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .WithMethods("GET", "PUT", "POST", "DELETE", "PATCH")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                })
            );

            // EntityFramework DB Context
            services.AddDbContext<RemittanceDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Inject Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICardService, CardService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IForexService, ForexService>();
            services.AddScoped<IGeneralService, GeneralService>();
            services.AddScoped<IOTPService, OTPService>();
            services.AddScoped<IRemittanceService, RemittanceService>();
            services.AddScoped<IStandingInstructionService, StandingInstructionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IBeneficiaryService, BeneficiaryService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "RemittanceApp", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            // Jwt Authentication
            var secret_Key = Configuration.GetSection("AppSettings:JWT_Secret").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret_Key));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddCookie(options => {
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/login";
                })
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = key,
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Exception Handling Middleware
            app.ConfigureExceptionHandler(env);

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "RemittanceApp v1"));

            app.UseRouting();
            app.UseCors("AllowCors");
            // Middleware To Handle Request & Response
            //app.UseMiddleware<RequestResponseLoggingMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        #endregion
    }
}
