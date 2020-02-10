using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace SearchEngine
{
    public enum IndexType
    {
        Boolean,
        ContentBased
    }
    public class IndexPreprocesser
    {
        public IInvertedIndex CreateIndex(IndexType kind)
        {
            LanguageFactory _languageFactory = new LanguageFactory();
            PageProcesser processer = new PageProcesser();
            IEnumerable<Page> allPages = fetchPages();
            IInvertedIndex index = null;

            if(kind.Equals(IndexType.Boolean))
                index = new InvertedIndexBoolean();
            else if(kind.Equals(IndexType.ContentBased))
                index = new InvertedIndexTF(allPages.Count());

            //var temp = allPages.Take(100);

            Parallel.ForEach(allPages, p =>
            {
                ILanguageBehaviour language = _languageFactory.GetLanguage(p.Url);
                p.Tokens = processer.PreprocessPage(p.SiteText, language);
                index.AddDocumentToIndex(p);
            });
            
            if(kind.Equals(IndexType.ContentBased))
                index.InitialiseIndex();

            return index;
        }

        private IEnumerable<Page> fetchPages()
        {
            List<Page> pages = new List<Page>();
            string text = File.ReadAllText("../WebCrawler/urls.txt");
            foreach (var p in text.Split("\n\n"))
            {
                Page page = new Page();
                var lines = p.Split("\n");
                foreach (var line in lines)
                {
                    var temp = line.Split(": ");
                    if (temp[0].Contains("url"))
                        page.Url = new Uri(temp[1]);
                    if (temp[0].Contains("sitetext"))
                        page.SiteText = temp[1];
                }
                if(page.Url != null)
                    pages.Add(page);
            }

            return pages;
        }
    }
}