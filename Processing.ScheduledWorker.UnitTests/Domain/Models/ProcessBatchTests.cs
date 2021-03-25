using Bogus;
using FluentAssertions;
using Processing.ScheduledWorker.Domain.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace Processing.ScheduledWorker.UnitTests.Domain.Models
{
    public class ProcessBatchTests
    {
        [Fact]
        public async Task ResetIdAfter_Shuold_Wait_ExpectedDelay_And_Return_WithNewId()
        {
            // arrange on constructor
            var expectedMillisecnods = new Faker().Random.Int(250, 3000);
            var sut = new ProcessBatch();
            var previousId = sut.Id;

            // act
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var result = await sut.ResetIdAfter(expectedMillisecnods);
            stopwatch.Stop();


            // assert
            result.Should().NotBeNull();
            result.Id.Should().NotBeNullOrEmpty().And.NotBe(previousId);
            stopwatch.ElapsedMilliseconds.Should().BeGreaterThan(expectedMillisecnods);
        }
    }
}
