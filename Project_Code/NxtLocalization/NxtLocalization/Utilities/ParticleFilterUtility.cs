using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace NxtLocalization.Utilities
{
    public enum HeadingDirectionName { East, West, South, North}

    /// <summary>
    /// Class defining methods that localize a robot using a particle filter.
    /// </summary>
    public class ParticleFilterUtility
    {
        #region fields

        //used only for visualizing the results
        private ObservableCollection<double> _xPositions;
        private ObservableCollection<double> _yPositions;
        private ObservableCollection<double> _weights;
        private int _uploadCounter = 0;

        //the number of particles used by the particle filter
        private readonly int _numberOfParticles;

        //the height of the world where the robot should localize itself
        private readonly double _worldHeight;

        //the width of the world where the robot should localize itself
        private readonly double _worldWidth;

        //a list of particles used by the algorithm
        private List<Particle> _particles;

        //a list containing the map of the world where the robot should localize itself
        private readonly List<List<double>> _worldMap;

        //used while correcting the robot's heading
        private double _desiredDirection;

        //an enumeration that denotes the current heading direction of the robot
        private HeadingDirectionName _currentHeadingDirection;

        #endregion

        #region constructors

        public ParticleFilterUtility(int numberOfParticles, double worldHeight, double worldWidth, string worldMapFileName)
        {
            this._numberOfParticles = numberOfParticles;
            this._worldHeight = worldHeight;
            this._worldWidth = worldWidth;
            this._particles = new List<Particle>();
            this._worldMap = new List<List<double>>();

            XDocument worldMap = XDocument.Load(Constants.ContentDirectory + worldMapFileName);
            List<XElement> rows = worldMap.Element("world").Elements().ToList();
            foreach (XElement row in rows)
            {
                List<XElement> columns = row.Elements().ToList();
                List<double> costs = columns.Select(column => Double.Parse(column.Value)).ToList();
                this._worldMap.Add(costs);
            }

            //used for visualizing the results
            this._xPositions = new ObservableCollection<double>();
            this._yPositions = new ObservableCollection<double>();
            this._weights = new ObservableCollection<double>();
        }

        #endregion

        #region methods

        /// <summary>
        /// Performs the particle filter algorithm by:
        ///     - moving the robot and the particles
        ///     - making sensor measurements
        ///     - updating the particle weights
        ///     - resampling the particles
        /// </summary>
        /// <param name="robotBehavior"> An object containing:
        /// - compass utility object.
        /// - odometry utility object.
        /// </param>
        public void ParticleFilter(ref LocalizeBehaviorInput robotBehavior)
        {
            this.InitializeParticles();

            for (int i = 0; i < 5; i++)
            {
                this.MoveRobotAndParticles(ref robotBehavior);
                List<double> distancesToObstacles = this.SenseWorld();
                this.UpdateParticleWeights(distancesToObstacles);
                this.ResampleParticles();
            }
        }

        /// <summary>
        /// Adds uniformly distributed particles to the world.
        /// </summary>
        private void InitializeParticles()
        {
            MainPage.LogText += "Initializing particles\n";

            Random randomGenerator = new Random();
            double weight = 1.0, weightSum = this._numberOfParticles * 1.0;

            //we add randomly distributed particles to the world
            for(int i=0; i<this._numberOfParticles; i++)
            {
                double x = randomGenerator.NextDouble() * (this._worldWidth - 1.0) + 1.0;
                double y = randomGenerator.NextDouble() * (this._worldHeight - 1.0) + 1.0;
                Particle newParticle = new Particle(x, y, weight);
                this._particles.Add(newParticle);

                //used for visuzalization purposes
                this._xPositions.Add(x);
                this._yPositions.Add(y);
            }

            //we normalize the particle weights, so that they form a valid probability distribution
            for (int i = 0; i < this._numberOfParticles; i++)
            {
                this._particles[i].Weight /= weightSum;
                this._weights.Add(this._particles[i].Weight);
            }

            //used for visualization
            NxtServiceClientWrapper.ServiceClient.SaveLocalizationDataAsync(this._xPositions, this._yPositions, this._weights, _uploadCounter);
            _uploadCounter++;
        }

        /// <summary>
        /// Moves the robot and all particles.
        /// </summary>
        /// <param name="robotBehavior"> An object containing:
        /// - compass utility object.
        /// - odometry utility object.
        /// </param>
        private void MoveRobotAndParticles(ref LocalizeBehaviorInput robotBehavior)
        {
            //we correct the heading of the robot to north, east, south, or west,
            //depending on the value of the current compass reading
            double currentHeading = robotBehavior.CompassUtility.CurrentCompassReading;

            //we make sure that the we have a compass reading; necessary in the first iteration
            while(Math.Abs(currentHeading - 0.0) < 0.0001)
                currentHeading = robotBehavior.CompassUtility.CurrentCompassReading;

            this.SetDesiredDirection(currentHeading);

            MainPage.LogText += "Correcting robot direction\n";

            bool correctHeadingDirection = Math.Abs(this._desiredDirection - robotBehavior.CompassUtility.CurrentCompassReading) < Constants.AllowedHeadingError;
            while (!correctHeadingDirection)
            {
                robotBehavior.CompassUtility.CorrectHeading(this._desiredDirection);
                correctHeadingDirection = Math.Abs(this._desiredDirection - robotBehavior.CompassUtility.CurrentCompassReading) < Constants.AllowedHeadingError;
            }

            int turnCounter = 0;

            //we measure if the robot is facing an obstacle and
            //turn it as long as an obstacle is on its way
            do
            {
                RobotBehavior.DistanceObtained = false;
                NxtServiceClientWrapper.ServiceClient.GetDistanceToObstacleAsync();
                while (!RobotBehavior.DistanceObtained) { }

                if (RobotBehavior.DistanceToObstacle < 30.0)
                {
                    MainPage.LogText += "Obstacle facing; correcting robot direction\n";

                    List<double> distancesToObstacles = this.SenseWorld(false);
                    this.UpdateParticleWeights(distancesToObstacles);

                    this.TurnRobotDirection();
                    correctHeadingDirection = Math.Abs(this._desiredDirection - robotBehavior.CompassUtility.CurrentCompassReading) < Constants.AllowedHeadingError;
                    while (!correctHeadingDirection)
                    {
                        robotBehavior.CompassUtility.CorrectHeading(this._desiredDirection);
                        correctHeadingDirection = Math.Abs(this._desiredDirection - robotBehavior.CompassUtility.CurrentCompassReading) < Constants.AllowedHeadingError;
                    }

                    turnCounter++;
                    MainPage.LogText += "Turn " + turnCounter + "\n";
                }
            //the robot has four directions to consider, so we don't want to allow it
            //to start checking from the initial direction again
            } while (RobotBehavior.DistanceToObstacle < 30.0 && turnCounter < 3);

            MainPage.LogText += "Moving robot\n";

            NxtServiceClientWrapper.ServiceClient.StopMotorsAsync();
            Thread.Sleep(750);
            NxtServiceClientWrapper.ServiceClient.GoForwardAsync();

            //the robot is not facing an obstacle anymore, so we allow it to move a predefined distance
            robotBehavior.OdometryUtility.ResetPosition();

            while (robotBehavior.OdometryUtility.Distances.X < Constants.AllowedDistanceToMove)
            {
                NxtServiceClientWrapper.ServiceClient.GetLeftMotorTachoCountAsync();
                NxtServiceClientWrapper.ServiceClient.GetRightMotorTachoCountAsync();
                Thread.Sleep(250);

                robotBehavior.OdometryUtility.CalculateDistances();
            }

            NxtServiceClientWrapper.ServiceClient.StopMotorsAsync();

            //we update the positions of the particles
            this.UpdateParticlePositions();
        }

        /// <summary>
        /// Performs a sensor measurement using the robot's ultrasonic sensor.
        /// After performing the actual measurement, calculates the distance
        /// from each particle to its nearest obstacle.
        /// </summary>
        /// <param name="measure">A variable indicating whether to perform an actual sensor measurement or not.</param>
        /// <returns>A list of distances from each particle to its nearest obstacle.</returns>
        private List<double> SenseWorld(bool measure = true)
        {
            MainPage.LogText += "Measuring distances\n";

            if (measure)
            {
                RobotBehavior.DistanceObtained = false;
                NxtServiceClientWrapper.ServiceClient.GetDistanceToObstacleAsync();
                while (!RobotBehavior.DistanceObtained) { }
            }

            List<double> distances = this.MeasureParticleDistancesToObstalces();
            return distances;
        }

        /// <summary>
        /// Updates the particle weights according to the obtained measurements.
        /// </summary>
        /// <param name="distancesToObstacles">A list containing distance from the i-th particle to the nearest obstacle.</param>
        private void UpdateParticleWeights(List<double> distancesToObstacles)
        {
            MainPage.LogText += "Updating particle weights\n";

            for(int i=0; i<this._numberOfParticles; i++)
            {
                if(this._particles[i].RobotPosition.X < 0.0 ||  this._particles[i].RobotPosition.X > this._worldWidth 
                   || this._particles[i].RobotPosition.Y < 0.0 ||  this._particles[i].RobotPosition.Y > this._worldHeight)
                {
                    this._particles[i].Weight = 0.0;
                }
                else
                    this._particles[i].Weight = this.SampleProbability(distancesToObstacles[i], RobotBehavior.DistanceToObstacle, 1.0);
            }
        }

        /// <summary>
        /// Resamples the particles by constructing a probability distribution of particles, where the number of times
        /// a particle is represented in the distribution is determined by the particle's weight.
        /// In addition to this weight-based resampling, adds 50 particles regardless of their weights
        /// in order to mitigate problems with incorrect measurements.
        /// </summary>
        private void ResampleParticles()
        {
            List<Particle> particleDistribution = new List<Particle>();
            int i;

            //looks for the particle whose weight is the highest;
            //this weight will later determine how many times a particle
            //will be represented in the resampling distribution
            Particle maxWeightParticle = this._particles[0];
            double maxWeight = this._particles[0].Weight;
            for(i = 1; i < this._numberOfParticles; i++)
            {
                if (this._particles[i].Weight > maxWeight)
                {
                    maxWeight = this._particles[i].Weight;
                    maxWeightParticle = new Particle(this._particles[i]);
                }
            }

            //determines how many leading decimal zeros does the highest weight have
            int leadingDecimalZeros = 0;
            while(maxWeight < 1.0)
            {
                maxWeight *= 10.0;
                leadingDecimalZeros++;
            }

            //we arbitrarily increase the number of leading zeros
            //for having more particles in the resampling distribution
            leadingDecimalZeros += 2;

            MainPage.LogText += "Creating particle distribution\n";
            //creates the particle distribution, such that the number of times a particle is represented
            //in this distribution is determined by the particle's weight,
            //which is multiplied by the number of leading decimal zeros
            for(i=0; i<this._numberOfParticles; i++)
            {
                int numberOfInstances = Convert.ToInt32(this._particles[i].Weight * (Math.Pow(10.0, leadingDecimalZeros)));
                for(int j=0; j<numberOfInstances; j++)
                    particleDistribution.Add(new Particle(this._particles[i]));
            }

            //if we don't have any particle in the distribution by any chance,
            //we add just one particle: the one with the highest weight
            if (particleDistribution.Count == 0)
                particleDistribution.Add(new Particle(maxWeightParticle));
            
            List<Particle> newParticles = new List<Particle>();
            Random randomNumberGenerator = new Random();
            double weightSum = 0.0;

            //we resample particle by choosing random particles
            //from the resampling distribution; we leave room for 50 particles
            //in the new particle distribution, however
            for(i=0; i<this._numberOfParticles - 50; i++)
            {
                int index = randomNumberGenerator.Next(0, particleDistribution.Count);
                newParticles.Add(new Particle(particleDistribution[index]));
                weightSum += particleDistribution[index].Weight;

                //used for visualization
                this._xPositions[i] = newParticles[i].RobotPosition.X;
                this._yPositions[i] = newParticles[i].RobotPosition.Y;
            }

            MainPage.LogText += "Resampling particles\n";

            //we want to avoid biased and incorrect measurements,
            //so we add some of the particles that might not have been considered
            for (i = i; i < this._numberOfParticles; i++)
            {
                int index = randomNumberGenerator.Next(0, this._numberOfParticles);
                newParticles.Add(new Particle(this._particles[index]));
                weightSum += this._particles[index].Weight;

                this._xPositions[i] = newParticles[i].RobotPosition.X;
                this._yPositions[i] = newParticles[i].RobotPosition.Y;
            }

            //we make the particle distribution a valid probability distribution
            //by dividing the each particle's weight by the sum of the weights
            for (i = 0; i < this._numberOfParticles; i++)
            {
                newParticles[i].Weight /= weightSum;
                this._weights[i] = newParticles[i].Weight;
            }
            this._particles = new List<Particle>(newParticles);

            //used for visualization
            NxtServiceClientWrapper.ServiceClient.SaveLocalizationDataAsync(this._xPositions, this._yPositions, this._weights, _uploadCounter);
            _uploadCounter++;
        }

        /// <summary>
        /// Calculates the probability that the measurement 'x' occurred in a distribution
        /// centered at 'mean' and with variance 'variance'. The calculated probability is a Gaussian probability.
        /// </summary>
        /// <param name="x">A sample measurement.</param>
        /// <param name="mean">Mean of the distribution.</param>
        /// <param name="variance">Variance of the distribution.</param>
        /// <returns>The probability of the sample in the underlying distribution.</returns>
        private double SampleProbability(double x, double mean, double variance)
        {
            double difference = Math.Abs(x - mean);
            double probability = 1.0 / Math.Sqrt(2.0 * Math.PI * variance) * Math.Exp( (difference * difference) / (2.0 * variance) * (-1.0));
            return probability;
        }

        /// <summary>
        /// Updates the positions of all particles after the robot has moved;
        /// uses the following convention while updating the positions of the particles:
        ///     - if the heading direction is north, the y position increases.
        ///     - if the heading direction is east, the x position increases.
        ///     - if the heading direction is south, the y position decreases.
        ///     - if the heading direction is west, the x position decreases.
        /// </summary>
        private void UpdateParticlePositions()
        {
            for(int i=0; i<this._numberOfParticles; i++)
            {
                if (this._currentHeadingDirection == HeadingDirectionName.North)
                    this._particles[i].RobotPosition.Y += 20.0;
                else if (this._currentHeadingDirection == HeadingDirectionName.East)
                    this._particles[i].RobotPosition.X += 20.0;
                else if (this._currentHeadingDirection == HeadingDirectionName.South)
                    this._particles[i].RobotPosition.Y -= 20.0;
                else if (this._currentHeadingDirection == HeadingDirectionName.West)
                    this._particles[i].RobotPosition.X -= 20.0;
            }
        }

        /// <summary>
        /// Calculates the distance from each particle to its nearest obstacle;
        /// uses the known map of the environment and the current robot's direction
        /// while calculating the distances.
        /// </summary>
        /// <returns>A list of distances from the particles to their nearest obstacles.</returns>
        private List<double> MeasureParticleDistancesToObstalces()
        {
            List<double> distances = new List<double>();

            for (int i = 0; i < this._numberOfParticles; i++)
            {
                //if the particle's position is outside the world dimensions,
                //we assign the value of the measurement to infinity
                if (this._particles[i].RobotPosition.X < 0.0 || this._particles[i].RobotPosition.X > this._worldWidth
                   || this._particles[i].RobotPosition.Y < 0.0 || this._particles[i].RobotPosition.Y > this._worldHeight)
                {
                    distances.Add(Constants.Infinity);
                }
                else
                {
                    int particleRow = (int)Math.Floor(this._particles[i].RobotPosition.Y / 20.0);
                    int particleColumn = (int)Math.Floor(this._particles[i].RobotPosition.X / 20.0);

                    int obstacleRow = particleRow, obstacleColumn = particleColumn;

                    //we look for the closest obstacle in order to obtain "measurements" for the current particle
                    if (this._currentHeadingDirection == HeadingDirectionName.North)
                    {
                        for (int row = particleRow + 1; row < this._worldMap.Count; row++)
                        {
                            if (Math.Abs(this._worldMap[row][obstacleColumn] - 100.0) < 0.05)
                            {
                                obstacleRow = row;
                                break;
                            }
                        }
                    }
                    else if (this._currentHeadingDirection == HeadingDirectionName.East)
                    {
                        for (int column = particleColumn + 1; column < this._worldMap[0].Count; column++)
                        {
                            if (Math.Abs(this._worldMap[obstacleRow][column] - 100.0) < 0.05)
                            {
                                obstacleColumn = column;
                                break;
                            }
                        }
                    }
                    else if (this._currentHeadingDirection == HeadingDirectionName.South)
                    {
                        for (int row = particleRow - 1; row >= 0; row--)
                        {
                            if (Math.Abs(this._worldMap[row][obstacleColumn] - 100.0) < 0.05)
                            {
                                obstacleRow = row;
                                break;
                            }
                        }
                    }
                    else if (this._currentHeadingDirection == HeadingDirectionName.West)
                    {
                        for (int column = particleColumn - 1; column >= 0; column--)
                        {
                            if (Math.Abs(this._worldMap[obstacleRow][column] - 100.0) < 0.05)
                            {
                                obstacleColumn = column;
                                break;
                            }
                        }
                    }

                    double distanceToObstacle = Constants.Infinity, obstaclePosition;
                    if (particleRow == obstacleRow && particleColumn == obstacleColumn)
                    {
                        if (this._currentHeadingDirection == HeadingDirectionName.North)
                            distanceToObstacle = this._worldHeight - this._particles[i].RobotPosition.Y;
                        else if (this._currentHeadingDirection == HeadingDirectionName.East)
                            distanceToObstacle = this._worldWidth - this._particles[i].RobotPosition.X;
                        else if (this._currentHeadingDirection == HeadingDirectionName.South)
                            distanceToObstacle = this._particles[i].RobotPosition.Y;
                        else if (this._currentHeadingDirection == HeadingDirectionName.West)
                            distanceToObstacle = this._particles[i].RobotPosition.X;
                    }
                    else if (particleRow != obstacleRow)
                    {
                        if (particleRow < obstacleRow)
                        {
                            obstaclePosition = obstacleRow * 20.0;
                            distanceToObstacle = obstaclePosition - this._particles[i].RobotPosition.Y;
                        }
                        else
                        {
                            obstaclePosition = (obstacleRow + 1) * 20.0;
                            distanceToObstacle = this._particles[i].RobotPosition.Y - obstaclePosition;
                        }
                    }
                    else if (particleColumn != obstacleColumn)
                    {
                        if (particleColumn < obstacleColumn)
                        {
                            obstaclePosition = obstacleColumn * 20.0;
                            distanceToObstacle = obstaclePosition - this._particles[i].RobotPosition.X;
                        }
                        else
                        {
                            obstaclePosition = (obstacleColumn + 1) * 20.0;
                            distanceToObstacle = this._particles[i].RobotPosition.X - obstaclePosition;
                        }
                    }

                    distances.Add(distanceToObstacle);
                }
            }

            return distances;
        }

        /// <summary>
        /// Sets a desired direction depending on the value of 'currentDirection';
        /// the current direction (north, east, south, or west) is chosen to be the one closest to
        /// the value of 'currentDirection'.
        /// </summary>
        /// <param name="currentDirection">The current direction of the robot.</param>
        private void SetDesiredDirection(double currentDirection)
        {
            if (Math.Abs(currentDirection - HeadingDirections.EastDirection) < Constants.AllowedHeadingError)
            {
                this._desiredDirection = HeadingDirections.EastDirection;
                this._currentHeadingDirection = HeadingDirectionName.East;
            }
            else if (Math.Abs(currentDirection - HeadingDirections.WestDirection) < Constants.AllowedHeadingError)
            {
                this._desiredDirection = HeadingDirections.WestDirection;
                this._currentHeadingDirection = HeadingDirectionName.West;
            }
            else if (Math.Abs(currentDirection - HeadingDirections.SouthDirection) < Constants.AllowedHeadingError)
            {
                this._desiredDirection = HeadingDirections.SouthDirection;
                this._currentHeadingDirection = HeadingDirectionName.South;
            }
            else
            {
                this._desiredDirection = HeadingDirections.NorthDirection;
                this._currentHeadingDirection = HeadingDirectionName.North;
            }
        }

        /// <summary>
        /// Chooses a new direction for the robot in a deterministic way, depending on the current direction:
        /// - current direction = north -> new direction = east
        /// - current direction = east -> new direction = south
        /// - current direction = south -> new direction = west
        /// - current direction = west -> new direction = north
        /// </summary>
        private void TurnRobotDirection()
        {
            if(this._currentHeadingDirection == HeadingDirectionName.North)
                this.SetDesiredDirection(HeadingDirections.EastDirection);
            else if(this._currentHeadingDirection == HeadingDirectionName.East)
                this.SetDesiredDirection(HeadingDirections.SouthDirection);
            else if (this._currentHeadingDirection == HeadingDirectionName.South)
                this.SetDesiredDirection(HeadingDirections.WestDirection);
            else if (this._currentHeadingDirection == HeadingDirectionName.West)
                this.SetDesiredDirection(HeadingDirections.NorthDirection);
        }

        #endregion
    }
}