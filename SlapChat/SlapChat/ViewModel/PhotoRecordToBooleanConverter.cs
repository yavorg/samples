using SlapChat.Model;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SlapChat.ViewModel
{
    public class PhotoRecordToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool result = true;
            PhotoRecord record = value as PhotoRecord;
            if (String.Equals(record.SenderMicrosoftAccount, App.CurrentUser.UserId)
                || record.Expired == true)
            {
                result = false;
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
