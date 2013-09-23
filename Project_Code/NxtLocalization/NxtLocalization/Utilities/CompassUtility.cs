using System;
using System.Threading;
using Microsoft.Devices.Sensors;

namespace NxtLocalization.Utilities
{
    /// <summary>
    /// Class defining a utility for making measurements with the phone's compass.
    /// </summary>
    public class CompassUtility : IDisposable
    {
        #region fields

        //reference to the phone's compass
        private Compass _compass;

        //stores the last compass reading
        public double CurrentCompassReading { get; set; }

        private bool _disposed;

        #endregion

        #region constructors

        public CompassUtility(int millisecondsBetweenUpdates)
        {
            this._compass = new Compass { TimeBetweenUpdates = TimeSpan.FromMilliseconds(millisecondsBetweenUpdates) };
            this._compass.CurrentValueChanged += CompassOnCurrentValueChanged;
            this._compass.Start();
            this._disposed = false;
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
                    if (this._compass != null)
                    {
                        this._compass.Dispose();
                        this._compass = null;
                    }
                }
            }

            this._disposed = true;
        }

        #endregion

        #region methods

        #region event handlers

        /// <summary>
        /// Saves the current compass reading in 'this.CurrentCompassReading'.
        /// </summary>
        private void CompassOnCurrentValueChanged(object sender, SensorReadingEventArgs<CompassReading> sensorReadingEventArgs)
        {
            this.CurrentCompassReading = sensorReadingEventArgs.SensorReading.TrueHeading;
        }

        #endregion

        #region utility methods

        /// <summary>
        /// Corrects the robot's heading direction by finding the clockwise and counterclockwise errors
        /// relative to the desired heading and moving the robot in the direction in which the error is smaller.
        /// If the heading error is less than 'RobotBehavior.AllowedHeadingError', returns without correcting the heading.
        /// </summary>
        /// <param name="desiredCompassReading">The desired direction for the robot.</param>
        public void CorrectHeading(double desiredCompassReading)
        {
            int desiredHeading = Convert.ToInt32(desiredCompassReading);
            int currentHeading = Convert.ToInt32(this.CurrentCompassReading);
            int headingError = desiredHeading - currentHeading;

            if (Math.Abs(headingError) < Constants.AllowedHeadingError)
                return;

            int clockwiseError = headingError % 360;

            //the modulo operator will return a negative number
            //if 'headingError' is negative, so we make sure that we 
            //get the correct answer by adding 360 to the clockwise error
            if (clockwiseError < 0)
                clockwiseError += 360;

            int counterclockwiseError = 360 - clockwiseError;

            //turn clockwise, or right
            if (clockwiseError <= counterclockwiseError)
                NxtServiceClientWrapper.ServiceClient.GoRightAsync();
            //turn counterclockwise, or left
            else
                NxtServiceClientWrapper.ServiceClient.GoLeftAsync();
            Thread.Sleep(250);
        }

        #endregion

        #endregion
    }
}