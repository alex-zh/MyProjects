using System;
using System.Collections.Generic;
using QuikConnector.Common.Classes;

namespace QuikConnector.Common.XlFormatParser
{
    /// <summary>
    /// 1. Первая строка заголовок
    /// 2. Первая колонка номер строки
    /// </summary>
    public class DdeCandlesParser
    {
        public List<Candle> Parse(byte[] data)
        {
            return XlTableFormatParser.Read(data, ParseCandles);
        }

        public List<Candle> ParseCandles(int rowsCount, int columnCount, object[] cells)
        {
            if (columnCount != 8)
            {
                throw new ArgumentException("В таблице свечей ожидается 8 полей, пришло " + columnCount);
            }

            var result = new List<Candle>();

            for (int row = 1; row < rowsCount; row++)
            {
                var rowIndex = (row * columnCount);

                var candle = new Candle()
                {
                    Date = ParseDate((string)cells[rowIndex + 1], (string)cells[rowIndex + 2]),
                    Open = (double)cells[rowIndex + 3],
                    High = (double)cells[rowIndex + 4],
                    Low = (double)cells[rowIndex + 5],
                    Close = (double)cells[rowIndex + 6],
                    Volume = (double)cells[rowIndex + 7]
                };

                result.Add(candle);
            }

            return result;
        }

        private DateTime ParseDate(string dateValue, string timeValue)
        {
            if (String.IsNullOrEmpty(dateValue) || String.IsNullOrEmpty(timeValue))
            {
                throw  new ArgumentException("Дата или время не имеют значения");
            }

            var year = int.Parse(dateValue.Substring(0, 4));
            var month = int.Parse(dateValue.Substring(4, 2));
            var day = int.Parse(dateValue.Substring(6, 2));

            var hour = int.Parse(timeValue.Substring(0, 2));
            var minute = int.Parse(timeValue.Substring(2, 2));
            var second = int.Parse(timeValue.Substring(4, 2));

            var dateTime = new DateTime(year, month, day, hour, minute, second);

            return dateTime;
        }
    }
}
