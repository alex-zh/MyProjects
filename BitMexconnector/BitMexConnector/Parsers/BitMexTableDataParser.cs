using System;
using Newtonsoft.Json.Linq;

namespace BitMexConnector.Parsers
{
    /// <summary>
    /// Data can be different depending on table
    /// Data can be different depenind on action
    /// </summary>
    public class BitMexTableDataParser
    {
        public static BitMexTableData ParseTableData(JObject jobject)
        {
            JToken actionNameJToken;
            if (!jobject.TryGetValue("action", out actionNameJToken))
            {
                return new BitMexTableData()
                {
                    HasErrors = true,
                    ErrorText = "'Action' is not present in table data"
                };
            }

            var actionName = actionNameJToken.Value<string>();
            
            JToken dataJToken;
            if (!jobject.TryGetValue("data", out dataJToken))
            {
                return new BitMexTableData()
                {
                    HasErrors = true,
                    ErrorText = "'Data' is not present in table data"
                };
            }

            var result = new BitMexTableData
            {
                ActionName = (ActionNames)Enum.Parse(typeof(ActionNames), actionName, true),
                Data = (JArray)dataJToken
            };

            return result;
        }
    }
}
