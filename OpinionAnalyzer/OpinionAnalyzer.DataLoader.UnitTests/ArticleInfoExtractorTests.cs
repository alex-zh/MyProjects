using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.DataLoader.Finam;

namespace OpinionAnalyzer.DataLoader.UnitTests
{    
    [TestClass]
    public class ArticleInfoExtractorTests
    {        
        [TestMethod]
        public void TestMethod1()
        {
            var extractedInfo = new ArticleInfoExtractor("").Extract();

            Assert.AreEqual("", extractedInfo.Title);
            Assert.AreEqual("", extractedInfo.Content);
            Assert.AreEqual(null, extractedInfo.PublishedDate);            
        }

        [TestMethod]
        public void TestMethod2()
        {
            var extractedInfo = new ArticleInfoExtractor(@"<html>
                                                             <div id='newsitem541884'>
                                                                <h2>Заголовок</h2>
                                                                <div>24.10.2014 14:35</div>
                                                                <div class='handmade mid'>Текст</div>
                                                             </div> 
                                                           </html>").Extract();

            Assert.AreEqual("Заголовок", extractedInfo.Title);
            Assert.AreEqual("Текст", extractedInfo.Content);
            Assert.AreEqual(new DateTime(2014, 10, 24, 14, 35, 0), extractedInfo.PublishedDate.Value);

        }

        [TestMethod]
        public void TestMethod3()
        {
            var content = Articles.Page1;

            var extractedInfo = new ArticleInfoExtractor(content).Extract();
            Assert.AreEqual("Стоимость акций на мировых фондовых рынках близка к справедливой", extractedInfo.Title);
            Assert.AreEqual("Текст комментария!", extractedInfo.Content);
            Assert.AreEqual(new DateTime(2014,10,24,14,35,0), extractedInfo.PublishedDate.Value);

        }
    }
}
