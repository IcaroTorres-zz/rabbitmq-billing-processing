using Bogus;
using FluentAssertions;
using Library.TestHelpers;
using Processing.Worker.Domain.Models;
using Processing.Worker.Domain.Services;
using Xunit;

namespace Processing.Worker.UnitTests.Domain.Services
{
    [Trait("unit-test", "processing.worker-domain")]
    public class AmountProcessorTests
    {
        [Fact]
        public void MathOnlyAmountProcessor_Calculate()
        {
            // arrange
            var cpf = Fakes.CPFs.Valid().Generate();
            var str = cpf.ToString();
            var customer = new Customer { Cpf = cpf };
            var billing = new Billing { Cpf = cpf, Amount = new Faker().Random.Decimal(1, 1000) };
            var processedAtValueBeforeProcess = billing.ProcessedAt;
            var digits = new char[4] { str[0], str[1], str[^2], str[^1] };
            var expectedAmount = decimal.Parse(digits);
            var sut = new MathOnlyAmountProcessor();

            // act
            var result = sut.Process(customer, billing);

            // assert
            result.Should().NotBeNull()
                .And.BeOfType<Billing>()
                .And.BeSameAs(billing);
            result.Amount.Should().Be(expectedAmount);
            result.Amount.ToString("0000").Should().Be(string.Join("", digits));
            result.ProcessedAt.Should().NotBeNull();
            processedAtValueBeforeProcess.Should().BeNull();
        }
    }
}
