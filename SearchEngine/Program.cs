using System;
using System.Collections.Generic;

namespace SearchEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            IInvertedIndex index = new IndexPreprocesser().CreateIndex(IndexType.ContentBased);
            var result = index.Search("aalborg");
            foreach(Page p in result)
                System.Console.WriteLine(p.Url);

        }
    }
}
