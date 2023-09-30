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
    class Download
    {
        static TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
        public static void Downloadfiles(string proxy, string port, int Attempts)
        {

            try
            {
                Utils.savePath = "";
                Utils.fileName = "";
                string saveDirectory = Application.StartupPath + "\\";
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(Utils.linkinfo[0]);
                Utils.fileName = fileNameWithoutExtension;
                Utils.savePath = saveDirectory + Utils.fileName + ".rar";
                Utils.AppendText(string.Concat(new object[] { $"Downloading File: {Utils.fileName.ToString()} from {Utils.linkinfo[0]} Attempt -> {Attempts}" }), ConsoleColor.Green);
                //var proxyParts = randomString.Split(':');
                WebProxy proxylink = new WebProxy(proxy, Convert.ToInt32(port));
                Utils.AppendText(string.Concat(new object[] { $"Using Proxy: {proxy} with port: {port} Attempt -> {Attempts}" }), ConsoleColor.Green);
                //Utils.AppendText(string.Concat(new object[] { $"Save path: {Utils.savePath}" }), ConsoleColor.Green);
                Utils.client = new Utils.WebClientWithTimeout();
                Utils.client.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)");
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Utils.client.Proxy = proxylink;
                Utils.client.DownloadFileAsync(new Uri(Utils.linkinfo[0]), Utils.savePath);
                Utils.client.DownloadProgressChanged += Utils.Client_DownloadProgressChanged;
                Utils.client.DownloadDataCompleted += Utils.Client_DownloadFileCompleted;

            }
            catch
            {
                //Utils.Attempts = false;
                //Console.WriteLine($"Proxy failed! Re-attempting..");

            }


        }


    }
}
