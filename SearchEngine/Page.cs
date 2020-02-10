using System;
using System.Collections.Generic;

namespace SearchEngine
{
    public class Page
    {
        public int Id { get; set; }
        public Uri Url { get; set; }
        public String SiteText { get; set; }
        public String Title { get; set; }
        public double PageRank { get; set; }
        public IEnumerable<string> Tokens { get; set; }

        public override bool Equals(object obj)
        {
            if (!(obj is Page))
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