using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapChat.ViewModel
{
    public class SendViewModel : ViewModelBase
    {
        private ViewModelLocator locator;

        public SendViewModel()
        {
            locator = App.Current.Resources["Locator"] as ViewModelLocator;
        }

    
        public Stream Image
        {
            get
            {
                return locator.Camera.Image;
            }
        }

    }


}
