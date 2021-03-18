using Customers.Api.Application.Abstractions;
using Customers.Api.Application.Requests;
using Issuance.Api.Application.Abstractions;
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
        private readonly IResponseConverter converter;

        public GetCustomerUsecase(ICustomerRepository repository, IResponseConverter converter)
        {
            this.repository = repository;
            this.converter = converter;
        }

        public async Task<IResult> Handle(GetCustomerRequest request, CancellationToken cancellationToken)
        {
            var id = request.Cpf.AsSpan().ParseUlong();
            var customer = await repository.GetAsync(id, cancellationToken);
            var response = converter.ToResponse(customer);
            return new SuccessResult(response);
        }
    }
}
