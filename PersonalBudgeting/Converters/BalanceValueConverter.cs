using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace PersonalBudgeting.Converters
{
    public class BalanceValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal balance)
            {
                // Format the balance with currency sign and use minus sign for negative values
                return balance.ToString("$#,##0.00;-$#,##0.00;$0.00", culture);
            }
            
            return "$0.00";
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 