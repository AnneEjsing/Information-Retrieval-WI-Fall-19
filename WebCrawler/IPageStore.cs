using System;
using System.Collections.Generic;

namespace WebCrawler
{
    public interface IPageStore
    {
        void SavePage(Page page);   
    }
}