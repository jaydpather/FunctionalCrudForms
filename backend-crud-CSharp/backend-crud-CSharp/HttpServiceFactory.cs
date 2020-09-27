using Microsoft.AspNetCore.Http;

namespace backend_crud_CSharp
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

