using Customers.Api.Domain.Models;
using Customers.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;

namespace ScheduledProcessing.Tests.Customers.Helpers
{
    public sealed class CustomersContextInMemoryMockBuilder
    {
        private readonly CustomersContext _context;
        private Action<CustomersContext> _itemsAction;

        private CustomersContextInMemoryMockBuilder()
        {
            var options = new DbContextOptionsBuilder<CustomersContext>()
                .UseInMemoryDatabase(databaseName: "CustomersContextInMemory")
                .Options;
            _context = new CustomersContext(options);
        }

        public static CustomersContextInMemoryMockBuilder Create()
        {
            return new CustomersContextInMemoryMockBuilder();
        }

        public CustomersContextInMemoryMockBuilder Customers(params Customer[] seed)
        {
            _itemsAction = (c =>
            {
                c.Customers.AddRange(seed);
                c.SaveChanges();
            });
            return this;
        }

        public CustomersContext Build()
        {
            _itemsAction(_context);
            return _context;
        }
    }
}
