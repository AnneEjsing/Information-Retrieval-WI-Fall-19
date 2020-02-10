using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace WebCrawler
{
    public class BackQueue : ConcurrentQueue<Uri>
    {
        public List<String> Disallows = new List<String>();
        public String Domain {get; private set;}
        DateTime lastVisited;

        public BackQueue(String domain)
        {
            Domain = domain;
            lastVisited = DateTime.Now;
            extractDisallows();
        }

        public bool RobotsAreObeyed(Uri url)
        {
            String host = url.GetLeftPart(UriPartial.Authority);
            String pathAndQuery = url.PathAndQuery;
            return Disallows.Contains(pathAndQuery) == false;
        }

        public bool EnoughTimeHasPassed(String host, DateTime currentTimeStamp)
        {
            return lastVisited.AddSeconds(1) <= currentTimeStamp;
        }

        private void extractDisallows()
        {
            String robotsText = "";
            try{
                robotsText = new WebClient().DownloadString(Domain + "/robots.txt");
            }
            catch(WebException)
            {
                return;
            }
            Regex regexObj = new Regex("(?:^User-agent: (?<UserAgent>.*?)$)|(?<Permission>^(?:Allow)|(?:Disallow)): (?<Url>.*?)$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match matchResults = regexObj.Match(robotsText);
            String currentUserAgent = "";
            
            while (matchResults.Success) {
                if(matchResults.Value.Contains("User-agent"))
                    currentUserAgent = matchResults.Value.Split(": ")[1];

                if(currentUserAgent == "*" && matchResults.Value.Contains("Disallow"))
                    Disallows.Add(matchResults.Value.Split(": ")[1]);

                matchResults = matchResults.NextMatch();
            }
        }
    }
}