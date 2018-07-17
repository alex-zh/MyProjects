using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.DataLoader.Classes;
using OpinionAnalyzer.DataLoader.Finam;

namespace OpinionAnalyzer.DataLoader.UnitTests
{
    [TestClass]
    public class ArticleListItemExtractorTests
    {
        [TestMethod]
        public void Test1()
        {
            var content = @"<td class=""ntitle bdotline"">
                            <a class=""author"" href=""/analysis/author005EA00002/"">Шелехов Павел</a>
                            <a href=""/analysis/marketnews36163/"" class=""f-fake-url"">
                            <span class=""f-fake-url__t"">
                            Самый высокий объем на рынке ММВБ прошел по акциям 
                            </span>
                            <span class=""f-fake-url__u"">
                            Газпрома (-3,7%), составив 60% от всего оборота
                            </span>
                            </a>
                            </td>";

            var result = new ArticleListItemExtractor(content).GetItems();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Шелехов Павел", result.First().Author);
            Assert.AreEqual(@"/analysis/marketnews36163/", result.First().Url);
        }

        [TestMethod]
        public void Test2()
        {
            var content = @"
                            <td class==""ntitle bdotline="">
                            <a class=""author="" href==""/analysis/author0057000002/="">Маликов Алексей, ИК =""ТРЕЙД-ПОРТАЛ=""</a>
                            <a href=""/analysis/marketnews/na-povestke-dnya-rossiiyskogo-fondovogo-rynka-vstaet-vopros-testirovaniya-urovneiy-1400-1380-punktov-po-indeksu-mmvb-20140919-1915/"" class==""f-fake-url="">
                            <span class==""f-fake-url__t="">
                            На повестке дня российского фондового рынка встает вопрос тестирования
                            </span>
                            </a>
                            </td>
                            <td class=""ntitle bdotline"">
                            <a class=""author"" href=""/analysis/author005EA00002/"">Шелехов Павел</a>
                            <a href=""/analysis/marketnews36163/"" class=""f-fake-url"">
                            <span class=""f-fake-url__t"">
                            Самый высокий объем на рынке ММВБ прошел по акциям 
                            </span>
                            <span class=""f-fake-url__u"">
                            Газпрома (-3,7%), составив 60% от всего оборота
                            </span>
                            </a>
                            </td>";

            var result = new ArticleListItemExtractor(content).GetItems();

            Assert.AreEqual(2, result.Count());
            
            Assert.AreEqual("Маликов Алексей, ИК =\"ТРЕЙД-ПОРТАЛ=\"", result.First().Author);
            Assert.AreEqual(@"/analysis/marketnews/na-povestke-dnya-rossiiyskogo-fondovogo-rynka-vstaet-vopros-testirovaniya-urovneiy-1400-1380-punktov-po-indeksu-mmvb-20140919-1915/", result.First().Url);

            Assert.AreEqual("Шелехов Павел", result.Skip(1).First().Author);
            Assert.AreEqual(@"/analysis/marketnews36163/", result.Skip(1).First().Url);
        }

        [TestMethod]
        public void Test3()
        {
            var content = @"<td class=""ntitle bdotline"">
                            <a class=""author"" href=""/analysis/author0062900002/"">Верников Андрей, ИК ""Церих Кэпитал Менеджмент""</a>
                            <img src=""/i/N/graph.gif"" width=""13"" height=""13"" class=""mr05"" style=""vertical-align: bottom; "">
                            <a href=""/analysis/marketnews/torgi-ponedelnika-na-rossiiyskom-fondovom-rynke-nachnutsya-s-neznachitelnym-povysheniem-20141020-0915/"" class=""f-fake-url"">
                            <span class=""f-fake-url__t"">Торги понедельника на российском фондовом </span><span class=""f-fake-url__u"">рынке начнутся с незначительным повышением</span></a></td>";

            var result = new ArticleListItemExtractor(content).GetItems();

            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Верников Андрей, ИК \"Церих Кэпитал Менеджмент\"", result.First().Author);
            Assert.AreEqual(@"/analysis/marketnews/torgi-ponedelnika-na-rossiiyskom-fondovom-rynke-nachnutsya-s-neznachitelnym-povysheniem-20141020-0915/", result.First().Url);
        }
    }
}
