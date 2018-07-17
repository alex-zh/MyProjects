using BitMexConnector.Tables.Classes;

namespace BitMexConnector.Parsers.ExecutionTable
{
    public static class Extensions
    {
        public static BitMexTrade ToTrade(this DataItems.PartialDataItem item)
        {
            return new BitMexTrade
            {
                TradeId =  item.ExecId,
                OrderId = item.ClientOrderId,
                SystemOrderId = item.SystemOrderId.ToString(),                
                Size = item.Size ?? 0,
                Side = item.Side,
                LeftSize = item.LeftSize,
                Price = item.Price,
                Symbol = item.Symbol,
                DateTime = item.TimeStamp,
            };
        }

        public static BitMexTrade ToTrade(this DataItems.InsertDataItem item)
        {
            return new BitMexTrade
            {
                TradeId = item.ExecId,
                OrderId = item.ClientOrderId,
                SystemOrderId = item.SystemOrderId.ToString(),
                Size = item.Size ?? 0,
                Side = item.Side,
                LeftSize = item.LeftSize,
                Price = item.Price,
                Symbol = item.Symbol,
                DateTime = item.TimeStamp,
            };
        }       

    }
}
