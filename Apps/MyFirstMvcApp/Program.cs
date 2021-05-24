using MyFirstMvcApp.Controllers;
using SUS.HTTP;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MyFirstMvcApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IHttpServer server = new HttpServer();

            server.AddRoute("/", new HomeController().Index);
            server.AddRoute("/favicon.ico", new StaticFilesController().Favicon);
            server.AddRoute("/about", new HomeController().About);
            server.AddRoute("/users/login", new UserController().Login);
            server.AddRoute("/users/register", new UserController().Register);
            Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", "http://localhost:80");
            await server.StartAsync(80);
        }
    }
}
