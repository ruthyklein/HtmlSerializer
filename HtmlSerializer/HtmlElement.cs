using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
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
            var currentElement = this.Parent;

            while (currentElement != null)
            {
                yield return currentElement;
                currentElement = currentElement.Parent;
            }
        }

        public static IEnumerable<HtmlElement> FindElementsBySelector(HtmlElement element, Selector selector)
        {
            var results = new HashSet<HtmlElement>();
            FindElementsBySelectorRecursive(element, selector, results);
            return results;
        }

        private static void FindElementsBySelectorRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> results)
        {
            if (!results.Contains(element) && element.MatchesSelector(selector))
            {
                results.Add(element);
            }

            foreach (var child in element.Children)
            {
                FindElementsBySelectorRecursive(child, selector, results);
            }
        }

        private bool MatchesSelector(Selector selector)
        {
            return 
            (string.IsNullOrEmpty(selector.TagName) || this.Name == selector.TagName) &&
            (string.IsNullOrEmpty(selector.Id) || this.Id == selector.Id) &&
            (selector.Classes.All(cls => this.Classes.Contains(cls)));
        }
    }


}


