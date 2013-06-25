using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using CrapChat.Model;

namespace CrapChat.ViewModel
{
    public class PhotoRecordToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = "";
            PhotoRecord record = value as PhotoRecord;
            if (String.Equals(record.SenderMicrosoftAccount, App.CurrentUser))
            {
                // Sent
                result = "\ue120";
            }
            else if (record.Expired == true)
            {
                // Read
                result = "\ue166";
            }
            else
            {
                // Unread
                result = "\ue119";
            }
            return result;

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
