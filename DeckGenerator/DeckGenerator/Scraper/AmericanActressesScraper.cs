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
    public class AmericanActressesScraper : DataScraper
    {
        public const string BaseUrl = @"https://en.wikipedia.org";
        public string Url;
        public const string ActressXpath = @"//div[@class='mw-category-group']/ul/li/a";
        public const string ImgXpath = @"//table[@class='infobox biography vcard']/tbody/tr/td/a/img";
        public const string NameXpath = @"//div[@class='fn']";

        public int Century { get; set; }

        public AmericanActressesScraper(int century)
        {
            Century = century;
            Url = @$"{BaseUrl}/w/index.php?title=Category:{Century}th-century_American_actresses";
        }

        public override async Task<string[]> GetUrlsAsync(int actressesCount)
        {
            string url = Url;
            List<string> actressesUrls = new List<string>();

            while (actressesUrls.Count < actressesCount)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    var doc = await client.GetStringAsync(url);
                    var html = new HtmlDocument();
                    html.LoadHtml(doc);
                    HtmlNodeCollection actressesNodes = html.DocumentNode.SelectNodes(ActressXpath);
                    actressesUrls.AddRange(GetUrlsFromNodes(actressesNodes,
                        actressesCount - actressesUrls.Count));
                    string[] lastActressName = GetLastTwoPartActressName(actressesUrls.ToArray(), actressesNodes);
                    string name = lastActressName[0]; 
                    string sureName = lastActressName[1];

                    url = BaseUrl +
                          $"/w/index.php?title=Category:{Century}th-century_American_actresses&pagefrom={sureName}+{name}";
                }
                catch (NullReferenceException)
                {
                    break;
                }
            }

            return actressesUrls.ToArray();
        }

        private string[] GetLastTwoPartActressName(string[] actressesUrls, HtmlNodeCollection actressesNodes)
        {
            string[] lastActressName = Array.Empty<string>();
            for (int i = 0; i < actressesUrls.Length; i++)
            {
                if (lastActressName.Length != 2)
                {
                    lastActressName = actressesNodes[actressesNodes.Count - 1 - i].InnerHtml.Split(" ");
                }
            }

            return lastActressName;
        }

        public override List<string> GetUrlsFromNodes(HtmlNodeCollection nodes, int urlsLeft)
        {
            List<string> urls = new List<string>();
            foreach (var node in nodes)
            {
                if (urls.Count >= urlsLeft) break;
                urls.Add(BaseUrl + node.GetAttributeValue("href", ""));
            }

            return urls;
        }

        public override async Task<List<CardItem>> GetDataAsync(string[] urls)
        {
            var html = new HtmlDocument();
            List<CardItem> data = new List<CardItem>();

            for (var i = 0; i < urls.Length; i++)
            {
                var url = urls[i];
                HttpClient client = new HttpClient();
                var rawDoc = await client.GetStringAsync(url);
                html.LoadHtml(rawDoc);
                string name = html.DocumentNode.SelectSingleNode(NameXpath)?.InnerHtml;
                string imgUrl = html.DocumentNode.SelectSingleNode(ImgXpath)
                    ?.GetAttributeValue("src", "Image not found");
                if (name == null || imgUrl == null) continue;
                Console.WriteLine($"{i}/{urls.Length} Getting data for {name}");
                data.Add(new CardItem(name, imgUrl, ""));
            }

            return data;
        }

    }
}
