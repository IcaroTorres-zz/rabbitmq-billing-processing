using Customers.Api.Application.Abstractions;
using Customers.Api.Application.Requests;
using Customers.Api.Application.Responses;
using Library.Abstractions;
using Library.Optimizations;
using Library.Results;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Customers.Api.Application.Usecases
{
    public class GetCustomerUsecase : IRequestHandler<GetCustomerRequest, IResult>
    {
        private readonly ICustomerRepository repository;

        public GetCustomerUsecase(ICustomerRepository repository)
        {
            this.repository = repository;
        }

        public async Task<IResult> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
        {
            var id = request.Cpf.AsSpan().ParseUlong();
            var customer = await repository.GetAsync(id, cancellationToken);
            var response = new CustomerResponse(customer);
            return new SuccessResult(response);
        }
    }
}
