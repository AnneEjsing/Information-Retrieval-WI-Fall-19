using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SearchEngine
{
    public interface IInvertedIndex
    {
        void AddDocumentToIndex(Page p);
        IEnumerable<Page> Search(string query);
        void InitialiseIndex();
    }
}