using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Phone.Controls;

using NxtLocalization.Utilities;
using NxtLocalization.AStar;

namespace NxtLocalization
{
    public partial class MainPage : PhoneApplicationPage
    {
        #region fields

        private readonly DispatcherTimer _timer;
        private AStarLibrary _aStarLibrary;
        private RobotBehavior _robotBehavior;
        private FindGoalBehaviorInput _desiredRobotBehavior;
        private LocalizeBehaviorInput _localizationInput;
        private PictureCapturingUtility _pictureCapturingUtility;

        private List<int> _charactersIdentified = new List<int>();

        public static double Heading = 0.0;
        private bool _userAbortedOperation;

        public static string LogText = "";

        #region background workers

        public static BackgroundWorker FindGoalBackgroundWorker;
        public static BackgroundWorker LocalizeBackgroundWorker;
        public static BackgroundWorker RecognizeCharactersBackgroundWorker;

        #endregion

        #endregion

        #region constructors

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this._timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            this._timer.Tick += TimerOnTick;

            //use this for pathfinding
            this._robotBehavior = new RobotBehavior();

            //use this for character recognition
            //this._pictureCapturingUtility = new PictureCapturingUtility(ref viewfinderBrush);
            //NxtServiceClientWrapper.ServiceClient.ClassifierTrainedAsync();

            //use this for voice recognition
            //this._soundRecorder = new SoundRecorder();

            FindGoalBackgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            FindGoalBackgroundWorker.DoWork += FindGoalBackgroundWorkerOnDoWork;
            FindGoalBackgroundWorker.RunWorkerCompleted += FindGoalBackgroundWorkerOnRunWorkerCompleted;

            LocalizeBackgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            LocalizeBackgroundWorker.DoWork += LocalizeBackgroundWorkerOnDoWork;
            LocalizeBackgroundWorker.RunWorkerCompleted += LocalizeBackgroundWorkerOnRunWorkerCompleted;

            RecognizeCharactersBackgroundWorker = new BackgroundWorker { WorkerSupportsCancellation = true };
            RecognizeCharactersBackgroundWorker.DoWork += RecognizeCharactersBackgroundWorkerOnDoWork;
            RecognizeCharactersBackgroundWorker.RunWorkerCompleted += RecognizeCharactersBackgroundWorkerOnRunWorkerCompleted;
        }

        #endregion

        #region event handlers

        #region background workers event handlers

        private void FindGoalBackgroundWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            this._desiredRobotBehavior = new FindGoalBehaviorInput { OdometryUtility = new OdometryUtility() };
            this._aStarLibrary = new AStarLibrary("WorldMap.xml");
            this._desiredRobotBehavior.GoalBehaviorInput = this._aStarLibrary.FindShortestPath(new Coordinates2D(4, 0), new Coordinates2D(1, 2));
            this._desiredRobotBehavior.CompassUtility = new CompassUtility(1000);

            this._robotBehavior = new RobotBehavior { RobotCoordinates = new Coordinates2D(4, 0) };
            this._robotBehavior.FindGoal(ref this._desiredRobotBehavior);
        }

        private void LocalizeBackgroundWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            this._localizationInput = new LocalizeBehaviorInput
            {
                NumberOfParticles = 500,
                WorldDimensions = new Vector2D(100.0, 100.0),
                WorldMapFileName = "WorldMap.xml",
                OdometryUtility = new OdometryUtility(),
                CompassUtility = new CompassUtility(1000)
            };

            this._robotBehavior.Localize(ref this._localizationInput);
        }

        private void RecognizeCharactersBackgroundWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            this._pictureCapturingUtility = new PictureCapturingUtility(ref viewfinderBrush);
            NxtServiceClientWrapper.ServiceClient.ClassifierTrainedAsync();
            this._charactersIdentified = this._robotBehavior.RecognizeCharacters(this._pictureCapturingUtility);
        }

        private void FindGoalBackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            this._desiredRobotBehavior.Dispose();
        }

        private void LocalizeBackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            this._localizationInput.Dispose();
        }

        private void RecognizeCharactersBackgroundWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            this._pictureCapturingUtility.Dispose();

            if (!this._userAbortedOperation)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    string operand1String = string.Empty, operand2String = string.Empty, mathOperator = string.Empty;

                    foreach (int character in this._charactersIdentified)
                    {
                        if(string.IsNullOrEmpty(mathOperator))
                        {
                            if (character < 10)
                                operand1String += character.ToString();
                            else if (character == 10)
                                mathOperator = "+";
                            else if (character == 11)
                                mathOperator = "-";
                        }
                        else
                        {
                            if(character < 10)
                                operand2String += character.ToString();
                        }

                        int operand1 = int.Parse(operand1String), operand2 = int.Parse(operand2String);
                        int result = 0;
                        if (mathOperator == "+")
                            result = operand1 + operand2;
                        else if (mathOperator == "-")
                            result = operand1 - operand2;
                        this.textBlockReading.Text = result.ToString();
                    }
                });
            }
        }

        #endregion

        #region button event handlers

        private void BtnConnectClick(object sender, RoutedEventArgs e)
        {
            NxtServiceClientWrapper.ServiceClient.ConnectBrickAsync();
            while(!NxtServiceClientWrapper.BrickConnected){ }

            btnConnect.IsEnabled = false;
            btnFindGoal.IsEnabled = true;
            btnLocalize.IsEnabled = true;
            btnRecognizeCharacters.IsEnabled = true;
            btnDisconnect.IsEnabled = true;
            //this._timer.Start();

            //this._desiredRobotBehavior.CompassUtility = new CompassUtility(2000);
            //FindGoalBackgroundWorker.RunWorkerAsync();
            
            this._timer.Start();
            //FindGoalBackgroundWorker.RunWorkerAsync();
        }

        private void BtnDisconnectClick(object sender, RoutedEventArgs e)
        {
            NxtServiceClientWrapper.ServiceClient.DisconnectBrickAsync();
            while(NxtServiceClientWrapper.BrickConnected){ }

            btnConnect.IsEnabled = true;
            btnFindGoal.IsEnabled = false;
            btnLocalize.IsEnabled = false;
            btnRecognizeCharacters.IsEnabled = false;
            btnDisconnect.IsEnabled = false;

            if(FindGoalBackgroundWorker.IsBusy)
                FindGoalBackgroundWorker.CancelAsync();
            if(LocalizeBackgroundWorker.IsBusy)
                LocalizeBackgroundWorker.CancelAsync();
            if(RecognizeCharactersBackgroundWorker.IsBusy)
                RecognizeCharactersBackgroundWorker.CancelAsync();

            this._userAbortedOperation = true;
            this._timer.Stop();
        }

        private void BtnFindGoalClick(object sender, RoutedEventArgs e)
        {
            FindGoalBackgroundWorker.RunWorkerAsync();
            //this._desiredRobotBehavior.GoalBehaviorInput = this._aStarLibrary.FindShortestPath(new Coordinates2D(4, 0), new Coordinates2D(0, 1));
            //this._desiredRobotBehavior.GoalBehaviorInput = this._aStarLibrary.FindShortestPath(new Coordinates2D(4, 0), new Coordinates2D(3, 1));
        }

        private void BtnLocalizeClick(object sender, RoutedEventArgs e)
        {
            LocalizeBackgroundWorker.RunWorkerAsync();
        }

        private void BtnRecognizeCharactersClick(object sender, RoutedEventArgs e)
        {
            RecognizeCharactersBackgroundWorker.RunWorkerAsync();
        }

        #endregion

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            this.textBlockReading.Text = Heading.ToString();
            //this.textBlockReading.Text = LogText;

            //this._serviceClient.GetDistanceToObstacleAsync();
            //if (NxtServiceClientWrapper.BrickConnected)
            //    NxtServiceClientWrapper.ServiceClient.GetColorIntensityAsync();

            //FindGoalBackgroundWorker.RunWorkerAsync();
            //this._timer.Stop();

            //this.textBlockReading.Text = Heading + "";

            //if (RobotBehavior.MotorsStopped)
            //{
            //    NxtServiceClientWrapper.ServiceClient.GetLeftMotorTachoCountAsync();
            //    NxtServiceClientWrapper.ServiceClient.GetRightMotorTachoCountAsync();

            //    Vector2D distances = this._desiredRobotBehavior.OdometryUtility.CalculateDistances(this._robotBehavior.DesiredHeading);
            //    this.textBlockReading.Text = distances.X + ", " + distances.Y;
            //}
        }

        #endregion
    }
}