using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace DeckGenerator.IO
{
    public class DeckIO
    {

        public const string TempFolderName = @"deckImages";
        public static string TempImageFolder => Path.Join(Path.GetTempPath(), TempFolderName);

        public static void DownloadImages(string[] urls)
        {
            if (!Directory.Exists(TempImageFolder))
            {
                Directory.CreateDirectory(TempImageFolder);
            }
            for (int i = 0; i < urls.Length; i++)
            {
                Console.WriteLine($"Downloading image {i + 1}/{urls.Length}");
                var url = urls[i];
                SaveImage(urls[i], Path.Join(TempImageFolder, $"image{i}.jpg"), ImageFormat.Jpeg);
            }
        }

        private static void SaveImage(string url, string filename, ImageFormat format)
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] data = webClient.DownloadData(url);

                using (MemoryStream mem = new MemoryStream(data))
                {
                    using (var image = Image.FromStream(mem))
                    {
                        image.Save(filename, format);
                    }
                }

            }
        }
    }
}
