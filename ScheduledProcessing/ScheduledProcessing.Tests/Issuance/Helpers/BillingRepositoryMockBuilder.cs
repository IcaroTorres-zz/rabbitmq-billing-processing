using Issuance.Api.Application.Abstractions;
using Issuance.Api.Domain.Models;
using Library.Optimizations;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScheduledProcessing.Tests.Issuance.Helpers
{
    public sealed class BillingRepositoryMockBuilder
    {
        private readonly Mock<IBillingRepository> _mock;

        private BillingRepositoryMockBuilder()
        {
            _mock = new Mock<IBillingRepository>();
        }

        public static BillingRepositoryMockBuilder Create()
        {
            return new BillingRepositoryMockBuilder();
        }

        public BillingRepositoryMockBuilder GetMany(string cpfString, byte month, ushort year, List<Billing> result)
        {
            var cpf = cpfString.AsSpan().ParseUlong();
            _mock.Setup(x => x.GetManyAsync(cpf, month, year, default)).ReturnsAsync(result);
            return this;
        }

        public BillingRepositoryMockBuilder GetPending(int count)
        {
            var result = InternalFakes.Billings.Valid().Generate(count);
            _mock.Setup(x => x.GetPendingAsync(default)).ReturnsAsync(result);
            return this;
        }

        public BillingRepositoryMockBuilder Insert(Billing entity, Task result)
        {
            _mock.Setup(x => x.InsertAsync(entity, default)).Returns(result);
            return this;
        }

        public BillingRepositoryMockBuilder Insert(List<Billing> entities, Task result)
        {
            _mock.Setup(x => x.UpdateProcessedBatchAsync(entities, default)).Returns(result);
            return this;
        }

        public IBillingRepository Build()
        {
            return _mock.Object;
        }
    }
}
