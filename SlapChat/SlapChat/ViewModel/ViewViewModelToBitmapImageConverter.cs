using SlapChat.Model;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SlapChat.ViewModel
{
    public class ViewViewModelToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            BitmapImage result = null;
            ViewViewModel vm = value as ViewViewModel;
            if (vm.Stream != null)
            {
                result = new BitmapImage();
                result.SetSource(vm.Stream);
            }
            else
            {
                result = new BitmapImage(vm.Uri);
            }

            return result;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
