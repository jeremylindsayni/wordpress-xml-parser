using System.Linq;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Resolvers;
using System.Collections.Generic;

namespace RssParser
{
    class Program
    {
        private static string pathToExportedXML = @"C:\Users\Jeremy\Desktop\jeremylindsay.wordpress.2017-08-06.post_type-post.status-publish.001.xml";

        private static string pathToGeneratedHTML = @"C:\users\jeremy\Desktop\html.txt";

        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings
            {
                DtdProcessing = DtdProcessing.Parse,

                XmlResolver = new XmlPreloadedResolver(XmlKnownDtds.Rss091)
            };

            var f = File.ReadAllText(pathToExportedXML);

            XmlReader reader = XmlReader.Create(new StringReader(f), settings);

            XDocument document = XDocument.Load(reader);

            XNamespace nsContent = "http://purl.org/rss/1.0/modules/content/";

            var posts = from item in document.Descendants("item")
                        where item.Element("title").Value != "About"
                        select new
                        {
                            Title = item.Element("title").Value,
                            Published = DateTime.Parse(item.Element("pubDate").Value),
                            PublishedText = item.Element("pubDate").Value,
                            Url = item.Element("link").Value,
                            Category = item.Elements("category"),
                            Content = item.Element(nsContent + "encoded").Value
                        };

            string templateString = "";

            int year = 0;
            int tmpYear = 0;
            string month = "";
            string tmpMonth = "";
            List<string> x;

            foreach (var c in posts.OrderByDescending(m => m.Published))
            {
                if (c.Content != string.Empty)
                {
                    year = c.Published.Year;
                    month = c.Published.ToString("MMMM");

                    x = new List<string>();

                    var cats = c.Category;
                    foreach (var cat in cats)
                    {
                        x.Add(cat.Value);
                    }

                    var catString = string.Join(", ", x);

                    if (year != tmpYear)
                    {
                        templateString += "<h2>" + year + "</h2>" + Environment.NewLine + Environment.NewLine;
                    }

                    if (month != tmpMonth)
                    {
                        templateString += "<h3>" + month + " " + year + "</h3>" + Environment.NewLine + Environment.NewLine;
                    }

                    templateString += $"<a href='{c.Url}'>{c.Title}</a>" + Environment.NewLine;
                    templateString += $"(<em>{catString}</em>)" + Environment.NewLine + Environment.NewLine;

                    tmpMonth = month;
                    tmpYear = year;
                }
            }

            File.WriteAllText(pathToGeneratedHTML, templateString);
        }
    }
}
