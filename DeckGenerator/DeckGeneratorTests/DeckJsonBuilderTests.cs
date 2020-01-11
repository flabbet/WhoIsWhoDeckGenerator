using DeckGenerator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DeckGeneratorTests
{
    public class DeckJsonBuilderTests
    {
        [Test]
        public void TestThatBuildsCorrectJson()
        {
            List<CardItem> items = new List<CardItem>();
            items.Add(new CardItem("answer1", "image.jpg", "Hey it's me, test"));
            items.Add(new CardItem("answer2", "image4.jpg", "Hey it's me, test"));
            items.Add(new CardItem("answer3", "image.jpg", "Hey it's me, test"));
            items.Add(new CardItem("answer4", "image.jpg", "Hey it's me, test"));
            string json = DeckJsonBuilder.CreateDeckJson(items);
            System.Console.WriteLine(json);
            Assert.True(IsValidJson(json));
        }

        private bool IsValidJson(string strInput)
        {
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}