using System.Collections.Generic;
using System.IO;
using System.ServiceModel;

namespace NxtWcfService
{
    [ServiceContract]
    public interface INxtService
    {
        #region object recognition methods

        [OperationContract]
        bool ClassifierTrained();

        [OperationContract]
        void TrainClassifier();

        [OperationContract]
        int[] ClassifyCharacters();

        [OperationContract]
        bool SaveImage(Stream pictureData);

        [OperationContract]
        bool ExtractCharacters();

        #endregion

        #region nxt related methods

        [OperationContract]
        void SaveLocalizationData(double[] xPositions, double[] yPositions, double[] weights, int counter);

        [OperationContract]
        void ConnectBrick();

        [OperationContract]
        void DisconnectBrick();

        [OperationContract]
        int GetLeftMotorTachoCount();

        [OperationContract]
        int GetRightMotorTachoCount();

        [OperationContract]
        void ResetMotorTachometers();

        [OperationContract]
        void MoveMotors(sbyte leftMotorPower, sbyte rightMotorPower);

        [OperationContract]
        void GoForward();

        [OperationContract]
        void GoBackward();

        [OperationContract]
        void GoLeft();

        [OperationContract]
        void GoRight();

        [OperationContract]
        byte GetDistanceToObstacle();

        [OperationContract]
        byte GetColorIntensity();

        [OperationContract]
        void StopMotors();

        [OperationContract]
        void StopLight();

        #endregion
    }
}