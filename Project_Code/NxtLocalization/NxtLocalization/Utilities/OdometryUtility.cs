using System;

namespace NxtLocalization.Utilities
{
    /// <summary>
    /// Class used for keeping track of the distance traveled by the robot
    /// using the NXT motor tachometers.
    /// </summary>
    public class OdometryUtility
    {
        #region fields

        //indicates the distance traveled in the x and y directions
        public Vector2D Distances { get; set; }

        //stores the tacho count of the left motor
        public static double LeftMotorTachoCount;

        //stores the tacho count of the right motor
        public static double RightMotorTachoCount;

        //stores the previous tacho count of the left motor
        public static double PreviousLeftMotorTachoCount;

        //stores the previous tacho count of the right motor
        public static double PreviousRightMotorTachoCount;

        #endregion

        #region constructors

        static OdometryUtility()
        {
            LeftMotorTachoCount = 0;
            RightMotorTachoCount = 0;
            PreviousLeftMotorTachoCount = 0;
            PreviousRightMotorTachoCount = 0;
        }

        public OdometryUtility()
        {
            LeftMotorTachoCount = 0;
            RightMotorTachoCount = 0;
            PreviousLeftMotorTachoCount = 0;
            PreviousRightMotorTachoCount = 0;

            this.Distances = new Vector2D();
        }

        #endregion

        #region methods

        /// <summary>
        /// Resets the vector containing traveled distances.
        /// </summary>
        public void ResetPosition()
        {
            this.Distances.Reset();
        }

        /// <summary>
        /// Calculates traveled distances; models the robot as a differential drive.
        /// Updates only the x direction currently.
        /// </summary>
        /// <param name="desiredHeading">The robot's heading.</param>
        /// <returns>A vector containing the traveled distance.</returns>
        public Vector2D CalculateDistances(double desiredHeading = 0.0)
        {
            desiredHeading = 0.0;
            
            double leftMotorTachoDelta = LeftMotorTachoCount - PreviousLeftMotorTachoCount;
            double rightMotorTachoDelta = RightMotorTachoCount - PreviousRightMotorTachoCount;

            double leftMotorDistance = 2 * Math.PI * Constants.MotorRadius * (leftMotorTachoDelta / 360.0);
            double rightMotorDistance = 2 * Math.PI * Constants.MotorRadius * (rightMotorTachoDelta / 360.0);
            double bothMotorDistances = (leftMotorDistance + rightMotorDistance) / 2.0;

            this.Distances += new Vector2D(bothMotorDistances * Math.Cos(desiredHeading), bothMotorDistances * Math.Sin(desiredHeading));
            return this.Distances;
        }

        #endregion
    }
}