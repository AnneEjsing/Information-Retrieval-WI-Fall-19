using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SearchEngine
{
    public class InvertedIndexBoolean : IInvertedIndex
    {
        public ConcurrentDictionary<string, HashSet<Page>> index { get ; set ; }

        public InvertedIndexBoolean()
        {
            index = new ConcurrentDictionary<string, HashSet<Page>>();
        }

        public bool Contains(Page page, string searchWord)
        {
            return index[searchWord].Contains(page);
        }

        public IEnumerable<Page> Search(string token)
        {
            if (index.ContainsKey(token))
                return index[token];
            else
                return new HashSet<Page>();
        }

        public void AddDocumentToIndex(Page page)
        {
            var tokens = page.Tokens;
            foreach (string token in tokens)
            {
                if (index.ContainsKey(token))
                    index[token].Add(page);
                else
                    index[token] = new HashSet<Page>() { page };
            }
        }

        public void InitialiseIndex() {}
    }
}