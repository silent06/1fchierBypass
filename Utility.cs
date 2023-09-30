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
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;
using HtmlAgilityPack;
using System.Net.Http;
using System.Xml.XPath;
using EO.WebBrowser.DOM;
using Newtonsoft.Json;
using System.Threading;

namespace HashHandler
{

    class CopyDownloader
    {
        public string RemoteFileUrl { get; set; }
        public string LocalFileName { get; set; }
        WebClient webClient = new WebClient();



        public CopyDownloader()
        {
            webClient.DownloadFileCompleted += WebClientOnDownloadFileCompleted;
        }

        public void StartDownload()
        {
            var tempFileName = Path.GetTempFileName();
            webClient.DownloadFile(RemoteFileUrl, tempFileName);
        }

        private void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            string tempFileName = asyncCompletedEventArgs.UserState as string;
            File.Copy(tempFileName, GetUniqueFileName());
        }

        private string GetUniqueFileName()
        {
            throw new NotImplementedException();
        }

        private string GetUniqueFilename(string str)
        {
            // Create an unused filename based on your original local filename or the remote filename
            return str;
        }
    }

    class Utils
    {
        internal static bool Linkdone;
        internal static int proxyPort;
        internal static int GoodProxyCount = 0;
        internal static int BadProxyCount = 0;
        internal static int linkCount;
        internal static string RandomProxyV;
        internal static string RandomProxyA;
        public static List<string> GoodProxies;
        public static List<string> links;
        public static List<string> passwords;
        public static List<string> links2;
        public static List<string> CompletedScrapedList;
        public static List<int> Goodports;
        public static CancellationTokenSource cts = new CancellationTokenSource();
        public static string ToHex(int value)
        {
            return String.Format("0x{0:X}", value);
        }

        public static bool PingHost(string hostUri, int portNumber)
        {
            try
            {
                using (var client = new TcpClient(hostUri, portNumber))
                {

                    Console.Write("Proxy Server Online!\n");
                    return true;
                }

            }
            catch (SocketException ex)
            {

                Console.Write("Error pinging host: {0}\n", ex.Message );
                return false;
            }
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException ex)
            {
                // Discard PingExceptions and return false;
                Console.Write("Ping ERROR: {0}\n", ex.Message);
                return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        public static bool WhatsMyip() {

            string RefURL = "https://www.whatismyip.com/";
            string myProxyIP = "119.81.197.124"; //check this is still available
            int myPort = 3128;
            string userId = string.Empty; //leave it blank
            string password = string.Empty;
            try
            {
                HtmlWeb web = new HtmlWeb();
                var doc = web.Load(RefURL.ToString(), myProxyIP, myPort, userId, password);
                Console.WriteLine(doc.DocumentNode.InnerHtml);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        public static string ProxyRandomDownload()
        {

            try
            {
                start:
                string GetProxy = new WebClient().DownloadString("http://pubproxy.com/api/proxy?type=socks5&country=US,CA&https=true");
                var doc = JsonDocument.Parse(GetProxy);
                var Proxy = doc.RootElement.GetProperty("data")[0].GetProperty("ipPort").ToString();
                var RandomProxy = doc.RootElement.GetProperty("data")[0].GetProperty("ip").ToString();
                var Port = doc.RootElement.GetProperty("data")[0].GetProperty("port").ToString();
                Console.Write("Checking Proxy: http://{0}\n", Proxy);               
                bool checkedProxy = PingHost(RandomProxy, Convert.ToInt32(Port));
                //PingHost(Proxy);
                if (checkedProxy)
                {

                    RandomProxyA = $"http://" + Proxy;
                    RandomProxyV =  RandomProxy;
                    proxyPort = Convert.ToInt32(Port);
                    Console.Write("Random Proxy Being Used: {0}\n", RandomProxy);
                    Console.Write("Port: {0}\n", Port);
                }
                else {
                    Console.Write("Proxy Ping failed..restarting...\n");
                    goto start;
                }


            }
            catch (Exception ex)
            {
                Console.Write("Unable to Fetch Random Proxy ERROR: {0}\n", ex.Message);
            }

            return "";
        }

        private static int GetNextIndexOf(char c, string source, int start)
        {
            if (start < 0 || start > source.Length - 1)
            {
                throw new ArgumentOutOfRangeException();
            }
            for (int i = start; i < source.Length; i++)
            {
                if (source[i] == c)
                {
                    return i;
                }
            }
            return -1;
        }

        public static List<string> LoadLinksFromFile(string filePath)
        {
            List<string> Links = new List<string>();

            // Read the file line by line and add each line (proxy) to the list
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Links.Add(line);
                    linkCount++;
                }
            }

            return Links;
        }

        public static string LoadRandomLine(string filepath) {

            string[] lines = File.ReadAllLines(filepath);

            Random random = new Random();
            int randomIndex = random.Next(0, lines.Length);

            string randomLine = lines[randomIndex];

            Console.WriteLine(randomLine);
            return randomLine;
        }


        public static List<string> LoadScrapedListFromFile(string filePath)
        {
            List<string> Links = new List<string>();

            // Read the file line by line and add each line (proxy) to the list
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Links.Add(line);

                }
            }

            return Links;
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

        public static async Task TestProxyListAsync() {

            string[] proxyList = File.ReadAllLines("proxy-list.txt");

            foreach (string proxy in proxyList)
            {

                string[] proxyParts = proxy.Split(':');

                string proxyAddress = proxyParts[0];
                int proxyPort = int.Parse(proxyParts[1]);

                HttpClientHandler handler = new HttpClientHandler
                {
                    Proxy = new WebProxy(proxyAddress, proxyPort),
                    UseProxy = true,
                };

                using (HttpClient client = new HttpClient(handler))
                {
                    try
                    {
                        HttpResponseMessage response = await client.GetAsync("https://www.example.com");
                        Console.WriteLine("Proxy Status: " + response.StatusCode);
                        if (response.IsSuccessStatusCode)
                        {
                            //Console.WriteLine($"Proxy {proxyAddress} is working.");
                            GoodProxyCount = + 1;
                            
                            GoodProxies.Add(proxyAddress);
                            Goodports.Add(proxyPort);
                        }
                        else
                        {
                            //Console.WriteLine($"Proxy {proxyAddress} is not working.");
                            BadProxyCount = +1;

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error occurred while testing proxy {proxyAddress}: {ex.Message}");
                        BadProxyCount = +1;

                    }
                }
            }
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
                    GoodProxyCount++;
                    return true;
                }
            }
            catch (Exception)
            {
                // Proxy is invalid or not working    
                //Utils.AppendText(string.Concat(new object[] { $"Proxy {proxyAddress} is not working." }), ConsoleColor.Red);
                BadProxyCount++;
                return false;
            }
        }
        static TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
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
                                string random = "";
                                Scrapper.startScraperAsync(proxyParts[0], proxyParts[1], random);
                                //int proxyPorti = int.Parse(proxyParts[1]);
                                //GoodProxies.Add(proxyParts[0]);
                                //Goodports.Add(proxyPorti);
                            }
                        }
                    }, cts.Token));
                }
            }

            await Task.WhenAll(tasks);
            Utils.AppendText(string.Concat(new object[] { "Down Checking proxies!" }), ConsoleColor.Green);
            return validProxies;
        }

        public static async Task MultithreadProxyListAsync()
        {

            // Example code to check proxies using multithreading
            List<string> proxyList = LoadProxiesFromFile("proxy-list.txt"); // Get the list of proxies

            List<Task<bool>> tasks = new List<Task<bool>>();
            foreach (string proxy in proxyList)
            {
                //tasks.Add(Task.Run(() => CheckProxy(proxy)));
            }

            await Task.WhenAll(tasks);

            //List<string> workingProxies = new List<string>();
            for (int i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].Result)
                {
                    //workingProxies.Add(proxyList[i]);
                    string[] proxyParts = proxyList[i].Split(':');

                    string proxyAddress = proxyParts[0];
                    int proxyPort = int.Parse(proxyParts[1]);

                    GoodProxies.Add(proxyAddress);
                    Goodports.Add(proxyPort);
                }
            }

        }


        public static void startCMD(WebClient webclient, string downloadUrl, string savePath, string proxy, string port) {


            //webclient.DownloadFileAsync(new Uri(downloadUrl), savePath);

            //string[] arguments = Environment.GetCommandLineArgs(proxy, port, downloadUrl, savePath);


        }

        public static void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            int progressPercentage = e.ProgressPercentage;
            string progressBar = new string('#', progressPercentage / 10) + new string('-', 10 - progressPercentage / 10);
            Console.WriteLine($"Progress: {progressBar} {progressPercentage}%");

        }

        public static string scrape1Fchiersite(string site)
        {
            var url = site; // Replace with the URL of the website you want to scrape
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            Console.WriteLine($"Scrapping website: {url}");
            HtmlNodeCollection Links = doc.DocumentNode.SelectNodes("//a[contains(@href, 'https://o-7.1fichier')]");
            string direct = "";
            foreach (HtmlNode link in Links)
            {
                string href = link.GetAttributeValue("href", "");
                // Process the link as needed
                direct = href;
            }

            Console.WriteLine($"Dynamic website: {direct}");
            return direct;
        }

        public static string GetFinalRedirect(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            int maxRedirCount = 8;  // prevent infinite loops
            string newUrl = url;
            do
            {
                HttpWebRequest req = null;
                HttpWebResponse resp = null;
                try
                {
                    req = (HttpWebRequest)HttpWebRequest.Create(url);
                    req.Method = "HEAD";
                    req.AllowAutoRedirect = false;
                    resp = (HttpWebResponse)req.GetResponse();
                    switch (resp.StatusCode)
                    {
                        case HttpStatusCode.OK:
                            return newUrl;
                        case HttpStatusCode.Redirect:
                        case HttpStatusCode.MovedPermanently:
                        case HttpStatusCode.RedirectKeepVerb:
                        case HttpStatusCode.RedirectMethod:
                            newUrl = resp.Headers["Location"];
                            if (newUrl == null)
                                return url;

                            if (newUrl.IndexOf("://", System.StringComparison.Ordinal) == -1)
                            {
                                // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                                Uri u = new Uri(new Uri(url), newUrl);
                                newUrl = u.ToString();
                            }
                            break;
                        default:
                            return newUrl;
                    }
                    url = newUrl;
                }
                catch (WebException)
                {
                    // Return the last known good URL
                    return newUrl;
                }
                catch (Exception ex)
                {
                    return null;
                }
                finally
                {
                    if (resp != null)
                        resp.Close();
                }
            } while (maxRedirCount-- > 0);

            return newUrl;
        }

        public static List<string> GetLinks()
        {
            var links = new List<string>();
            HtmlDocument doc = new HtmlDocument();
            doc.Load("YourHtmlFileInHere");
            foreach (HtmlNode link in doc.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute attribute = link.Attributes["href"];
                if (attribute != null)
                {
                    links.Add(attribute.Value);
                }
            }
            return links;
        }

        public static async Task DumpRequestToTxtFile(HttpResponseMessage response, string filePath) {

            // Retrieve the request headers
            var headers = response.RequestMessage.Headers;

            // Retrieve the request body
            var body = await response.Content.ReadAsStringAsync();

            StringBuilder sb = new StringBuilder();

            // Append the headers to the string
            foreach (var header in headers)
            {
                sb.AppendLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            // Append a separator
            sb.AppendLine("--------------------");

            // Append the body to the string
            sb.AppendLine(body);
            File.WriteAllText(filePath, sb.ToString());
        }

        public static async Task DumpRequestToFile(HttpResponseMessage request, string filePath)
        {
            // Create a new instance of StreamWriter to write the request dump to a file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write the request headers to the file
                await writer.WriteLineAsync("Request Headers:");
                foreach (var header in request.Headers)
                {
                    await writer.WriteLineAsync($"{header.Key}: {string.Join(", ", header.Value)}");
                }

                // Write a new line to separate the headers from the body
                await writer.WriteLineAsync();

                // Write the request body to the file
                await writer.WriteLineAsync("Request Body:");
                await writer.WriteLineAsync(await request.Content.ReadAsStringAsync());
            }
        }

        public static string GetFilenameFromContentDisposition(string headerValue)
        {
            string[] parts = headerValue.Split(';');
            foreach (string part in parts)
            {
                if (part.Trim().StartsWith("filename="))
                {
                    return part.Substring("filename=".Length).Trim('"');
                }
            }
            return null;
        }

        public static string GetspecificLine(string value) {

            string filePath = $"responseheaders\\response_{Utils.GoodProxyCount-1}.txt";
            string searchString = value;
            string foundString = null;

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(searchString))
                    {
                        foundString = line;
                        break;
                    }
                }
            }

            if (foundString != null)
            {
                Console.WriteLine("URL Found string: " + foundString);
                return foundString;
            }
            else
            {
                Console.WriteLine("URL String not found.");
                return null;
            }

            return null;
        }

        public static async Task<string> Scrape1FchiersiteAsync(string site, string proxyServer, string port)
        {
            var url = site; // Replace with the URL of the website you want to scrape
            string userId = string.Empty; //leave it blank
            string password = string.Empty;
            Utils.AppendText(string.Concat(new object[] { $"Using  Proxy: {proxyServer} with port Number: {port}" }), ConsoleColor.Green);
            try
            {

                Utils.AppendText(string.Concat(new object[] { $"Checking download link: {url}" }), ConsoleColor.Green);
                WebProxy proxy = new WebProxy(proxyServer, Convert.ToInt32(port));

                var handler = new HttpClientHandler
                {
                    Proxy = proxy,
                    UseProxy = true
                };
                using (HttpClient client = new HttpClient(handler))
                {

                    client.DefaultRequestHeaders.Referrer = new Uri(site);
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("curl/8.0.1"); 
                    string noSSL = "on";
                    /*Post Request*/
                    var data = "{\"dl_no_ssl\":" + $"\"{noSSL}\"" + "}";
                    //var json2 = JsonSerializer.Serialize(data);
                    var payload = new { key1 = data };
                    string jsonPayload = JsonConvert.SerializeObject(payload);
                    var content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
                    Utils.AppendText(string.Concat(new object[] { $"Scrapping website: {url}" }), ConsoleColor.Green);
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Utils.AppendText(string.Concat(new object[] { $"Proxy Status: {response.StatusCode}" }), ConsoleColor.Green);
                    // Continue with parsing the response body to extract the ID

                    string direct = "";

                    HtmlDocument htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(responseBody);
                    HtmlNodeCollection fichierlinkpass = htmlDoc.DocumentNode.SelectNodes($"//*[@id='pass']");

                    /*Check for password*/
                    if (fichierlinkpass != null) {

                        Utils.AppendText(string.Concat(new object[] { $"Password Found! Requesting download link using password." }), ConsoleColor.Green);
                        string passwordA = Utils.passwords[0];
                        string payloadf = $"\"dl_no_ssl=on&pass={passwordA}\"";
                        string POST = $"start curl -s -X POST -d {payloadf} -D responseheaders/response_{Utils.GoodProxyCount}.txt -x http://{proxyServer}:{port} --location {url} \nexit";
                        Utils.AppendText(string.Concat(new object[] { $"Sending CURL request for PW: {POST}" }), ConsoleColor.Green);
                        ShellHelper.Shell(POST);
                        string link = "Location:";
                        string workinglink = GetspecificLine(link);
                        if (workinglink != null) {
                            Utils.Linkdone = true;
                            tcs.SetResult(true);
                            File.Delete($"responseheaders\\response_{Utils.GoodProxyCount-1}.txt");
                            return workinglink;
                        }
                        else
                        {
                            //File.Delete($"responseheaders\\response_{Utils.GoodProxyCount-1}.txt");
                            Utils.Linkdone = true;
                            tcs.SetResult(true);
                            return "False";
                        }

                    }
                    //htmlDoc2.DocumentNode.SelectNodes("//a[contains(@href, 'https://a-34.1fichier.com')]") : 
                    HtmlNodeCollection fichierlink = htmlDoc.DocumentNode.SelectNodes("//a[contains(@href, 'https://o-7.1fichier.com')]");

                    await DumpRequestToTxtFile(response, "RequestHeaders.html");

                    if (fichierlink != null)
                    {
                        foreach (HtmlNode link in fichierlink)
                        {
                            string href = link.GetAttributeValue("href", "");
                            // Process the link as needed
                            direct = href;
                        }
                        Utils.links.Remove(url);
                        Utils.AppendText(string.Concat(new object[] { $"Dynamic website: {direct}" }), ConsoleColor.Green);
                        Utils.Linkdone = true;

                        tcs.SetResult(true);

                        return direct;

                    }
                    else {

                        Utils.AppendText(string.Concat(new object[] { $"failed to find fichierlink" }), ConsoleColor.Red);
                    }
                    //Utils.linksbeingchecked.Remove(site);
                    return "False";
                }                

            }
            catch (Exception ex)
            {
                Utils.AppendText(string.Concat(new object[] { ex.Message }), ConsoleColor.Red);
                Utils.AppendText(string.Concat(new object[] { $"Failed to download File with proxy!" }), ConsoleColor.Red);
                //Utils.linksbeingchecked.Remove(site);
                return "False";
            }

        }


        public static string scrapeMediaFiresite(string site, string proxyServer, string port) {
            var url = site; // Replace with the URL of the website you want to scrape
            string userId = string.Empty; //leave it blank
            string password = string.Empty;
            //ProxyRandomDownload(); 
            //Console.WriteLine($"Using  Proxy: {proxyServer} with port Number: {port}");
            Utils.AppendText(string.Concat(new object[] { $"Using  Proxy: {proxyServer} with port Number: {port}" }), ConsoleColor.Green);
            try {

                Utils.AppendText(string.Concat(new object[] { $"Checking download link: {url}" }), ConsoleColor.Green);
                WebProxy proxy = new WebProxy(proxyServer, Convert.ToInt32(port));
                ServicePointManager.ServerCertificateValidationCallback += (send, certificate, chain, sslPolicyErrors) => { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | SecurityProtocolType.Ssl3;
                HtmlWeb web = new HtmlWeb();
                web.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.9.2) Gecko/20100115 Firefox/3.6";
                web.Timeout = 5000;
                HtmlDocument doc = web.Load(url, "GET", proxy, null);               
                doc.OptionReadEncoding = false;
                Utils.AppendText(string.Concat(new object[] { $"Proxy Status: {web.StatusCode}" }), ConsoleColor.Green);
                if (doc != null)
                {
                    // Access properties and methods of the HtmlDocument object here
                    //HtmlDocument doc = web.Load(url);
                    Utils.AppendText(string.Concat(new object[] { $"Scrapping website: {url}" }), ConsoleColor.Green);
                    HtmlNodeCollection mediafireLinks = doc.DocumentNode.SelectNodes("//a[contains(@href, 'https://download')]");
                    string direct = "";
                    foreach (HtmlNode link in mediafireLinks)
                    {
                        string href = link.GetAttributeValue("href", "");
                        // Process the link as needed
                        direct = href;
                    }

                    //Console.WriteLine($"Dynamic website: {direct}");
                    Utils.AppendText(string.Concat(new object[] { $"Dynamic website: {direct}" }), ConsoleColor.Green);
                    return direct;
                }

                return "";

            }
            catch (Exception ex)
            {
                Utils.AppendText(string.Concat(new object[] { ex.Message }), ConsoleColor.Red);
                Utils.AppendText(string.Concat(new object[] { $"Failed to download File with proxy!" }), ConsoleColor.Red);
                return "False";
            }

        }

        public static string scrapeSiteFindSubStrings(string site)
        {
            var url = site; // Replace with the URL of the website you want to scrape
            var web = new HtmlWeb();
            var doc = web.Load(url);

            Console.WriteLine($"Scrapping website: {url}");
            var links = doc.DocumentNode.Descendants("a")
                            .Select(a => a.GetAttributeValue("href", ""))
                            .Where(href => href.Contains("sub-string"))
                            .ToList();
            string direct = "";
            foreach (var link in links)
            {
                Console.WriteLine(link);
            }
            Console.WriteLine("Dynamic URL String {0}", direct);
            return direct;
        }

        public static string scrapeSite(string site) {

            var url = site; // Replace with the URL of the website you want to scrape
            var web = new HtmlWeb();
            var doc = web.Load(url);

            Console.WriteLine($"Scrapping website: {url}");
            var link = doc.DocumentNode.SelectSingleNode("//a[@href='download']");
            //var link = doc.DocumentNode.Descendants("a").FirstOrDefault(a => a.GetAttributeValue("href", "") == "your-link");
            string direct = "";
            if (link != null)
            {
                var href = link.GetAttributeValue("href", "");
                var innerText = link.InnerText;
                Console.WriteLine($"Link: {href}");
                Console.WriteLine($"Inner Text: {innerText}");
                direct = link.ToString();
            }
            Console.WriteLine("Dynamic URL String {0}", direct);
            return direct;
        }

        public static string Mediafire(string download)
        {
            //ProxyRandomDownload();
            Console.Write("RandomProxyA: {0}\n", RandomProxyA);
            Console.Write("Proxy port: {0}\n", proxyPort);

            /*var proxy = new WebProxy
            {
                Address = new Uri(RandomProxyA),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
            };*/

            /*string original = RandomProxyV.ToString();
            if (!original.StartsWith("http:"))
                original = "http://" + original;
                
            Uri proxyUri = new UriBuilder
            {
                Scheme = Uri.UriSchemeHttp,
                Host = RandomProxyV,
                Port = proxyPort
            }.Uri;*/

            /*if (!Uri.TryCreate(original, UriKind.Absolute, out proxyUri))
            {
                Console.Write("URI Proxy Build failed!\n");
                //Bad bad bad!
            }
            else {


            }*/


            //Console.Write("Proxy URI: {0}\n", proxyUri);
            //WebProxy myproxy = new WebProxy(RandomProxyV, proxyPort);

            HttpWebRequest req;
            HttpWebResponse res;
            string str = "";
            req = (HttpWebRequest)WebRequest.Create(download);
            //req.Proxy = myproxy;
            //req.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            res = (HttpWebResponse)req.GetResponse();
            str = new StreamReader(res.GetResponseStream()).ReadToEnd();        
            int indexurl = str.IndexOf("https://o-7.1fichier");//o-7.1fichier https://download
            int indexend = GetNextIndexOf('"', str, indexurl);
            string direct = str.Substring(indexurl, indexend - indexurl);
            Console.WriteLine("Dynamic URL String {0}", direct);
            return direct;
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


    }
}
