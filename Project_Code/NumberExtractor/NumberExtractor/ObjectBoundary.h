#ifndef OBJECT_BOUNDARY_H
#define OBJECT_BOUNDARY_H

//<summary>
//Contains upper, lower, left, and right
//boundaries of an object in an image.
//</summary>
struct ObjectBoundary
{
	int LeftBoundary;
	int RightBoundary;
	int UpperBoundary;
	int LowerBoundary;
};

#endif