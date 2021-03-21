using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Processing.Worker.Domain.Models;
using Processing.Worker.Domain.Services;
using Processing.Worker.Workers;
using System;
using System.Collections.Generic;

namespace Processing.Worker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthEndpoints();
            var rabbitMQ = Configuration.GetSection("RabbitMQ").Get<RabbitMQSettings>();
            services.AddSingleton(Configuration.GetSection("ScheduledProcessor").Get<ScheduledProcessorSettings>());
            services.AddSingleton(new CustomerCpfComparer());
            services.AddSingleton<IAmountProcessor>(_ => new MathOnlyAmountProcessor());
            services.AddSingleton<IConnectionFactory, ConnectionFactory>(_ => new ConnectionFactory { Uri = new Uri(rabbitMQ.AmqpUrl) });
            services.AddSingleton<IConnection>(x => x.GetRequiredService<IConnectionFactory>().CreateConnection());
            services.AddSingleton<IRpcClient<List<Customer>>>(x =>
            {
                var connection = x.GetRequiredService<IConnection>();
                var channel = connection.CreateModel();
                return new RpcClient<List<Customer>>(channel, nameof(Customer));
            });
            services.AddSingleton<IRpcClient<List<Billing>>>(x =>
            {
                var connection = x.GetRequiredService<IConnection>();
                var channel = connection.CreateModel();
                return new RpcClient<List<Billing>>(channel, nameof(Billing));
            });
            services.AddHostedService<ScheduledProcessorWorker>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthAllEndpoints();
        }
    }
}
