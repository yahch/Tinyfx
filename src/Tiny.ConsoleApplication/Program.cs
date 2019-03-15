using Nancy.Hosting.Self;
using System;
using Tinyfx.Cores;

namespace Tiny.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            TinyfxCore.Initialize();

            if (args == null || args.Length < 1)
            {
                StartNancyServer();
            }
            else
            {

                if (args.Length == 1)
                {
                    if (args[0] == "-g" || args[0] == "--generate")
                    {
                        PagesGenerater.GenerateStaticPages(System.IO.Path.Combine(Environment.CurrentDirectory, "publish"), s =>
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
        static void StartNancyServer()
        {
            var config = TinyfxCore.Configuration;
            int port = config.Port;
            using (var nancyHost = new NancyHost(new Uri("http://localhost:" + port + "/")))
            {
                nancyHost.Start();
                Console.WriteLine(config.SiteName + " now listening on port " + config.Port + " ...");

                Console.ReadKey();
            }
            Console.WriteLine("Bye!");
        }
    }
}
