using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DeckGenerator.IO;
using DeckGenerator.Scraper;

namespace DeckGenerator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Not enough arguments! Provide target path.");
                return;
            }
            Console.WriteLine("Getting URLs...");
            string[] urls = await AmericanActressesScraper.GetActressesUrlsAsync(10);
            Console.WriteLine("Downloading data...");
            var actressesData = await AmericanActressesScraper.GetActressesDataAsync(urls);
            List<string> imageUrls = new List<string>();
            for (int i = 0; i < actressesData.Count; i++)
            {
                imageUrls.Add("https:" + actressesData[i].Item2);
            }
            Console.WriteLine("Downloading images...");
            DeckIO.DownloadImages(imageUrls.ToArray());
            string targetPath = args[0];
            DeckGenerator deckGenerator = new DeckGenerator(targetPath,
                DeckIO.TempImageFolder, actressesData);
            Console.WriteLine("Generating deck...");
            deckGenerator.Generate();
            Console.WriteLine($"Done! Deck was saved at {deckGenerator.FinalFilePath}");
        }
    }
}
