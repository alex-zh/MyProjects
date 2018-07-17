using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Common;
using Common.Classes.General;
using Common.Classes.Logger;
using Common.Classes.Robot;
using Common.Robots.Common;
using QuikConnector.Common.XlFormatParser;
using Robot.Quik2Net.Classes;

namespace Robot.Quik2Net.XlFormatParser
{
    /// <summary>
    /// 1. Первая строка заголовок
    /// 2. Первая колонка номер строки
    /// </summary>
    public class DdeDataParser
    {
        public DateTime ParseServerTime(byte[] data)
        {
           return  XlTableFormatParser.Read(data, ParseServerTime);
        }

        public List<Candle> ParseCandles(byte[] data)
        {
            var candles = XlTableFormatParser.Read(data, ParseCandles);
            candles.Reverse(0, candles.Count);

            return candles;
        }

        public List<QuikTrade> ParseTrades(byte[] data)
        {
            return XlTableFormatParser.Read(data, ParseTrades);
        }

        public List<LimitOrder> ParseLimitedOrders(byte[] data)
        {
            return XlTableFormatParser.Read(data, ParseLimitedOrders);
        }

        public DateTime ParseServerTime(int rowsCount, int columnCount, object[] cells)
        {
            if (columnCount != 2)
            {
                throw new ArgumentException("В таблице времени сервера ожидается 2 поля Идентификатор и время, пришло " + columnCount);
            }

            const int startIndex = 1*2;

            if (startIndex + 1 > cells.Count())
            {
                throw new ArgumentException("В таблице текущего времени нет данных");
            }

            return ParseServerTime((string)cells[startIndex + 1]);
        }

        private List<QuikTrade> ParseTrades(int rowsCount, int columnCount, object[] cells)
        {
            if (columnCount != 8)
            {
                throw new ArgumentException("В таблице свечей ожидается 7 полей, пришло " + columnCount);
            }

            var result = new List<QuikTrade>();

            for (int row = 1; row < rowsCount; row++)
            {
                var rowIndex = (row * columnCount);

                var trade = new QuikTrade();

                trade.TradeNumber = long.Parse((string)(cells[rowIndex + 1]));
                trade.DateTime = ParseDate((string)cells[rowIndex + 2], (string)cells[rowIndex + 3]);

                var direction = ParseOrderSide((string) cells[rowIndex + 4]);

                trade.Price = (double) cells[rowIndex + 5];
                trade.Size = (int) (double) cells[rowIndex + 6]*direction.ToInt();

                trade.OrderNumber = long.Parse((string) (cells[rowIndex + 7]));
                    
                result.Add(trade);
            }

            return result;
        }


        /// <remarks>
        /// Following fields are expected:
        /// 1. OrderNumber, 
        /// 2. Date,
        /// 3. Time, 
        /// 4. Operation (buy/sell), 
        /// 5. Price, 
        /// 6. Size, 
        /// 7. Remainder (if partially completed), 
        /// 8. Status (ACTIVE/FILLED/KILLED)
        ///</remarks>
        private List<LimitOrder> ParseLimitedOrders(int rowsCount, int columnCount, object[] cells)
        {
            if (columnCount != 9)
            {
                throw new ArgumentException("В таблице свечей ожидается 8 полей, пришло " + columnCount);
            }

            var result = new List<LimitOrder>();

            for (int row = 1; row < rowsCount; row++)
            {
                var rowIndex = (row * columnCount);

                var limitedOrder = new LimitOrder
                {
                    SystemOrderId = (string) (cells[rowIndex + 1]),
                    DateTime = ParseDate((string) cells[rowIndex + 2], (string) cells[rowIndex + 3]),
                    OrderSide = ParseOrderSide((string)cells[rowIndex + 4]),
                    Price = (double)cells[rowIndex + 5],
                    Size = (double)cells[rowIndex + 6],                    

                    Balance = (int)(double)cells[rowIndex + 7],

                    Status = ParseStatus((string) cells[rowIndex + 8])
                };

                result.Add(limitedOrder);
            }

            return result;
        }

        private OrderSides ParseOrderSide(string text)
        {
            if(text.ToUpper() == "BUY")
                return OrderSides.Buy;
            else if(text.ToUpper() == "SELL")
                return OrderSides.Sell;
            else
            {
                throw new ArgumentException("DDE. Parse orders table. Incorrect direction text: "+text);
            }
        }

        private OrderStatuses ParseStatus(string text)
        {
            switch (text.ToUpper())
            {
                case "ACTIVE":
                    return OrderStatuses.Active;
                case "FILLED":
                    return OrderStatuses.Completed;
                case "KILLED":
                    return OrderStatuses.Withdrawn;
                default:
                    throw new ArgumentException("DDE. Parse orders table. Incorrect status text: " + text);
            }
        }

        private List<Candle> ParseCandles(int rowsCount, int columnCount, object[] cells)
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

        /// <summary>
        /// Формат строки зависит от локализации.
        /// Для США будет 10:00:00PM
        /// Для России 22:00:00 
        /// </summary>
        private DateTime ParseServerTime(string serverTimeValue)
        {
            if (String.IsNullOrEmpty(serverTimeValue))
            {
                return DateTime.MinValue;
            }

            var timeValue = serverTimeValue.ToUpper();
            timeValue = Regex.Replace(timeValue, @"(?<=\d)(?=(P|A)M)", " ");

            DateTime serverTime;

            if (DateTime.TryParseExact(timeValue, CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern, null, DateTimeStyles.AllowWhiteSpaces,
                out  serverTime))
            {
                return serverTime;
            }

            if (DateTime.TryParseExact(timeValue, CultureInfo.CurrentUICulture.DateTimeFormat.LongTimePattern, null, DateTimeStyles.AllowWhiteSpaces,
                out  serverTime))
            {
                return serverTime;
            }

            return DateTime.MinValue;
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
