using Library.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Library.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IWebHostEnvironment env, ILogger<ExceptionMiddleware> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(exception, context, logger, env.IsProduction());
            }
        }
        private static Task HandleExceptionAsync(Exception ex, HttpContext context, ILogger<ExceptionMiddleware> logger, bool isProduction)
        {
            var extractedMessages = ex.ExtractMessages();
            logger.LogError($"Unexpected error occurred. Messages: {string.Join(Environment.NewLine, extractedMessages)}");

            var errors = isProduction
                ? new List<string>() { "Unexpected error occurred. Please contact system's administrators." }
                : extractedMessages;

            var result = new FailResult(StatusCodes.Status500InternalServerError, errors);
            return WriteErrorAsync(result, context);
        }

        private static Task WriteErrorAsync(FailResult result, HttpContext context)
        {
            var stringResult = JsonConvert.SerializeObject(result.Value, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Include,
            });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = result.StatusCode ?? 500;
            return context.Response.WriteAsync(stringResult);
        }
    }
}
