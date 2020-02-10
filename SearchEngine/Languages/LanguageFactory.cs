using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchEngine
{
    public class LanguageFactory
    {
        Danish danish;
        English english;
        German german;
        public LanguageFactory()
        {
            danish = new Danish();
            english = new English();
            german = new German();

        }
        public ILanguageBehaviour GetLanguage(Uri uri)
        {
            List<String> splitDomain = uri.GetLeftPart(UriPartial.Authority).Split('.').ToList();
            String tld = splitDomain.Last();

            switch (tld)
            {
                case "dk": return danish;
                case "uk": return english;
                case "com": return english;
                case "de": return german;
                default: return english;
            }
        }
    }
}