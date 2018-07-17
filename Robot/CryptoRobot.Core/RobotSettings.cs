using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Robot;

namespace Robot.Core
{
    public  class RobotSettings
    {
        public static string PathToQuik
        {
            get { return "C:\\QUIK"; }
        }

        public static string Account
        {
            get { return "SPBFUT00LI3"; }
        }

        public static string FuturesClassCode
        {
            get { return "SPBFUT"; }
        }

        public static string SecurityCodeSi
        {
            get { return "SiZ5"; }
        }

        /// <summary>
        /// Register is important, RiZ5 is not found!
        /// </summary>
        public static string SecurityCodeRi
        {
            get { return "RIZ5"; }
        }

        //public static int MaxPositionSize
        //{
        //    get { return 10; }
        //}

        //public static int MinPositionSize
        //{
        //    get { return -1 * MaxPositionSize; }
        //}

        /// <summary>
        /// Стандартная рабочий размер, для Si_1min пока возвращаем 2. 
        /// Для удобства возвращаем четное число, чтобы удобно было брать половину
        /// </summary>
        //public static int PositionSize
        //{
        //    get { return 10; }
        //}

        //public static int HalfPositionSize
        //{
        //    get { return _halfPositionSize; }
        //    set { _halfPositionSize = value; }
        //}

        public static string QuikDdeServerTimeTopicName
        {
            get
            {
                return "QuikServerTime";
            }
        }

        public static string QuikDdeCandlesTopicName
        {
            get
            {
                return "QuikCandles_";                
            }
        }

        /// <summary>
        /// Имя сервера DDE. 
        /// Quik в качестве клиента использует это имя
        /// </summary>
        public static string DdeServerName
        {
            get
            {
                return "RobotDdeServer";
            }
        }

        /// <summary>
        /// Как часто quik отдает свечи через dde. 
        /// Задается при конфигурировании quik.
        /// </summary>
        public static int QuikDdeCandlesUpdateOffset
        {
            get { return 5000; }
        }

        /// <summary>
        /// Определяет как часто будет вызыватся метод Iteration
        /// В миллисекудах
        /// </summary>
        public static int IterationOffset
        {
            get { return 5000; }
        }

        public static string QuikDdeTradesTopicName
        {
            get
            {
                return "QuikTrades";
            }
        }

        public static string QuikDdeLimitedOrdersTopicName
        {
            get
            {
                return "QuikLimitedOrders";
            }            
        }

        public static IEnumerable<string> GetDdeCandlesTopicNames()
        {
            return from Securities security in Enum.GetValues(typeof(Securities)) select GetSecurityName(security);
        }

        public static string GetSecurityName(Securities security)
        {
            return QuikDdeCandlesTopicName + security;
        }
    }
}
