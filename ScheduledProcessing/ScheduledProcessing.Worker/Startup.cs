using Library.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using ScheduledProcessing.Worker.Domain.Models;
using ScheduledProcessing.Worker.Domain.Services;
using ScheduledProcessing.Worker.Workers;
using System;
using System.Collections.Generic;

namespace ScheduledProcessing.Worker
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
            services.AddSingleton<IAmountProcessor>(_ => new MathOnlyAmountProcessor());
            services.AddSingleton<IConnectionFactory, ConnectionFactory>(_ => new ConnectionFactory { Uri = new Uri(rabbitMQ.AmqpUrl) });
            services.AddSingleton<IConnection>(x => x.GetRequiredService<IConnectionFactory>().CreateConnection());
            services.AddSingleton<IRpcClient<List<Customer>>>(x =>
            {
                var connection = x.GetRequiredService<IConnection>();
                var channel = connection.CreateModel();
                return new RpcClient<List<Customer>>(connection, channel, nameof(Customer));
            });
            services.AddSingleton<IRpcClient<List<Billing>>>(x =>
            {
                var connection = x.GetRequiredService<IConnection>();
                var channel = connection.CreateModel();
                return new RpcClient<List<Billing>>(connection, channel, nameof(Billing));
            });
            services.AddHostedService<ScheduledBillingProcessingClientWorker>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHealthAllEndpoints();
        }
    }
}
