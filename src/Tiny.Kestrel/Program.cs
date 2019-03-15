using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Text;
using Tinyfx.Cores;
using Tinyfx.Utils;

namespace Tiny.Kestrel
{
    class Program
    {
        static void Main(string[] args)
        {
            TinyfxCore.Initialize();

            if (args == null || args.Length < 1)
            {
                StartKestrelServer();
            }
            else
            {

                if (args.Length == 1)
                {
                    if (args[0] == "-g" || args[0] == "--generate")
                    {
                        PagesGenerater.GenerateStaticPages(Path.Combine(Environment.CurrentDirectory, "publish"), s => 
                        {
                            Console.WriteLine(s);
                        });
                    }
                    else
                    {
                        Console.WriteLine("Option " + args[0] + " has no ideas.");
                        ShowUseages();
                    }
                }
                else
                {
                    ShowUseages();
                }
            }
        }

        static void ShowUseages()
        {
            Console.WriteLine();
            Console.WriteLine("\t-g, --generate    generate static pages");
            Console.WriteLine("\t-v, --version     show version infos");
        }

        /// <summary>
        /// 启动 Kestrel 服务器
        /// </summary>
        static void StartKestrelServer()
        {
            var config = TinyfxCore.Configuration;
            int port = config.Port;
            var host = new WebHostBuilder()
               .UseUrls("http://*:" + port + "/")
               .UseContentRoot(config.DataDirectory)
               .UseKestrel()
               .UseStartup<Startup>()
               .Build();

            host.Run();
        }
    }
}
