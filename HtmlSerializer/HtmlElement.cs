using HtmlSerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HtmlSerializer
{
    public class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }
        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }

        public HtmlElement()
        {
            Classes = new List<string>();
            Attributes = new List<string>();
            Children = new List<HtmlElement>();
        }

        public IEnumerable<HtmlElement> Descendants()
        {
            var queue = new Queue<HtmlElement>();
            queue.Enqueue(this);

            while (queue.Count > 0)
            {
                var currentElement = queue.Dequeue();
                yield return currentElement;

                foreach (var child in currentElement.Children)
                {
                    queue.Enqueue(child);
                }
            }
        }
        public IEnumerable<HtmlElement> Ancestors()
        {
            var currentElement = this;
            while (currentElement != null)
            {
                yield return currentElement;
                currentElement = currentElement.Parent;
            }
        }
        public string ToString2()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append($"<{Name}");
            if (!string.IsNullOrEmpty(Id))
            {
                sb.Append($" id=\"{Id}\"");
            }
            if (Classes.Any())
            {
                sb.Append(" class=\"");
                sb.Append(string.Join(" ", Classes));
                sb.Append("\"");
            }
            if (Attributes.Any())
            {
                sb.Append(" attribute :");

                foreach (var attribute in Attributes)
                {
                    sb.Append($" {attribute}");
                }
            }
            sb.Append(">");
            if (!string.IsNullOrEmpty(InnerHtml))
            {
                sb.Append(InnerHtml);
            }

            sb.Append($"</{Name}>");
            sb.Append("count of children elements: " + Children.Count());

            return sb.ToString();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"name: {Name}");

            // Add classes if they exist
            if (Classes.Any())
            {
                sb.AppendLine($"classes: {string.Join(" ", Classes)}");
            }

            // Add other attributes if they exist
            if (Attributes.Any())
            {
                sb.AppendLine("attributes:");
                foreach (var attribute in Attributes)
                {
                    sb.AppendLine($"- {attribute}");
                }
            }

            // Add inner HTML if it exists
            if (!string.IsNullOrEmpty(InnerHtml))
            {
                sb.AppendLine(InnerHtml);
            }

            //// Add child elements recursively
            //foreach (var child in Children)
            //{
            //    sb.AppendLine(child.ToString());
            //}

            sb.AppendLine($"count of children elements: {Children.Count}");

            return sb.ToString();
        }
    }
}
#region 2
//public static class HtmlElementExtensions
//{
//    public static HashSet<HtmlElement> FindElementsBySelector(this HtmlElement element, Selector selector)
//    {
//        var results = new HashSet<HtmlElement>();
//        FindElementsBySelectorRecursive(element, selector, results);
//        return results;
//    }

//    private static void FindElementsBySelectorRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
//    {
//        var descendants = element.Descendants();

//        foreach (var descendant in descendants)
//        {
//            if (descendant.MatchesSelector(selector))
//            {
//                results.Add(descendant);
//            }
//        }

//        foreach (var child in element.Children)
//        {
//            FindElementsBySelectorRecursive(child, selector, results);
//        }
//    }

//    private static bool MatchesSelector(this HtmlElement element, Selector selector)
//    {
//        return
//            (string.IsNullOrEmpty(selector.TagName) || element.Name == selector.TagName) &&
//            (string.IsNullOrEmpty(selector.Id) || element.Id == selector.Id) &&
//            (selector.Classes.All(cls => element.Classes.Contains(cls)));
//    }

//}

#endregion
public static class HtmlElementExtensions
{
    public static HashSet<HtmlElement> FindElementsBySelector(this HtmlElement element, Selector selector)
    {
        var results = new HashSet<HtmlElement>();
        FindElementsBySelectorRecursive(element, selector, results);
        return results;
    }
    private static void FindElementsBySelectorRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
    {

        // Check if there is a next selector in the sequence
        if (selector.Child != null)
        {
            // Continue recursively on the filtered descendants with the next selector
            var filteredDescendants = element.Descendants().Where(descendant => descendant.MatchesSelector(selector));
            foreach (var filteredDescendant in filteredDescendants)
            {
                FindElementsBySelectorRecursive(filteredDescendant, selector.Child, results);
            }
        }
        else
        {
            //results.Add(element);
            // If the current selector is the last one, add the filtered descendants to the final result
            results.UnionWith(element.Descendants().Where(descendant => descendant.MatchesSelector(selector)));
        }
    }
    private static bool MatchesSelector(this HtmlElement element, Selector selector)
    {
        return
            (string.IsNullOrEmpty(selector.TagName) || element.Name == selector.TagName) &&
            (string.IsNullOrEmpty(selector.Id) || element.Id == selector.Id) &&
            (selector.Classes.All(cls => element.Classes.Contains(cls)));
    }
}
