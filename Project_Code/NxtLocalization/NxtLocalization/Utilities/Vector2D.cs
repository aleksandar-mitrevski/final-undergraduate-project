namespace NxtLocalization.Utilities
{
    /// <summary>
    /// Defines a two-dimensional vector.
    /// </summary>
    public class Vector2D
    {
        #region fields

        public double X { get; set; }
        public double Y { get; set; }

        #endregion

        #region constructors

        public Vector2D()
        {
            this.X = 0.0;
            this.Y = 0.0;
        }

        public Vector2D(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        #endregion

        #region operator overloads

        public static Vector2D operator +(Vector2D firstVector, Vector2D secondVector)
        {
            Vector2D result = new Vector2D(firstVector.X + secondVector.X,
                                           firstVector.Y + secondVector.Y);
            return result;
        }

        public static Vector2D operator -(Vector2D firstVector, Vector2D secondVector)
        {
            Vector2D result = new Vector2D(firstVector.X - secondVector.X,
                                           firstVector.Y - secondVector.Y);
            return result;
        }

        #endregion

        #region methods

        public void Reset()
        {
            this.X = 0.0;
            this.Y = 0.0;
        }

        #endregion
    }
}