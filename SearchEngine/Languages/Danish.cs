using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Annytab.Stemmer;

namespace SearchEngine
{
    public class Danish : ILanguageBehaviour
    {
        public IEnumerable<String> StopWords { get; private set; }
        private String _language = "Danish";

        public Danish()
        {
            StopWords = generateStopWords();
        }

        public IEnumerable<String> Stem(IEnumerable<String> tokensToStem)
        {
            Stemmer stemmer = new DanishStemmer();
            return stemmer.GetSteamWords(tokensToStem.ToArray());
        }

        private IEnumerable<String> generateStopWords()
        {
            var lines = File.ReadLines($"Languages/StopWords/{_language}StopWords.txt");
            foreach (var line in lines)
                yield return line;
        }
    }
}
