#ifndef COORDINATES_2D_H
#define COORDINATES_2D_H

//<summary>
//Class for storing x and y integer coordinates.
//</summary>
class Coordinates2D
{
public:
	Coordinates2D();
	Coordinates2D(int x, int y);
	Coordinates2D& operator=(const Coordinates2D& rightHandSide);

	friend bool operator ==(const Coordinates2D& leftHandSide, const Coordinates2D& rightHandSide)
	{
		return leftHandSide.X == rightHandSide.X && leftHandSide.Y == rightHandSide.Y;
	}

	friend bool operator !=(const Coordinates2D& leftHandSide, const Coordinates2D& rightHandSide)
	{
		return leftHandSide.X != rightHandSide.X || leftHandSide.Y != rightHandSide.Y;
	}

	int X;
	int Y;
};


//<summary>
//Default constructor; assigns X and Y to -1.
//</summary>
Coordinates2D::Coordinates2D()
{
	this->X = -1;
	this->Y = -1;
}

//<summary>
//Constructor that assigns 'x' to 'X' and 'y' to 'Y'.
//</summary>
Coordinates2D::Coordinates2D(int x, int y)
{
	this->X = x;
	this->Y = y;
}

//<summary>
//Copies 'rightHandSide' to the current object.
//</summary>
Coordinates2D& Coordinates2D::operator=(const Coordinates2D& rightHandSide)
{
	this->X = rightHandSide.X;
	this->Y = rightHandSide.Y;
	return *this;
}

#endif