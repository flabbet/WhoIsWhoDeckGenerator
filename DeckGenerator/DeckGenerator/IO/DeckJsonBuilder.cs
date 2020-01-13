using System;
using System.Collections.Generic;
using System.Text;

namespace DeckGenerator
{
    public class DeckJsonBuilder
    {
        public const string CardSchema = @"

    ""name"": ""{0}"",
    ""path"": ""{1}"",
    ""description"": ""{2}""
";

        public static string CreateDeckJson(List<CardItem> items)
        {
            string finalJson = "[";
            for (int i = 0; i < items.Count; i++)
            {
                finalJson += "\n {" + string.Format(CardSchema, items[i].Answer.Replace("\"", "'"),
                    items[i].ImageFileName, items[i].Description) + "\n}" + (i == items.Count - 1 ? "" : ",");
            }
            finalJson += "\n]";
            return finalJson;
        }
        
    }
}
