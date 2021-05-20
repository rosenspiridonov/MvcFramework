﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SUS.HTTP
{
    public class HttpServer : IHttpServer
    {
        IDictionary<string, Func<HttpRequest, HttpResponse>> routeTable =
            new Dictionary<string, Func<HttpRequest, HttpResponse>>();

        public void AddRoute(string path, Func<HttpRequest, HttpResponse> action)
        {
            if (routeTable.ContainsKey(path))
            {
                routeTable[path] = action;
            }
            else
            {
                routeTable.Add(path, action);
            }
        }

        public async Task StartAsync(int port)
        {
            TcpListener tcpListner = new TcpListener(IPAddress.Loopback, port);
            tcpListner.Start();
            while (true)
            {
                TcpClient tcpClient = await tcpListner.AcceptTcpClientAsync();

                ProcessClientAsync(tcpClient);
            }
        }

        private async Task ProcessClientAsync(TcpClient tcpClient)
        {
            try
            {
                using NetworkStream stream = tcpClient.GetStream();
                while (true)
                {
                    List<byte> data = new List<byte>();
                    int position = 0;
                    byte[] buffer = new byte[HttpConstants.BufferSize];

                    // Process Http Request
                    while (true)
                    {
                        int count = await stream.ReadAsync(buffer, position, buffer.Length);
                        position += count;

                        if (count < buffer.Length)
                        {
                            var partialBuffer = new byte[count];
                            Array.Copy(buffer, partialBuffer, count);

                            data.AddRange(partialBuffer);
                            break;
                        }
                        else
                        {
                            data.AddRange(buffer);
                        }
                    }

                    var requestAsString = Encoding.UTF8.GetString(data.ToArray());
                    var request = new HttpRequest(requestAsString);
                    Console.WriteLine(request.Method + " " + request.Path + " => Headers: " + request.Headers.Count);

                    HttpResponse response;

                    if (routeTable.ContainsKey(request.Path))
                    {
                        var action = this.routeTable[request.Path];
                        response = action(request);
                    }
                    else
                    {
                        response = new HttpResponse("text/html", new byte[0], Enums.HttpStatusCode.NotFound);
                    }

                    response.Headers.Add(new Header("Server", "SUS Server 1.0"));
                    response.Cookies.Add(new ResponseCookie("sid", Guid.NewGuid().ToString())
                    { HttpOnly = true, MaxAge = 60 * 24 * 60 * 60 });

                    var responseHeaderBytes = Encoding.UTF8.GetBytes(response.ToString());

                    await stream.WriteAsync(responseHeaderBytes, 0, responseHeaderBytes.Length);
                    await stream.WriteAsync(response.Body, 0, response.Body.Length);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }


        }
    }
}
