#ifndef VISUAL_WORLD_MAP_H
#define VISUAL_WORLD_MAP_H

#include "Coordinates2D.h"
#include "VisualNodeInfo.h"
#include "DrawingConstants.h"
#include <vector>
using std::vector;

//<summary>
//Class that stores a visual grid.
//</summary>
class VisualWorldMap
{
public:
	//sets the fields of the grid
	void SetVisualMap(vector<vector<double>> worldMap);
	
	//stores the grid
	vector<vector<VisualNodeInfo>> Vertices;
};


//<summary>
//Sets a visual grid from the grid given by 'worldMap'.
//</summary>
//<param name='worldMap'>A logical grid that is used for making a visual grid.</param>
void VisualWorldMap::SetVisualMap(vector<vector<double>> worldMap)
{
	unsigned int numberOfRows = worldMap.size();
	unsigned int numberOfColumns = worldMap[0].size();

	//we calculate the width of a field as the window width (not width in pixels)
	//divided by the number of columns
	float fieldWidth = X_RANGE / numberOfColumns;

	//we calculate the height of a field as the window height (not height in pixels)
	//divided by the number of rows
	float fieldHeight = Y_RANGE / numberOfRows;

	float currentY = UPPER_Y_POINT;

	//for each field in the logical grid, we create a field in the visual grid,
	//storing the vertices of the field in the following order: top left, top right, bottom right, bottom left
	for(unsigned int i=0; i<numberOfRows; i++)
	{
		float currentX = LEFT_X_POINT;
		vector<VisualNodeInfo> tempVector;
		for(unsigned int j=0; j<numberOfColumns; j++)
		{
			VisualNodeInfo fieldInfo;
			fieldInfo.IsObstacle = abs(worldMap[i][j] - OBSTACLE_DELIMITER) < 0.005 ? true : false;

			//top left
			fieldInfo.Vertices[0].X = currentX;
			fieldInfo.Vertices[0].Y = currentY;

			//top right
			fieldInfo.Vertices[1].X = currentX + fieldWidth;
			fieldInfo.Vertices[1].Y = currentY;

			//bottom right
			fieldInfo.Vertices[2].X = currentX + fieldWidth;
			fieldInfo.Vertices[2].Y = currentY - fieldHeight;

			//bottom left
			fieldInfo.Vertices[3].X = currentX;
			fieldInfo.Vertices[3].Y = currentY - fieldHeight;

			fieldInfo.VertexCenter.X = (currentX + (currentX + fieldWidth)) / 2.0f;
			fieldInfo.VertexCenter.Y = (currentY + (currentY - fieldHeight)) / 2.0f;

			currentX += fieldWidth;
			tempVector.push_back(fieldInfo);
		}

		currentY -= fieldHeight;
		this->Vertices.push_back(tempVector);
	}
}

#endif