using Customers.Api.Domain.Models;
using Customers.Api.Domain.Services;
using Moq;

namespace Customers.Api.UnitTests.Helpers
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

        public ModelFactoryMockBuilder CreateCustomer(string cpfString, string name, string state, Customer model)
        {
            _mock.Setup(x => x.CreateCustomer(cpfString, name, state)).Returns(model);
            return this;
        }

        public IModelFactory Build()
        {
            return _mock.Object;
        }
    }
}
