using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using SlapChat.Model;

namespace SlapChat.ViewModel
{
    public class PhotoRecordToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string result = String.Empty;
            PhotoRecord record = value as PhotoRecord;
            if (String.Equals(record.SenderUserId, App.CurrentUser.UserId))
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
