using CrapChat.Model;
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

namespace CrapChat.ViewModel
{
    public class PhotosViewModel : ViewModelBase
    {
        private IChatService chatService;

        public PhotosViewModel()
        {
            chatService = ServiceLocator.Current.GetInstance<IChatService>();

            RefreshCommand = new RelayCommand(() =>
            {
                RaisePropertyChanged(PhotosPropertyName);
            });

            ViewPhoto = new RelayCommand(() =>
            {
                App.RootFrame.Navigate(new Uri("/View/ViewPage.xaml", UriKind.RelativeOrAbsolute));
            });
        }

        public const string PhotosPropertyName = "Photos";
        public ObservableCollection<PhotoRecord> Photos
        {
            get
            {
                return chatService.ReadPhotoRecords();
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
