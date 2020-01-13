using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DeckGenerator.Scraper
{
    public abstract class DataScraper
    {
        public abstract Task<string[]> GetUrlsAsync(int urlsCount);
        public abstract List<string> GetUrlsFromNodes(HtmlNodeCollection nodes, int urlsLeft);
        public abstract Task<List<CardItem>> GetDataAsync(string[] urls);
    }
}
