using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Indexer
{
    public static class FeatureConstructor
    {
        public static List<string> RemoveAndStem(List<string> tokens)
        {
            // We don't know the language of the page
            // We don't think it's a big deal removing a few words
            // that might not actually be a stopword in the relevant language
            tokens.RemoveAll(x => StopWords.danishStopWords.Contains(x));
            tokens.RemoveAll(x => StopWords.englishStopWords.Contains(x));
            tokens = Stemmer.DoStemmer(tokens.ToArray());
            return tokens;
        }
    }
}
