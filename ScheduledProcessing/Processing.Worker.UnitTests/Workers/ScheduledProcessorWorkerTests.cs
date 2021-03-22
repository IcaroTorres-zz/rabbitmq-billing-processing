using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Processing.Worker.Domain.Models;
using Processing.Worker.Domain.Services;
using Processing.Worker.UnitTests.Helpers;
using Processing.Worker.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Processing.Worker.UnitTests.Workers
{
    [Trait("unit-test", "processing.worker")]
    public class ScheduledProcessorWorkerTests
    {
        private readonly Mock<IRpcClient<List<Customer>>> _customerClientMock;
        private readonly Mock<IRpcClient<List<Billing>>> _billingClientMock;
        private readonly Mock<IAmountProcessor> _amountProcessorMock;
        private readonly IComparer<ICpfCarrier> _comparer;
        private readonly Mock<ScheduledProcessorSettings> _settingsMock;
        private readonly Mock<ILogger<ScheduledProcessorWorker>> _loggerMock;
        private readonly ScheduledProcessorWorker _sut;

        public ScheduledProcessorWorkerTests()
        {
            _customerClientMock = new Mock<IRpcClient<List<Customer>>>();
            _billingClientMock = new Mock<IRpcClient<List<Billing>>>();
            _amountProcessorMock = new Mock<IAmountProcessor>();
            _comparer = new CpfCarrierComparer();
            _settingsMock = new Mock<ScheduledProcessorSettings>();
            _loggerMock = new Mock<ILogger<ScheduledProcessorWorker>>();
            _sut = new ScheduledProcessorWorker(
                _customerClientMock.Object,
                _billingClientMock.Object,
                _amountProcessorMock.Object,
                _comparer,
                _settingsMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public void ProcessBatch_Shuold_Return_BillingsProcessed_ForMatchingCustomers()
        {
            // arrange on constructor
            const int billingsCount = 100;
            const int processableCount = 20;
            var billings = InternalFakes.Billings.Valid().Generate(billingsCount);
            var billingArray = billings.ToArray();
            var processableCpfs = billingArray[..processableCount]
                .Select((x, index) =>
                {
                    var customer = (ICpfCarrier)new Customer { Cpf = x.Cpf };
                    _amountProcessorMock.Setup(y => y.Process(customer, billingArray[index]))
                        .Returns(new Billing
                        {
                            Id = billingArray[index].Id,
                            Cpf = x.Cpf,
                            Amount = 100,
                            ProcessedAt = DateTime.UtcNow
                        });
                    return customer;
                }).ToList();
            var unprocessableCpfs = billingArray.Except(processableCpfs).ToList();
            processableCpfs.Sort(_comparer);
            var batch = new ProcessBatch { Customers = processableCpfs, Billings = billings };

            // act
            var result = _sut.ProcessBatch(batch);

            // assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrEmpty();
            result.Billings.Should()
                .HaveCount(billingsCount).And
                .OnlyContain(x =>
                    (unprocessableCpfs.Any(a => a.Cpf == x.Cpf) && x.ProcessedAt == null) ||
                    (processableCpfs.Any(x => x.Cpf == x.Cpf) && x.ProcessedAt != null)).And
                .Match(items => items.Count(x => x.ProcessedAt == null) == billingsCount - processableCount).And
                .Match(items => items.Count(x => x.ProcessedAt != null) == processableCount);
        }

        [Fact]
        public void ProcessBatch_Without_Customers_Shuold_Returninstance_And_SkipProcess()
        {
            // arrange on constructor
            const int billingsCount = 2;
            var billings = InternalFakes.Billings.Valid().Generate(billingsCount);
            var batch = new ProcessBatch { Customers = new List<ICpfCarrier>(), Billings = billings };

            // act
            var result = _sut.ProcessBatch(batch);

            // assert
            result.Should().NotBeNull().And.Be(batch);
            result.Billings.Should().HaveCount(billingsCount).And.OnlyContain(x => x.ProcessedAt == null);
        }

        [Fact]
        public void ProcessBatch_Without_Billings_Shuold_ReturnInstance_And_SkipProcess()
        {
            // arrange on constructor
            var customers = InternalFakes.Customer.Valid().Generate(2);
            var batch = new ProcessBatch { Customers = new List<ICpfCarrier>(customers), Billings = new List<Billing>() };

            // act
            var result = _sut.ProcessBatch(batch);

            // assert
            result.Should().NotBeNull().And.Be(batch);
            result.Billings.Should().HaveCount(0);
            result.Customers.Should().HaveCount(2);
        }

        [Fact]
        public void ProcessBatch_Without_BillingsAndCustomers_Shuold_ReturnInstance_And_SkipProcess()
        {
            // arrange on constructor
            var batch = new ProcessBatch();

            // act
            var result = _sut.ProcessBatch(batch);

            // assert
            result.Should().NotBeNull().And.Be(batch);
            result.Billings.Should().HaveCount(0);
            result.Customers.Should().HaveCount(0);
        }
    }
}
