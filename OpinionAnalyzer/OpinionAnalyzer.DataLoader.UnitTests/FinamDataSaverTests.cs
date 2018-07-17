using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.DataLoader.Finam;

namespace OpinionAnalyzer.DataLoader.UnitTests
{
    /// <summary>
    /// Summary description for FinamDataSaverTests
    /// </summary>
    [TestClass]
    public class FinamDataSaverTests
    {     
        [TestMethod]
        public void TestMethod1()
        {
            var fds = new FinamDataSaver(@"C:\");
            var article = new LoadedArticle()
            {
                Author = "Автор",
                Company = "Компания",
                PublishDate = DateTime.Now,
                Title = "Заголовок",
                Content = "Содержание страницы!"
            };

            fds.Save(article);

        }
    }
}
