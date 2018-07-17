using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BitMexConnector.Tables.Classes;
using Common.Classes.Logger;
using Newtonsoft.Json;

namespace BitMexConnector
{
    public class BitMexRestClient
    {
        private readonly BitMexSignatureCalculator _bitMexSignatureCalculator;
        private readonly string _domain;
        private readonly string _baseApiPath;

        private string _publicKey;
        private readonly ILogger _logger;
        private string _privateKey;


        public BitMexRestClient(string domain, string baseApiPath, string privateKey, string publicKey, ILogger logger)
        {
            _domain = domain;
            _baseApiPath = baseApiPath;
            _privateKey = privateKey;
            _publicKey = publicKey;
            _logger = logger;
            _bitMexSignatureCalculator = new BitMexSignatureCalculator(_privateKey);
        }

        public async Task PostOrder(BitMexLimitOrder order)
        {
            var urlPath = $"/{_baseApiPath}/order";
            var url = $"{_domain}{urlPath}";

            dynamic payload = new
            {
                symbol = order.Symbol,
                ordType = "Limit",
                price = BitMexPriceConverter.Convert(order.Price, order.Symbol),
                orderQty = order.Size,
                clOrdID = order.OrderId,
                side = order.Size > 0 ? "Buy" : "Sell",
            };

            string content = JsonConvert.SerializeObject(payload);

            using (var client = new HttpClient())
            {
                AddBitMexHeaders(client, HttpMethod.Post, urlPath, content);

                var response = await
                    client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json")).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
            }
        }


        public async Task DeleteOrder(string clientOrderId)
        {
            var urlPath = $"/{_baseApiPath}/order";
            var url = $"{_domain}{urlPath}";

            dynamic payload = new
            {
                clOrdID = clientOrderId,
            };

            string content = JsonConvert.SerializeObject(payload);

            using (var client = new HttpClient())
            {
                AddBitMexHeaders(client, HttpMethod.Delete, urlPath, content);

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url),
                    Content = new StringContent(content, Encoding.UTF8, "application/json"),
                };

                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    _logger.Error($"Error while order deletion: {message}");
                }

                response.EnsureSuccessStatusCode();
            }
        }

        private long GetNonce()
        {
            DateTime yearBegin = new DateTime(1990, 1, 1);
            return DateTime.Now.AddDays(1).Ticks - yearBegin.Ticks;            
        }

        private void AddBitMexHeaders(HttpClient client, HttpMethod method, string url, string content)
        {
            client.DefaultRequestHeaders.Add("api-key", _publicKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string apiExpires =  new BitmexExpiresCalculator().Calculate();
            //string apiExpires = GetNonce().ToString();// new BitmexExpiresCalculator().Calculate();

            client.DefaultRequestHeaders.Add("api-expires", apiExpires);
            //client.DefaultRequestHeaders.Add("api-nonce", apiExpires);

            //apiExpires = "";

            client.DefaultRequestHeaders.Add("api-signature", _bitMexSignatureCalculator.Calculate(method, url, apiExpires, content));
        }

        public async Task PostOrders(List<BitMexLimitOrder> orders)
        {
            var urlPath = $"/{_baseApiPath}/order/bulk";
            var url = $"{_domain}{urlPath}";

            var ordersPayload = new List<dynamic>();

            foreach (var order in orders)
            {
                ordersPayload.Add(new
                {
                    symbol = order.Symbol,
                    ordType = "Limit",
                    price = BitMexPriceConverter.Convert(order.Price, order.Symbol),
                    simpleOrderQty = Math.Abs(order.Size),
                    clOrdID = order.OrderId,
                    side = order.Side.ToString(),
                });
            }
            
            var content = $"{{\"orders\": { JsonConvert.SerializeObject(ordersPayload)}}}";

            using (var client = new HttpClient())
            {
                AddBitMexHeaders(client, HttpMethod.Post, urlPath, content);
                
                var response = await
                    client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json")).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    _logger.Error($"Error while postng orders: {message}");
                }

                response.EnsureSuccessStatusCode();
            }           
        }

        public async Task DeleteOrders(List<string> orderIds)
        {
            var urlPath = $"/{_baseApiPath}/order";
            var url = $"{_domain}{urlPath}";

            var ordersPayload = new List<dynamic>();

            foreach (var orderId in orderIds)
            {
                ordersPayload.Add(new { clOrdID = orderId });
            }

            // var content = $"{{\"clOrdIDs\": { JsonConvert.SerializeObject(ordersPayload)}}}";            

            var content = $"{{\"clOrdIDs\": { JsonConvert.SerializeObject(orderIds)}}}";

            using (var client = new HttpClient())
            {
                AddBitMexHeaders(client, HttpMethod.Delete, urlPath, content);

                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(url),
                    Content = new StringContent(content, Encoding.UTF8, "application/json"),
                };

                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

                if (!response.IsSuccessStatusCode)
                {
                    var message = await response.Content.ReadAsStringAsync();
                    _logger.Error($"Error while deleting orders: {message}");
                }

                response.EnsureSuccessStatusCode();
            }
        }
    }
}