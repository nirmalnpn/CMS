using CMS.DAL.Interface;
using CMS.DAL.Interface.Users;
using CMS.DAL.Repository;
using CMS.DAL.Repository.Users;
using CMS.Domain.DBConnection;
using CMS.Shared.Services.Extensions;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace CMS.Api.Controllers.Comman
{
    public static class ServiceExtensions
    {
        internal static IServiceCollection AddCustomService(this IServiceCollection services)
        {
            // Register Swagger documentation
            services.AddSwaggerDocumentation();


            // Register application services and repositories
            services.AddScoped<TokenHandler>();
            services.AddSingleton<DapperContext>();
            services.AddScoped<IDBConnect, DBConnection>();
            services.AddScoped<IDapperDao, DapperDaoRepo>();
            services.AddScoped<IGender, GenderRepo>();
            services.AddScoped<IUser, UserRepo>();
  

            services.AddScoped<EmailService>();
            services.AddScoped<IOtpRepository, OtpRepository>();


            return services;
        }

        internal static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // Basic Swagger document settings
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "CMS API",
                    Version = "v1",
                    Description = "CMS API",
                    TermsOfService = new Uri("https://google.com"),
                    Contact = new OpenApiContact
                    {
                        Name = "CMS API",
                        Email = "info@test.com"
                    }
                });

                // Add security definition for JWT Bearer
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                // Add security requirement for all endpoints
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

                // Include XML comments if the XML file exists
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
                else
                {
                    // Optionally log a warning or handle the missing file case
                    // For example, Log.Warning("XML comments file not found: {xmlPath}", xmlPath);
                }
            });

            return services;
        }
    }
}
