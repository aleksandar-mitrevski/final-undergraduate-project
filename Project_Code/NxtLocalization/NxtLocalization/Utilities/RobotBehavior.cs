using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NxtLocalization.AStar;

namespace NxtLocalization.Utilities
{
    public class RobotBehavior
    {
        #region fields

        //stores a heading that the robot wants to reach
        public double DesiredHeading { get; set; }

        //stores the current robot coordinates within a known grid
        public Coordinates2D RobotCoordinates { get; set; }

        //used to indicate whether the character classifier is trained or not
        public static bool ClassifierTrained;

        //indicates whether characters have been extracted from an image or not
        public static bool CharactersExtracted;

        //indicates whether the extracted characters have been classified or not
        public static bool CharactersClassified;

        //used to indicate whether a distance to an obstacle has been measured or not
        public static bool DistanceObtained;

        //used to store the distance to an obstacle
        public static double DistanceToObstacle;

        //used to store a list of classified characters
        public static List<int> IdentifiedCharacters;

        #endregion

        #region constructors

        static RobotBehavior()
        {
            CharactersExtracted = false;
            ClassifierTrained = false;
            CharactersClassified = false;

            IdentifiedCharacters = new List<int>();
        }

        public RobotBehavior()
        {
            this.DesiredHeading = 0.0;
            this.RobotCoordinates = new Coordinates2D(0, 0);
        }

        #endregion

        #region methods

        /// <summary>
        /// If the character classifier is trained, captures an image with the phone's camera,
        /// extracts the characters from the image, and classifies them.
        /// </summary>
        /// <param name="pictureCapturingUtility">Object containing a reference to the phone's camera.</param>
        /// <returns>A list of extracted characters. Returns an empty list if the classifier is not trained.</returns>
        public List<int> RecognizeCharacters(PictureCapturingUtility pictureCapturingUtility)
        {
            if(ClassifierTrained)
            {
                pictureCapturingUtility.CaptureImage();

                while(!CharactersExtracted){ }
                NxtServiceClientWrapper.ServiceClient.ClassifyCharactersAsync();

                while(!CharactersClassified){ }

                CharactersExtracted = false;
                CharactersClassified = false;

                return IdentifiedCharacters;
            }

            return new List<int>();
        }

        /// <summary>
        /// Localizes the robot.
        /// </summary>
        /// <param name="robotBehavior">An object containing input parameters for the localization algorithm.</param>
        public void Localize(ref LocalizeBehaviorInput robotBehavior)
        {
            ParticleFilterUtility particleFilterUtility = new ParticleFilterUtility(robotBehavior.NumberOfParticles, robotBehavior.WorldDimensions.X, robotBehavior.WorldDimensions.Y, robotBehavior.WorldMapFileName);
            particleFilterUtility.ParticleFilter(ref robotBehavior);
        }

        /// <summary>
        /// Leads the robot to a desired goal location, assuming that the initial position in the world is known.
        /// </summary>
        /// <param name="desiredRobotBehavior">An object containing input parameters for the pathfinding algorithm.</param>
        public void FindGoal(ref FindGoalBehaviorInput desiredRobotBehavior)
        {
            //we start from the initial node in the shortest path and 
            //direct the robot to the goal location by considering one node at a time
            Coordinates2D goalCoordinates = desiredRobotBehavior.GoalBehaviorInput.ShortestPath.Last();
            int currentNodeCounter = 1;

            while (this.RobotCoordinates != goalCoordinates)
            {
                //we take the next node that we want to reach
                Coordinates2D nextNode = desiredRobotBehavior.GoalBehaviorInput.ShortestPath.ElementAt(currentNodeCounter);

                //we change the robot's heading towards the next desired node
                this.ChangeDirection(nextNode);

                //we correct the heading as long as it differs from the desired one by a significant difference
                bool correctHeadingDirection = Math.Abs(this.DesiredHeading - desiredRobotBehavior.CompassUtility.CurrentCompassReading) < Constants.AllowedHeadingError;
                while (!correctHeadingDirection)
                {
                    desiredRobotBehavior.CompassUtility.CorrectHeading(this.DesiredHeading);
                    correctHeadingDirection = Math.Abs(this.DesiredHeading - desiredRobotBehavior.CompassUtility.CurrentCompassReading) < Constants.AllowedHeadingError;
                }

                //we reset any existing distance measurements
                desiredRobotBehavior.OdometryUtility.ResetPosition();

                //we allow the robot to come to a stop before moving it towards the next node
                NxtServiceClientWrapper.ServiceClient.StopMotorsAsync();
                Thread.Sleep(750);
                NxtServiceClientWrapper.ServiceClient.GoForwardAsync();

                //we allow the robot to move some allowed distance
                while (desiredRobotBehavior.OdometryUtility.Distances.X < Constants.AllowedDistanceToMove)
                {
                    NxtServiceClientWrapper.ServiceClient.GetLeftMotorTachoCountAsync();
                    NxtServiceClientWrapper.ServiceClient.GetRightMotorTachoCountAsync();
                    Thread.Sleep(250);

                    desiredRobotBehavior.OdometryUtility.CalculateDistances(this.DesiredHeading);
                }

                //we stop the motors and assign the current position of the robot
                //to the coordinates of the node that we wanted to reach in the current iteration
                currentNodeCounter++;
                NxtServiceClientWrapper.ServiceClient.StopMotorsAsync();
                Thread.Sleep(500);
                this.RobotCoordinates.Copy(nextNode);
            }
        }

        /// <summary>
        /// Changes the heading of the robot using the following convention:
        /// - if the desired cell is above the current node, we set the heading to north.
        /// - if the desired cell is below the current node, we set the heading to south.
        /// - if the desired cell is left of the current node, we set the heading to west.
        /// - if the desired cell is right of the current node, we set the heading to east.
        /// </summary>
        /// <param name="nextDestination">The coordinates of a grid cell that we want to reach.</param>
        public void ChangeDirection(Coordinates2D nextDestination)
        {
            //we are going right/east
            if (this.RobotCoordinates.X == nextDestination.X && this.RobotCoordinates.Y < nextDestination.Y)
                this.DesiredHeading = HeadingDirections.EastDirection;
            //we are going left/west
            else if (this.RobotCoordinates.X == nextDestination.X && this.RobotCoordinates.Y > nextDestination.Y)
                this.DesiredHeading = HeadingDirections.WestDirection;
            //we are going down/south
            else if (this.RobotCoordinates.X < nextDestination.X && this.RobotCoordinates.Y == nextDestination.Y)
                this.DesiredHeading = HeadingDirections.SouthDirection;
            //we are going up/north
            else if (this.RobotCoordinates.X > nextDestination.X && this.RobotCoordinates.Y == nextDestination.Y)
                this.DesiredHeading = HeadingDirections.NorthDirection;
        }

        #endregion
    }
}