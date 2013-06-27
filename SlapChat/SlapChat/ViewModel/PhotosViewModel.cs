using SlapChat.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SlapChat.ViewModel
{
    public class PhotosViewModel : ViewModelBase
    {
        private IChatService chatService;

        public PhotosViewModel()
        {
            chatService = ServiceLocator.Current.GetInstance<IChatService>();

            RefreshCommand = new RelayCommand(async () =>
            {
                Photos = await chatService.ReadPhotoRecordsAsync(App.CurrentUser.UserId);
            });

            ViewPhoto = new RelayCommand(() =>
            {
                App.RootFrame.Navigate(new Uri("/View/ViewPage.xaml", UriKind.RelativeOrAbsolute));
            });
        }

        public const string PhotosPropertyName = "Photos";
        private ObservableCollection<PhotoRecord> photos;
        public ObservableCollection<PhotoRecord> Photos
        {
            get
            {
                return photos;
            }

            private set
            {
                if (photos == value)
                {
                    return;
                }

                photos = value;
                RaisePropertyChanged(PhotosPropertyName);
            }
        }

        public const string SelectedPhotoPropertyName = "SelectedPhoto";
        private PhotoRecord selectedPhoto;

        public PhotoRecord SelectedPhoto
        {
            get
            {
                return selectedPhoto;
            }

            private set
            {
                if (selectedPhoto == value)
                {
                    return;
                }

                selectedPhoto = value;
                RaisePropertyChanged(SelectedPhotoPropertyName);
            }
        }

        public RelayCommand RefreshCommand
        {
            get;
            private set;
        }

        public RelayCommand ViewPhoto
        {
            get;
            private set;
        }
    }
}
