using System.Collections.Generic;

namespace BitMexConnector
{
    internal class BitMexConfig
    {
        /* testnet keys */
        public static string PublicBitmexKey => "3uFBfBaA6FItdS8LbNr3_k1C";
        public static string PrivateBitmexKey => "OIMZo3Hj46TygavxKPrVy7ys0hj9Z2YSgU81pusc4ejBs6R-";

        public static string ApiPath => "api/v1";
        // https://bitmex.com
        public static string RestApiHost => "https://testnet.bitmex.com";

        // wss://www.bitmex.com/realtime        
        public static string WebSocketUrl => "wss://testnet.bitmex.com/realtime";
    }

    //public static class Symbols
    //{
    //    public static string XbtUsd => "XBTUSD";

    //    public static string XbtUsd => "XBTUSD";

    //    public static IEnumerable<string> GetAll()
    //    {
    //        yield return XbtUsd;
    //    }
    //}


    public static class SystemTables
    {
        public static string Account => "account";
        public static string Order => "order";
        public static string Execution => "execution";
        public static string Affiliate => "affiliate";
        public static string Funds => "funds";
        public static string Insurance => "insurance";
        public static string Margin => "margin";
        public static string Transact => "transact";
        public static string Wallet => "wallet";
        public static string Announcement => "announcement";
        public static string Connected => "connected";
        public static string Chat => "chat";        
        public static string PublicNotifications => "publicNotifications";
        public static string PrivateNotifications => "privateNotifications";
    }

    public static class SymbolTables
    {
        public static string Instrument => "instrument";

        /*Full level 2 orderBook*/
        public static string OrderBookL2 => "orderBookL2";

        public static string QuoteBin1m => "quoteBin1m";

        public static string TradeBin1m => "tradeBin1m";

        /*Top 10 levels using traditional full book push*/
        //public static string OrderBook10 => "orderBook10";
    }

    /* PUBLIC
    0 = "announcement"
1 = "connected"
2 = "chat"
3 = "publicNotifications"
4 = "instrument"
5 = "settlement"
6 = "funding"
7 = "insurance"
8 = "liquidation"
9 = "orderBookL2"
10 = "orderBook10"
11 = "quote"
12 = "trade"
13 = "quoteBin1m"
14 = "quoteBin5m"
15 = "quoteBin1h"
16 = "quoteBin1d"
17 = "tradeBin1m"
18 = "tradeBin5m"
19 = "tradeBin1h"
20 = "tradeBin1d"

    */

    /* Required auth

    0 = "privateNotifications"
1 = "account"
2 = "wallet"
3 = "affiliate"
4 = "margin"
5 = "position"
6 = "transact"
7 = "order"
8 = "execution"
    */

}