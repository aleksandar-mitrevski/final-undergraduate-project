#ifndef A_STAR_VISUAL_WORLD_MAP_H
#define A_STAR_VISUAL_WORLD_MAP_H

#include "VisualWorldMap.h"
#include "AStarResult.h"

class AStarVisualWorldMap : public VisualWorldMap
{
public:
	AStarVisualWorldMap();

	//stores a source vertex
	Coordinates2D Source;

	//stores a destination vertex
	Coordinates2D Destination;

	//indicates whether the shortest path was found or not
	bool ShortestPathFound;

	//stores the shortest path and the expanded nodes by the algorithm
	AStarResult ShortestPathAndExpandedNodes;
};

AStarVisualWorldMap::AStarVisualWorldMap()
{
	this->ShortestPathFound = false;
}

#endif