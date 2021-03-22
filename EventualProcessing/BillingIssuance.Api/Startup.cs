using BillingIssuance.Api.Application.Workers;
using BillingIssuance.Api.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PrivatePackage.Caching;
using PrivatePackage.DependencyInjection;
using PrivatePackage.Messaging;
using PrivatePackage.Middlewares;
using PrivatePackage.Swagger;
using System.IO.Compression;

namespace Charges.Presentation
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor()
                .BootstrapDomainServices()
                .BootstrapPersistenceServices(Configuration)
                .BootstrapValidators()
                .BootstrapMessagingServices(Configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>())
                .BootstrapCache(Configuration.GetSection("Redis").Get<RedisSettings>())
                .BootstrapPipelinesServices()
                .BootstrapSwaggerConfig(Configuration.GetSection("Swagger").Get<SwaggerSettings>())
                .AddHostedService<ConsumersBackgroundService>()
                .Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal)
                .AddResponseCompression(options => options.Providers.Add<GzipCompressionProvider>())
                .AddControllers()
                .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true)
                .SetCompatibilityVersion(CompatibilityVersion.Latest);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, SwaggerSettings swagger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else app.UseHsts();

            app.UseMiddleware(typeof(ExceptionMiddleware));

            app.UseSwagger(option => option.RouteTemplate = swagger.Template)
               .UseSwaggerUI(option => option.SwaggerEndpoint(swagger.Url, swagger.Title));

            app.UseHttpsRedirection()
               .UseRouting()
               .UseAuthentication()
               .UseAuthorization()
               .UseEndpoints(e => e.MapControllers());
        }
    }
}
