using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace OpinionAnalyzer.DataLoader.Finam
{
    /// <summary>
    /// Класс ищет ссылки на статьи за конкретный день    
    /// <td class="ntitle bdotline">
    /// <a class="author" href="/analysis/author0057000002/">Маликов Алексей, ИК "ТРЕЙД-ПОРТАЛ"</a>
    /// <a href="/analysis/marketnews/na-povestke-dnya-rossiiyskogo-fondovogo-rynka-vstaet-vopros-testirovaniya-urovneiy-1400-1380-punktov-po-indeksu-mmvb-20140919-1915/" class="f-fake-url">
    /// <span class="f-fake-url__t">
    /// На повестке дня российского фондового рынка встает вопрос тестирования
    /// </span>
    /// <span class="f-fake-url__u">
    /// уровней 1400-1380 пунктов по индексу ММВБ
    /// </span>
    /// </a>
    /// </td>
    /// </summary>
    public class ArticleListItemExtractor
    {
        private const string regexAuthorPart = @"class=[""]author.*?\>(?<author>.+?)\<\/a\>";
        private const string regexUlrPart = @"\<a(\s+)href=[""](?<link>.+?)[""]";

        private Regex _regex;
        private readonly string _content;
        
        public ArticleListItemExtractor(string content)
        {
            _content = content;

            _content = _content.Replace("\r\n", "");
            _content = _content.Replace("\n", "");
            _content = Regex.Replace(_content, @"\<img.+?\>", "");

            _regex = new Regex(regexAuthorPart + "\\W*" + regexUlrPart, RegexOptions.Singleline | RegexOptions.Compiled);
        }

        public IEnumerable<ArticleListItem> GetItems()
        {
            var matches = _regex.Matches(_content);

            foreach (Match match in matches)
            {
                var author = match.Groups["author"].Value;
                var authorAndCompany = author.Split(new string[] {","}, StringSplitOptions.None);

                var company = String.Empty;
 
                if (authorAndCompany.Length == 2)
                {
                    author = authorAndCompany[0];
                    company = authorAndCompany[1];

                    company = company.Replace("\"", "");
                    company = company.Replace("&quot;", "");                    
                }
                
                var url = match.Groups["link"].Value;

                yield return new ArticleListItem(){Author = author, Company = company, Url = url};
            }
        }
    }
}