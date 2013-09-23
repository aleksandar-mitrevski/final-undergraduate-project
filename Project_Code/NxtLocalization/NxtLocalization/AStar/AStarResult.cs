using System.Collections.Generic;

namespace NxtLocalization.AStar
{
    /// <summary>
    /// Class defining objects that contain:
    /// - A list of nodes expanded by the A* algorithm.
    /// - The shortest path found by the algorithm.
    /// </summary>
    public class AStarResult
    {
        #region fields

        public List<Coordinates2D> ShortestPath { get; set; }
        public List<Coordinates2D> ExpandedNodes { get; set; }

        #endregion

        #region constructors

        public AStarResult()
        {
            this.ShortestPath = new List<Coordinates2D>();
            this.ExpandedNodes = new List<Coordinates2D>();
        }

        #endregion
    }
}
