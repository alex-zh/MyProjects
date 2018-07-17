using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BitMexConnector.UnitTests
{
    [TestClass]
    public class BitMexSignatureCalculatorTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var apiSecret = "chNOOS4KvNXR_Xq4k4c9qsfoKWvnDecLATCRlcBwyKDYnWgO";

            var calculator = new BitMexSignatureCalculator(apiSecret);

            var verb = "POST";
            var path = "/api/v1/order";
            var apiExpires = "1518064238";

            dynamic payload = new
            {
                symbol = "XBTM15",
                price = 219.0,
                clOrdID = "mm_bitmex_1a/oemUeQ4CAJZgP3fjHsA",
                orderQty = 98
            };

            string content = JsonConvert.SerializeObject(payload);

            var result = calculator.Calculate(HttpMethod.Post, path, apiExpires, content);

            Assert.AreEqual("1749cd2ccae4aa49048ae09f0b95110cee706e0944e6a14ad0b3a8cb45bd336b", result);

        }
    }
}
