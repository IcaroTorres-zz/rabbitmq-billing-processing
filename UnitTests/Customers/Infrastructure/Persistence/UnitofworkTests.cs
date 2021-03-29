using Customers.Application.Abstractions;
using Customers.Infrastructure.Persistence;
using FluentAssertions;
using Library.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using UnitTests.Customers.Helpers;
using Xunit;

namespace UnitTests.Customers.Infrastructure.Persistence
{
    [Trait("customers", "infrastructure")]
    public class UnitofworkTests : IDisposable
    {
        private IUnitofwork _sut;

        [Fact]
        public void Constructor_Should_Create_Unitofwork_Without_TransactionOpen()
        {
            // arrange
            var customersContext = CustomersContextMockBuilder.Create().Build();

            // act
            _sut = new Unitofwork(customersContext);

            // assert
            _sut.Should().NotBeNull().And.BeOfType<Unitofwork>();
            _sut.HasTransactionOpen().Should().BeFalse();
        }

        [Fact]
        public void BeginTransaction_Without_TransactionOpen_Should_ReturnSameInstance_With_TransactionOpen()
        {
            // arrange
            var customersContext = CustomersContextMockBuilder
                .Create()
                .DatabaseBeginTransaction()
                .Build();
            _sut = new Unitofwork(customersContext);

            // act
            var result = _sut.BeginTransaction();

            // assert
            result.Should().NotBeNull().And.BeOfType<Unitofwork>().And.BeSameAs(_sut);
            result.HasTransactionOpen().Should().BeTrue();
        }

        [Fact]
        public void BeginTransaction_With_TransactionOpen_Should_ReturnNewInstance_With_TransactionOpen()
        {
            // arrange
            var customersContext = CustomersContextMockBuilder
                .Create()
                .DatabaseBeginTransaction()
                .Build();
            _sut = new Unitofwork(customersContext);
            _sut = _sut.BeginTransaction();

            // act
            var result = _sut.BeginTransaction();

            // assert
            result.Should().NotBeNull().And.BeOfType<Unitofwork>().And.NotBeSameAs(_sut);
            result.HasTransactionOpen().Should().BeTrue();
        }

        [Fact]
        public async Task CommitAsync_With_TransactionOpen_Should_Return_SuccessResult_And_CloseTransaction()
        {
            // arrange
            const int expectedChanges = 1;
            const int expectedStatus = 200;
            var customersContext = CustomersContextMockBuilder
                .Create()
                .DatabaseBeginTransaction()
                .SaveChanges(expectedChanges)
                .Build();
            _sut = new Unitofwork(customersContext);
            _sut.BeginTransaction();
            var hasTransactionBeforeCommit = _sut.HasTransactionOpen();

            // act
            var result = await _sut.CommitAsync();

            // assert
            _sut.HasTransactionOpen().Should().BeFalse().And.Be(!hasTransactionBeforeCommit);
            result.Should().NotBeNull().And.BeOfType<SuccessResult>();
            result.IsSuccess().Should().BeTrue();
            result.GetStatus().Should().Be(expectedStatus);
            result.GetData().Should().NotBeNull().And.Be(expectedChanges);
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task CommitAsync_Without_TransactionOpen_Should_Return_SuccessResult_And_StaysWithTransactionClosed()
        {
            // arrange
            const int expectedChanges = 1;
            const int expectedStatus = 200;
            var customersContext = CustomersContextMockBuilder
                .Create()
                .SaveChanges(expectedChanges)
                .Build();
            _sut = new Unitofwork(customersContext);
            var hasTransactionBeforeCommit = _sut.HasTransactionOpen();

            // act
            var result = await _sut.CommitAsync();

            // assert
            _sut.HasTransactionOpen().Should().BeFalse().And.Be(hasTransactionBeforeCommit);
            result.Should().NotBeNull().And.BeOfType<SuccessResult>();
            result.IsSuccess().Should().BeTrue();
            result.GetStatus().Should().Be(expectedStatus);
            result.GetData().Should().NotBeNull().And.Be(expectedChanges);
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public async Task CommitAsync_With_TransactionOpen_And_ContextThrowsException_Should_Return_FailResult_With_StatusConflict_And_CloseTransaction()
        {
            // arrange
            const int expectedStatus = 409;
            var expectedMessages = new[] { "dummy exception mssage" };
            var expectedException = new DbUpdateException(expectedMessages[0]);
            var customersContext = CustomersContextMockBuilder
                .Create()
                .DatabaseBeginTransaction()
                .SaveChangesException(expectedException)
                .Build();
            _sut = new Unitofwork(customersContext);
            _sut.BeginTransaction();
            var hasTransactionBeforeCommit = _sut.HasTransactionOpen();

            // act
            var result = await _sut.CommitAsync();

            // assert
            _sut.HasTransactionOpen().Should().BeFalse().And.Be(!hasTransactionBeforeCommit);
            result.Should().NotBeNull().And.BeOfType<FailResult>();
            result.IsSuccess().Should().BeFalse();
            result.GetStatus().Should().Be(expectedStatus);
            result.GetData().Should().BeNull();
            result.Errors.Should().NotBeEmpty().And.Contain(expectedMessages);
        }

        [Fact]
        public async Task CommitAsync_Without_TransactionOpen_And_ContextThrowsException_Should_Return_FailResult_With_StatusConflict_And_StaysWithTransactionClosed()
        {
            // arrange
            const int expectedStatus = 409;
            var expectedMessages = new[] { "dummy exception mssage" };
            var expectedException = new DbUpdateException(expectedMessages[0]);
            var customersContext = CustomersContextMockBuilder
                .Create()
                .SaveChangesException(expectedException)
                .Build();
            _sut = new Unitofwork(customersContext);
            var hasTransactionBeforeCommit = _sut.HasTransactionOpen();

            // act
            var result = await _sut.CommitAsync();

            // assert
            _sut.HasTransactionOpen().Should().BeFalse().And.Be(hasTransactionBeforeCommit);
            result.Should().NotBeNull().And.BeOfType<FailResult>();
            result.IsSuccess().Should().BeFalse();
            result.GetStatus().Should().Be(expectedStatus);
            result.GetData().Should().BeNull();
            result.Errors.Should().NotBeEmpty().And.Contain(expectedMessages);
        }

        public void Dispose()
        {
            _sut.Dispose();
        }
    }
}
