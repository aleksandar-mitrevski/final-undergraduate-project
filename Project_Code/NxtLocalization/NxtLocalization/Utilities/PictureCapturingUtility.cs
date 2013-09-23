using System;
using System.Windows.Media;
using Microsoft.Devices;
using System.IO;

namespace NxtLocalization.Utilities
{
    /// <summary>
    /// Class defining a utility for taking pictures with the phone's camera.
    /// </summary>
    public class PictureCapturingUtility : IDisposable
    {
        #region fields

        //reference to the phone's camera
        private PhotoCamera _camera;
        private bool _disposed;

        #endregion

        #region constructors

        public PictureCapturingUtility(ref VideoBrush videoBrush)
        {
            this._disposed = false;

            if ((PhotoCamera.IsCameraTypeSupported(CameraType.Primary) == true) || (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing) == true))
            {
                if (PhotoCamera.IsCameraTypeSupported(CameraType.FrontFacing))
                    this._camera = new PhotoCamera(CameraType.FrontFacing);
                else
                    this._camera = new PhotoCamera(CameraType.Primary);

                this._camera.CaptureImageAvailable += CameraImageAvailable;

                videoBrush.SetSource(this._camera);
            }
        }

        #endregion

        #region destructors

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    this._camera.Dispose();
                    this._camera.CaptureImageAvailable -= CameraImageAvailable;
                }
            }

            this._disposed = true;
        }

        #endregion

        #region methods

        /// <summary>
        /// Takes a picture with the phone's camera;
        /// note that the method doesn't take the direction into account.
        /// </summary>
        public void CaptureImage()
        {
            if(this._camera != null)
                this._camera.CaptureImage();
        }

        /// <summary>
        /// Calls a method in ''NxtServiceClientWrapper for saving the captured image to local PC storage.
        /// </summary>
        private void CameraImageAvailable(object sender, ContentReadyEventArgs e)
        {
            try
            {
                e.ImageStream.Seek(0, SeekOrigin.Begin);

                byte[] imageBytes = new byte[1024];
                byte[] imageStream;
                using (MemoryStream stream = new MemoryStream())
                {
                    while (e.ImageStream.Read(imageBytes, 0, imageBytes.Length) > 0)
                        stream.Write(imageBytes, 0, imageBytes.Length);

                    imageStream = stream.ToArray();
                    NxtServiceClientWrapper.ServiceClient.SaveImageAsync(imageStream);
                }
            }
            finally
            {
                e.ImageStream.Close();
            }
        }

        #endregion
    }
}