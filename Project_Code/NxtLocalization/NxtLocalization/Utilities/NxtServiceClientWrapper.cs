using System.ComponentModel;
using System.Linq;
using NxtLocalization.NxtService;

namespace NxtLocalization.Utilities
{
    public class NxtServiceClientWrapper
    {
        #region fields

        //client for communicating with the WCF service used by the windows app
        public static NxtServiceClient ServiceClient { get; set; }

        //indicates whether the WCF has successfully connected to the NXT brick
        public static bool BrickConnected { get; set; }

        #endregion

        #region constructors

        static NxtServiceClientWrapper()
        {
            ServiceClient = new NxtServiceClient();

            ServiceClient.ConnectBrickCompleted += ServiceClientOnConnectBrickCompleted;
            ServiceClient.DisconnectBrickCompleted += ServiceClientOnDisconnectBrickCompleted;
            ServiceClient.ClassifierTrainedCompleted += ServiceClientOnClassifierTrainedCompleted;
            ServiceClient.TrainClassifierCompleted += ServiceClientOnTrainClassifierCompleted;
            ServiceClient.ClassifyCharactersCompleted += ServiceClientOnClassifyCharactersCompleted;
            ServiceClient.SaveImageCompleted += ServiceClientOnSaveImageCompleted;
            ServiceClient.ExtractCharactersCompleted += ServiceClientOnExtractCharactersCompleted;
            ServiceClient.StopMotorsCompleted += ServiceClientOnStopMotorsCompleted;
            ServiceClient.GetDistanceToObstacleCompleted += ServiceClientOnGetDistanceToObstacleCompleted;
            ServiceClient.GetLeftMotorTachoCountCompleted += ServiceClientOnGetLeftMotorTachoCountCompleted;
            ServiceClient.GetRightMotorTachoCountCompleted += ServiceClientOnGetRightMotorTachoCountCompleted;
            
            BrickConnected = false;
        }

        #endregion

        #region event handlers

        /// <summary>
        /// Assigns 'BrickConnected' to true.
        /// </summary>
        private static void ServiceClientOnConnectBrickCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            BrickConnected = true;
        }

        /// <summary>
        /// Assigns 'BrickConnected' to false.
        /// </summary>
        private static void ServiceClientOnDisconnectBrickCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            BrickConnected = false;
        }

        private static void ServiceClientOnClassifierTrainedCompleted(object sender, ClassifierTrainedCompletedEventArgs classifierTrainedCompletedEventArgs)
        {
            bool classifierTrained = classifierTrainedCompletedEventArgs.Result;
            if (!classifierTrained) { }
            //ServiceClient.TrainClassifierAsync();
            else
                RobotBehavior.ClassifierTrained = true;
        }
        
        /// <summary>
        /// Assigns 'RobotBehavior.ClassifierTrained' to true.
        /// </summary>
        private static void ServiceClientOnTrainClassifierCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            RobotBehavior.ClassifierTrained = true;
        }

        /// <summary>
        /// Saves the returned list of characters in 'RobotBehavior.IdentifiedCharacters'
        /// and assigns 'RobotBehavior.CharactersClassified' to true.
        /// </summary>
        private static void ServiceClientOnClassifyCharactersCompleted(object sender, ClassifyCharactersCompletedEventArgs classifyCharactersCompletedEventArgs)
        {
            RobotBehavior.IdentifiedCharacters = classifyCharactersCompletedEventArgs.Result.ToList();
            RobotBehavior.CharactersClassified = true;
        }

        /// <summary>
        /// Calls a method for extracting characters from the saved image.
        /// </summary>
        private static void ServiceClientOnSaveImageCompleted(object sender, SaveImageCompletedEventArgs saveImageCompletedEventArgs)
        {
            ServiceClient.ExtractCharactersAsync();
        }

        /// <summary>
        /// Saves the extracted characters in 'RobotBehavior.CharactersExtracted'.
        /// </summary>
        private static void ServiceClientOnExtractCharactersCompleted(object sender, ExtractCharactersCompletedEventArgs extractCharactersCompletedEventArgs)
        {
            RobotBehavior.CharactersExtracted = extractCharactersCompletedEventArgs.Result;
        }

        /// <summary>
        /// Saves the measured distance in 'RobotBehavior.DistanceToObstacle'
        /// and assigns 'RobotBehavior.DistanceObtained' to true.
        /// </summary>
        private static void ServiceClientOnGetDistanceToObstacleCompleted(object sender, GetDistanceToObstacleCompletedEventArgs getDistanceToObstacleCompletedEventArgs)
        {
            RobotBehavior.DistanceToObstacle = getDistanceToObstacleCompletedEventArgs.Result;
            RobotBehavior.DistanceObtained = true;
        }

        /// <summary>
        /// Calls a method for resetting the motor tachometers.
        /// </summary>
        private static void ServiceClientOnStopMotorsCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            ServiceClient.ResetMotorTachometersAsync();
        }

        /// <summary>
        /// Saves the previous tacho count in 'OdometryUtility.PreviousLeftMotorTachoCount'
        /// and the new tacho count in 'OdometryUtility.LeftMotorTachoCount'.
        /// </summary>
        private static void ServiceClientOnGetLeftMotorTachoCountCompleted(object sender, GetLeftMotorTachoCountCompletedEventArgs getLeftMotorTachoCountCompletedEventArgs)
        {
            OdometryUtility.PreviousLeftMotorTachoCount = OdometryUtility.LeftMotorTachoCount;
            OdometryUtility.LeftMotorTachoCount = getLeftMotorTachoCountCompletedEventArgs.Result;
        }

        /// <summary>
        /// Saves the previous tacho count in 'OdometryUtility.PreviousRightMotorTachoCount'
        /// and the new tacho count in 'OdometryUtility.RightMotorTachoCount'.
        /// </summary>
        private static void ServiceClientOnGetRightMotorTachoCountCompleted(object sender, GetRightMotorTachoCountCompletedEventArgs getRightMotorTachoCountCompletedEventArgs)
        {
            OdometryUtility.PreviousRightMotorTachoCount = OdometryUtility.RightMotorTachoCount;
            OdometryUtility.RightMotorTachoCount = getRightMotorTachoCountCompletedEventArgs.Result;
        }

        #endregion
    }
}