using System;
using System.Collections.Generic;
using System.Text;

namespace DeckGenerator
{
    public struct CardItem
    {
        public string Answer { get; set; }
        public string ImageFileName { get; set; }
        public string Description { get; set; }

        public CardItem(string answer, string imageFileName, string description)
        {
            Answer = answer;
            ImageFileName = imageFileName;
            Description = description;
        }
    }
}
