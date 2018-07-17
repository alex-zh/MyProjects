using System;
using Connector.Common;

namespace CryptoDataLoader
{
    public class ConnectorsFactory
    {
        public static IConnector Get(Connectors selectedConnector)
        {
            switch (selectedConnector)
            {
                case Connectors.BitFinex:
                    return new BitfinexConnector.Connector();
                case Connectors.BitMex:
                    return new BitMexConnector.Connector();
                default:
                    throw new NotSupportedException(selectedConnector.ToString());
            }
        }
    }
}