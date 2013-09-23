#ifndef A_STAR_NODE_H
#define A_STAR_NODE_H

#include "Coordinates2D.h"

//<summary>
//Class that stores info for each node in a grid, including:
//	- its grid coordinates.
//	- the coordinates of a node that leads to the current node with the least cost.
//	- the cost to reach the current node.
//	- the total cost for reaching the current node (cost + heuristic)
//</summary>
class AStarNode
{
public:
	AStarNode();
	AStarNode(Coordinates2D nodeCoordinates, double cost, double totalCost);
	AStarNode(Coordinates2D nodeCoordinates, Coordinates2D parentCoordinates, double cost, double totalCost);
	AStarNode& operator=(const AStarNode& rightHandSide);

	Coordinates2D NodeCoordinates;
	Coordinates2D ParentCoordinates;
	double Cost;
	double TotalCost;
};

//<summary>
//Default constructor; initializes 'Cost' and 'TotalCost' to -1.0.
//</summary>
AStarNode::AStarNode()
{
	this->Cost = -1.0;
	this->TotalCost = -1.0;
}

//<summary>
//Constructor that initializes:
//	- 'Cost' to 'cost'.
//	- 'TotalCost' to 'totalCost'.
//	- 'NodeCoordinates' to 'nodeCoordinates'.
//</summary>
AStarNode::AStarNode(Coordinates2D nodeCoordinates, double cost, double totalCost)
{
	this->NodeCoordinates = nodeCoordinates;
	this->Cost = cost;
	this->TotalCost = totalCost;

	this->ParentCoordinates.X = -1;
	this->ParentCoordinates.Y = -1;
}

//<summary>
//Constructor that initializes:
//	- 'Cost' to 'cost'.
//	- 'TotalCost' to 'totalCost'.
//	- 'NodeCoordinates' to 'nodeCoordinates'.
//	- 'ParentCoordinates' to 'parentCoordinates'.
//</summary>
AStarNode::AStarNode(Coordinates2D nodeCoordinates, Coordinates2D parentCoordinates, double cost, double totalCost)
{
	this->NodeCoordinates = nodeCoordinates;
	this->ParentCoordinates = parentCoordinates;
	this->Cost = cost;
	this->TotalCost = totalCost;
}

//<summary>
//Copies 'rightHandSide' to the current object.
//</summary>
AStarNode& AStarNode::operator =(const AStarNode& rightHandSide)
{
	this->NodeCoordinates = rightHandSide.NodeCoordinates;
	this->ParentCoordinates = rightHandSide.ParentCoordinates;
	this->Cost = rightHandSide.Cost;
	this->TotalCost = rightHandSide.TotalCost;
	return *this;
}

#endif