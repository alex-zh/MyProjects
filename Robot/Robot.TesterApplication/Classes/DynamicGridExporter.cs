using System;
using System.Text;
using Common.Classes;
using Common.Classes.General;

namespace Robot.TesterApplication.Classes
{
    public class DynamicGridExporter
    {
        public string Export<K1, K2, V>(TwoDimensionsDictionary<K1, K2, V> dictionary,
                                        Func<K1, V, string> formatValue,
                                        Func<K1, string> formatRowHeader)
        {
            return ExportContent(dictionary, formatRowHeader, formatValue);
        }

        private string ExportContent<K1, K2, V>(TwoDimensionsDictionary<K1, K2, V> dictionary,
                                                Func<K1, string> formatRowHeader,
                                                Func<K1, V, string> formatValue)
        {
            var contentBuilder = new StringBuilder();

            for (int columnIndex = 0; columnIndex < dictionary.ColumnsCount; columnIndex++)
            {
                contentBuilder.Append("\t");
                contentBuilder.Append(dictionary.ColumnKeys[columnIndex].ToString().ToSplitText());
            }

            foreach (K1 rowKey in dictionary.RowKeys)
            {
                contentBuilder.AppendLine("");

                contentBuilder.AppendFormat("{0}\t", formatRowHeader(rowKey));

                foreach (K2 columnKey in dictionary.ColumnKeys)
                {
                    var text = formatValue(rowKey, dictionary[rowKey, columnKey]);
                    contentBuilder.AppendFormat("{0}\t", text);
                }
            }

            return contentBuilder.ToString();
        }
    }
}
