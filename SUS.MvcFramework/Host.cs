﻿using SUS.HTTP;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace SUS.MvcFramework
{
    public static class Host
    {
        public static async Task CreateHostAsync(IMvcApplication application, int port = 80)
        {
            List<Route> routeTable = new List<Route>();
            application.ConfigureServices();
            application.Configure(routeTable);

            // TODO: {controller}/{action}/{id}
            IHttpServer server = new HttpServer(routeTable);

            Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", "http://localhost:80");
            await server.StartAsync(80);
        }
    }
}
