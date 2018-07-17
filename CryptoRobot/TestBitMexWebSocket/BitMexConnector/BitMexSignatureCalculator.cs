using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace BitMexConnector
{
    public class BitMexSignatureCalculator
    {
        private readonly HMACSHA256 _hmacSha256;

        public BitMexSignatureCalculator(string privateKey)
        {
            _hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(privateKey));
        }

        public string Calculate(HttpMethod verb, string url, string apiExpires, string content)
        {
            if (url.Substring(0, 1) != "/"){
                throw new ArgumentException("Url for encryption should start with /");
            }

            var message = Encoding.UTF8.GetBytes(verb + url + apiExpires + content);
            byte[] hash = _hmacSha256.ComputeHash(message);

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}