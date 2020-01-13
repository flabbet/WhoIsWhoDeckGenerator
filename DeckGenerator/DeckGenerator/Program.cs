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
            if (args.Length < 2)
            {
                Console.WriteLine(@"Not enough arguments! Provide path and cards amount. Ex. E:Documents\AmericanActresses\ 1000");
                return;
            }

            DataScraper[] scrapers = {
                new AmericanActressesScraper(18),
                new AmericanActressesScraper(19),
                new AmericanActressesScraper(20),
                new AmericanActressesScraper(21)
            };
            
            List<CardItem> data = new List<CardItem>();
            int targetRecordsCount = Convert.ToInt32(args[1]);
            int urlsLeft = targetRecordsCount;
            int totalUrlCount = 0;
            for (int i = 0; i < scrapers.Length; i++)
            {
                List<string> urls = new List<string>();
                totalUrlCount += urls.Count;
                urls.Clear();
                if (totalUrlCount >= targetRecordsCount) break;
                Console.WriteLine($"Getting URLs from {i + 1}/{scrapers.Length} available sources...");
                urls.AddRange(await scrapers[i].GetUrlsAsync(urlsLeft));
                urlsLeft -= urls.Count;
                Console.WriteLine($"Downloading data from {urls.Count} URLs...");
                data.AddRange(await scrapers[i].GetDataAsync(urls.ToArray()));
            }
            List<string> imageUrls = new List<string>();
            for (int i = 0; i < data.Count; i++)
            {
                imageUrls.Add("https:" + data[i].ImageFileName);
            }
            Console.WriteLine($"Downloading {imageUrls.Count} images... \n Estimated size equals {imageUrls.Count * 0.017}MB");
            DeckIO.DownloadImages(imageUrls.ToArray());
            string targetPath = args[0];
            DeckGenerator deckGenerator = new DeckGenerator(targetPath,
                DeckIO.TempImageFolder, data);
            Console.WriteLine("Generating deck...");
            deckGenerator.Generate();
            Console.WriteLine($"Done! Deck was saved at {deckGenerator.FinalFilePath}");
        }
    }
}
