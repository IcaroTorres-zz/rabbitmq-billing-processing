using Library.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Library.Results
{
    [ExcludeFromCodeCoverage]
    public class SuccessResult : OkObjectResult, IResult
    {
        public SuccessResult(object data, int status = StatusCodes.Status200OK) : base(default)
        {
            this.data = data;
            errors = new string[] { };
            StatusCode = status;
            Value = new { Data = data, Errors = errors };
        }

        private readonly object data;
        private readonly string[] errors;

        public bool IsSuccess() => true;
        public int GetStatus() => StatusCode ?? StatusCodes.Status200OK;
        public object GetData() => data;
        public IReadOnlyList<string> Errors => errors;
    }
}
