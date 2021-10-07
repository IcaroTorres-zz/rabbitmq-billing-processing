using System.Threading.Tasks;

namespace Library.Messaging
{
    /// <summary>
    /// Messaging Publisher abstraction accepting messages implementing <see cref="MessageBase"/>.
    /// </summary>
    public interface IMessagePublisher
    {
        /// <summary>
        /// Publishes configured messages implementing <see cref="MessageBase"/>.
        /// </summary>
        /// <param name="messageSettings"></param>
        Task Publish(MessageBase messageSettings);
    }
}
