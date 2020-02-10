using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SearchEngine
{
    public class PageProcesser
    {
        private IEnumerable<String> _stopWords;
        private Char[] illegalCharacters = new Char[] { '{', '}', '[', ']', '<', '>', '=', '|', '&' };
        private Char[] badCharacters = new Char[] { '\\', '.', '?', '!', ':', ';', ',', '\'', '(', ')' };

        private Char[] _splitCharactors = new Char[] { ' ', '/', '-', '_' };

        public IEnumerable<string> PreprocessPage(String pageToIndex, ILanguageBehaviour language)
        {
            IEnumerable<String> tokens = tokenise(pageToIndex);
            tokens = prettifyTokens(tokens);
            _stopWords = language.StopWords;
            tokens = removeStopWords(tokens);
            tokens = language.Stem(tokens);
            return tokens;
        }

        public IEnumerable<IEnumerable<string>> PreprocessPage(IEnumerable<string> pagesToIndex, ILanguageBehaviour language)
        {
            foreach (String page in pagesToIndex)
            {
                yield return PreprocessPage(page, language);
            }
        }

        private IEnumerable<String> tokenise(String pageToTokenise)
        {
            return pageToTokenise.ToLower().Split(_splitCharactors, StringSplitOptions.RemoveEmptyEntries);
        }

        private IEnumerable<String> prettifyTokens(IEnumerable<String> tokensToPrettify)
        {
            foreach (String token in tokensToPrettify)
            {
                String newToken = token;
                if (!illegalCharacters.Any(token.Contains))
                {
                    foreach(Char chr in badCharacters)
                    {
                        newToken = token.Replace(chr.ToString(), "");
                    }
                    yield return newToken;
                }
            }
        }

        private IEnumerable<String> removeStopWords(IEnumerable<String> tokens)
        {
            return tokens.Where(x => !_stopWords.Contains(x));
        }
    }
}