using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;

namespace WebCrawler
{
    public class Crawler
    {
        ConcurrentDictionary<int, BackQueue> BackQueues = new ConcurrentDictionary<int, BackQueue>();
        HashSet<Page> _webGraph = new HashSet<Page>();
        List<Thread> threads = new List<Thread>();

        int _numberOfPages = 1000;
        int _numberOfThreads = 10;
        int i = -1;
        IParser parser;
        IPageStore _pageSaver;
        List<Uri> initialSeeds = new List<Uri>() {
            new Uri("https://www.aau.dk"),
            new Uri("https://google.com"),
            new Uri("https://www.arnoldbusck.dk"),
            new Uri("https://www.timeout.com/London"),
            new Uri("https://www.overleaf.com/"),
            new Uri("https://nordjyske.dk"),
            new Uri("https://www.skala.fm"),
            new Uri("https://politiken.dk")
        };

        public Crawler(IParser parser, IPageStore pageSaver)
        {
            this.parser = parser;
            this._pageSaver = pageSaver;
        }

        public void Crawl()
        {
            //1. Begin with initial set of URLs in queue/frontier – “the seed”
            initialiseSeed();
            startThreads();

            // Spin lock until the desired number of pages are crawled
            while (_webGraph.Count() < _numberOfPages)
            {
            }
            
            // Makes sure the webgraph is selfcontained before computing page ranks
            foreach(Page pp in _webGraph)
            {
                List<Uri> selfContainedOutLinks = new List<Uri>();
                foreach(Uri u in pp.OutLinks)
                {
                    if(_webGraph.Where(x => x.Url == u).Any())
                        selfContainedOutLinks.Add(u);
                }
                pp.OutLinks = selfContainedOutLinks;
                pp.NumberOfOutLinks = selfContainedOutLinks.Count();
            }
            PageRank p = new PageRank(0.85, 100);
            //p.ComputePageRank(_webGraph, _numberOfPages);
            
            // Save pages til file urls.txt
            savePages();
        }

        private void initialiseSeed()
        {
            foreach (Uri seed in initialSeeds)
            {
                String domain = seed.GetLeftPart(UriPartial.Authority);
                BackQueue backQueue = new BackQueue(domain);
                backQueue.Enqueue(seed);
                BackQueues.TryAdd(Interlocked.Increment(ref i), backQueue);
            }
        }

        private void startThreads()
        {
            for (int i = 0; i < _numberOfThreads; i++)
                new Thread(() => NewMethod()).Start();
        }

        private void NewMethod()
        {
            var random = new Random();
            while (_webGraph.Count() < _numberOfPages)
            {
                BackQueue b = new BackQueue(null); //dummy
                Uri nextUrl = new Uri("https://www.aau.dk"); // dummy

                if (BackQueues.Count() == 0)
                    continue;

                b = BackQueues[random.Next(0, BackQueues.Count)];

                if (b.Count == 0 || !b.TryPeek(out nextUrl))
                    continue;
                if(_webGraph.ToList().Where(x => x.Url == nextUrl).Count() > 0)
                    continue;

                //2. Fetch next page from URL in queue
                Page newPage = fetchNextPage(b, nextUrl);
                if (newPage is null)
                    continue;

                addToWebGraph(newPage);

                //For each extracted URL
                //• Obey robots.txt (freshness caveat)
                //c. Check that not already in frontier
                var paths = newPage.OutLinks.Where(x => b.RobotsAreObeyed(x)
                                        && b.Contains(x) == false);
                //d. Add to frontier if passing tests

                addPathToFrontierIfTestsPassed(b, paths);
            }
            //5. Delete or re-prioritize current URL from queue
        }
        private Page fetchNextPage(BackQueue backQueue, Uri currentUrl)
        {
            String host = currentUrl.GetLeftPart(UriPartial.Authority);
            while (backQueue.EnoughTimeHasPassed(host, DateTime.Now) == false)
                Thread.Yield();

            Page newPage = new Page(currentUrl);

            parser.AddHtmlToPage(newPage);
            if (String.IsNullOrEmpty(newPage.Html))
                return null;

            parser.AddBodyToPage(newPage);
            if (String.IsNullOrEmpty(newPage.SiteText))
                return null;
            
            parser.AddPathsToPage(newPage);
            return newPage;
        }

        private void savePages()
        {
            foreach (Page page in _webGraph)
                _pageSaver.SavePage(page);
        }

        private void addToWebGraph(Page page)
        {
            if (!_webGraph.Contains(page) && _webGraph.Count() < _numberOfPages)
            {
                System.Console.WriteLine(page.Url);
                _webGraph.Add(page);
            }
        }
        private void addPathToFrontierIfTestsPassed(BackQueue b, IEnumerable<Uri> paths)
        {
            foreach (Uri path in paths)
            {
                String pathDomain = path.GetLeftPart(UriPartial.Authority);

                if (pathDomain.Equals(b.Domain))
                    b.Enqueue(path);

                else if (BackQueues.Any(x => x.Value.Domain.Equals(pathDomain)))
                    BackQueues.First(x => x.Value.Domain.Equals(pathDomain)).Value.Enqueue(path);

                else
                {
                    BackQueue newQueue = new BackQueue(pathDomain);
                    newQueue.Enqueue(path);
                    BackQueues.TryAdd(Interlocked.Increment(ref i), newQueue);
                }
            }
        }
    }
}
