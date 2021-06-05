using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using SUS.HTTP;
using SUS.HTTP.Enums;
using System.Linq;
using System;

namespace SUS.MvcFramework
{
    public static class Host
    {
        public static async Task CreateHostAsync(IMvcApplication application, int port = 80)
        {
            List<Route> routeTable = new List<Route>();

            AutoRegisterStaticFiles(routeTable);
            AutoRegisterRoutes(routeTable, application);

            application.ConfigureServices();
            application.Configure(routeTable);

            Console.WriteLine("All registered routes");
            foreach (var route in routeTable)
            {
                Console.WriteLine($"{route.Method} {route.Path}");
            }

            // TODO: {controller}/{action}/{id}
            IHttpServer server = new HttpServer(routeTable);

            Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", "http://localhost:80");
            await server.StartAsync(80);
        }

        private static void AutoRegisterRoutes(List<Route> routeTable, IMvcApplication application)
        {
            var controllerTypes = application.GetType().Assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(Controller)));

            foreach (var controllerType in controllerTypes)
            {
                var methods = controllerType.GetMethods()
                    .Where(x => x.IsPublic && !x.IsStatic && x.DeclaringType == controllerType
                    && !x.IsAbstract && !x.IsConstructor && !x.IsSpecialName);

                foreach (var method in methods)
                {
                    var path = $"/{controllerType.Name.Replace("Controller", string.Empty)}/{method.Name}";

                    var attribute = method.GetCustomAttributes(false)
                        .Where(x => x.GetType().IsSubclassOf(typeof(BaseHttpAttribute)))
                        .FirstOrDefault() as BaseHttpAttribute;

                    if (!string.IsNullOrEmpty(attribute?.Url))
                    {
                        path = attribute.Url; 
                    }

                    var httpMethod = HttpMethod.GET;

                    if (attribute != null)
                    {
                        httpMethod = attribute.Method;
                    }

                    routeTable.Add(new Route(path, httpMethod, (request) =>
                    {
                        var instance = Activator.CreateInstance(controllerType);
                        var response = method.Invoke(instance, new[] { request }) as HttpResponse;

                        return response;
                    }));
                }
            }
        }

        private static void AutoRegisterStaticFiles(List<Route> routeTable)
        {
            var staticFiles = Directory.GetFiles("wwwroot", "*", SearchOption.AllDirectories);

            foreach (var file in staticFiles)
            {
                var url = file.Replace("wwwroot", string.Empty)
                    .Replace("\\", "/");

                routeTable.Add(new Route(url, HttpMethod.GET, (request) =>
                {
                    var fileContent = File.ReadAllBytes(file);
                    var fileExt = new FileInfo(file).Extension;
                    var contentType = fileExt switch
                    {
                        ".txt" => "text/plain",
                        ".js" => "text/javascript",
                        ".css" => "text/css",
                        ".jpg" => "image/jpg",
                        ".jpeg" => "image/jpg",
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        ".ico" => "image/vnd.microsoft.icon",
                        ".html" => "text/html",
                        _ => "text/plain"
                    };

                    return new HttpResponse(contentType, fileContent, HttpStatusCode.Ok);
                }));
            }
        }
    }
}
