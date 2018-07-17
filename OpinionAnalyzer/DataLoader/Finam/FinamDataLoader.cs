using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using OpinionAnalyzer.DataLoader.Classes;

namespace OpinionAnalyzer.DataLoader.Finam
{
    public class FinamDataLoader
    {
        public string FinamUrl = "http://www.finam.ru";
        public string ArticleListUrl = "http://www.finam.ru/analysis/nslent";
        /* Url к странице с комментариями:
         http://www.finam.ru/analysis/nslent/rqdate13097DE/  
         * 13-09-7DE = 1*16+3 - 0*16+9 - 7*16^2+13*16+14 = 19/09/2014
         *
         * На станице присудствуют ссылки на конкретный комментарий
         <a href="/analysis/marketnews/rezkogo-razvitiya-korrekcii-po-indeksu-mmvb-ne-poluchilos-seiychas-byki-mogut-perexvatit-iniciativu-20140915-2100/" class="f-fake-url">
          <span class="f-fake-url__t">Резкого развития коррекции по индексу ММВБ не получилось, </span>
          <span class="f-fake-url__u">сейчас "быки" могут перехватить инициативу</span>
         </a>
         *
         * 
         * На каждой странице есть секция
         * #newsitem536869 > div.handmade.mid
         * <div class="handmade mid" >
         */
        private string DailyListOfCommentsUrl = "http://www.finam.ru/analysis/nslent/";

        /// <summary>
        /// Возращаем все статьи на указанную дату
        /// </summary>        
        public IEnumerable<ArticleListItem> GetList(DateTime day)
        {
            var wc = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
            var content =  wc.DownloadString(GetArticleListUrlForDay(day));

            return new ArticleListItemExtractor(content).GetItems();            
        }

        private Uri GetArticleListUrlForDay(DateTime day)
        {
            return new Uri(String.Format("{0}/rqdate{1}", ArticleListUrl, day.GetHexValue()));
        }


        public LoadedArticle LoadArticle(ArticleListItem listItem, DateTime day)
        {
            var url = FinamUrl + listItem.Url;

            var loadedArticle = new LoadedArticle
            {
                Author = listItem.Author,
                Company = listItem.Company,
                Url = url
            };

            var wc = new WebClient { Encoding = Encoding.GetEncoding("windows-1251") };
            var pageContent = wc.DownloadString(url);

            var extractedInfo = new ArticleInfoExtractor(pageContent).Extract();

            loadedArticle.Title = extractedInfo.Title;
            loadedArticle.Content = extractedInfo.Content;
            loadedArticle.PublishDate = extractedInfo.PublishedDate ?? day;

            return loadedArticle;
        }
    }
}
