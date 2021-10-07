using Bogus;
using FluentAssertions;
using Library.TestHelpers;
using Processing.Scheduled.Worker.Models;
using Processing.Scheduled.Worker.Services;
using Xunit;

namespace UnitTests.Scheduled.Worker.Services
{
    [Trait("processing", "scheduled-worker-services")]
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
