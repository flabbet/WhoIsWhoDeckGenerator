using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using DeckGenerator.IO;

namespace DeckGenerator
{
    public class DeckGenerator
    {

        public string PathToImages;
        public string TargetPath;
        public string TargetImagesPath => Path.Join(TargetPath, "images");
        public string FinalFilePath = "";
        public List<CardItem> CardsData;

        public DeckGenerator(string targetPath, string pathToImages, List<CardItem> cardsData)
        {
            PathToImages = pathToImages;
            TargetPath = targetPath;
            CardsData = cardsData;
        }

        public void Generate()
        {
            Console.WriteLine("Generating folder structure...");
            GenerateFolderStructure();
            Console.WriteLine("Building deck json...");
            string json = DeckJsonBuilder.CreateDeckJson(GenerateCardItems());
            Console.WriteLine("Saving data.json at target path...");
            File.WriteAllText(Path.Join(TargetPath, "data.json"), json);
            Console.WriteLine("Moving images to target directory...");
            MoveImagesToTargetDirectory();
            Console.WriteLine("Zipping deck...");
            ZipDeck();
            Console.WriteLine("Clearing files...");
            DeleteDirectory(TargetPath);
        }

        private void GenerateFolderStructure()
        {
            Directory.CreateDirectory(TargetPath);
            Directory.CreateDirectory(TargetImagesPath);
        }

        private List<CardItem> GenerateCardItems()
        {
            List<CardItem> cardItems = new List<CardItem>();

            for (var i = 0; i < CardsData.Count; i++)
            {
                var cardData = CardsData[i];
                cardItems.Add(new CardItem(cardData.Answer, $"image{i}.jpg", cardData.Description));
            }

            return cardItems;
        }

        private void MoveImagesToTargetDirectory()
        {
            if (Directory.Exists(DeckIO.TempImageFolder))
            {
                if (Directory.GetFiles(TargetImagesPath).Length > 0)
                {
                    RemoveImagesFromDirectory();
                }
                string[] filesPath = Directory.GetFiles(DeckIO.TempImageFolder);
                for (var i = 0; i < filesPath.Length; i++)
                {
                    var filePath = filesPath[i];
                    File.Move(filePath, Path.Join(TargetImagesPath, $"image{i}.jpg"));
                }
            }
            else
            {
                Console.WriteLine($"Path to downloaded images doesn't exist. {DeckIO.TempImageFolder}");
            }
        }

        private void RemoveImagesFromDirectory()
        {
            string[] filePaths = Directory.GetFiles(TargetImagesPath);
            foreach (var filePath in filePaths)
            {
                File.Delete(filePath);
            }
        }


        private void ZipDeck()
        {
            string directoryName = Path.GetDirectoryName(TargetPath);
            string targetFileName = $"{directoryName}.deck";
            if (File.Exists(targetFileName))
            {
                File.Delete(targetFileName);
            }
            ZipFile.CreateFromDirectory(TargetPath,targetFileName, CompressionLevel.Optimal, true);
            FinalFilePath = targetFileName;
        }

        public void DeleteDirectory(string targetDir)
        {
            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(targetDir, false);
        }
    }
}
