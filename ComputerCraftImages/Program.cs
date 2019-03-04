using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComputerCraftImages
{
    class Program
    {
        public static string imgUrl = "https://cdn.discordapp.com/attachments/327126086234406912/543968256696909834/mfw.PNG";

        static void Main(string[] args)
        {
            Thread t = new Thread(() =>
            {
                while(true)
                {
                    imgUrl = Console.ReadLine();
                }
            });
            t.IsBackground = true;
            t.Start();
            MainAsync().GetAwaiter().GetResult();
        }

        public static Task MainAsync()
        {
            var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    IPAddress addr = IPAddress.Any;
                    options.Listen(addr, 43301);

                })
                .UseStartup<Program>()
                .Build();

            return host.RunAsync();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.Run(HttpHandler.OnHttpRequest);

        }

        public static Task QuickWriteToDoc(Microsoft.AspNetCore.Http.HttpContext context, string content, string type = "text/html", int code = 200)
        {
            var response = context.Response;
            response.StatusCode = code;
            response.ContentType = type;

            //Load the template.
            string html = content;
            var data = Encoding.UTF8.GetBytes(html);
            response.ContentLength = data.Length;
            return response.Body.WriteAsync(data, 0, data.Length);
        }
    }
}
