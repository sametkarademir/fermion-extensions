using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Fermion.Extensions.ServiceCollections;

/// <summary>
/// Provides extension methods for adding services to the service collection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core services required by the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <remarks>
    /// Configures JSON serialization, API behavior options, and adds essential services like HttpContextAccessor.
    /// </remarks>
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.WriteIndented = true;
                options.JsonSerializerOptions.ReferenceHandler =
                    System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

        services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();

        return services;
    }

    /// <summary>
    /// Adds Swagger documentation generation to the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <remarks>
    /// Configures Swagger based on settings from the "SwaggerSettings" section in appsettings.json.
    /// If Swagger is disabled in configuration, this method returns without adding Swagger services.
    /// 
    /// Required appsettings.json structure:
    /// <code>
    /// {
    ///   "SwaggerSettings": {
    ///     "Enabled": true,
    ///     "Title": "API Documentation",
    ///     "Description": "API Documentation for MyService",
    ///     "ContactName": "API Support",
    ///     "ContactEmail": "support@example.com",
    ///     "ContactUrl": "https://example.com/support",
    ///     "LicenseName": "MIT",
    ///     "LicenseUrl": "https://opensource.org/licenses/MIT"
    ///   }
    /// }
    /// </code>
    /// </remarks>
    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services, IConfiguration configuration)
    {
        var swaggerSettings = configuration.GetSection("SwaggerSettings");
        var enabled = swaggerSettings.GetValue<bool>("Enabled");

        if (!enabled)
        {
            return services;
        }

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = swaggerSettings.GetValue<string>("Title") ?? "API Documentation",
                Version = "v1",
                Description = swaggerSettings.GetValue<string>("Description") ?? "API Documentation",
                Contact = new OpenApiContact
                {
                    Name = swaggerSettings.GetValue<string>("ContactName"),
                    Email = swaggerSettings.GetValue<string>("ContactEmail"),
                    Url = new Uri(swaggerSettings.GetValue<string>("ContactUrl") ?? "https://example.com")
                },
                License = new OpenApiLicense
                {
                    Name = swaggerSettings.GetValue<string>("LicenseName") ?? "MIT",
                    Url = new Uri(swaggerSettings.GetValue<string>("LicenseUrl") ?? "https://opensource.org/licenses/MIT")
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

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
        });

        return services;
    }

    /// <summary>
    /// Adds Cross-Origin Resource Sharing (CORS) services to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    /// <remarks>
    /// Configures CORS based on settings from the "CorsSettings" section in appsettings.json.
    /// 
    /// Required appsettings.json structure:
    /// <code>
    /// {
    ///   "CorsSettings": {
    ///     "AllowedOrigins": ["https://example.com", "https://api.example.com"],
    ///     "CorsPolicyName": "ApiCorsPolicy",
    ///     "AllowCredentials": true
    ///   }
    /// }
    /// </code>
    /// 
    /// If AllowedOrigins contains "*", it will allow any origin.
    /// If AllowCredentials is true, credentials will be allowed.
    /// Always allows any method and any header.
    /// Exposes the "X-Pagination" and "X-Rate-Limit-Remaining" headers.
    /// </remarks>
    public static IServiceCollection AddCustomCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection("CorsSettings");
        var allowedOrigins = corsSettings.GetSection("AllowedOrigins").Get<string[]>() ?? new[] { "*" };

        services.AddCors(options =>
        {
            options.AddPolicy(corsSettings.GetValue<string>("CorsPolicyName") ?? "ApiCorsPolicy", builder =>
            {
                if (allowedOrigins.Contains("*"))
                {
                    builder.AllowAnyOrigin();
                }
                else
                {
                    builder.WithOrigins(allowedOrigins);
                }

                builder.AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("X-Pagination", "X-Rate-Limit-Remaining");

                if (corsSettings.GetValue<bool>("AllowCredentials"))
                {
                    builder.AllowCredentials();
                }
            });
        });

        return services;
    }
}