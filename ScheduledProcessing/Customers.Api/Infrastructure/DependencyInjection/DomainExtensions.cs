﻿using Customers.Api.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Customers.Api.Infrastructure.DependencyInjection
{
    public static class DomainExtensions
    {
        public static IServiceCollection BootstrapDomainServices(this IServiceCollection services)
        {
            return services.AddScoped<IModelFactory, ModelFactory>();
        }
    }
}