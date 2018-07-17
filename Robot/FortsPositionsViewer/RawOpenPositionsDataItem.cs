using System;
using System.Globalization;
using FileHelpers;

namespace FortsPositionsViewer
{
    [IgnoreFirst(1)]
    [IgnoreEmptyLines]
    [DelimitedRecord(";")]
    public class RawOpenPositionsDataItem
    {
        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd")]
        public DateTime Moment; //2016-08-05
        public string ShortName; // isin;
        public string LongName; // name;
        public string ContractType; //F - futures, C - call option, P - put option

        [FieldConverter(typeof(FortsBoolConverter))]
        [FieldNullValue(typeof(bool), "false")]
        public bool IsIndividuals; // iz_fiz;  1,000 if it is individual, otherwise - empty

        [FieldConverter(typeof(FortsIntConverter))]
        [FieldNullValue(typeof(int), "0")]
        public int NumberOfClientsInLong; //clients_in_long;

        [FieldConverter(typeof(FortsIntConverter))]
        [FieldNullValue(typeof(int), "0")]
        public int NumberOfClientsInShort; //clients_in_lshort;

        [FieldConverter(typeof(FortsDoubleConverter))]
        [FieldNullValue(typeof(double), "0")]
        public double ShortPosition; //        short_position;

        [FieldConverter(typeof(FortsDoubleConverter))]
        [FieldNullValue(typeof(double), "0")]
        public double LongPosition; //  long_position;

        [FieldValueDiscarded]
        [FieldNullValue(typeof(double),"0")]
        private double? change_prev_week_short_abs;

        [FieldValueDiscarded]
        [FieldNullValue(typeof(double), "0")]
        private double? change_prev_week_long_abs;

        [FieldValueDiscarded]
        [FieldNullValue(typeof(double), "0")]
        private double? change_prev_week_short_perc;

        [FieldValueDiscarded]
        [FieldNullValue(typeof(double), "0")]
        private double? change_prev_week_long_perc;

        [FieldValueDiscarded]
        [FieldNullValue(typeof(double), "0")]
        private double? emptySemicolon;

    }

    public class FortsDoubleConverter: ConverterBase
    {
        public override object StringToField(string from)
        {
            if (String.IsNullOrEmpty(from))
                return 0.0;

            from = from.Replace(" ", "");
            from = from.Replace(",", ".");
            return double.Parse(from);
        }
    }

    public class FortsIntConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            if (String.IsNullOrEmpty(from))
                return 0;

            from = from.Replace(" ", "");
            from = from.Replace(",", ".");
            return (int) double.Parse(from);
        }
    }

    public class FortsBoolConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            if (String.IsNullOrEmpty(from))
                return false;

            from = from.Replace(" ", "");
            from = from.Replace(",", ".");
            return double.Parse(from) > 0;
        }
    }
}