using Customers.Application.Abstractions;
using Customers.Application.Requests;
using Customers.Application.Responses;
using Library.Optimizations;
using Library.Results;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Application.Usecases
{
    public class GetCustomerUsecase : IRequestHandler<GetCustomerRequest, IResult>
    {
        private readonly ICustomerRepository _repository;

        public GetCustomerUsecase(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<IResult> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
        {
            var id = request.Cpf.AsSpan().ParseUlong();
            var customer = await _repository.GetAsync(id, cancellationToken);
            var response = new CustomerResponse(customer);
            return new SuccessResult(response);
        }
    }
}
