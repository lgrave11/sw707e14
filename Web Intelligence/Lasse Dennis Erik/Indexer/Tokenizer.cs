using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace Indexer
{
    public static class Tokenizer
    {
        public static List<string> ExtractText(string html)
        {
            string fixedEncoding = Encoding.UTF8.GetString(Encoding.GetEncoding("utf-8").GetBytes(html));
            string fixedHtmlEntities = WebUtility.HtmlDecode(fixedEncoding);
            Regex removeScriptAndCss = new Regex(
               "(<!--(.+?)-->)|(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)",
               RegexOptions.Singleline | RegexOptions.IgnoreCase
            );
            string result = removeScriptAndCss.Replace(fixedHtmlEntities, " ");

            result = Regex.Replace(result, @"<[^>]*>", " ");
            RegexOptions options = RegexOptions.Singleline | RegexOptions.IgnoreCase;
            Regex removeWhitespaceAndPunctuation = new Regex(@"[\s\p{P}]+", options);
            string tmp = removeWhitespaceAndPunctuation.Replace(result, @" ");
            tmp = tmp.Replace("'", "").ToLower().Trim();

            List<string> extractedText = tmp.Split(' ').ToList();
            
            return extractedText;
        }
    }
}
