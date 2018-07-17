using System.Collections.Generic;
using BitMexConnector.Parsers.ExecutionTable;
using BitMexConnector.Parsers.ExecutionTable.DataItems;
using BitMexConnector.Tables.Classes;
using Common.Classes.Logger;

namespace BitMexConnector.DataStoreUpdators
{
    public class ExecutionTableUpdator
    {
        private readonly DataStore _store;
        private readonly ILogger _logger;

        public ExecutionTableUpdator(DataStore store, ILogger logger)
        {
            _store = store;
            _logger = logger;
        }

        public void Update(IEnumerable<PartialDataItem> items)
        {
            foreach (var entry in items)
            {                
               _store.Trades.TryAdd(entry.ExecId, entry.ToTrade());             
            }

            _store.OnTradesChanged();            
        }

        public void Update(IEnumerable<InsertDataItem> items)
        {
            foreach (var entry in items)
            {               
               _store.Trades.TryAdd(entry.ExecId, entry.ToTrade());             
            }

            _store.OnTradesChanged();
        }

        public void Update(IEnumerable<UpdateDataItem> items)
        {
            foreach (var entry in items)
            {
                //частичное исполненение, как приходит?                
                BitMexTrade trade;
                if (_store.Trades.TryGetValue(entry.ExecId, out trade))
                {
                    trade.LeftSize = entry.LeftSize;                    
                    _store.Trades.TryUpdate(trade.TradeId, trade, trade);
                }
                else
                {
                    _logger.Warning($" Execution with id {entry.ExecId} was not found in store");
                }
            }

            _store.OnTradesChanged();
        }    

    }
}