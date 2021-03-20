using Issuance.Api.Domain.Models;
using Issuance.Api.Domain.Services;
using Moq;

namespace ScheduledProcessing.Tests.Issuance.Helpers
{
    public sealed class ModelFactoryMockBuilder
    {
        private readonly Mock<IModelFactory> _mock;

        private ModelFactoryMockBuilder()
        {
            _mock = new Mock<IModelFactory>();
        }

        public static ModelFactoryMockBuilder Create()
        {
            return new ModelFactoryMockBuilder();
        }

        public ModelFactoryMockBuilder CreateBilling(string cpfString, double amount, string dueDate, Billing model)
        {
            _mock.Setup(x => x.CreateBilling(cpfString, amount, dueDate)).Returns(model);
            return this;
        }

        public IModelFactory Build()
        {
            return _mock.Object;
        }
    }
}
