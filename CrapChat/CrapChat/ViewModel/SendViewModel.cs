using CrapChat.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Media.Imaging;

namespace CrapChat.ViewModel
{
    public class SendViewModel : ViewModelBase
    {
        private IChatService chatService;
        private CameraViewModel parentViewModel;

        public SendViewModel()
        {
            parentViewModel = ServiceLocator.Current.GetInstance<CameraViewModel>();
            parentViewModel.PropertyChanged += parentViewModel_PropertyChanged;
            ResetImageSource();

            chatService = ServiceLocator.Current.GetInstance<IChatService>();
            Friends = chatService.LoadFriends();
        }

        public const string FriendsPropertyName = "Friends";
        private ObservableCollection<Friend> friends;

        public ObservableCollection<Friend> Friends
        {
            get
            {
                return friends;
            }

            private set
            {
                if (friends == value)
                {
                    return;
                }

                friends = value;
                RaisePropertyChanged(FriendsPropertyName);
            }
        }

        public const string ImagePropertyName = "Image";
        private BitmapImage image;

        public BitmapImage Image
        {
            get
            {
                return image;
            }

            private set
            {
                if (image == value)
                {
                    return;
                }

                image = value;
                RaisePropertyChanged(ImagePropertyName);
            }
        }

        private void ResetImageSource(){
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Image = new BitmapImage();
                Image.SetSource(parentViewModel.Image);
            });
        }

        void parentViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (String.Equals(e.PropertyName, CameraViewModel.ImagePropertyName))
            {
                ResetImageSource();
            }
        }
    }


}
