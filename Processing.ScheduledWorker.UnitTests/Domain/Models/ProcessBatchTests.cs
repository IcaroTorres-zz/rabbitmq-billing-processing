using Bogus;
using FluentAssertions;
using Processing.ScheduledWorker.Domain.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Processing.ScheduledWorker.UnitTests.Domain.Models
{
    [Trait("unit-test", "processing.scheduled-worker-domain")]
    public class ProcessBatchTests
    {
        [Fact]
        public async Task ResetIdAfter_Shuold_Wait_ExpectedDelay_And_Return_WithNewIdAsync()
        {
            // arrange on constructor
            var expectedMillisecnods = 250;
            var sut = new ProcessBatch();
            var previousId = sut.Id;

            // act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await sut.ResetIdAfter(expectedMillisecnods);
            stopwatch.Stop();


            // assert
            result.Should().NotBeNull();
            result.Billings.Should().BeSameAs(sut.Billings);
            result.Customers.Should().BeSameAs(sut.Customers);
            result.Id.Should().NotBeNullOrEmpty().And.NotBe(previousId);
            stopwatch.ElapsedMilliseconds.Should().BeGreaterThan(expectedMillisecnods);
        }
    }
}
