#include "AStarLibrary.h"
#include "DrawingLibrary.h"
#include <iostream>
#include <vector>
#include <string>
#include <fstream>
#include <sstream>
#include <ctime>

using std::cout;
using std::ifstream;
using std::stringstream;
using std::string;
using std::vector;
using std::ios;

void readDataFromFile(const char* filename, const char delimiter);

AStarLibrary aStarLibrary;
	
int main()
{
	readDataFromFile("worldMap 50x50.txt", ',');

	//Coordinates2D sourceVertex(0,0), destinationVertex(3,3);		//used for testing the 5x5 grid
	//Coordinates2D sourceVertex(3,0), destinationVertex(1,7);		//used for testing the 10x10 grid
	Coordinates2D sourceVertex(5,4), destinationVertex(44,23);		//used for testing the 50x50 grid

	clock_t start = clock();
	WorldMap.ShortestPathAndExpandedNodes = aStarLibrary.AStar(sourceVertex, destinationVertex);
	clock_t end = clock();
	double endTime = ((double)end-start)/(double(CLK_TCK/1000));
	cout.setf(ios::fixed);
	cout.setf(ios::showpoint);
	cout.precision(2);
	cout << "time elapsed = " << endTime;

	WorldMap.ShortestPathFound = true;

	DrawingLibrary drawingLibrary;
	WorldMap.SetVisualMap(aStarLibrary.WorldMap);
	WorldMap.Source = sourceVertex;
	WorldMap.Destination = destinationVertex;
	drawingLibrary.StartDrawing();

	getchar();
	return 0;
}

//<summary>Reads data from the file with name 'filename'.
//</summary>
//<param name='filename'>Name of a file containing numerical data.</param>
//<param name='delimiter'>Delimiter used to separate numbers in the file.</param>
void readDataFromFile(const char* filename, const char delimiter)
{
	//stream for reading data from the file
	ifstream document;

	//string for storing a line from the file
	string lineReader;

	//stream for converting strings to numbers
	stringstream converter;

	//variable for storing a converted string to number
	double tempNumber;
	
	try
	{
		document.exceptions(ifstream::badbit | ifstream::failbit);
		document.open(filename);

		while(!document.eof())
		{
			getline(document,lineReader);
			vector<double> currentRow;

			//we make sure that the file does not contain letters
			for(unsigned int character=0; character<lineReader.size(); character++)
			{
				if(isalpha(lineReader[character]))
					throw "Wrong format";
			}

			//used for storing the previous position of a delimiter in the string;
			//initially assigned to 0 because we haven't found a delimiter yet
			int delimiterIndex = 0;

			//used for looping through the file line
			//which was read in the current iteration
			unsigned int i = 0;

			//we loop through the file line and extract numbers
			while(i<lineReader.size())
			{
				//if we find a delimiter, we extract a number,
				//convert it to 'float' and then store it in the data matrix
				if(lineReader[i] == delimiter)
				{
					converter << lineReader.substr(delimiterIndex, i-delimiterIndex);
					converter >> tempNumber;
					currentRow.push_back(tempNumber);

					delimiterIndex = i+1;

					//we check for repetitive occurences of a delimiter
					while(lineReader[delimiterIndex] == delimiter)
					{
						delimiterIndex++;
						i++;
					}
					converter.clear();
				}
				i++;
			}

			//we extract the last number in the line
			converter << lineReader.substr(delimiterIndex, lineReader.size()-delimiterIndex);
			converter >> tempNumber;
			currentRow.push_back(tempNumber);
			converter.clear();

			aStarLibrary.WorldMap.push_back(currentRow);
		}

		document.close();
	}
	catch(...)
	{
		//we close the file stream in case it is open
		if(document.is_open())
			document.close();

		throw "Error while reading file";
	}
}