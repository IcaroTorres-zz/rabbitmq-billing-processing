using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace PrivatePackage.Abstractions
{
    public interface ICreationRequest<T> : IRequest<IResult>, ITransactionRequest
    {
        IUrlHelper GetUrlHelper();
        string GetRouteName();
        Func<T, object> GetRouteValuesFunc();
        ICreationRequest<T> SetupForCreation(IUrlHelper urlHelper, string routeName, Func<T, object> routeValuesFunc);
    }
}
