﻿using HtmlSerializer;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

static HtmlElement Serialize(List<string> htmlLines)
{
    var root = new HtmlElement();
    var currentElement = root;

    foreach (var line in htmlLines)
    {
        var firstWord = line.Split(' ')[0];

        if (firstWord == "/html")
        {
            break; // Reached end of HTML
        }
        else if (firstWord.StartsWith("/"))
        {
            if (currentElement.Parent != null) // Make sure there is a valid parent
            {
                currentElement = currentElement.Parent; // Go to previous level in the tree
            }
        }
        else if (HtmlHelper.Instance.HtmlTags.Contains(firstWord))
        {
            var newElement = new HtmlElement();
            newElement.Name = firstWord;

            // Handle attributes
            var restOfString = line.Remove(0, firstWord.Length);
            var attributes = Regex.Matches(restOfString, "([a-zA-Z]+)=\\\"([^\\\"]*)\\\"")
                .Cast<Match>()
                .Select(m => $"{m.Groups[1].Value}=\"{m.Groups[2].Value}\"")
                .ToList();

            if (attributes.Any(attr => attr.StartsWith("class")))
            {
                // Handle class attribute
                var classAttr = attributes.First(attr => attr.StartsWith("class"));
                var classes = classAttr.Split('=')[1].Trim('"').Split(' ');
                newElement.Classes.AddRange(classes);
            }

            newElement.Attributes.AddRange(attributes);

            // Handle ID
            var idAttribute = attributes.FirstOrDefault(attr => attr.StartsWith("id"));
            if (!string.IsNullOrEmpty(idAttribute))
            {
                newElement.Id = idAttribute.Split('=')[1].Trim('"');
            }

            newElement.Parent = currentElement;
            currentElement.Children.Add(newElement);

            // Check if self-closing tag
            if (line.EndsWith("/") || HtmlHelper.Instance.HtmlVoidTags.Contains(firstWord))
            {
                currentElement = newElement.Parent;
            }
            else
            {
                currentElement = newElement;
            }
        }
        else
        {
            // Text content
            currentElement.InnerHtml = line;
        }
    }

    return root;
}
static void PrintHtmlTree(HtmlElement element, string indent = "")
{
    Console.Write($"{indent}<{element.Name}");

    //if (!string.IsNullOrEmpty(element.Id))
    //{
    //    Console.Write($" id=\"{element.Id}\"");
    //}

    //if (element.Classes.Any())
    //{
    //    Console.Write($" class=\"{string.Join(" ", element.Classes)}\"");
    //}

    //if (element.Attributes.Any())
    //{
    //    Console.Write($" {string.Join(" ", element.Attributes)}");
    //}


    //Console.WriteLine($"{indent}</{element.Name}>");

    Console.WriteLine(">");

    if (element.Children.Any())
    {
        foreach (var child in element.Children)
        {
            PrintHtmlTree(child, indent + "  ");
        }
    }
    //else if (!string.IsNullOrEmpty(element.InnerHtml))
    //{
    //    Console.WriteLine($"{indent}  {element.InnerHtml}");
    //}

    //Console.WriteLine($"{indent}</{element.Name}>");
}
static void PrintHtmlElement(HtmlElement element, string indent = "")
{
    Console.Write($"{indent}<{element.Name}");
    if (element.Attributes.Any())
    {
        Console.Write($" {string.Join(" ", element.Attributes)}");
    }
    Console.WriteLine($"{indent}</{element.Name}>");
}
static async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

//loading an html page:
var html = await Load("https://forum.netfree.link/category/1/%D7%94%D7%9B%D7%A8%D7%96%D7%95%D7%AA");
var cleanHtml = new Regex("\\s+").Replace(html, " ");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0).ToList();

//Build the tree:
var htmlTree = Serialize(htmlLines);

//queryStrings:
string queryString1 = "a i.fa";//45 results
string queryString2 = "nav#menu.slideout-menu";//only 1 result
string queryString3 = "div .category";//only 1 result

//selector:
var selector = Selector.FromQueryString(queryString2);
var elementsList = htmlTree.FindElementsBySelector(selector);

//Print the elements:
Console.WriteLine("List of " + elementsList.ToList().Count() + " elements found !!");
foreach (var element in elementsList)
{
    Console.WriteLine("My ancestors are:");
    foreach (var father in element.Ancestors().ToList())
    {
        Console.Write("  " + father.Name);
    }
    Console.WriteLine();
    PrintHtmlElement(element);
    Console.WriteLine("--------------------------------------------------------");
}



