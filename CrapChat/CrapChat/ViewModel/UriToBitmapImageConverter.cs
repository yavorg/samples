using CrapChat.Model;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace CrapChat.ViewModel
{
    public class UriToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage result = null;
            IChatService chatService = ServiceLocator.Current.GetInstance<IChatService>();
            Stream source = chatService.ReadPhoto(value as Uri);

            if (source != null)
            {
                result = new BitmapImage();
                result.SetSource(source);
            }

            return result;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
