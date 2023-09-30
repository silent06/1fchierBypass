using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace HashHandler
{

    class Utils
    {
        public static string savePath;
        public static string fileName;
        public static int Attempts;
        public static bool isProxyError;
        public static List<string> linkinfo;
        public static List<string> GoodProxies;
        //public static bool Attempts;
        public static WebClient client;
        public static WebClient client2;
        public static List<string> LoadFile(string filePath)
        {
            List<string> file = new List<string>();

            // Read the file line by line and add each line (proxy) to the list
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    file.Add(line);
                }
            }

            return file;
        }

        public static void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            // Display the progress information
            //Console.WriteLine($"Downloaded {e.BytesReceived} bytes out of {e.TotalBytesToReceive} bytes ({e.ProgressPercentage}%).");
            Utils.AppendText(string.Concat(new object[] { $"Downloaded {e.BytesReceived} bytes out of {e.TotalBytesToReceive} bytes ({e.ProgressPercentage}%)." }), ConsoleColor.Green);
            // Request cancellation
            Utils.cts.Cancel();
            // Check if the download has stopped
            if (e.BytesReceived == e.TotalBytesToReceive && e.TotalBytesToReceive > 0)
            {
                // Exit the current proxy
                //client.Proxy = null;

                // Set a flag to indicate a proxy error
                //isProxyError = true;

                //Utils.client2 = new Utils.WebClientWithTimeout();
                Utils.AppendText(string.Concat(new object[] { $"Download Completed for file: {fileName}" }), ConsoleColor.Green);
                Utils.AppendText(string.Concat(new object[] { "Download completed." }), ConsoleColor.Green);
                Environment.Exit(0);
                // Switch to a new proxy
                /*Random random = new Random();
                int randomIndex = random.Next(0, Utils.GoodProxies.Count);
                string randomString = Utils.GoodProxies[randomIndex];
                var proxyParts = randomString.Split(':');
                Utils.AppendText(string.Concat(new object[] { $"Download stopped swithcing Proxy to: {proxyParts[0]} with port: {proxyParts[1]}" }), ConsoleColor.Green);
                client.Proxy = new WebProxy(proxyParts[0], Convert.ToInt32(proxyParts[1]));
                */
                // Restart the download asynchronously
                //client2.DownloadFileAsync(new Uri(Utils.linkinfo[0]), Utils.savePath);

                //Utils.client2.DownloadProgressChanged += Utils.Client_DownloadProgressChanged;
                //Utils.client2.DownloadDataCompleted += Utils.Client_DownloadFileCompleted;
            }

        }

        public static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // Display the completion message
            //Console.WriteLine("Download completed.");
            Utils.AppendText(string.Concat(new object[] { "Download completed." }), ConsoleColor.Green);
            Environment.Exit(0);

        }
        public static async Task<bool> CheckProxyAsync(string proxyAddress, int proxyPort)
        {
            try
            {
                var proxy = new WebProxy(proxyAddress, proxyPort);
                var testUrl = "https://www.example.com"; // Replace with your desired test URL

                var request = WebRequest.Create(testUrl);
                request.Proxy = proxy;
                request.Timeout = 5000;
                using (var response = await request.GetResponseAsync())
                {
                    // Proxy is valid and working
                    //Utils.AppendText(string.Concat(new object[] { $"Proxy {proxyAddress} is working." }), ConsoleColor.Green);
                    return true;
                }
            }
            catch (Exception)
            {
                // Proxy is invalid or not working    
                //Utils.AppendText(string.Concat(new object[] { $"Proxy {proxyAddress} is not working." }), ConsoleColor.Red);

                return false;
            }
        }
        public static CancellationTokenSource cts = new CancellationTokenSource();

        public static async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            // Check if cancellation has been requested
            if (cancellationToken.IsCancellationRequested)
            {
                // Handle cancellation
                return;
            }
            List<string> proxyList = Utils.LoadProxiesFromFile("proxy-list.txt"); // Get the list of proxies
            // Perform the work
            // ...
            await Utils.CheckProxiesAsync(proxyList);
        }


        public static async Task<List<string>> CheckProxiesAsync(List<string> proxyList)
        {
            var validProxies = new List<string>();

            var tasks = new List<Task>();

            foreach (var proxy in proxyList)
            {
                var proxyParts = proxy.Split(':');
                if (proxyParts.Length == 2 && int.TryParse(proxyParts[1], out int proxyPort))
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        var isValid = await CheckProxyAsync(proxyParts[0], proxyPort);
                        if (isValid)
                        {
                            lock (validProxies)
                            {
                                validProxies.Add(proxy);
                                Utils.GoodProxies.Add(proxy);
                                Attempts++;
                                Download.Download.Downloadfiles(proxyParts[0], proxyParts[1], Attempts);
                            }
                        }
                    }, cts.Token));
                }
            }

            await Task.WhenAll(tasks);
            Utils.AppendText(string.Concat(new object[] { "Down Checking proxies!" }), ConsoleColor.Green);
            return validProxies;
        }


        public class WebClientWithTimeout : WebClient
        {
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest wr = base.GetWebRequest(address);
                wr.Timeout = 5000; // timeout in milliseconds (ms)
                return wr;
            }
        }

        public static List<string> LoadProxiesFromFile(string filePath)
        {
            List<string> proxies = new List<string>();

            // Read the file line by line and add each line (proxy) to the list
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    proxies.Add(line);
                }
            }

            return proxies;
        }

        public static void AppendText(string str, ConsoleColor color)
        {
            StreamWriter log;
            //StreamWriter log = new StreamWriter("Bypass.log", true);
            //log.BaseStream.Seek(0, SeekOrigin.End);
            //log.AutoFlush = true;
            try
            {
                string time = string.Format("{0:hh:mm:ss tt}", DateTime.Now.ToUniversalTime().ToLocalTime());
                Console.ForegroundColor = color;
                Console.WriteLine(string.Concat(new object[] { "[", time, "]", " ", str, "" }));
                //if (!File.Exists("Bypass.log")) { log = new StreamWriter("Bypass.log"); } else { log = File.AppendText("Bypass.log"); }
                //log.WriteLine(string.Concat(new object[] { "[", time, "]", " ", str, "" })); log.Close();
            }
            catch
            {
                string time = string.Format("{0:hh:mm:ss tt}", DateTime.Now.ToUniversalTime().ToLocalTime());
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(string.Concat(new object[] { "[", time, "]", "An Error Has Occured" }));
                //if (!File.Exists("Bypass.log")) { log = new StreamWriter("Bypass.log"); } else { log = File.AppendText("Bypass.log"); }
                //log.WriteLine(string.Concat(new object[] { "[", time, "]", "An Error Has Occured" })); log.Close();
            }
        }

        // Event handler for progress updates
        public static void Client_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update the progress on the client side
            Console.WriteLine("Progress: " + e.ProgressPercentage + "%");
        }

        public static void RC4(ref byte[] Data, byte[] Key)
        {
            byte[] array = new byte[256];
            byte[] array2 = new byte[256];
            int i;
            for (i = 0; i < 256; i++)
            {
                array[i] = (byte)i;
                array2[i] = Key[i % Key.GetLength(0)];
            }
            int num = 0;
            for (i = 0; i < 256; i++)
            {
                num = (num + (int)array[i] + (int)array2[i]) % 256;
                byte b = array[i];
                array[i] = array[num];
                array[num] = b;
            }
            num = (i = 0);
            for (int j = 0; j < Data.GetLength(0); j++)
            {
                i = (i + 1) % 256;
                num = (num + (int)array[i]) % 256;
                byte b = array[i];
                array[i] = array[num];
                array[num] = b;
                int num2 = (int)(array[i] + array[num]) % 256;
                Data[j] ^= array[num2];
            }
        }
    }
}
