using System;
using System.Collections.Generic;
using Annytab.Stemmer;

namespace SearchEngine
{
    public interface ILanguageBehaviour
    {
        IEnumerable<String> Stem(IEnumerable<String> tokensToStem);
        IEnumerable<String> StopWords {get ; }
    }
}