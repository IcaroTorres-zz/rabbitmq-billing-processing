using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PrivatePackage.Swagger;
using System;
using System.IO;
using System.Reflection;

namespace PrivatePackage.DependencyInjection
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection BootstrapSwaggerConfig(this IServiceCollection services, SwaggerSettings swagger)
        {
            services.AddSingleton(_ => swagger);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(swagger.Version, new OpenApiInfo
                {
                    Title = swagger.Title,
                    Description = swagger.Description,
                    Version = swagger.Version
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            }).AddSwaggerGenNewtonsoftSupport();

            return services;
        }
    }
}
