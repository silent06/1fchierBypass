using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.Serialization.Formatters.Binary;
using HashHandler;

namespace Download
{
    internal class Program
    {

        static void Main(string[] args)
        {

            new Program().MainAsync().GetAwaiter().GetResult();

        }
        public async Task MainAsync()
        {

            try
            {
                Utils.Attempts = 0;
                Utils.linkinfo = Utils.LoadFile("linkinfo.txt");
                Utils.GoodProxies = Utils.LoadFile("GoodProxies.txt");

                // Pass the CancellationToken to the method
                await Utils.DoWorkAsync(Utils.cts.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            Console.ReadKey();

        }

    }
}
