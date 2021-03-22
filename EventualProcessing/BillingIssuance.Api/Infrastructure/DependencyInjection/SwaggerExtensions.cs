using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace BillingIssuance.Api.Infrastructure.DependencyInjection
{
    public static class SwaggerExtensions
    {
        private static readonly OpenApiInfo _swaggerOpenApiInfo = new OpenApiInfo { Title = "Billing Issuance", Version = "v1.0" };

        public static IServiceCollection BootstrapSwaggerConfig(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(_swaggerOpenApiInfo.Version, _swaggerOpenApiInfo);

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

                c.IncludeXmlComments(xmlPath);
            }).AddSwaggerGenNewtonsoftSupport();

            return services;
        }
    }
}
