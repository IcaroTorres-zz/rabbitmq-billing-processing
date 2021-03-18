using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Library.Results
{
    [ExcludeFromCodeCoverage]
    public class FailResult : OkObjectResult, IResult
    {
        public FailResult(int status, IEnumerable<string> errors) : base(default)
        {
            StatusCode = status;
            this.errors = errors.ToArray();
            Value = new { Data = (object)null, Errors = errors };
        }
        public FailResult(IEnumerable<ValidationFailure> failures) : base(default)
        {
            errors = failures.ExtractMessages().ToArray();
            StatusCode = int.TryParse(failures.FirstOrDefault()?.ErrorCode, out var statusFromValidation)
                ? statusFromValidation
                : StatusCodes.Status400BadRequest;
            Value = new { Data = (object)null, Errors = errors };
        }

        private readonly string[] errors;

        public bool IsSuccess() => false;
        public int GetStatus() => StatusCode ?? StatusCodes.Status409Conflict;
        public object GetData() => default;
        public IReadOnlyList<string> Errors => errors;
    }
}
