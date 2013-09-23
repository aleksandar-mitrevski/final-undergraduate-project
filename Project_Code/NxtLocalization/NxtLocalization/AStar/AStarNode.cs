namespace NxtLocalization.AStar
{
    /// <summary>
    /// Class that stores info for each node in a grid, including:
    /// 	- its grid coordinates.
    /// 	- the coordinates of a node that leads to the current node with the least cost.
    /// 	- the cost to reach the current node.
    /// 	- the total cost for reaching the current node (cost + heuristic)
    /// </summary>
    public class AStarNode
    {
        #region fields

        public Coordinates2D NodeCoordinates;
        public Coordinates2D ParentCoordinates;
        public double Cost { get; set; }
        public double TotalCost { get; set; }

        #endregion

        #region constructors

        /// <summary>
        /// Default constructor; initializes 'Cost' and 'TotalCost' to -1.0.
        /// </summary>
        public AStarNode()
        {
            this.Cost = -1.0;
            this.TotalCost = -1.0;
        }

        /// <summary>
        /// Constructor that initializes:
        /// 	- 'Cost' to 'cost'.
        /// 	- 'TotalCost' to 'totalCost'.
        /// 	- 'NodeCoordinates' to 'nodeCoordinates'.
        /// </summary>
        public AStarNode(Coordinates2D nodeCoordinates, double cost, double totalCost)
        {
	        this.NodeCoordinates = nodeCoordinates;
            this.ParentCoordinates = new Coordinates2D(-1, -1);
	        this.Cost = cost;
	        this.TotalCost = totalCost;
        }

        /// <summary> Constructor that initializes:
        ///	 - 'Cost' to 'cost'.
        ///	 - 'TotalCost' to 'totalCost'.
        ///	 - 'NodeCoordinates' to 'nodeCoordinates'.
        ///	 - 'ParentCoordinates' to 'parentCoordinates'.
        /// </summary>
        public AStarNode(Coordinates2D nodeCoordinates, Coordinates2D parentCoordinates, double cost, double totalCost)
        {
	        this.NodeCoordinates = new Coordinates2D(nodeCoordinates);
	        this.ParentCoordinates = new Coordinates2D(parentCoordinates);
	        this.Cost = cost;
	        this.TotalCost = totalCost;
        }

        /// <summary>
        /// Constructor that copies 'node' to the new object.
        /// </summary>
        /// <param name="node">The node that will be copied to the new object.</param>
        public AStarNode(AStarNode node)
        {
            this.NodeCoordinates = new Coordinates2D(node.NodeCoordinates);
            this.ParentCoordinates = new Coordinates2D(node.ParentCoordinates);
            this.Cost = node.Cost;
            this.TotalCost = node.TotalCost;
        }

        #endregion
    }
}
