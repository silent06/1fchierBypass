using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace HashHandler
{
    class Bypass
    {

        public static async  Task startAsync() {

            // Open the file in truncate mode
            using (StreamWriter writer = new StreamWriter("Completedlist.txt", false))
            {
                // Truncate the file, removing all existing content
                writer.BaseStream.SetLength(0);
            }
            using (StreamWriter writer = new StreamWriter("proxy-list.txt", false))
            {
                // Truncate the file, removing all existing content
                writer.BaseStream.SetLength(0);
            }
            string command = "downloadProxies.bat";
            Utils.AppendText(string.Concat(new object[] { "Downloading latest public ProxyList!" }), ConsoleColor.Green);
            ShellHelper.Shell(command);
            Utils.AppendText(string.Concat(new object[] { "Loading scrapperList!" }), ConsoleColor.Green);
            Utils.links = Utils.LoadLinksFromFile("scraplist.txt");
            Utils.AppendText(string.Concat(new object[] { "Loading passwordList!" }), ConsoleColor.Green);
            Utils.passwords = Utils.LoadScrapedListFromFile("passwordlist.txt");
            Utils.links2 = Utils.LoadScrapedListFromFile("scraplist.txt");
            Utils.CompletedScrapedList = Utils.LoadScrapedListFromFile("linkschecked.txt");
            List<string> proxyList = Utils.LoadProxiesFromFile("proxy-list.txt"); // Get the list of proxies
            Utils.AppendText(string.Concat(new object[] { "Checking proxies! Please wait..." }), ConsoleColor.Green);
            //var validProxies = await Utils.CheckProxiesAsync(proxyList);

            for (int i2 = 0; i2 < Utils.links2.Count; i2++) {

                //string proxy = validProxies[i2];
                //var proxyParts = proxy.Split(':');
                //string website = Utils.LoadRandomLine($"files\\Completedlist_{Utils.links2.Count}.txt");  
                //Scrapper.startScraperAsync(proxyParts[0], proxyParts[1], Utils.links[i2]);
                //Scrapper.startDownloads(proxyParts[0], proxyParts[1], website);
                //int proxyPorti = int.Parse(proxyParts[1]);
                //Console.WriteLine($"Using proxy: {proxy}");
                //var validProxies = await Utils.CheckProxiesAsync(proxyList);
                await Utils.CheckProxiesAsync(proxyList);
            }

            Console.WriteLine("Done! Press any key to exit... ");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
