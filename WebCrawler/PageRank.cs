using System;
using System.Collections.Generic;
using System.Linq;

namespace WebCrawler
{
    public class PageRank
    {
        HashSet<Page> _webGraph;
        int _numberOfPages;
        double _decay;
        int _convergence;
        public PageRank(double decay = 0.8, int convergence = 10)
        {
            _decay = decay;
            _convergence = convergence;
        }

        public void ComputePageRank(HashSet<Page> webGraph, int numberOfPages) 
        {
            _webGraph = webGraph;
            _numberOfPages = numberOfPages;
            Dictionary<Page, double> oldRanks = initialiseRanks(1/(double)_numberOfPages);
            Dictionary<Page, double> newRanks = new Dictionary<Page, double>();
            bool done = false;
            int i = 0;

            while(!done)
            {
                newRanks = initialiseRanks(0);
                foreach(Page p in _webGraph)
                {
                    //System.Console.WriteLine(p.PageRank);
                    newRanks[p] = updatePageRank(oldRanks, p);
                }
                
                if (_convergence == i++)
                    done = true;
                
                foreach(var kvp in newRanks)
                    oldRanks[kvp.Key] = newRanks[kvp.Key];
            }

            foreach(Page p in webGraph)
            {
                p.PageRank = newRanks[p];
                System.Console.WriteLine(p.Url.ToString() + " " + p.PageRank);
            }
        }


        private double updatePageRank(Dictionary<Page, double> oldRanks, Page p)
        {
            double rank = 0.0;
            if (p.NumberOfOutLinks == 0)
                rank = _decay * (oldRanks[p] / (double)_numberOfPages);

            else
            {
                foreach (Uri u in p.OutLinks)
                {
                    if(_webGraph.Where(x => x.Url == u).Any())
                    {
                        Page pp = _webGraph.First(x => x.Url == u);
                        rank += _decay * (oldRanks[pp] / pp.NumberOfOutLinks);
                    }
                }
            }
            rank += (double)(1 - _decay) / (double)_numberOfPages;
            return rank;
        }

        private Dictionary<Page, double> initialiseRanks(double initialRank)
        {
            Dictionary<Page, double> ranks = new Dictionary<Page, double>();
            foreach(Page p in _webGraph)
                ranks.Add(p, initialRank);
            
            return ranks;
        }
    }
}
