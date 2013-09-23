using System;

namespace NxtLocalization.Utilities
{
    /// <summary>
    /// Class containing fields that allow 'RobotBehavior' to localize the robot.
    /// </summary>
    public class LocalizeBehaviorInput : IDisposable
    {
        #region fields

        //utility used for measuring the distance traveled by the robot
        public OdometryUtility OdometryUtility { get; set; }

        //utility used for navigation in the desired direction
        public CompassUtility CompassUtility { get; set; }

        //number of particles used by the particle filter
        public int NumberOfParticles { get; set; }

        //dimensions of the world where the robot should localize itself
        public Vector2D WorldDimensions { get; set; }

        //name of a file containing description of the world where the robot should localize itself
        public string WorldMapFileName { get; set; }
        
        private bool _disposed;

        #endregion

        #region constructors

        public LocalizeBehaviorInput()
        {
            this.OdometryUtility = new OdometryUtility();
            this.CompassUtility = null;
            this.NumberOfParticles = 0;
            this.WorldDimensions = new Vector2D();
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
                    if (this.CompassUtility != null)
                        this.CompassUtility.Dispose();
                }
            }

            this._disposed = true;
        }

        #endregion
    }
}