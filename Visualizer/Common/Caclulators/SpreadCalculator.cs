using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes;
using Common.Classes.General;

namespace Common
{
    /// <summary>
    /// Считает спред между двумя сериями. Самое важное здесь, это валидация ошибок.
    /// Если для какой-то даты нет значения, выбрасываем эту дату
    /// </summary>
    public class SpreadCalculator
    {
        public IEnumerable<SpreadPoint> GetSpread(IEnumerable<Candle> firstSeries, IEnumerable<Candle> secondSeries)
        {
            return firstSeries.Join(secondSeries, x => x.Date, x => x.Date,
                (x, y) => new SpreadPoint() { Date = x.Date, Price = (int)(x.Close * 100000 - y.Close), Percent = ((x.Close * 100000 - y.Close) / (x.Close * 1000)) });
        }
    }

    public class SpreadPoint
    {
        public DateTime Date { get; set; }
        public int Price { get; set; }
        public double Percent { get; set; }
    }
}
