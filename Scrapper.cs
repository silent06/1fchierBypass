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
using System.Threading;
namespace HashHandler
{
    class Scrapper
    {
        public static void startDownloads(string proxy, string port, string link)
        {

            //client.DownloadProgressChanged += Utils.WebClientDownloadProgressChanged;
            WebProxy proxylink = new WebProxy(proxy, Convert.ToInt32(port));

            // test links
            //https://1fichier.com/?5ciygoc5waex29nsf2ft
            // https://www.mediafire.com/file/gge73e3yd3buln8/VPS_install_files_local_only.zip/file

            if (link.ToString() != "False")
            {

                try
                {
                    List<string> names = new List<string> { link, proxy, port };

                    using (StreamWriter writer = new StreamWriter("linkinfo.txt"))
                    {
                        foreach (string name in names)
                        {
                            writer.WriteLine(name);
                        }
                        writer.Close();
                    }

                    Process process = new Process();
                    process.StartInfo.FileName = "Download.exe";
                    //process.StartInfo.Arguments = arguments;
                    //process.StartInfo.Arguments = $"{savePath}";

                    // Start the process
                    process.Start();

                    Thread.Sleep(5000);
                    process.WaitForExit();

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

            }
        }

        public static async void startScraperAsync(string proxy, string port, string randomString)
        {

            if (Utils.links.Count > 0) {

                Random random = new Random();
                int randomIndex = random.Next(0, Utils.links.Count);
                randomString = Utils.links[randomIndex];
                Utils.Linkdone = false;
                var mfdirect = await Utils.Scrape1FchiersiteAsync(randomString, proxy, port);

                if (mfdirect.ToString() != "False")
                {

                    if (!Utils.CompletedScrapedList.Contains(mfdirect.ToString()))
                    {

                        if (Utils.Linkdone == true) {

                            Utils.AppendText(string.Concat(new object[] { $"Adding to CompletedScrapedList: {mfdirect.ToString()}" }), ConsoleColor.Green);
                            Utils.CompletedScrapedList.Add(mfdirect.ToString());

                            string filePath = $"files\\Completedlist_{randomIndex}.txt";

                            using (StreamWriter writer = new StreamWriter(filePath))
                            {
                                foreach (string item in Utils.CompletedScrapedList)
                                {
                                    writer.WriteLine(item);
                                }
                            }

                            //string website = Utils.LoadRandomLine($"files\\Completedlist_{randomIndex}.txt");
                            startDownloads(proxy, port, mfdirect);
                        }

                    }
                }

            }

        }
    }
}
