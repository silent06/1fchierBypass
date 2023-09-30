using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using HashHandler;
using System.Threading;
namespace KvHashHandler
{
    internal class Program
    {
        private static Thread ConsoleThread;
        static void Start() {

            ConsoleThread = new Thread(new ThreadStart(() => {

                while (true)
                {
                    
                    Console.Title = string.Format("Bypass Handler | Good Proxies: {0} | Bad Proxies: {1} | Link Count: {2} | ", Utils.GoodProxyCount, Utils.BadProxyCount, Utils.linkCount);

                    Thread.Sleep(5000);
                }
            }));
            ConsoleThread.Start();
        }

        static void Main(string[] args)
        {
            
            new Program().MainAsync().GetAwaiter().GetResult();
 
        }
        public async Task MainAsync()
        {

            Start();
            await Bypass.startAsync();


        }


    }
}
