using System;
using System.Collections.Generic;

namespace WebCrawler
{
    public class Page
    {
        public Uri Url;
        public string Title = "";
        public string SiteText = "";
        public IEnumerable<Uri> OutLinks;
        public int NumberOfOutLinks;
        public string Html;
        public double PageRank = 0;
        public Page(Uri url)
        {
            Url = url;
        }

        public override bool Equals(object obj)
        {
            if(!(obj is Page))
                return false;
            Page other = (Page)obj;
            return other.Url == Url;
        }

        public override int GetHashCode()
        {
            return Url.GetHashCode();
        }
    }
}