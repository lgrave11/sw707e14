using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MiniProject2
{
    public static class Tokenizer
    {
        #region negation_string
        static string negation_string = @"(?:" +
                            @"^(?:never|no|nothing|nowhere|noone|none|not|havent|hasnt|hadnt|cant|couldnt|shouldnt|wont|wouldnt|dont|doesnt|didnt|isnt|arent|aint" +
                        @")$" +
                   @")|" +
                   @"n't";
        #endregion
        static string punctuation_string = @"^[.:;!?]$";
        #region emoticon_string
        static string emoticon_string = @"(?:" +
                                      @"[<>]?" +
                                      @"[:;=8]" +
                                      @"[\-o\*\']?" +
                                      @"[\)\]\(\[dDpP/\:\}\{@\|\\]" +
                                      @"|" +
                                      @"[\)\]\(\[dDpP/\:\}\{@\|\\]" +
                                      @"[\-o\*\']?" +
                                      @"[:;=8]" +
                                      @"[<>]?" +
                                    @")";
        #endregion
        #region regex_strings
        static List<string> regex_strings = new List<string> {
        // Phone numbers:
        @"(?:" +
          @"(?:" +
            @"\+?[01]" +
            @"[\-\s.]*" +
          @")?" +  
          @"(?:" +
            @"[\(]?" +
            @"\d{3}" +
            @"[\-\s.\)]*" +
          @")?" +
          @"\d{3}" +          
          @"[\-\s.]*" +  
          @"\d{4}" +          
        @")",
        // Emoticons:
        emoticon_string,    
        // HTML tags:
        @"<[^>]+>",
        // Twitter username:
        @"(?:@[\w_]+)",
        // Twitter hashtags:
        @"(?:\#+[\w_]+[\w\'_\-]*[\w_]+)",
        // Remaining word types:
        @"(?:[a-z][a-z'\-_]+[a-z])"+
        @"|"+
        @"(?:[+\-]?\d+[,/.:-]\d+[+\-]?)"+
        @"|"+
        @"(?:[\w_]+)"+                    
        @"|"+
        @"(?:\.(?:\s*\.){1,})"+
        @"|"+
        @"(?:\S)"
        };
        #endregion
        public static Regex re_strings = new Regex(String.Format(@"({0})", string.Join("|", regex_strings)), RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex re_emoticons = new Regex(emoticon_string, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex re_negation = new Regex(negation_string, RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public static Regex re_punctuation = new Regex(punctuation_string, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static List<string> tokenize(string s)
        {
            s = HttpUtility.HtmlDecode(s);
            MatchCollection matches = re_strings.Matches(s);
            List<string> words = new List<string>();
            foreach (Match match in matches)
            {
                foreach (Capture capture in match.Captures)
                {
                    words.Add(capture.Value);
                }
            }

            List<string> words_fixed = new List<string>();
            foreach (string w in words)
            {
                if (re_emoticons.IsMatch(w))
                {
                    words_fixed.Add(w);
                }
                else
                {
                    words_fixed.Add(w.ToLower());
                }
            }

            bool add_neg = false;
            List<string> words_fixed2 = new List<string>();
            foreach (string w in words_fixed)
            {
                if (add_neg && !re_punctuation.IsMatch(w) && !re_emoticons.IsMatch(w))
                {
                    words_fixed2.Add(w + "_NEG");
                }
                else
                {
                    words_fixed2.Add(w);
                }
                if (re_negation.IsMatch(w))
                {
                    add_neg = true;
                }
                if (re_punctuation.IsMatch(w) || re_emoticons.IsMatch(w))
                {
                    add_neg = false;
                }
            }


            return words_fixed2;

        }
    }
}