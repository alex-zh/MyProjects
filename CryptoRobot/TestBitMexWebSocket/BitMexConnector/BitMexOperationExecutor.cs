using System;
using System.Collections.Generic;
using BitMexConnector.Tables.Classes;

namespace BitMexConnector
{
    public class BitMexOperationExecutor : IOperationExecutor
    {
        private readonly ILogger _logger;
        private readonly BitMexRestClient _restClient;

        public BitMexOperationExecutor(ILogger logger)
        {
            _logger = logger;
            _restClient = new BitMexRestClient(BitMexConfig.RestApiHost, 
                                               BitMexConfig.ApiPath, 
                                               BitMexConfig.PrivateBitmexKey, 
                                               BitMexConfig.PublicBitmexKey);
        }

        public async void SetOrder(LimitOrder order)
        {
            try
            {
                await _restClient.PostOrder(order);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }

        public async void DeleteOrder(LimitOrder order)
        {
            try
            {
               await _restClient.DeleteOrder(order);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }

        public async void SetOrders(List<LimitOrder> orders)
        {
            try
            {
               await _restClient.PostOrders(orders);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }

        public async void DeleteOrders(List<LimitOrder> orders)
        {
            try
            {
                await _restClient.DeleteOrders(orders);
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
        }
    }
}
