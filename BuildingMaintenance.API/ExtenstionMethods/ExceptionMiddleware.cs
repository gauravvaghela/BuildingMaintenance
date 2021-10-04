using BuildingMaintenance.API.Response;
using BuildingMaintenance.Repositories.IRepository.IFileLogging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BuildingMaintenance.API.ExtenstionMethods
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IFileLoggingRepository _fileLogging;

        public ExceptionMiddleware(RequestDelegate next, IFileLoggingRepository fileLogging)
        {
            _fileLogging = fileLogging;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _fileLogging.Error("Log time: " + DateTime.Now + " Log Exception: " + ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            await httpContext.Response.WriteAsync(new ErrorResponse()
            {
                Message = ex.Message,
                StatusCode = httpContext.Response.StatusCode
            }.ToString());
        }
    }
}

