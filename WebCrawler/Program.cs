using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace WebCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
            Crawler c = new Crawler(new HtmlParser(), new FileStorer());
            c.Crawl();
        }
    }
}
