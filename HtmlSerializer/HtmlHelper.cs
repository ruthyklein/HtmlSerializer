﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HtmlSerializer
{
    public class HtmlHelper
    {
        private readonly static HtmlHelper _instance = new HtmlHelper();
        public static HtmlHelper Instance => _instance;

        public string[] HtmlTags { get; set; }
        public string[] HtmlVoidTags { get; set; }
        private HtmlHelper()
        {
            HtmlTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("Files/HtmlTags.json"));
            HtmlVoidTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText("Files/HtmlVoidTags.json"));
        }

        //private HtmlHelper()
        //{

        //    HtmlTags = LoadTagsFromFile("Files\\HtmlTags.json");
        //    HtmlVoidTags = LoadTagsFromFile("Files\\HtmlVoidTags.json");

        //}
        //private string[] LoadTagsFromFile(string filePath)
        //{
        //    string jsonContent = File.ReadAllText(filePath);
        //    return JsonSerializer.Deserialize<string[]>(jsonContent);
        //}

    }
}

