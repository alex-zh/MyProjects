using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpinionAnalyzer.Core;
using OpinionAnalyzer.Core.Classes;
using OpinionAnalyzer.Core.Providers;

namespace OpinionAnalyzer.UnitTests.GeneralTests
{
    /* 
     Смысл предложения может полностью поменяться при добавлении частицы "не"
     * 
     Мы увидим новые низы в ближайшее время
     * 
     Мы не увидим новые низы в ближайшее время
     Маловероятно, что мы увидим новые низы в ближайшее время
     Шансов на то, что мы увидим новые низы в ближайшее время, немного
     Кажется сомнительными, что мы увидим новые низы в ближайшее время
     */
    /*
     Алгоритм
     1. указывается слово для проверки, например "увидим"
     2. Испльзуются паттерны проверки, например 
        не -@-
        ( маловероятно | сомнительно | кажется сомнительным ) .* -@-
        Шансов .* -@- .* ( мало | немного )
     * 
     3. Если присудствует отрицание, паттерн должен просто отклонятся. 
         Мнение должно будет учтено другим паттерном, например паттерном на повышение
     4. Применятся это должно только если указана подобная проверка. 
     * При составлении паттернов нужно учитывать возможность изменения смысла
     */
    [TestClass]
    public class NegativeFormTests
    {                
        [TestMethod]
        public void Initial()
        {
            var item = new Article("", "");

            var classifier = new OpinionClassifier(new PatternsProvider(new RawPattern[0], new MacroWord[0]));

            Trend result = classifier.Classify(item).Trend;

            Assert.AreEqual(Trend.Unknown, result);
        }
      
    }
}
