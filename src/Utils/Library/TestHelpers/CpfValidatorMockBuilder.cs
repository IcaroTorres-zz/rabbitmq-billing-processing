using FluentValidation;
using FluentValidation.Results;
using Library.Validators;
using Moq;

namespace Library.TestHelpers
{
    public sealed class CpfValidatorMockBuilder
    {
        private readonly Mock<ICpfValidator> _mock;
        private CpfValidatorMockBuilder()
        {
            _mock = new Mock<ICpfValidator>();
        }

        public static CpfValidatorMockBuilder Create()
        {
            return new CpfValidatorMockBuilder();
        }

        public ICpfValidator Build()
        {
            return _mock.Object;
        }

        public CpfValidatorMockBuilder ValidateTrue()
        {
            var validation = new ValidationResult();
            _mock.Setup(x => x.Validate(It.IsAny<string>())).Returns(validation);
            _mock.Setup(x => x.Validate(It.IsAny<ValidationContext<string>>())).Returns(validation);
            _mock.Setup(x => x.ValidateAsync(It.IsAny<string>(), default)).ReturnsAsync(validation);
            _mock.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<string>>(), default)).ReturnsAsync(validation);

            return this;
        }

        public CpfValidatorMockBuilder ValidateFalse()
        {
            var validation = new ValidationResult(new[] { new ValidationFailure("", "") });
            _mock.Setup(x => x.Validate(It.IsAny<string>())).Returns(validation);
            _mock.Setup(x => x.Validate(It.IsAny<ValidationContext<string>>())).Returns(validation);
            _mock.Setup(x => x.ValidateAsync(It.IsAny<string>(), default)).ReturnsAsync(validation);
            _mock.Setup(x => x.ValidateAsync(It.IsAny<ValidationContext<string>>(), default)).ReturnsAsync(validation);

            return this;
        }
    }
}
