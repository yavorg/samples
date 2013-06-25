using SlapChat.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Threading;

namespace SlapChat.ViewModel
{
    public class ViewViewModel : ViewModelBase
    {
        private IChatService chatService;
        private PhotosViewModel parentViewModel;
        private Timer timer;

        public ViewViewModel()
        {
            chatService = ServiceLocator.Current.GetInstance<IChatService>();
            parentViewModel = ServiceLocator.Current.GetInstance<PhotosViewModel>();
            timer = new Timer(new TimerCallback((c) => {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    App.RootFrame.Navigate(new Uri("/View/PhotosPage.xaml", UriKind.RelativeOrAbsolute));
                });
            }),
            null,
            Timeout.Infinite,
            Timeout.Infinite);

            HideCommand = new RelayCommand(() =>
            
            {
                timer.Change(TimeSpan.FromSeconds(6), TimeSpan.FromMilliseconds(-1));
                PhotoContent content = chatService.ReadPhotoContent(
                    parentViewModel.SelectedPhoto.PhotoContentId);
                if (content != null)
                {
                    Uri = content.Uri;
                }
                else
                {
                    Uri = null;
                }

            });
            
         
        }

        public const string UriPropertyName = "Uri";
        private Uri uri;

        public Uri Uri
        {
            get
            {
                return uri;
            }

            private set
            {
                if (uri == value)
                {
                    return;
                }

                uri = value;
                RaisePropertyChanged(UriPropertyName);
            }
        }


        public RelayCommand HideCommand
        {
            get;
            private set;
        }


      
    }

}
