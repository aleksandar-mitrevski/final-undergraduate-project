#ifndef A_STAR_RESULT_H
#define A_STAR_RESULT_H

#include "Coordinates2D.h"
#include <vector>
using std::vector;

struct AStarResult
{
	vector<Coordinates2D> ShortestPath;
	vector<Coordinates2D> ExpandedNodes;
};

#endif