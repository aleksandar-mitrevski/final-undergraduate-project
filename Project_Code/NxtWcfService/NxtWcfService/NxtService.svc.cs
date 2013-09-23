using System.Configuration;
using System.IO;
using System.Threading;
using NKH.MindSqualls;
using ObjectRecognition;
using NumberExtractor;

namespace NxtWcfService
{
    public class NxtService : INxtService
    {
        #region constants

        //COM port of the NXT bluetooth connection
        private const byte NxtComPort = 3;

        //motor power used by NXT's motors
        private const sbyte MotorPower = 20;

        //motor power used for one of the motors while turning the robot left or right
        private const sbyte TurnPower = -5;

        private const int NetworkInputNeurons = 256;
        private const int NetworkHiddenNeurons = 40;
        private const int NetworkOutputNeurons = 12;

        private const string ObjectRecognitionWeightsFile = "C:\\NxtPath\\classifierData\\Output Weights.txt";
        private const string NumbersToRecognizeFile = "C:\\NxtPath\\classifierData\\characters.txt";

        #endregion

        #region fields

        private static NxtBrick Brick;
        private Timer _timer;
        private bool _timeoutFinished;

        #endregion

        #region service contract methods

        #region object recognition methods

        public bool ClassifierTrained()
        {
            bool weightsFileExists = File.Exists(ObjectRecognitionWeightsFile);
            return weightsFileExists;
        }

        public void TrainClassifier()
        {
            ObjectRecognitionLibrary library = new ObjectRecognitionLibrary(NetworkInputNeurons, NetworkHiddenNeurons, NetworkOutputNeurons);
            library.TrainClassifier(0.05, 0.1, 100000);
        }

        public int[] ClassifyCharacters()
        {
            ObjectRecognitionLibrary library = new ObjectRecognitionLibrary(NetworkInputNeurons, NetworkHiddenNeurons, NetworkOutputNeurons);
            int[] classes = library.ClassifyCharacters(NumbersToRecognizeFile);
            return classes;
        }

        public bool SaveImage(Stream pictureData)
        {
            try
            {
                string uploadPath = ConfigurationManager.AppSettings["ImageUploadDirectory"];
                string fullName = string.Format("{0}\\{1}", uploadPath, "characterImage.jpg");

                using (FileStream fileStream = new FileStream(fullName, FileMode.Create, FileAccess.Write))
                {
                    pictureData.CopyTo(fileStream);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ExtractCharacters()
        {
            string imageFileName = "characterImage.jpg";

            NumberExtractorLibrary library = new NumberExtractorLibrary();
            bool processingSuccessful = library.ProcessImages(imageFileName);
            return processingSuccessful;
        }

        #endregion

        #region nxt related methods

        public void SaveLocalizationData(double[] xPositions, double[] yPositions, double[] weights, int counter)
        {
            string localizationFilesDirectory = ConfigurationManager.AppSettings["LocalizationDataDirectory"];
            string localizationFile = string.Format("{0}\\{1}.txt", localizationFilesDirectory, counter);

            using (StreamWriter writer = new StreamWriter(localizationFile, true))
            {
                string line = "";
                for(int i=0; i<xPositions.Length - 1; i++)
                    line += xPositions[i] + ",";
                line += xPositions[xPositions.Length - 1];
                writer.WriteLine(line);

                line = "";
                for (int i = 0; i < yPositions.Length - 1; i++)
                    line += yPositions[i] + ",";
                line += yPositions[yPositions.Length - 1];
                writer.WriteLine(line);

                line = "";
                for (int i = 0; i < weights.Length - 1; i++)
                    line += weights[i] + ",";
                line += weights[weights.Length - 1];
                writer.WriteLine(line);
            }
        }

        /// <summary>
        /// Establishes a bluetooth connection with an NXT brick,
        /// using 'NxtComPort' as port; assumes that the motors are
        /// connected to the NXT's B and C ports.
        /// </summary>
        public void ConnectBrick()
        {
            try
            {
                if (Brick == null || !Brick.IsConnected)
                {
                    Nxt2ColorSensor colorSensor = new Nxt2ColorSensor();

                    Brick = new NxtBrick(NxtCommLinkType.Bluetooth, NxtComPort)
                    {
                        MotorB = new NxtMotor(),
                        MotorC = new NxtMotor(),
                        Sensor1 = new NxtTouchSensor(),
                        Sensor2 = new NxtTouchSensor(),
                        Sensor3 = colorSensor,
                        Sensor4 = new NxtUltrasonicSensor()
                    };

                    colorSensor.SetLightSensorMode(Nxt2Color.Red);
                    Brick.Connect();
                }
            }
            catch
            {
                this.DisconnectBrick();
            }
        }

        /// <summary>
        /// Stops the connection with the NXT brick.
        /// </summary>
        public void DisconnectBrick()
        {
            if (Brick != null && Brick.IsConnected)
            {
                this.StopMotors();
                this.StopLight();

                Brick.Disconnect();
            }

            Brick = null;
        }

        /// <summary>
        /// Returns the cumulative tacho count for the left motor, which is assumed to be motor C.
        /// </summary>
        public int GetLeftMotorTachoCount()
        {
            try
            {
                Brick.MotorC.Poll();
                return Brick.MotorC.TachoCount.Value;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns the cumulative tacho count for the right motor, which is assumed to be motor B.
        /// </summary>
        public int GetRightMotorTachoCount()
        {
            try
            {
                Brick.MotorB.Poll();
                return Brick.MotorB.TachoCount.Value;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Resets the motor tachometers.
        /// </summary>
        public void ResetMotorTachometers()
        {
            try
            {
                Brick.MotorB.ResetMotorPosition(false);
                Brick.MotorC.ResetMotorPosition(false);
            }
            catch
            {
                this.DisconnectBrick();
            }
        }

        /// <summary>
        /// Runs motor C (assumed to be the left motor) with 'leftMotorPower',
        /// and motor B (assumed to be the right motor) with 'rightMotorPower'.
        /// </summary>
        /// <param name="leftMotorPower">The power we want to use for motor C.</param>
        /// <param name="rightMotorPower">The power we want to use for motor B.</param>
        public void MoveMotors(sbyte leftMotorPower, sbyte rightMotorPower)
        {
            try
            {
                Brick.MotorC.Run(leftMotorPower, 0);
                Brick.MotorB.Run(rightMotorPower, 0);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Moves both motors of the NXT forward with power equal to 'MotorPower'.
        /// Assumes that the motors are connected to the NXT's B and C ports.
        /// </summary>
        public void GoForward()
        {
            try
            {
                if (Brick != null && Brick.IsConnected)
                {
                    //int leftTacho = this.GetLeftMotorTachoCount();
                    //int rightTacho = this.GetRightMotorTachoCount();

                    //sbyte leftMotorPower = (sbyte)(MotorPower + 0.05 * (leftTacho - rightTacho));
                    //sbyte rightMotorPower = (sbyte)(MotorPower - 0.05 * (leftTacho - rightTacho));

                    //Brick.MotorB.Run(rightMotorPower, 0);
                    //Brick.MotorC.Run(leftMotorPower, 0);

                    this.StopMotors();

                    //right leg sensor
                    NxtTouchSensor rightTouchSensor = (NxtTouchSensor)Brick.Sensor1;

                    //left left sensor
                    NxtTouchSensor leftTouchSensor = (NxtTouchSensor)Brick.Sensor2;

                    this._timeoutFinished = false;
                    this._timer = new Timer(OnTimerTick, null, 5000, 5000);

                    rightTouchSensor.Poll();
                    while (!(rightTouchSensor.IsPressed.HasValue || this._timeoutFinished)) { }

                    this._timeoutFinished = false;

                    leftTouchSensor.Poll();
                    while (!(leftTouchSensor.IsPressed.HasValue || this._timeoutFinished)) { }

                    Brick.MotorC.Run(MotorPower, 0);
                    while (!leftTouchSensor.IsPressed.Value)
                    {
                        Thread.Sleep(100);
                        leftTouchSensor.Poll();
                    }
                    Brick.MotorC.Idle();

                    Brick.MotorB.Run(MotorPower, 0);
                    while (!rightTouchSensor.IsPressed.Value)
                    {
                        Thread.Sleep(100);
                        rightTouchSensor.Poll();
                    }

                    Thread.Sleep(1000);
                    Brick.MotorB.Idle();

                    Brick.MotorB.Run(MotorPower, 0);
                    Brick.MotorC.Run(MotorPower, 0);
                }
            }
            catch
            {
                this.DisconnectBrick();
            }
        }

        /// <summary>
        /// Moves both motors of the NXT backward with power equal to 'MotorPower'.
        /// Assumes that the motors are connected to the NXT's B and C ports.
        /// </summary>
        public void GoBackward()
        {
            //this.ConnectBrick();

            try
            {
                if (Brick != null && Brick.IsConnected)
                {
                    //this._motorSync.Run(MotorPower, ushort.MaxValue, 0);
                    Brick.MotorB.Run(-MotorPower, 0);
                    Brick.MotorC.Run(-MotorPower, 0);
                    //this.DisconnectBrick();
                }
            }
            catch
            {
                this.DisconnectBrick();
            }
        }

        /// <summary>
        /// Moves the left motor with power equal to 'MotorPower'
        /// and the right motor with power equal to 'TurnPower'.
        /// Assumes that the left motor is connected to the C port,
        /// while the right motor to the B port.
        /// </summary>
        public void GoLeft()
        {
            //this.ConnectBrick();

            try
            {
                if (Brick != null && Brick.IsConnected)
                {
                    Brick.MotorB.Run(TurnPower, 0);
                    Brick.MotorC.Run(MotorPower, 0);
                    //this.DisconnectBrick();
                }
            }
            catch
            {
                this.DisconnectBrick();
            }
        }

        /// <summary>
        /// Moves the right motor with power equal to 'MotorPower'
        /// and the left motor with power equal to 'TurnPower'.
        /// Assumes that the left motor is connected to the C port,
        /// while the right motor to the B port.
        /// </summary>
        public void GoRight()
        {
            //this.ConnectBrick();

            try
            {
                if (Brick != null && Brick.IsConnected)
                {
                    Brick.MotorB.Run(MotorPower, 0);
                    Brick.MotorC.Run(TurnPower, 0);
                    //this.DisconnectBrick();
                }
            }
            catch
            {
                this.DisconnectBrick();
            }
        }

        public byte GetDistanceToObstacle()
        {
            Brick.Sensor4.Poll();
            this._timer = new Timer(OnTimerTick, null, 5000, 5000);

            NxtUltrasonicSensor sensor = (NxtUltrasonicSensor)Brick.Sensor4;

            while (!(sensor.DistanceCm.HasValue || this._timeoutFinished))
            {
            }

            if (sensor.DistanceCm.HasValue)
            {
                string errorLogPath = ConfigurationManager.AppSettings["ErrorLogDirectory"];
                string errorLogFile = string.Format("{0}\\{1}", errorLogPath, "errorLog.txt");

                if (!this.FileOpen(errorLogFile))
                {
                    using (StreamWriter writer = new StreamWriter(errorLogFile, true))
                    {
                        writer.WriteLine("Sensor reading: " + sensor.DistanceCm.Value);
                    }
                }

                this._timer.Dispose();
                return sensor.DistanceCm.Value;
            }
            else
            {
                string errorLogPath = ConfigurationManager.AppSettings["ErrorLogDirectory"];
                string errorLogFile = string.Format("{0}\\{1}", errorLogPath, "errorLog.txt");

                if (!this.FileOpen(errorLogFile))
                {
                    using (StreamWriter writer = new StreamWriter(errorLogFile, true))
                    {
                        writer.WriteLine("No sensor reading");
                    }
                }
            }

            this._timer.Dispose();
            return 0;
        }

        public byte GetColorIntensity()
        {
            Brick.Sensor3.Poll();
            this._timer = new Timer(OnTimerTick, null, 5000, 5000);

            Nxt2ColorSensor sensor = (Nxt2ColorSensor)Brick.Sensor3;

            while (!(sensor.Intensity.HasValue || this._timeoutFinished))
            {
            }

            if (sensor.Intensity.HasValue)
            {
                string errorLogPath = ConfigurationManager.AppSettings["ErrorLogDirectory"];
                string errorLogFile = string.Format("{0}\\{1}", errorLogPath, "errorLog.txt");

                if (!this.FileOpen(errorLogFile))
                {
                    using (StreamWriter writer = new StreamWriter(errorLogFile, true))
                    {
                        writer.WriteLine("Sensor reading: " + sensor.Intensity.Value);
                    }
                }

                this._timer.Dispose();
                return sensor.Intensity.Value;
            }
            else
            {
                string errorLogPath = ConfigurationManager.AppSettings["ErrorLogDirectory"];
                string errorLogFile = string.Format("{0}\\{1}", errorLogPath, "errorLog.txt");

                if (!this.FileOpen(errorLogFile))
                {
                    using (StreamWriter writer = new StreamWriter(errorLogFile, true))
                    {
                        writer.WriteLine("No sensor reading");
                    }
                }
            }

            this._timer.Dispose();
            return 0;
        }

        /// <summary>
        /// Stops both motors, assuming that they are connected to NXT's B and C ports.
        /// </summary>
        public void StopMotors()
        {
            //this.ConnectBrick();

            try
            {
                if (Brick != null && Brick.IsConnected)
                {
                    //this._motorSync.Idle();
                    Brick.MotorB.Brake();
                    Brick.MotorC.Brake();
                }
            }
            catch
            {
                this.DisconnectBrick();
            }
        }

        /// <summary>
        /// Sets the light of the color sensor to black.
        /// Assumes that the color sensor is connected on port 3.
        /// </summary>
        public void StopLight()
        {
            try
            {
                if (Brick != null && Brick.IsConnected)
                {
                    Nxt2ColorSensor sensor = (Nxt2ColorSensor)Brick.Sensor3;
                    sensor.SetLightSensorMode(Nxt2Color.Black);
                }
            }
            catch
            {
                this.DisconnectBrick();
            }
        }

        #endregion

        #endregion

        #region private methods

        private void OnTimerTick(object state)
        {
            this._timeoutFinished = true;
        }

        private bool FileOpen(string filename)
        {
            FileStream stream = null;

            try
            {
                stream = File.Open(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }

        #endregion
    }
}