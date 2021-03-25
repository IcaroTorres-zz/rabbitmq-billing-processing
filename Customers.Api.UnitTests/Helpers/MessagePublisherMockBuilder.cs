using Library.Messaging;
using Moq;
using System.Threading.Tasks;

namespace Customers.Api.UnitTests.Helpers
{
    public sealed class MessagePublisherMockBuilder
    {
        private readonly Mock<IMessagePublisher> _mock;

        private MessagePublisherMockBuilder()
        {
            _mock = new Mock<IMessagePublisher>();
            _mock.Setup(x => x.Publish(It.IsAny<MessageBase>())).Returns(Task.CompletedTask);
        }

        public static MessagePublisherMockBuilder Create()
        {
            return new MessagePublisherMockBuilder();
        }

        public IMessagePublisher Build()
        {
            return _mock.Object;
        }
    }
}
