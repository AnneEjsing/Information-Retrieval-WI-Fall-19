using System.IO;

namespace WebCrawler
{
    public class FileStorer : IPageStore
    {
        public void SavePage(Page page)
        {
            string[] lines = { $"url: {page.Url}", $"sitetext: {page.SiteText}", $"title: {page.Title}", $"pagerank: {page.PageRank}" };
            string path = "urls.txt";

            using (StreamWriter sw = File.AppendText(path))
            {
                foreach (var line in lines)
                    sw.WriteLine(line);
                sw.WriteLine("\n");
            }
        }
    }
}