#ifndef VISUAL_NODE_INFO_H
#define VISUAL_NODE_INFO_H

#include "VisualCoordinates.h"

//<summary>
//Stores info for a visual grid field, including:
//	- the four vertices of the square that defines the field.
//	- the center of the field.
//	- whether the field is an obstacle or not.
//</summary>
struct VisualNodeInfo
{
	//each field will be a square
	VisualCoordinates Vertices[4];

	//used for drawing a path between the vertices
	VisualCoordinates VertexCenter;
	
	//indicates whether the field is an obstacle or nots
	bool IsObstacle;
};

#endif