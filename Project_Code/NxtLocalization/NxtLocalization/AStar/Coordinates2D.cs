namespace NxtLocalization.AStar
{
    ///<summary>
    ///Class storing x and y integer coordinates.
    ///</summary>
    public class Coordinates2D
    {
        #region fields

        public int X { get; set; }
        public int Y { get; set; }

        #endregion

        #region constructors

        /// <summary>
        /// Default constructor; assigns X and Y to -1.
        /// </summary>
        public Coordinates2D()
        {
	        this.X = -1;
	        this.Y = -1;
        }

        /// <summary>
        /// Constructor that assigns 'x' to 'X' and 'y' to 'Y'.
        /// </summary>
        public Coordinates2D(int x, int y)
        {
	        this.X = x;
	        this.Y = y;
        }

        public Coordinates2D(Coordinates2D coordinates)
        {
            this.X = coordinates.X;
            this.Y = coordinates.Y;
        }

        #endregion

        #region overloads

        public override bool Equals(object obj)
        {
            Coordinates2D coordinates = (Coordinates2D) obj;
            return this.X == coordinates.X && this.Y == coordinates.Y;
        }

        public override int GetHashCode()
        {
            int hash = 4;
            hash = (hash*3) + this.X.GetHashCode();
            hash = (hash*3) + this.Y.GetHashCode();
            return hash;
        }

        public static bool operator ==(Coordinates2D firstCoordinates, Coordinates2D secondCoordinates)
        {
            return firstCoordinates.X == secondCoordinates.X && firstCoordinates.Y == secondCoordinates.Y;
        }

        public static bool operator !=(Coordinates2D firstCoordinates, Coordinates2D secondCoordinates)
        {
            return firstCoordinates.X != secondCoordinates.X || firstCoordinates.Y != secondCoordinates.Y;
        }

        #endregion

        #region methods

        public void Copy(Coordinates2D coordinates)
        {
            this.X = coordinates.X;
            this.Y = coordinates.Y;
        }

        #endregion
    }
}
