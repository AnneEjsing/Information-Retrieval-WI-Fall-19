using System;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Linq;

namespace WebCrawler
{
    public class HtmlParser : IParser
    {

        public void AddHtmlToPage(Page page)
        {
            WebClient wc = new WebClient();
            try
            {
                page.Html = wc.DownloadString(page.Url);
            }
            catch (Exception) { page.Html = ""; }
        }

        public void AddBodyToPage(Page page)
        {
            var htmlDoc = new HtmlDocument();
            StringBuilder sb = new StringBuilder();

            try
            {
                htmlDoc.LoadHtml(page.Html);
            }
            catch (Exception) { return; }
            if (htmlDoc is null)
                return;

            var htmlNodes = htmlDoc.DocumentNode.SelectNodes("//body[normalize-space()]");
            if (htmlNodes is null)
                return;

            foreach (var node in htmlNodes)
            {
                sb.Append(node.InnerText);
            }

            page.SiteText = formatInnerText(sb.ToString());

        }

        private string formatInnerText(string innerText)
        {
            innerText = innerText.Replace("\n", " ");
            innerText = innerText.Replace("\t", " ");
            innerText = innerText.Replace("\r", " ");
            Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
            innerText = regex.Replace(innerText, " ");
            return innerText;
        }

        public void AddPathsToPage(Page page)
        {
            // Using regex to parse HTML. I know..
            Match m = Regex.Match(page.Html, "href\\s*=\\s*(?:[\"'](?<1>[^\"']*)[\"']|(?<1>\\S+))", RegexOptions.IgnoreCase);

            List<Uri> ListOfURLs = new List<Uri>();

            while (m.Success)
            {
                string urlChecked = m.Groups[1].ToString().Trim();
                Uri outUri;
                if (Uri.TryCreate(urlChecked, UriKind.Absolute, out outUri) && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps))
                {
                    ListOfURLs.Add(outUri);
                }
                m = m.NextMatch();
            }

            page.OutLinks = ListOfURLs;
        }
    }
}