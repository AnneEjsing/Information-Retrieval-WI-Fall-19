using System;
using System.Collections.Generic;

namespace WebCrawler
{
    public interface IParser
    {
        void AddHtmlToPage(Page page);
        void AddBodyToPage(Page page);
        void AddPathsToPage(Page page);
    }
}