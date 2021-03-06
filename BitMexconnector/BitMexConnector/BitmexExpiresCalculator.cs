using System;

namespace BitMexConnector
{
    /*   временная метка UNIX, после наступления которой запрос больше не действителен. Необходима для предотвращения атак повторением.     */
    public class BitmexExpiresCalculator
    {
        public string Calculate(int secondsNumber = 60*60)
        {
            int unixTime = (int)(DateTime.UtcNow.AddSeconds(secondsNumber) - new DateTime(1970, 1, 1)).TotalSeconds;
            string apiExpires = unixTime.ToString();
            return apiExpires;
        }
    }
}