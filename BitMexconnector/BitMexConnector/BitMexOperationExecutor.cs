using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BitMexConnector.Tables.Classes;
using Common.Classes.Logger;

namespace BitMexConnector
{
    public class BitMexOperationExecutor
    {
        private readonly ILogger _logger;
        private readonly BitMexRestClient _restClient;        

        public BitMexOperationExecutor(ILogger logger)
        {
            _logger = logger;            
            _restClient = new BitMexRestClient(BitMexConfig.RestApiHost,
                                               BitMexConfig.ApiPath,
                                               BitMexConfig.PrivateBitmexKey,
                                               BitMexConfig.PublicBitmexKey, 
                                               _logger);
        }

        public async Task<bool> SetOrder(BitMexLimitOrder order)
        {
            try
            {
                await _restClient.PostOrder(order);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());
                return false;
            }
        }

        public async Task<bool> DeleteOrder(string clientOrderId)
        {
            try
            {
                await _restClient.DeleteOrder(clientOrderId);
                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());                
            }

            return false;
        }

        public async Task<bool> SetOrders(List<BitMexLimitOrder> orders)
        {
            try
            {
                await _restClient.PostOrders(orders);

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());

                return false;
            }
        }

        public async Task<bool> DeleteOrders(List<string> orders)
        {
            try
            {
                await _restClient.DeleteOrders(orders);

                return true;
            }
            catch (Exception e)
            {
                _logger.Error(e.ToString());

                return false;
            }
        }
    }
}
