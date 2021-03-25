using FluentValidation;
using Library.Results;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Library.PipelineBehaviors
{
    public class RequestValidation<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
        where TRequest : IRequest<TResult>
        where TResult : IResult
    {
        private readonly IEnumerable<IValidator<TRequest>> validators;

        public RequestValidation(IEnumerable<IValidator<TRequest>> validators)
        {
            this.validators = validators;
        }

        public async Task<TResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResult> next)
        {
            var context = new ValidationContext<TRequest>(request);
            var tasks = validators.Select(x => x.ValidateAsync(context, cancellationToken));
            var results = await Task.WhenAll(tasks);
            var failures = results.SelectMany(x => x.Errors).Where(x => x != null).ToList();

            return failures.Count > 0
                ? (TResult)(IResult)new FailResult(failures)
                : await next();
        }
    }
}
