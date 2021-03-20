using FluentAssertions;
using FluentValidation.Results;

namespace ScheduledProcessing.Tests.SharedHelpers
{
    public static class ValidatorTestExtensions
    {
        public static void AssertValidationFailuresCount(this ValidationResult result, int expectedErrorsCount)
        {
            // assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(expectedErrorsCount);
        }
    }
}
