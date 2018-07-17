using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Common.Classes;
using Common.Classes.General;

namespace Visualizer.VisualControls.Controls
{
    /// <summary>
    /// Строит динамический грид по переданному двухмерному словарю
    /// </summary>
    public partial class DynamicGridControl
    {        
        public DynamicGridControl()
        {
            InitializeComponent();
        }

        public void Build<K1, K2, V>(TwoDimensionsDictionary<K1, K2, V> dictionary, 
                                     Func<K1, V, string> formatValue,
                                     Action<K1, V, TextBlock> formatElement,
                                     Func<K1, string> formatRowHeader )
        {
            DynamicGrid.Children.Clear();

            AddRowsAndColumns(dictionary);
            AddRowsAndColumnsHeaders(dictionary, formatRowHeader);
            AddContent(dictionary, formatValue, formatElement);
        }

        private void AddRowsAndColumns<K1, K2, V>(TwoDimensionsDictionary<K1, K2, V> dictionary)
        {
            for (int rowIndex = 0; rowIndex < dictionary.RowsCount + 1; rowIndex++)
            {
                DynamicGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            for (int columnIndex = 0; columnIndex < dictionary.ColumnsCount + 1; columnIndex++)
            {
                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            }
        }

        private void AddRowsAndColumnsHeaders<K1, K2, V>(TwoDimensionsDictionary<K1, K2, V> dictionary, Func<K1, string> formatRowHeader )
        {
            for (int rowIndex = 0; rowIndex < dictionary.RowsCount; rowIndex++)
            {
                var textBlock = new TextBlock
                {
                    Style = (Style)TryFindResource("TableRowHeader"),
                    Text = formatRowHeader(dictionary.RowKeys[rowIndex])
                };

                Grid.SetRow(textBlock, rowIndex + 1);
                Grid.SetColumn(textBlock, 0);

                DynamicGrid.Children.Add(textBlock);
            }

            for (int columnIndex = 0; columnIndex < dictionary.ColumnsCount; columnIndex++)
            {
                var textBlock = new TextBlock
                {
                    Style = (Style)TryFindResource("TableColumnHeader"),
                    Text = dictionary.ColumnKeys[columnIndex].ToString().ToSplitText()
                };

                DynamicGrid.ColumnDefinitions[0].MinWidth = 120; 

                Grid.SetRow(textBlock, 0);
                Grid.SetColumn(textBlock, columnIndex + 1);

                DynamicGrid.Children.Add(textBlock);
            }
        }

        private void AddContent<K1, K2, V>(TwoDimensionsDictionary<K1, K2, V> dictionary,
                                           Func<K1, V, string> formatValue,
                                           Action<K1, V, TextBlock> formatElement)
        {
            foreach (K1 rowKey in dictionary.RowKeys)
            {
                foreach (K2 columnKey in dictionary.ColumnKeys)
                {
                    V value = dictionary[rowKey, columnKey];

                    var textBlock = new TextBlock
                    {
                        Style = (Style)TryFindResource("TableValue"),
                        Text = formatValue(rowKey, value )
                    };

                    formatElement(rowKey, value, textBlock);

                    Grid.SetRow(textBlock, dictionary.RowKeys.IndexOf(rowKey) + 1);
                    Grid.SetColumn(textBlock, dictionary.ColumnKeys.IndexOf(columnKey) + 1);

                    DynamicGrid.Children.Add(textBlock);
                }
            }
        }
        
    }
}
