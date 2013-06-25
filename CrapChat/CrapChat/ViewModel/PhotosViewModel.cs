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
        }

        public const string PhotosPropertyName = "Photos";
        public ObservableCollection<Photo> Photos
        {
            get
            {
                return chatService.ReadPhotos();
            }
        }

        public RelayCommand RefreshCommand
        {
            get;
            private set;
        }
 
    }
}
