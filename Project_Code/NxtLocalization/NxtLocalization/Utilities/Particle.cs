namespace NxtLocalization.Utilities
{
    /// <summary>
    /// Class defining a particle used by the particle filter algorithm.
    /// </summary>
    public class Particle
    {
        #region fields

        //stores the position of the particle
        public Vector2D RobotPosition { get; set; }
        
        //stores the weight of the particle, i.e. the belief
        //that the particle represents the true position of the robot
        public double Weight { get; set; }

        #endregion

        #region constructors

        public Particle(double x, double y, double weight)
        {
            this.RobotPosition = new Vector2D(x, y);
            this.Weight = weight;
        }

        public Particle(Particle particleToCopy)
        {
            this.RobotPosition = new Vector2D(particleToCopy.RobotPosition.X, particleToCopy.RobotPosition.Y);
            this.Weight = particleToCopy.Weight;
        }

        #endregion
    }
}