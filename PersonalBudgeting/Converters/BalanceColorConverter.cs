using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace PersonalBudgeting.Converters
{
    public class BalanceColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal balance)
            {
                if (balance < 0)
                {
                    // Return danger color for negative balance
                    return new SolidColorBrush(Color.Parse("#FF5252"));
                }
                else if (balance == 0)
                {
                    // Return neutral color for zero balance
                    return new SolidColorBrush(Color.Parse("#FFFFFF"));
                }
                else
                {
                    // Return success color for positive balance
                    return new SolidColorBrush(Color.Parse("#4CAF50"));
                }
            }
            
            // Default color
            return new SolidColorBrush(Color.Parse("#FFFFFF"));
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 