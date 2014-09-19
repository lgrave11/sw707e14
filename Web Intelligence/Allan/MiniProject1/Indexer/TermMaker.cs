﻿using System;
using System.Collections.Generic;

namespace MiniProject1
{
    /// <summary>
    /// Description of Tokenizer
    /// </summary>
    public sealed class TermMaker
    {
        #region singletonpart
        private static TermMaker instance = new TermMaker();

        public static TermMaker Instance
        {
            get
            {
                return instance;
            }
        }

        private TermMaker()
        {
        }

        #endregion


        public List<string> GetTerms(string content)
        {

            content = Crawler.ExtractHtmlInnerText(content);
            List<string> tokenStream = GetTokens(content);
            Stemmer stemmer = new Stemmer();
            return Stemmer.DoStemmer(tokenStream.ToArray());
        }

        List<string> uselessSigns = new List<string>() { "'", ".", "-", ",", ":", ";", "*", "\"", "(", ")", "[", "]", "!", "´" };
        private List<string> GetTokens(string content)
        {
            List<string> returnList = new List<string>();
            foreach (string s in content.Split(' ', '\n', '\t'))
            {
                if (!stopWords.Contains(s))
                {
                    string toAdd = s;
                    uselessSigns.ForEach(x=> toAdd = toAdd.Replace(x, ""));
                    returnList.Add(toAdd.ToLower());
                }
            }

            return returnList;
        }

        #region DenGrimmeListe
        List<string> stopWords = new List<string>(){
            "a","about","above","after","again","against","all","am","an","and","any","are","aren't","as","at","be","because","been","before","being","below","between","both","but","by","can't","cannot","could","couldn't","did","didn't","do","does","doesn't","doing","don't","down","during","each","few","for","from","further","had","hadn't","has","hasn't","have","haven't","having","he","he'd","he'll","he's","her","here","here's","hers","herself","him","himself","his","how","how's","i","i'd","i'll","i'm","i've","if","in","into","is","isn't","it","it's","its","itself","let's","me","more","most","mustn't","my","myself","no","nor","not","of","off","on","once","only","or","other","ought","our","ours	ourselves","out","over","own","same","shan't","she","she'd","she'll","she's","should","shouldn't","so","some","such","than","that","that's","the","their","theirs","them","themselves","then","there","there's","these","they","they'd","they'll","they're","they've","this","those","through","to","too","under","until","up","very","was","wasn't","we","we'd","we'll","we're","we've","were","weren't","what","what's","when","when's","where","where's","which","while","who","who's","whom","why","why's","with","won't","would","wouldn't","you","you'd","you'll","you're","you've","your","yours","yourself","yourselves","af","alle","andet","andre","at","begge","da","de","den","denne","der","deres","det","dette","dig","din","dog","du","ej","eller","en","end","ene","eneste","enhver","et","fem","fire","flere","fleste","for","fordi","forrige	fra","få","før","god","han","hans","har","hendes","her","hun","hvad","hvem","hver","hvilken","hvis","hvor","hvordan","hvorfor","hvornår","i","ikke","ind","ingen","intet","jeg","jeres","kan","kom","kommer","lav","lidt","lille","man	mand","mange","med","meget","men","mens","mere","mig","ned","ni","nogen","noget","ny","nyt","nær","næste","næsten","og","op","otte","over","på","se","seks","ses","som","stor","store","syv","ti","til","to","tre","ud","var"
        };
        #endregion
    }
}
