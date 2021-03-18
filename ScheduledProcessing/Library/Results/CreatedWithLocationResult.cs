using Library.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Library.Results
{
    /// <summary>
    /// Represents a success result configured to generate loaction response header
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ExcludeFromCodeCoverage]
    public class CreatedWithLocationResult<T> : CreatedResult, IResult
    {
        public CreatedWithLocationResult(T data, ICreationRequest<T> creationCommand)
            : base(GenerateLocation(creationCommand, data), default)
        {
            _data = data;
            _errors = new string[] { };
            Value = new { Data = data, Errors = _errors };
        }

        private readonly object _data;
        private readonly string[] _errors;

        public bool IsSuccess() => true;
        public int GetStatus() => StatusCodes.Status201Created;
        public object GetData() => _data;
        public IReadOnlyList<string> Errors => _errors;

        private static string GenerateLocation(ICreationRequest<T> creationCommand, T data)
        {
            var urlHelper = creationCommand.GetUrlHelper();
            var routeName = creationCommand.GetRouteName();
            var routeValuesFunction = creationCommand.GetRouteValuesFunc();

            return urlHelper.Link(routeName, routeValuesFunction(data));
        }
    }
}
