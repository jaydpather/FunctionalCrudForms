using System.IO;
using Microsoft.AspNetCore.Http;

namespace RebelSoftware.HttpService
{
    internal class HttpService : IHttpService
    {
        private HttpContext _httpContext;
        public HttpService(HttpContext context)
        {
            _httpContext = context;
        }

        public async void WriteHttpResponse(string response)
        {
            //_httpContext.Response.Headers["Access-Control-Allow-Origin"] = "*";
            _httpContext.Response.Headers["Access-Control-Allow-Origin"] = new string[] {"*"}; //Microsoft.Extensions.Primitives.StringValues("*");
            await _httpContext.Response.WriteAsync(response);
        }

        public string ReadRequestBody()
        {
            using(var reader = new StreamReader(_httpContext.Request.Body))
            {
                var bodyTask = reader.ReadToEndAsync();
                bodyTask.Wait();
                var body = bodyTask.Result;
                return body;
            }
        }
    }
}