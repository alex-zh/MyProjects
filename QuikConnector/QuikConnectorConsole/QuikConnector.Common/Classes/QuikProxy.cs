using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trans2Quik.Core;
using Trans2Quik.Core.Entities.Transaction.Order;

namespace QuikConnector.Common.Classes
{
    /// <summary>
    /// Trade - сделка, trades - таблица сделок
    /// Order - заявка, orders - таблица заявок
    /// </summary>
    public class QuikProxy
    {
        private readonly Gateway _gw;
        private readonly ILogger _logger;

        public QuikProxy(ILogger logger)
        {
            _logger = logger;

            _gw = new Gateway("C:\\QUIK", "SPBFUT00LI3");            
            _gw.OrderChanged += gw_OrderChanged;
            _gw.NewTransaction += gw_NewTransaction;
            _gw.TradeChanged += gw_TradeChanged;
        }

        public void Start()
        {
            _gw.Start("SPBFUT", "SiM5");              
        }

        public void Stop()
        {
            _gw.Stop();
        }

        private TransactionCallResult res;

        public  void SetLimitedOrder()
        {                   
            var p = new OrderTradeParams("SPBFUT", "SiM5", Direction.Buy, 1, decimal.Zero , false);
             res = _gw.SendOrderSync(p);
            _logger.Log(res.ToString());
            //_gw.Stop();
            //_gw.
        }

         void gw_TradeChanged(object sender, TradeInfoEventArgs e)
        {
             if (e.TradeInfo.Mode == TradeInfoMode.NewTrade)
             {
                 _logger.Log(e.TradeInfo.ToString());
                 _logger.Log(e.TradeInfoDetails.ToString());
             }
        }

        void gw_NewTransaction(object sender, TransactionEventArgs e)
        {
            _logger.Log(e.TransactionResult.ToString());
            //throw new NotImplementedException();
        }

        void gw_OrderChanged(object sender, OrderInfoEventArgs e)
        {
            if (e.OrderInfo.Mode == OrderInfoMode.NewOrder)
            {
                _logger.Log(e.OrderInfo.ToString());
                _logger.Log(e.OrderInfoDetails.ToString());    
            }
            
            //e.OrderInfoDetails.
            //throw new NotImplementedException();
        }

        public void KillLimitedOrder()
        {
           var res1 = _gw.KillOrderSync(new Security("SPBFUT", "SiM5"), res.OrderNumber.ToString() );
            _logger.Log(res1.ToString());
        }
    }
}
