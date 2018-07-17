using System;
using System.Text;

namespace Common
{
    public class ObjectExporter<T>
    {
        private Func<string> _lineSeparator = () => "\n";
        private Func<string, string, string> _lineFormatter = (name, value) => $"{name} \t {value}";


        public ObjectExporter()
        {
        }

        public ObjectExporter(Func<string, string, string> lineFormatter, Func<string> lineSeparator)
        {
            _lineFormatter = lineFormatter;
            _lineSeparator = lineSeparator;
        }

        public string Export(T objectToExport)
        {
            var resultBuilder = new StringBuilder();
            return ExportInternal(objectToExport, resultBuilder).ToString();
        }

        public StringBuilder ExportInternal(object objectToExport, StringBuilder resultBuilder)
        {
            var properties = objectToExport.GetType().GetProperties();

            foreach (var property in properties)
            {
                string name = property.Name; 
                var value = property.GetValue(objectToExport, null);

                if (value == null)
                {
                    resultBuilder.Append(FormatLine(name, "NA"));
                    resultBuilder.Append(_lineSeparator());
                    continue;
                }

                var type = value.GetType();
                if (IsSimple(type))
                {
                    resultBuilder.Append(FormatLine(name, value));
                    resultBuilder.Append(_lineSeparator());
                }
                else
                {
                    resultBuilder.Append(name + ": ");
                    resultBuilder.Append(_lineSeparator());

                    ExportInternal(value, resultBuilder);
                }
            }

            return resultBuilder;
        }

        private string FormatLine(string name, object value)
        {
            var stringValue = "";
            var type = value.GetType();

            if (type == typeof(DateTime))
            {
                stringValue = Convert.ToDateTime(value).ToString("yyyy MM dd H:mm:ss");
            }
            else
            {
                stringValue = value.ToString();
            }

            return _lineFormatter(name, stringValue);
        }

        public static TR GetPropValue<TR>(T source, string propName)
        {
            var propertyInfo = typeof(T).GetProperty(propName);
            if (propertyInfo != null)
                return (TR)propertyInfo.GetValue(source, null);

            return default(TR);
        }

        private bool IsSimple(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsSimple(type.GetGenericArguments()[0]);
            }
            return type.IsPrimitive
              || type.IsEnum
              || type == typeof(string)
              || type == typeof(decimal)
              || type == typeof(DateTime);
        }
    }
}
