using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Navigation;

namespace SlapChat.ViewModel
{
    public class CameraViewModel : ViewModelBase
    {
        private PhotoCamera cam;

        public CameraViewModel()
	    {
            SetUpCameraCommand = new RelayCommand<NavigationEventArgs>((e) =>
                {
                    // Check to see if the camera is available on the phone.
                    if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) ||
                         (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
                    {
                        // Initialize the camera, when available.
                        if (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing))
                        {
                            // Use front-facing camera if available.
                            cam = new PhotoCamera(CameraType.FrontFacing);
                        }
                        else
                        {
                            // Otherwise, use standard camera on back of phone.
                            cam = new PhotoCamera(CameraType.Primary);
                        }

                        //// Event is fired when the PhotoCamera object has been initialized.
                        //cam.Initialized += new EventHandler<Microsoft.Devices.CameraOperationCompletedEventArgs>(cam_Initialized);

                        // Event is fired when the capture sequence is complete.
                        //cam.CaptureCompleted += new EventHandler<CameraOperationCompletedEventArgs>(cam_CaptureCompleted);

                        //// Event is fired when the capture sequence is complete and an image is available.
                        cam.CaptureImageAvailable += new EventHandler<Microsoft.Devices.ContentReadyEventArgs>(cam_CaptureImageAvailable);

                        //// Event is fired when the capture sequence is complete and a thumbnail image is available.
                        //cam.CaptureThumbnailAvailable += new EventHandler<ContentReadyEventArgs>(cam_CaptureThumbnailAvailable);

                        CameraBackground = new VideoBrush();
                        CameraBackground.SetSource(cam);
                        CameraBackground.Stretch = Stretch.Uniform;

                    }
                    else
                    {
                        // TODO The camera is not supported on the phone, do something smart
                    }

                });

            CleanUpCameraCommand = new RelayCommand<NavigatingCancelEventArgs>((e) =>
                {
                    if (cam != null)
                    {
                        // Dispose camera to minimize power consumption and to expedite shutdown.
                        cam.Dispose();

                        // Release memory, ensure garbage collection.
                        //cam.Initialized -= cam_Initialized;
                        //cam.CaptureCompleted -= cam_CaptureCompleted;
                        cam.CaptureImageAvailable -= cam_CaptureImageAvailable;
                        //cam.CaptureThumbnailAvailable -= cam_CaptureThumbnailAvailable;
                    }

                });

            TakePhotoCommand = new RelayCommand(() =>
                {
                    if (cam != null)
                    {
                        try
                        {
                            // Start image capture.
                            cam.CaptureImage();
                        }
                        catch
                        {
                            // TODO
                        }
                    }

                });

	    }

        public const string CameraBackgroundPropertyName = "CameraBackground";
        private VideoBrush cameraBackground;

        public VideoBrush CameraBackground
        {
            get
            {
                return cameraBackground;
            }

            private set
            {
                if (cameraBackground == value)
                {
                    return;
                }

                cameraBackground = value;
                RaisePropertyChanged(CameraBackgroundPropertyName);
            }
        }

        public const string ImagePropertyName = "Image";
        private Stream image;

        public Stream Image
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


        public RelayCommand<NavigationEventArgs> SetUpCameraCommand
        {
            get;
            private set;
        }

        public RelayCommand<NavigatingCancelEventArgs> CleanUpCameraCommand
        {
            get;
            private set;
        }

        public RelayCommand TakePhotoCommand
        {
            get;
            private set;
        }

        void cam_CaptureImageAvailable(object sender, Microsoft.Devices.ContentReadyEventArgs e)
        {
            Image = e.ImageStream;
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                App.RootFrame.Navigate(new Uri("/View/SendPage.xaml", UriKind.RelativeOrAbsolute));
            });
        }

    }
}
