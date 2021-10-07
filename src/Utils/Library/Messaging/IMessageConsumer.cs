using Library.Results;
using MediatR;
using System.Threading.Tasks;

namespace Library.Messaging
{
    public interface IMessageConsumer<T> where T : IRequest<IResult>
    {
        Task ConsumeWithUsecase(string consumerUsecase);
    }
}
