using Microsoft.AspNetCore.Mvc;
using System;

namespace Library.Requests
{
    public abstract class CreationRequestBase<T> : ICreationRequest<T>
    {
        protected string Route { get; set; }
        protected IUrlHelper UrlHelper { get; set; }
        protected Func<T, object> RouteValuesFunc { get; set; }

        public string GetRouteName() => Route;
        public IUrlHelper GetUrlHelper() => UrlHelper;
        public Func<T, object> GetRouteValuesFunc() => RouteValuesFunc;

        public ICreationRequest<T> SetupForCreation(IUrlHelper urlHelper, string route, Func<T, object> routeValuesFunc)
        {
            UrlHelper = urlHelper;
            Route = route;
            RouteValuesFunc = routeValuesFunc;
            return this;
        }
    }
}
