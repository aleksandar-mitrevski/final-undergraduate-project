#ifndef MIN_HEAP_H
#define MIN_HEAP_H

#include "AStarNode.h"
#include <vector>
using std::vector;

//<summary>
//Class defining a min-heap used by the A* algorithm.
//</summary>
class MinHeap
{
public:
	//inserts a new node in the heap
	void Insert(AStarNode node);

	//extracts the node with lowest cost
	AStarNode ExtractMin();

	//restores the heap properties using the bubble up operation
	void BubbleUp(unsigned int index);

	//restores the heap properties using the bubble down operation
	void BubbleDown(unsigned int index);
	
	//swaps two elements of the heap
	void Swap(unsigned int index1, unsigned int index2);

	//returns the indes of the input node
	unsigned int GetIndex(Coordinates2D node);

	//checks whether the heap is empty
	bool Empty();

	//stores the elements of the heap
	vector<AStarNode> nodes;
};


//<summary>
//Inserts 'node' in the heap and restores the heap properties
//afterwards by performing a bubble up operation.
//</summary>
//<param name='node'>The element that we want to insert in the heap</param>
void MinHeap::Insert(AStarNode node)
{
	this->nodes.push_back(node);
	this->BubbleUp(this->nodes.size()-1);
}

//<summary>
//Returns the heap element that has the lowest total cost
//(thus it is on the top of the heap).
//</summary>
AStarNode MinHeap::ExtractMin()
{
	//we get the minimum node
	AStarNode minimumNode = this->nodes[0];

	//we replace the first node in the heap by the last node
	this->nodes[0] = this->nodes[this->nodes.size() - 1];

	//we remove the last node from the heap
	this->nodes.erase(this->nodes.begin() + (this->nodes.size()-1));

	//we restore the heap properties
	this->BubbleDown(0);

	return minimumNode;
}

//<summary>
//Performs a bubble up operation on the heap,
//pushing the element with index 'index' up as long as
//its cost is less than the cost of a parent.
//</summary>
//<param name='index'>Index of the element that we want to push up in the heap.</param>
void MinHeap::BubbleUp(unsigned int index)
{
	int currentChild = index;
	int currentParent;

	//if 'currentChild' is odd, its parent
	//is in position 'currentChild' / 2;
	//otherwise, its parent is in position
	//'currentChild' / 2 - 1
	if(currentChild & 1)
		currentParent = currentChild / 2;
	else
		currentParent = currentChild / 2 - 1;

	//as long as the total cost of the parent is greater than the total cost of the child and
	//the child is not in position 0, we replace the parent and the child
	while(currentChild > 0 && this->nodes[currentParent].TotalCost > this->nodes[currentChild].TotalCost)
	{
		this->Swap(currentParent, currentChild);
		currentChild = currentParent;

		//if 'currentChild' is odd, its parent
		//is in position 'currentChild' / 2;
		//otherwise, its parent is in position
		//'currentChild' / 2 - 1
		if(currentChild & 1)
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
void MinHeap::BubbleDown(unsigned int index)
{
	int currentParent = index;
	int leftChild = currentParent * 2 + 1;
	int rightChild = currentParent * 2 + 2;
	int currentChild = -1;

	//we are looking for the child that has lower cost
	if(leftChild < this->nodes.size() && rightChild < this->nodes.size())
	{
		if(this->nodes[leftChild].TotalCost < this->nodes[rightChild].TotalCost)
			currentChild = leftChild;
		else
			currentChild = rightChild;
	}
	else if(leftChild < this->nodes.size())
		currentChild = leftChild;
	else if(rightChild < this->nodes.size())
		currentChild = rightChild;

	//as long as the total cost of the parent is greater than the total cost of the child and
	//we have children to consider, we replace the parent and the child
	while(currentChild > -1 && this->nodes[currentParent].TotalCost > this->nodes[currentChild].TotalCost)
	{
		this->Swap(currentChild, currentParent);
		currentParent = currentChild;

		leftChild = currentParent * 2 + 1;
		rightChild = currentParent * 2 + 2;

		//we are looking for the child that has lower cost
		if(leftChild < this->nodes.size() && rightChild < this->nodes.size())
		{
			if(this->nodes[leftChild].TotalCost < this->nodes[rightChild].TotalCost)
				currentChild = leftChild;
			else
				currentChild = rightChild;
		}
		else if(leftChild < this->nodes.size())
			currentChild = leftChild;
		else if(rightChild < this->nodes.size())
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
void MinHeap::Swap(unsigned int index1, unsigned int index2)
{
	AStarNode temp = this->nodes[index1];
	this->nodes[index1] = this->nodes[index2];
	this->nodes[index2] = temp;
}


//<summary>
//If 'node' is present in the heap, returns its index; otherwise, returns -1.
//</summary>
//<param name='node'>The element that we are looking for in the heap.<param>
unsigned int MinHeap::GetIndex(Coordinates2D node)
{
	unsigned int index = -1;
	for(unsigned int i=0; i<this->nodes.size(); i++)
	{
		if(this->nodes[i].NodeCoordinates == node)
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
bool MinHeap::Empty()
{
	return this->nodes.size() == 0;
}

#endif