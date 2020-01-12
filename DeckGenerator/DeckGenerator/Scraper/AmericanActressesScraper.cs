using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DeckGenerator.Scraper
{
    public class AmericanActressesScraper
    {
        public const string BaseUrl = @"https://en.wikipedia.org";
        public static string Url = @$"{BaseUrl}/wiki/Category:20th-century_American_actresses";
        public const string ActressXpath = @"//div[@class='mw-category-group']/ul/li/a";
        public const string NextPageXpath = @"//div[@id='mw-pages']/a";
        public const string ImgXpath = @"//table[@class='infobox biography vcard']/tbody/tr/td/a/img";
        public const string NameXpath = @"//div[@class='fn']";
        public static HttpClient Client = new HttpClient();


        public static async Task<string[]> GetActressesUrlsAsync(int actressesCount)
        {
            var html = new HtmlDocument();
            string url = Url;
            List<string> actressesUrls = new List<string>();

            while (actressesUrls.Count < actressesCount)
            {
                var doc = await Client.GetStringAsync(url);
                html.LoadHtml(doc);
                url = BaseUrl +"/wiki/" + html.DocumentNode.SelectNodes(NextPageXpath).First(x => x.InnerHtml == "next page")
                    .GetAttributeValue("href", "");
                var actressesNodes = html.DocumentNode.SelectNodes(ActressXpath);
                actressesUrls.AddRange(GetActressesUrlsFromNodes(actressesNodes, actressesCount - actressesUrls.Count));
            }

            return actressesUrls.ToArray();
        }

        public static List<string> GetActressesUrlsFromNodes(HtmlNodeCollection nodes, int urlsLeft)
        {
            List<string> urls = new List<string>();
            foreach (var node in nodes)
            {
                if (urls.Count >= urlsLeft) break;
                urls.Add(BaseUrl + node.GetAttributeValue("href", ""));
            }

            return urls;
        }

        public static async Task<List<Tuple<string, string>>> GetActressesDataAsync(string[] urls)
        {
            var html = new HtmlDocument();
            List<Tuple<string, string>> data = new List<Tuple<string, string>>();

            foreach (var url in urls)
            {
                var rawDoc = await Client.GetStringAsync(url);
                html.LoadHtml(rawDoc);
                string name = html.DocumentNode.SelectSingleNode(NameXpath)?.InnerHtml;
                string imgUrl = html.DocumentNode.SelectSingleNode(ImgXpath)?.GetAttributeValue("src", "Image not found");
                if(name == null || imgUrl == null) continue;
                data.Add(new Tuple<string, string>(name, imgUrl));
            }

            return data;
        }
    }
}
