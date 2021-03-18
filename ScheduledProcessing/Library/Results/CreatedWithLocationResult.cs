using Library.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Library.Results
{
    [ExcludeFromCodeCoverage]
    public class CreatedWithLocationResult<T> : CreatedResult, IResult
    {
        public CreatedWithLocationResult(T data, ICreationRequest<T> creationCommand)
            : base(GenerateLocation(creationCommand, data), default)
        {
            this.data = data;
            errors = new string[] { };
            Value = new { Data = data, Errors = errors };
        }

        private readonly object data;
        private readonly string[] errors;

        public bool IsSuccess() => true;
        public int GetStatus() => StatusCodes.Status201Created;
        public object GetData() => data;
        public IReadOnlyList<string> Errors => errors;

        private static string GenerateLocation(ICreationRequest<T> creationCommand, T data)
        {
            var urlHelper = creationCommand.GetUrlHelper();
            var routeName = creationCommand.GetRouteName();
            var routeValuesFunction = creationCommand.GetRouteValuesFunc();

            return urlHelper.Link(routeName, routeValuesFunction(data));
        }
    }
}
