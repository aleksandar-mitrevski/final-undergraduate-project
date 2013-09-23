#ifndef A_STAR_LIBRARY_H
#define A_STAR_LIBRARY_H

#include "Coordinates2D.h"
#include "AStarResult.h"
#include "MinHeap.h"
#include <vector>
#include <algorithm>
using std::vector;

//<summary>
//Class used for finding a best path between
//two points on a grid using the A* algorithm.
//</summary>
class AStarLibrary
{
public:
	//implementation of the A* algorithm for grids
	AStarResult AStar(Coordinates2D source, Coordinates2D destination);

	//used for storing the map of the environment
	vector<vector<double>> WorldMap;

private:
	//calculates a heuristic between the source and destination vertex
	double CalculateHeuristic(Coordinates2D source, Coordinates2D destination);

	//returns the nodes adjacent to 'node'
	vector<AStarNode> GetAdjacent(AStarNode node, Coordinates2D destination);
};


//<summary>
//Implementation of the A* algorithm for finding a shortest path between 'source' and 'destination'.
//Uses a heap for speeding up the operation that looks for the least costly vertex at a given iteration.
//</summary>
//<param name='source'>Object containing the grid coordinates of the source field.</param>
//<param name='destination'>Object containing the grid coordinates of the destination field.</param>
AStarResult AStarLibrary::AStar(Coordinates2D source, Coordinates2D destination)
{
	//used for storing the vertices currently on the open list
	MinHeap open;

	//used for storing the vertices that were already considered by the algorithm
	vector<AStarNode> closed;

	//we create a node for the source vertex and insert it on the heap
	AStarNode node(source, 0.0, 0.0);
	open.Insert(node);

	//used for indicating if the shortest path to the goal is found or not
	bool pathFound = false;

	//used for storing the node processed by the algorithm at each iteration
	AStarNode currentNode;

	//used for storing the nodes adjacent to the node processed at a given iteration
	vector<AStarNode> adjacent;

	//the algorithm runs as long as there are vertices that we need to process and the desired path is not found
	while(!open.Empty() && !pathFound)
	{
		//we take the node with the least cost at the moment
		currentNode = open.ExtractMin();

		//we append the node to the list of processed nodes
		closed.push_back(currentNode);

		//if the currently processed node is the destination node, we end the search
		if(currentNode.NodeCoordinates == destination)
		{
			pathFound = true;
			continue;
		}

		//we take the vertices adjacent to the currently processed node
		adjacent = this->GetAdjacent(currentNode, destination);

		//we look at each of the adjacent nodes and perform appropriate actions
		//depending on whether the node is already on the open list, is already on the closed list,
		//or is not on any of the lists
		for(unsigned int i=0; i<adjacent.size(); i++)
		{
			//we look if the current adjacent node is already on the open list;
			//if it is, its position in the heap will be returned; otherwise, -1 will be returned
			int nodePosition = open.GetIndex(adjacent[i].NodeCoordinates);

			//if the node is on the open list, we check if we found a better path to it;
			//if that is the case, we update the node's info and restore the heap properties
			if(nodePosition != -1)
			{
				if(open.nodes[nodePosition].TotalCost > adjacent[i].TotalCost)
				{
					open.nodes[nodePosition] = adjacent[i];
					open.BubbleUp(nodePosition);
				}
			}
			//if the node is not on the open list, we check if it is on the closed list;
			//if it is, we don't do anything; otherwise, we add it to the open list
			else
			{
				bool nodeClosed = false;
				for(unsigned int j=0; j<closed.size(); j++)
				{
					if(adjacent[i].NodeCoordinates == closed[j].NodeCoordinates)
					{
						nodeClosed = true;
						break;
					}
				}

				if(!nodeClosed)
					open.Insert(adjacent[i]);
			}
		}
	}//end of the main loop in the algorithm

	//we return an empty vector if we could not find a path to the destination node
	if(currentNode.NodeCoordinates != destination)
		return AStarResult();
	//if we found a path to the destination node, we retrieve the shortest path
	else
	{
		AStarResult result;

		unsigned int numberOfExploredNodes = closed.size();

		//we are saving all the expanded nodes
		for(unsigned int i=0; i<numberOfExploredNodes; i++)
			result.ExpandedNodes.push_back(closed[i].NodeCoordinates);

		//the current parent coordinates are those of the destination node
		Coordinates2D currentParentCoordinates = closed[numberOfExploredNodes-1].ParentCoordinates;

		//we add the destination node to the shortest path
		result.ShortestPath.push_back(closed[numberOfExploredNodes-1].NodeCoordinates);

		//if the source is not the same as the destination, we are trying to retrieve the shortest path
		if(currentParentCoordinates.X != -1 && currentParentCoordinates.Y != -1)
		{
			do
			{
				//we add the parent of the current node to the path
				result.ShortestPath.push_back(currentParentCoordinates);

				//we look for the parent of the current parent node and
				//save its coordinates in 'currentParentCoordinates'
				for(unsigned int i=0; i<numberOfExploredNodes; i++)
				{
					if(currentParentCoordinates == closed[i].NodeCoordinates)
					{
						currentParentCoordinates = closed[i].ParentCoordinates;
						break;
					}
				}
			//we end the loop once we reach the source node
			} while(currentParentCoordinates.X != -1 && currentParentCoordinates.Y != -1);
		}

		//since the nodes were added in the reverse order to the path,
		//we just reverse the list in order to get the original path
		reverse(result.ShortestPath.begin(), result.ShortestPath.end());

		//we return the shortest path and the expanded nodes
		return result;
	}
}

//<summary>
//Returns a value for the heuristic function as Euclidean distance between
//the coordinates of 'source' and 'destination'.
//</summary>
//<param name='source'>The vertex for which we want to return a heuristic function.</param>
//<param name='destination'>The vertex that is the destination of the desired path.</param>
//<returns>A value for the heuristic function.</returns>
double AStarLibrary::CalculateHeuristic(Coordinates2D source, Coordinates2D destination)
{
	//we calculate the heuristic as a Euclidean distance between the coordinates of the source and destination cells
	double heuristic = sqrt(((destination.X - source.X) * (destination.X - source.X) * 1.0)
					 +		((destination.Y- source.Y) * (destination.Y - source.Y) * 1.0));
	return heuristic;
}

//<summary>
//Returns a list of nodes adjacent to 'node'. Assumes that we can't make diagonal movements.
//For each adjacent vertex, calculates the value of 
//			f(x) = g(x) + h(x)
//where g(x) is the cost to reach the adjacent vertex, while h(x)
//is the value of the heuristic function for the adjacent vertex.
//</summary>
//<param name='node'>The vertex whose adjacent vertices we want to take.</param>
//<param name='destination'>The destination vertex of the desired shortest path.</param>
vector<AStarNode> AStarLibrary::GetAdjacent(AStarNode node, Coordinates2D destination)
{
	vector<AStarNode> adjacentNodes;
	
	//we take the left adjacent vertex if the current node is not on the left bound
	if(node.NodeCoordinates.X+1 < this->WorldMap[0].size())
	{
		Coordinates2D newCoordinates(node.NodeCoordinates.X+1, node.NodeCoordinates.Y);

		//we calculate the cost as a sum of the cost to reach the current vertex and the
		//cost to go to the adjacent vertex (the function g(x))
		double cost = node.Cost + this->WorldMap[newCoordinates.X][newCoordinates.Y];

		//we calculate the heuristic (the function h(x))
		double heuristic = this->CalculateHeuristic(newCoordinates, destination);

		//we calculate f(x) = g(x) + h(x)
		double totalCost = cost + heuristic;

		//we add the new node to the list of adjacent nodes
		AStarNode newNode(newCoordinates, node.NodeCoordinates, cost, totalCost);
		adjacentNodes.push_back(newNode);
	}

	if(node.NodeCoordinates.X-1 >= 0)
	{
		Coordinates2D newCoordinates(node.NodeCoordinates.X-1, node.NodeCoordinates.Y);

		//we calculate the cost as a sum of the cost to reach the current vertex and the
		//cost to go to the adjacent vertex (the function g(x))
		double cost = node.Cost + this->WorldMap[newCoordinates.X][newCoordinates.Y];

		//we calculate the heuristic (the function h(x))
		double heuristic = this->CalculateHeuristic(newCoordinates, destination);

		//we calculate f(x) = g(x) + h(x)
		double totalCost = cost + heuristic;

		//we add the new node to the list of adjacent nodes
		AStarNode newNode(newCoordinates, node.NodeCoordinates, cost, totalCost);
		adjacentNodes.push_back(newNode);
	}

	if(node.NodeCoordinates.Y-1 >= 0)
	{
		Coordinates2D newCoordinates(node.NodeCoordinates.X, node.NodeCoordinates.Y-1);

		//we calculate the cost as a sum of the cost to reach the current vertex and the
		//cost to go to the adjacent vertex (the function g(x))
		double cost = node.Cost + this->WorldMap[newCoordinates.X][newCoordinates.Y];

		//we calculate the heuristic (the function h(x))
		double heuristic = this->CalculateHeuristic(newCoordinates, destination);

		//we calculate f(x) = g(x) + h(x)
		double totalCost = cost + heuristic;

		//we add the new node to the list of adjacent nodes
		AStarNode newNode(newCoordinates, node.NodeCoordinates, cost, totalCost);
		adjacentNodes.push_back(newNode);
	}

	if(node.NodeCoordinates.Y+1 < this->WorldMap.size())
	{
		Coordinates2D newCoordinates(node.NodeCoordinates.X, node.NodeCoordinates.Y+1);

		//we calculate the cost as a sum of the cost to reach the current vertex and the
		//cost to go to the adjacent vertex (the function g(x))
		double cost = node.Cost + this->WorldMap[newCoordinates.X][newCoordinates.Y];

		//we calculate the heuristic (the function h(x))
		double heuristic = this->CalculateHeuristic(newCoordinates, destination);

		//we calculate f(x) = g(x) + h(x)
		double totalCost = cost + heuristic;

		//we add the new node to the list of adjacent nodes
		AStarNode newNode(newCoordinates, node.NodeCoordinates, cost, totalCost);
		adjacentNodes.push_back(newNode);
	}


	//uncomment the lines below if you want to allow diagonal movements

	//if(canGoUp && canGoRight)
	//{
	//	Coordinates2D newCoordinates(node.NodeCoordinates.X+1, node.NodeCoordinates.Y-1);
	//	double cost = node.Cost + this->WorldMap[newCoordinates.X][newCoordinates.Y];
	//	double heuristic = this->CalculateHeuristic(newCoordinates, destination);
	//	double totalCost = cost + heuristic;

	//	AStarNode newNode(newCoordinates, node.NodeCoordinates, cost, totalCost);
	//	adjacentNodes.push_back(newNode);
	//}

	//if(canGoUp && canGoLeft)
	//{
	//	Coordinates2D newCoordinates(node.NodeCoordinates.X-1, node.NodeCoordinates.Y-1);
	//	double cost = node.Cost + this->WorldMap[newCoordinates.X][newCoordinates.Y];
	//	double heuristic = this->CalculateHeuristic(newCoordinates, destination);
	//	double totalCost = cost + heuristic;

	//	AStarNode newNode(newCoordinates, node.NodeCoordinates, cost, totalCost);
	//	adjacentNodes.push_back(newNode);
	//}

	//if(canGoDown && canGoRight)
	//{
	//	Coordinates2D newCoordinates(node.NodeCoordinates.X+1, node.NodeCoordinates.Y+1);
	//	double cost = node.Cost + this->WorldMap[newCoordinates.X][newCoordinates.Y];
	//	double heuristic = this->CalculateHeuristic(newCoordinates, destination);
	//	double totalCost = cost + heuristic;

	//	AStarNode newNode(newCoordinates, node.NodeCoordinates, cost, totalCost);
	//	adjacentNodes.push_back(newNode);
	//}

	//if(canGoDown && canGoLeft)
	//{
	//	Coordinates2D newCoordinates(node.NodeCoordinates.X-1, node.NodeCoordinates.Y+1);
	//	double cost = node.Cost + this->WorldMap[newCoordinates.X][newCoordinates.Y];
	//	double heuristic = this->CalculateHeuristic(newCoordinates, destination);
	//	double totalCost = cost + heuristic;

	//	AStarNode newNode(newCoordinates, node.NodeCoordinates, cost, totalCost);
	//	adjacentNodes.push_back(newNode);
	//}

	return adjacentNodes;
}

#endif