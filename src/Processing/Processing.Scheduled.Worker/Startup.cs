using Library.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;
using Processing.Scheduled.Worker.Workers;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;

namespace Processing.Scheduled.Worker
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
            services.AddSingleton<IComparer<ICpfCarrier>>(_ => new CpfCarrierComparer());
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
