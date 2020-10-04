using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace TradeArcher.Converters
{
    public class NumberToStyleConverter : IValueConverter
    {
        public Style TrueStyle { get; set; }
        public Style FalseStyle { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double comparisonValue = 0.0;
            if (parameter != null && parameter is int intCompareValue)
            {
                comparisonValue = System.Convert.ToDouble(intCompareValue);
            }

            if (parameter != null && parameter is double doubleCompareValue)
            {
                comparisonValue = doubleCompareValue;
            }

            if (parameter != null && parameter is string stringCompareValue && !string.IsNullOrWhiteSpace(stringCompareValue))
            {
                comparisonValue = double.Parse(stringCompareValue);
            }
            
            if (value is bool && (bool) value)
            {
                return TrueStyle;
            }
            else if (value is double doubleValue)
            {
                return doubleValue >= comparisonValue ? TrueStyle : FalseStyle;
            }
            else if (value is int integerValue)
            {
                return integerValue >= comparisonValue ? TrueStyle : FalseStyle;
            }
            return FalseStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }}
