using HtmlSerializer;
using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;

static HtmlElement BuildHtmlTree(List<string> htmlLines)
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

    if (!string.IsNullOrEmpty(element.Id))
    {
        Console.Write($" id=\"{element.Id}\"");
    }

    if (element.Classes.Any())
    {
        Console.Write($" class=\"{string.Join(" ", element.Classes)}\"");
    }

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


var html = await Load("https://learn.malkabruk.co.il");
var cleanHtml = new Regex("\\s+").Replace(html," ");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0).ToList();
var htmlTree = BuildHtmlTree(htmlLines);


Console.WriteLine("HTML Tree construction completed.");
string s = "div#profile-menu";
Selector selector = Selector.FromQueryString(s);
//List <HtmlElement> list = htmlTree.FindElementsBySelector(selector).ToList();
var all = htmlTree.FindElementsBySelector(selector);
Console.WriteLine("match element: " + all.ToList().Count());
foreach (var element in all)
{
    PrintHtmlElement(element);
}



