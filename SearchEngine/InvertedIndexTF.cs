using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SearchEngine
{
    public class InvertedIndexTF : IInvertedIndex
    {
        // Maps a term to relevant values being tf, idf, and tf-idf.
        private ConcurrentDictionary<string, TermValues> _index { get ; set ; }

        private Dictionary<Page, double> _vectorLengths;
        int _allPagesCount;

        public InvertedIndexTF(int allPagesCount)
        {
            _index = new ConcurrentDictionary<string, TermValues>();
            _vectorLengths = new Dictionary<Page, double>();
            _allPagesCount = allPagesCount;
        }


        public void AddDocumentToIndex(Page page)
        {
            var tokens = page.Tokens;
            if (page is null)
                throw new ArgumentNullException("The page cannot be null");

            foreach(string token in tokens)
            {
                if (!_index.ContainsKey(token))
                    _index.TryAdd(token, new TermValues(_allPagesCount));
                
                if (!_index[token].PageToTermFrequency.ContainsKey(page))
                    _index[token].PageToTermFrequency.Add(page, 0);
                    
                _index[token].PageToTermFrequency[page]++;
            }
        }

        // Initialised the index by precomputing the lengths of each vector in the index
        public void InitialiseIndex()
        {
            Dictionary<Page, double> lengthVector = new Dictionary<Page, double>();

            foreach(var (term, indexValue) in _index)
            {
                foreach(var (page, _) in indexValue.PageToTermFrequency)
                {
                    if (!lengthVector.ContainsKey(page))
                        lengthVector.Add(page, 0);
                        
                    lengthVector[page] += indexValue.GetTfidf(page);
                }
            }

            foreach(var (page, length) in lengthVector)
                _vectorLengths[page] = Math.Sqrt(length);
        }

        // Implements Cosine similarity to do content-based searching.
        public IEnumerable<Page> Search(String query)
        {
            Dictionary<Page, double> Scores = new Dictionary<Page,double>();

            foreach(String term in query.Split(" "))
            {
                if(!_index.ContainsKey(term))
                    continue;

                var indexValue = _index[term];

                // Iterate over all pages, that contain the term
                foreach(Page page in indexValue.PageToTermFrequency.Keys)
                {
                    if(!Scores.ContainsKey(page))
                        Scores.Add(page, 0);
                    
                    Scores[page] += indexValue.GetTfidf(page);
                    //TODO: Weight with respect to pagerank.
                }
            }

            foreach(Page p in Scores.Keys.ToList())
                Scores[p] /= _vectorLengths[p];
                
            return Scores.OrderByDescending(x => x.Value).Select(x => x.Key).Take(10);
        }
    }
}