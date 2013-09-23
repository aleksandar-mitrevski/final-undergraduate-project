using System;
using NxtLocalization.AStar;

namespace NxtLocalization.Utilities
{
    /// <summary>
    /// Class containing fields that allow 'RobotBehavior' to guide the robot to a desired goal location.
    /// </summary>
    public class FindGoalBehaviorInput : IDisposable
    {
        #region fields

        //results produced by the A* algorithm; used for guiding the robot to the desired direction
        public AStarResult GoalBehaviorInput { get; set; }

        //utility used for navigation in the desired direction
        public CompassUtility CompassUtility { get; set; }

        //utility used for measuring the distance traveled by the robot
        public OdometryUtility OdometryUtility { get; set; }

        private bool _disposed;

        #endregion

        #region constructors

        public FindGoalBehaviorInput()
        {
            this.GoalBehaviorInput = new AStarResult();
            this.OdometryUtility = new OdometryUtility();
            this.CompassUtility = null;
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