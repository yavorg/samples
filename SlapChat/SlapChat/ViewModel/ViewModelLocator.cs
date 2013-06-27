using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using SlapChat.Model;

namespace SlapChat.ViewModel
{

    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<Messenger>();
            SimpleIoc.Default.Register<CameraViewModel>();
            SimpleIoc.Default.Register<FriendsViewModel>();
            SimpleIoc.Default.Register<PhotosViewModel>();
            SimpleIoc.Default.Register<SendViewModel>();
            SimpleIoc.Default.Register<ViewViewModel>();
            SimpleIoc.Default.Register<IChatService, ConnectedChatService>();
            SimpleIoc.Default.Register<INotificationService, NotificationService>();
            
        }

        public CameraViewModel Camera
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CameraViewModel>();
            }
        }

        public FriendsViewModel Friends
        {
            get
            {
                return ServiceLocator.Current.GetInstance<FriendsViewModel>();
            }
        }

        public PhotosViewModel Photos
        {
            get
            {
                return ServiceLocator.Current.GetInstance<PhotosViewModel>();
            }
        }

        public SendViewModel Send
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SendViewModel>();
            }
        }

        public ViewViewModel View
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ViewViewModel>();
            }
        }


        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}
