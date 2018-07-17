using System;
using System.Collections.Generic;
using System.Linq;
using Common.Classes.Robot;

namespace JointAssetsViewer
{
    public class JointAssertsSettings
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
        public static string SecurityCodeBr
        {
            get { return "BRX5"; }
        }
      
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
                return "JointAssetsQuikCandles_";
            }
        }

        /// <summary>
        /// »м€ сервера DDE. 
        /// Quik в качестве клиента использует это им€
        /// </summary>
        public static string DdeServerName
        {
            get
            {
                return "JointAssetsDdeServer";
            }
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