#ifndef ROW_OBJECTS_H
#define ROW_OBJECTS_H

#include <vector>
#include "RowBoundary.h"
#include "ObjectBoundary.h"

using std::vector;

//<summary>
//Contains boundaries of a row in an image
//and boundaries of objects identified in the row.
//</summary>
struct RowObjects
{
	RowBoundary RowBoundaries;
	vector<ObjectBoundary> Objects;
};

#endif