using Library.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Library.Requests
{
    /// <summary>
    /// Abstraction for transactional requests resulting in 201 status codes
    /// allowing the construction of the location response header
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICreationRequest<T> : IRequest<IResult>, ITransactionRequest
    {
        IUrlHelper GetUrlHelper();
        string GetRouteName();
        Func<T, object> GetRouteValuesFunc();

        /// <summary>
        /// Setup the request to generate responses with location header filled when returned from use cases;
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="route"></param>
        /// <param name="routeValuesFunc"></param>
        /// <returns></returns>
        ICreationRequest<T> SetupForCreation(IUrlHelper urlHelper, string route, Func<T, object> routeValuesFunc);
    }
}
