using MediatR;
using System.Threading.Tasks;

namespace PrivatePackage.Abstractions
{
    public interface IMessageConsumer<T> where T : IRequest<IResult>
    {
        Task ConsumeWithUsecase(string consumerUsecase);
    }
}
