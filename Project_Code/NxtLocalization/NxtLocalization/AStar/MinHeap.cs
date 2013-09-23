using System.Collections.Generic;
using System.Linq;

namespace NxtLocalization.AStar
{
    /// <summary>
    /// Class defining a min heap data structure.
    /// </summary>
    public class MinHeap
    {
        #region fields

        //stores the elements of the heap
        public List<AStarNode> Nodes { get; set; }

        #endregion

        #region constructors

        public MinHeap()
        {
            this.Nodes = new List<AStarNode>();
        }

        #endregion

        #region methods

        //<summary>
        //Inserts 'node' in the heap and restores the heap properties
        //afterwards by performing a bubble up operation.
        //</summary>
        //<param name='node'>The element that we want to insert in the heap</param>
        public void Insert(AStarNode node)
        {
	        this.Nodes.Add(node);
	        this.BubbleUp(this.Nodes.Count() - 1);
        }

        //<summary>
        //Returns the heap element that has the lowest total cost
        //(thus it is on the top of the heap).
        //</summary>
        public AStarNode ExtractMin()
        {
	        //we get the minimum node
	        AStarNode minimumNode = this.Nodes[0];

	        //we replace the first node in the heap by the last node
	        this.Nodes[0] = this.Nodes[this.Nodes.Count() - 1];

	        //we remove the last node from the heap
            this.Nodes.RemoveAt(this.Nodes.Count() - 1);

	        //we restore the heap properties
	        this.BubbleDown(0);

	        return minimumNode;
        }

        //<summary>
        //Performs a bubble up operation on the heap,
        //pushing the element with index 'index' up as long as
        //its cost is less than the cost of a parent.
        //</summary>
        //<param name='index'>Index of the element that we want to push up in the heap.</param>
        public void BubbleUp(int index)
        {
	        int currentChild = index;
	        int currentParent;

	        //if 'currentChild' is odd, its parent
	        //is in position 'currentChild' / 2;
	        //otherwise, its parent is in position
	        //'currentChild' / 2 - 1
	        if((currentChild & 1) == 1)
		        currentParent = currentChild / 2;
	        else
		        currentParent = currentChild / 2 - 1;

	        //as long as the total cost of the parent is greater than the total cost of the child and
	        //the child is not in position 0, we replace the parent and the child
	        while(currentChild > 0 && this.Nodes[currentParent].TotalCost > this.Nodes[currentChild].TotalCost)
	        {
		        this.Swap(currentParent, currentChild);
		        currentChild = currentParent;

		        //if 'currentChild' is odd, its parent
		        //is in position 'currentChild' / 2;
		        //otherwise, its parent is in position
		        //'currentChild' / 2 - 1
		        if((currentChild & 1) == 1)
			        currentParent = currentChild / 2;
		        else
			        currentParent = currentChild / 2 - 1;
	        }
        }

        //<summary>
        //Performs a bubble down operation on the heap,
        //pushing the element with index 'index' down as long as
        //its cost is greater than the cost of the child with lower cost.
        //</summary>
        //<param name='index'>Index of the element that we want to push up in the heap.</param>
        public void BubbleDown(int index)
        {
	        int currentParent = index;
	        int leftChild = currentParent * 2 + 1;
	        int rightChild = currentParent * 2 + 2;
	        int currentChild = -1;

	        //we are looking for the child that has lower cost
	        if(leftChild < this.Nodes.Count() && rightChild < this.Nodes.Count())
	        {
		        if(this.Nodes[leftChild].TotalCost < this.Nodes[rightChild].TotalCost)
			        currentChild = leftChild;
		        else
			        currentChild = rightChild;
	        }
	        else if(leftChild < this.Nodes.Count())
		        currentChild = leftChild;
	        else if(rightChild < this.Nodes.Count())
		        currentChild = rightChild;

	        //as long as the total cost of the parent is greater than the total cost of the child and
	        //we have children to consider, we replace the parent and the child
	        while(currentChild > -1 && this.Nodes[currentParent].TotalCost > this.Nodes[currentChild].TotalCost)
	        {
		        this.Swap(currentChild, currentParent);
		        currentParent = currentChild;

		        leftChild = currentParent * 2 + 1;
		        rightChild = currentParent * 2 + 2;

		        //we are looking for the child that has lower cost
		        if(leftChild < this.Nodes.Count() && rightChild < this.Nodes.Count())
		        {
			        if(this.Nodes[leftChild].TotalCost < this.Nodes[rightChild].TotalCost)
				        currentChild = leftChild;
			        else
				        currentChild = rightChild;
		        }
		        else if(leftChild < this.Nodes.Count())
			        currentChild = leftChild;
		        else if(rightChild < this.Nodes.Count())
			        currentChild = rightChild;
		        else
			        currentChild = -1;
	        }
        }

        //<summary>
        //Swaps the elements with indices 'index1' and 'index2' in the heap.
        //</summary>
        //<param name='index1'>Index of the first element.</param>
        //<param name='index2'>Index of the second element.</param>
        public void Swap(int index1, int index2)
        {
	        AStarNode temp = this.Nodes[index1];
	        this.Nodes[index1] = this.Nodes[index2];
	        this.Nodes[index2] = temp;
        }


        //<summary>
        //If 'node' is present in the heap, returns its index; otherwise, returns -1.
        //</summary>
        //<param name='node'>The element that we are looking for in the heap.<param>
        public int GetIndex(Coordinates2D node)
        {
	        int index = -1;
	        for(int i=0; i<this.Nodes.Count(); i++)
	        {
		        if(this.Nodes[i].NodeCoordinates == node)
		        {
			        index = i;
			        break;
		        }
	        }

	        return index;
        }

        //<summary>
        //Returns true if the heap is empty and false otherwise.
        //</summary>
        public bool Empty()
        {
	        return !this.Nodes.Any();
        }

        #endregion
    }
}
