# Search-Engine-WI-Fall-19
This repository contains a web crawler and a search engine created in the information retrieval part of the web intelligence (Software, MSc.) course at AAU.

<h2>Web Crawling</h2>

**The principle of the web crawler**

The web crawler is a multi-threaded crawler built using the Mercator scheme used for the URL frontier, which means it implements frontqueues as well as back queues. The front queues manage prioritization and the back queues enforce politeness. Each backqueue is FIFO. A visual representation of the mercator scheme is seen on the figure below.

<img src="MercatorScheme.png" width="300">

The general algorithm that the web crawler implements is given below.

1. Begin with initial set of URLs in queue/frontier – “the seed”
2. Fetch next page from URL in queue
3. Parse page
    1. Extract text and pass to indexer
    2. Check if URL has content already seen. If not:
        *  Add to index
        *  Extract “link-to” URLs and add to frontier queue
4. For each extracted URL
    1. Normalize URL
    2. Check that it passes certain URL filter tests. E.g.:
        * Focused crawl: only crawl .dk
        * Obey robots.txt (freshness caveat)
    3. Check that not already in frontier
    4. Add to frontier if passing tests
5. Delete or re-prioritize current URL from queue

**Politeness**

In terms of politness of a web crawler we are concerned with explicit and implicit politeness.

* Explicit politeness: specifications from webmasters on what portions of site can be crawled (robots.txt)
* Implicit politeness: avoid hitting any site too often!

My webcrawler implements a simple robots.txt parser, which ensures that we do not crawl pages we are not authorised to. The robots.txt is fetched right before the page is retrived, and not when committing to the frontier. This ensures freshness meaning, that the robots.txt is up-to-date. In terms of implicit politeness, we ensure that only one connection is open to a host at any time. Also, we guarantee a few secs. waiting time between successive requests to a host

**PageRank**

The computation of pageranks is a preprocessing step that is query independent, therefore it is placed with the webcrawler. The PageRank algorithm outputs a probability distribution representing the likelihood that a person randomly clicking on links will arrive at any particular page. The intuition of the pagerank algorithm is thus to perform a random walk on the crawled web pages. This means, we start on a random page, and at each step continue to the next page along one of the links on the current page, equiprobably. This is run to convergence. The PageRank of a page is then the fraction of steps the surfer spends at the page in the limit. This score is between 0 and 1.  The rank (prestige) of a web-page is then proportional to:
* the proportion of random web-surfers that will be visiting the page at a given point in time
* the probability that a random web-surfer is at this page at any point in time

The implementation of pagerank is currently a little wonky, as it will return -infinite scores in some cases.