namespace SUS.HTTP
{
    public class HttpServer : IHttpServer
    {
        public void AddRoute(string path, System.Func<HttpRequest, HttpResponse> action)
        {
            throw new System.NotImplementedException();
        }

        public void Start(int port)
        {
            throw new System.NotImplementedException();
        }
    }
}
