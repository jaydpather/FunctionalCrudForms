using Microsoft.AspNetCore.Http;

namespace RebelSoftware.HttpService
{
    public interface IHttpService
    {
        void WriteHttpResponse(string response);
        string ReadRequestBody();
    }

    public static class HttpServiceFactory
    {
        public static IHttpService CreateHttpService(HttpContext httpContext)
        {
            return new HttpService(httpContext);
        }
    }
}

