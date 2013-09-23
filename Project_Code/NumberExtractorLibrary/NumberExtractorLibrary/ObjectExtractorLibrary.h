#ifndef OBJECT_EXTRACTOR_LIBRARY_H
#define OBJECT_EXTRACTOR_LIBRARY_H

#include <string>
#include <vector>
#include <fstream>
#include <opencv\cv.h>
#include <opencv\highgui.h>
#include "RowObjects.h"
#include "Constants.h"

using namespace cv;
using std::string;
using std::vector;
using std::ofstream;

//<summary>
//Class that extracts objects from an image.
//</summary>
class ObjectExtractorLibrary
{
public:
	//default constructor
	ObjectExtractorLibrary(string fileName);

	//gets the value of 'this->fileName'
	string GetFileName() { return this->fileName; }

	//sets the value of 'this->fileName'
	void SetFileName(string fileName) { this->fileName = fileName; }

	//function that extracts objects and returns a vector of images containing them
	void ExtractAndSaveObjects();
private:
	//extracts boundaries of rows of objects from the input image
	vector<RowObjects> ExtractHorizontalBoundaries(Mat binaryImage);

	//extracts objects from previously identified rows in the image
	void ExtractRowObjects(Mat binaryImage, vector<RowObjects>& boundaries);

	//extracts vertical boundaries of objects in the row with index 'currentRow'
	void ExtractVerticalObjectBoundaries(Mat binaryImage, vector<RowObjects>& boundaries, unsigned int currentRow);

	//extracts horizontal boundaries of objects in the row with index 'currentRow'
	void ExtractHorizontalObjectBoundaries(Mat binaryImage, vector<RowObjects>& boundaries, unsigned int currentRow);

	//uses the boundaries of the previously identified objects for extracting
	//these objects as subimages from the input image
	vector<Mat> ExtractObjects(Mat binaryImage, vector<RowObjects> boundaries);

	//saves the pixels of the objects in a text file, making them usable for a classifier
	void SavePixelsInTextFile(vector<Mat> objects);

	//saves the extracted charactes as images
	void SaveCharacterImages(vector<Mat> objects);
	
	//stores the name of the image in which we want to find objects
	string fileName;

	//stores the image with name 'this->fileName'
	Mat originalImage;
};


//<summary>
//Default constructor; assigns 'fileName' to 'this->fileName'.
//</summary>
//<param name='fileName'>Name of an image file.</param>
ObjectExtractorLibrary::ObjectExtractorLibrary(string fileName)
{
	this->fileName = "C:\\mindstormImages\\" + fileName;
}

//<summary>
//Returns a vector of extracted objects from the image with name 'this->fileName'.
//The objects are identified with the following steps:
//	1. extracting rows of objects in the image.
//	2. locating objects within the extracted rows.
//</summary>
void ObjectExtractorLibrary::ExtractAndSaveObjects()
{
	//we are reading the image as a grayscale image
	this->originalImage = imread(this->fileName, CV_LOAD_IMAGE_GRAYSCALE);

	//GaussianBlur(this->originalImage, this->originalImage, Size(0,0), 0.5);

	//we are creating a new image that will store a binary version of the original image
	Mat binaryImage;

	//we are converting the grayscale image to a binary image, using 128 as a threshold value;
	//note that we are inverting the image because we want black for the 'background' and white for the numbers
	threshold(this->originalImage, binaryImage, 128, 255, THRESH_BINARY_INV | THRESH_OTSU);

	//we are looking for rows of objects in the image
	vector<RowObjects> boundaries = this->ExtractHorizontalBoundaries(binaryImage);

	unsigned int i = 0;
	while(i < boundaries.size())
	{
		if((boundaries[i].RowBoundaries.LowerBoundary - boundaries[i].RowBoundaries.UpperBoundary) <= 20)
			boundaries.erase(boundaries.begin() + i);
		else
			i++;
	}

	//the boundaries of the objects in the image are extracted
	this->ExtractRowObjects(binaryImage, boundaries);

	//we extract the objects as separate images
	vector<Mat> objects = this->ExtractObjects(binaryImage, boundaries);

	unsigned int numberOfExtractedObjects = objects.size();

	//we resize the images to size Constants::NumberOfImageRows x Constants::NumberOfImageColumns
	Size imageSize(Constants::NumberOfImageRows,Constants::NumberOfImageColumns);
	int numberOfImagePixels = Constants::NumberOfImageRows * Constants::NumberOfImageColumns;

	//we resize all images
	for(unsigned int i=0; i<numberOfExtractedObjects; i++)
		resize(objects[i], objects[i], imageSize);

	//we discard the images with all white pixels and all black pixelss
	//because there is no chance that they have a character
	i = 0;
	while(i < numberOfExtractedObjects)
	{
		int whitePixels = countNonZero(objects[i]);
		int blackPixels = numberOfImagePixels - whitePixels;

		if(blackPixels == 0)
		{
			numberOfExtractedObjects--;
			objects.erase(objects.begin() + i);
		}
		else
			i++;
	}

	this->SavePixelsInTextFile(objects);
	this->SaveCharacterImages(objects);
}

//<summary>
//Extracts upper and lower boundaries of rows of objects in the input image.
//Returns a vector containing the boundaries of all the rows.
//</summary>
//<param name='binaryImage'>An image in which we want to find rows.</param>
vector<RowObjects> ObjectExtractorLibrary::ExtractHorizontalBoundaries(Mat binaryImage)
{
	vector<RowObjects> boundaries;

	//indicates whether the upper boundary has been found
	bool boundaryFound = false;

	//we are looking for white pixels in each row of the image;
	//if we find white pixels, but 'boundaryFound' is false, then
	//we have found an upper row boundary; if, on the other hand,
	//we don't find white pixels, while 'boundaryFound' is true,
	//then we have found a lower boundary
	for(int row=0; row<binaryImage.rows; row++)
	{
		//indicates whether there is a white pixel in the current row
		bool whitePixelFound = false;
		
		//we are looking for white pixels in all columns of the current row
		for(int column=0; column<binaryImage.cols; column++)
		{
			whitePixelFound = binaryImage.at<uchar>(row,column) == 255 ? true : false;

			//we stop iterating when we find a white pixel
			if(whitePixelFound)
				break;
		}

		//if we have found a white pixel, we check if we already have an upper boundary
		if(whitePixelFound)
		{
			//if we haven't found an upper boundary previously,
			//we add a new object in the 'boundaries' vector,
			//setting the current row as an upper boundary
			if(!boundaryFound)
			{
				boundaryFound = true;

				RowObjects rowObject;
				rowObject.RowBoundaries.UpperBoundary = row;
				boundaries.push_back(rowObject);
			}
		}
		//if we couldn't find a white pixel,
		//we check if we found an upper boundary previously
		else
		{
			//if we have an upper boundary, we are setting the current row as a lower boundary
			if(boundaryFound)
			{
				boundaryFound = false;
				boundaries[boundaries.size() - 1].RowBoundaries.LowerBoundary = row;
			}
		}
	}

	//if by some chance we found an upper boundary, but no lower boundary,
	//(this might happen if we have noise in the image)
	//we are setting the upper boundary of the row as a lower boundary
	for(unsigned int i=0; i<boundaries.size(); i++)
	{
		if(boundaries[i].RowBoundaries.LowerBoundary < 0 || boundaries[i].RowBoundaries.LowerBoundary > binaryImage.rows)
			boundaries[i].RowBoundaries.LowerBoundary = boundaries[i].RowBoundaries.UpperBoundary;
	}

	return boundaries;
}

//<summary>
//Finds objects within previously identified rows in the image.
//</summary>
//<param name='binaryImage'>An image in which we want to find objects.<param>
//<param name='boundaries'>Reference to a vector of objects that contain:
//	- boundaries of previously identified rows.
//	- a vector for storing objects in each row.
//<param>
void ObjectExtractorLibrary::ExtractRowObjects(Mat binaryImage, vector<RowObjects>& boundaries)
{
	unsigned int numberOfRowsDetected = boundaries.size();
	
	//for each of the previously identified rows, we are
	//locating vertical object boundaries and then horizontal object boundaries
	for(unsigned int i=0; i<numberOfRowsDetected; i++)
	{
		this->ExtractVerticalObjectBoundaries(binaryImage, boundaries, i);
		this->ExtractHorizontalObjectBoundaries(binaryImage, boundaries, i);
	}
}

//<summary>
//Extracts vertical boundaries of objects within the boundaries of the row with indes 'currentRow'.
//</summary>
//<param name='binaryImage'>An image in which we want to find objects.</param>
//<param name='boundaries'>Reference to a vector of objects that contain:
//	- boundaries of previously identified rows.
//	- a vector for storing objects in each row.
//</param>
//<param name='currentRow'>Index of the identified row that we want to investigate.</param>
void ObjectExtractorLibrary::ExtractVerticalObjectBoundaries(Mat binaryImage, vector<RowObjects>& boundaries, unsigned int currentRow)
{
	//we don't want to miss objects whose boundary is the lower boundary of
	//the current row, so we check if there is a row below the row boundary;
	//if there is, we set 'lowerBoundary' to the index of the row below
	//the boundary of the current row; otherwise, we set it to the index of the row boundary
	int lowerBoundary = boundaries[currentRow].RowBoundaries.LowerBoundary < binaryImage.rows 
		? boundaries[currentRow].RowBoundaries.LowerBoundary + 1 : boundaries[currentRow].RowBoundaries.LowerBoundary;

	//indicates whether a left boundary has been found
	bool boundaryFound = false;

	//we loop through all columns in the image, but we
	//look for objects witnin the boundaries of the current row
	for(int column=0; column<binaryImage.cols; column++)		
	{
		//indicates whether there is a white pixel in the current row
		bool whitePixelFound = false;
		
		//we are looking for white pixels within the row boundaries
		for(int row=boundaries[currentRow].RowBoundaries.UpperBoundary; row<lowerBoundary; row++)
		{
			whitePixelFound = binaryImage.at<uchar>(row,column) == 255 ? true : false;

			//we stop iterating when we find a white pixel
			if(whitePixelFound)
				break;
		}

		//if we have found a white pixel, we check if we already have a left boundary
		if(whitePixelFound)
		{
			//if we haven't found a left boundary previously,
			//we add a new object in the 'boundaries.Objects' vector,
			//setting the current column in the image as a left boundary of the object
			if(!boundaryFound)
			{
				boundaryFound = true;

				ObjectBoundary boundary;
				boundary.LeftBoundary = column;
				boundaries[currentRow].Objects.push_back(boundary);
			}
		}
		//if we couldn't find a white pixel,
		//we check if we found a left boundary previously
		else
		{
			//if we have a left boundary, we are setting the current image column as a right boundary of the object
			if(boundaryFound)
			{
				boundaryFound = false;
				boundaries[currentRow].Objects[boundaries[currentRow].Objects.size() - 1].RightBoundary = column;
			}
		}
	}

	if(boundaries[currentRow].Objects[boundaries[currentRow].Objects.size() - 1].RightBoundary < 0
		|| boundaries[currentRow].Objects[boundaries[currentRow].Objects.size() - 1].RightBoundary > binaryImage.cols)
	{
		boundaries[currentRow].Objects[boundaries[currentRow].Objects.size() - 1].RightBoundary = binaryImage.cols - 1;
	}

	unsigned int i = 0;
	while(i < boundaries[currentRow].Objects.size())
	{
		if((boundaries[currentRow].Objects[i].RightBoundary - boundaries[currentRow].Objects[i].LeftBoundary) <= 20)
			boundaries[currentRow].Objects.erase(boundaries[currentRow].Objects.begin() + i);
		else
			i++;
	}
}

//<summary>
//Extracts horizontal boundaries of objects within the boundaries of the row with indes 'currentRow'.
//</summary>
//<param name='binaryImage'>An image in which we want to find objects.</param>
//<param name='boundaries'>Reference to a vector of objects that contain:
//	- boundaries of previously identified rows.
//	- a vector for storing objects in each row.
//</param>
//<param name='currentRow'>Index of the identified row that we want to investigate.</param>
void ObjectExtractorLibrary::ExtractHorizontalObjectBoundaries(Mat binaryImage, vector<RowObjects>& boundaries, unsigned int currentRow)
{
	unsigned int numberOfPotentialObjectsFound = boundaries[currentRow].Objects.size();
	if(numberOfPotentialObjectsFound == 0)
		return;

	//we don't want to miss objects whose boundary is the lower boundary of
	//the current row, so we check if there is a row below the row boundary;
	//if there is, we set 'lowerBoundary' to the index of the row below
	//the boundary of the current row; otherwise, we set it to the index of the row boundary
	int lowerBoundary = boundaries[currentRow].RowBoundaries.LowerBoundary < binaryImage.rows 
		? boundaries[currentRow].RowBoundaries.LowerBoundary + 1 : boundaries[currentRow].RowBoundaries.LowerBoundary;

	//we are looking for horizontal boundaries of each object whose vertical boundaries were previously identified
	for(unsigned int objectCounter=0; objectCounter<numberOfPotentialObjectsFound; objectCounter++)
	{
		//indicates whether an upper boundary has been found
		bool boundaryFound = false;
		
		//we loop within the boundaries of the current row and look for object boundaries there
		for(int row=boundaries[currentRow].RowBoundaries.UpperBoundary; row<lowerBoundary; row++)
		{
			//indicates whether there is a white pixel in the current row
			bool whitePixelFound = false;

			//indicates whether there is a white pixel in the next row in the image;
			//useful in cases when we might be missing values in one row
			bool whitePixelFoundInNextRow = false;

			//we don't want to miss the right boundary of the objects,
			//so we check if there is a column next to the object boundary;
			//if there is, we set 'rightBoundary' to the index of the column next to
			//the boundary of the current object; otherwise, we set it to the index of the object boundary
			int rightBoundary = boundaries[currentRow].Objects[objectCounter].RightBoundary < binaryImage.cols 
				? boundaries[currentRow].Objects[objectCounter].RightBoundary + 1 : boundaries[currentRow].Objects[objectCounter].RightBoundary;

			//we look for horizontal boundaries within the vertical boundaries of the object
			for(int column=boundaries[currentRow].Objects[objectCounter].LeftBoundary; column<rightBoundary; column++)
			{
				whitePixelFound = binaryImage.at<uchar>(row,column) == 255 ? true : false;
				if(whitePixelFound)
					break;

				if(row + 1 < binaryImage.rows)
					whitePixelFoundInNextRow = binaryImage.at<uchar>(row+1,column) == 255 ? true: false;
			}

			//there might have been missing pixel values in the current row,
			//so we take the white pixel values in the next row
			if(whitePixelFoundInNextRow && !whitePixelFound)
				whitePixelFound = whitePixelFoundInNextRow;

			//if we have found a white pixel, we check if we already have an upper boundary
			if(whitePixelFound)
			{
				//we set the upper boundary of the currently investigated object to the current image row
				if(!boundaryFound)
				{
					boundaryFound = true;
					boundaries[currentRow].Objects[objectCounter].UpperBoundary = row;
				}
			}
			//if we couldn't find a white pixel,
			//we check if we found an upper boundary previously
			else
			{
				//if we have an upper boundary, we are setting the current image row as a lower boundary of the object
				if(boundaryFound)
				{
					boundaryFound = false;
					boundaries[currentRow].Objects[objectCounter].LowerBoundary = row;
					break;
				}
			}
		}
	}

	if(boundaries[currentRow].Objects[boundaries[currentRow].Objects.size()-1].LowerBoundary < 0 ||
		boundaries[currentRow].Objects[boundaries[currentRow].Objects.size()-1].LowerBoundary > lowerBoundary)
	{
		boundaries[currentRow].Objects[boundaries[currentRow].Objects.size()-1].LowerBoundary = lowerBoundary;
	}

	unsigned int i = 0;
	while(i < boundaries[currentRow].Objects.size())
	{
		if((boundaries[currentRow].Objects[i].LowerBoundary - boundaries[currentRow].Objects[i].UpperBoundary) <= 20)
			boundaries[currentRow].Objects.erase(boundaries[currentRow].Objects.begin() + i);
		else
			i++;
	}
}

//<summary>
//Uses the object boundaries stored in 'boundaries[index].Objects' for extracting subimages from the image.
//Returns a vector containing the extracted subimages.
//</summary>
//<param name='binaryImage'>An image from which we are extracting subimages.</param>
//<param name='boundaries'>Vector of objects that contain:
//	- boundaries of previously identified rows.
//	- a vector for storing objects in each row.
//</param>
vector<Mat> ObjectExtractorLibrary::ExtractObjects(Mat binaryImage, vector<RowObjects> boundaries)
{
	unsigned int numberOfRowsFound = boundaries.size();
	vector<Mat> objects;
	
	Scalar color(255, 0, 0);

	//we loop through each row in the image,
	//extracting the identified objects as subimages
	for(unsigned int i=0; i<numberOfRowsFound; i++)
	{
		unsigned int numberOfObjectsFound = boundaries[i].Objects.size();
		for(unsigned int objectCounter=0; objectCounter<numberOfObjectsFound; objectCounter++)
		{
			//if we don't have all boundaries of an object by some accident, we skip the object
			if(boundaries[i].Objects[objectCounter].LeftBoundary < 0 || boundaries[i].Objects[objectCounter].RightBoundary < 0 || 
				boundaries[i].Objects[objectCounter].UpperBoundary < 0 || boundaries[i].Objects[objectCounter].LowerBoundary < 0)
				continue;

			//if the left and right boundaries or the upper and lower boundaries are not valid, we skip the object
			if(boundaries[i].Objects[objectCounter].LeftBoundary >= boundaries[i].Objects[objectCounter].RightBoundary
				|| boundaries[i].Objects[objectCounter].UpperBoundary >= boundaries[i].Objects[objectCounter].LowerBoundary)
				continue;

			//we are creating a subimage that starts in the upper left boundary of the object
			//and whose width and height are equal to 'right boundary - left boundary' and
			//'lower boundary - upper boundary' respectively
			Mat object(binaryImage, Rect(boundaries[i].Objects[objectCounter].LeftBoundary, 
											boundaries[i].Objects[objectCounter].UpperBoundary, 
											boundaries[i].Objects[objectCounter].RightBoundary - boundaries[i].Objects[objectCounter].LeftBoundary, 
											boundaries[i].Objects[objectCounter].LowerBoundary - boundaries[i].Objects[objectCounter].UpperBoundary));
			objects.push_back(object);
		}
	}

	return objects;
}

//<summary>
//Saves the pixels of the objects in a text file with name
//'Constants::CharacterPixelsFile', making them usable for a classifier.
//</summary>
//<param name='objects'>A vector containing the objects whose pixels we want to save in the file.</param>
void ObjectExtractorLibrary::SavePixelsInTextFile(vector<Mat> objects)
{
	unsigned int numberOfExtractedObjects = objects.size();

	//we save the images in a text file so that they are usable by a classifier
	ofstream images;
	images.open(Constants::CharacterPixelsFile);

	bool whitePixelFound;
	for(unsigned int i=0; i<numberOfExtractedObjects; i++)
	{
		for(int row=0; row<Constants::NumberOfImageRows; row++)
		{
			for(int column=0; column<Constants::NumberOfImageColumns; column++)
			{
				whitePixelFound = objects[i].at<uchar>(row,column) == 255 ? true : false;

				if(whitePixelFound)
					images << "1.0 ";
				else
					images << "0.0 ";
			}
		}

		if(i != (numberOfExtractedObjects-1))
			images << "\n";
	}

	images.close();
}

//<summary>
//Saves the extracted charactes as images. The images are saved in a folder with name
//'Constants::CharacterImagesFolder', having file name 'character ' + counter.
//</summary>
//<param name='objects'>A vector containing the extracted characters.</param>
void ObjectExtractorLibrary::SaveCharacterImages(vector<Mat> objects)
{
	unsigned int numberOfExtractedObjects = objects.size();
	for(unsigned int i=0; i<numberOfExtractedObjects; i++)
	{
		string filename = Constants::CharacterImagesFolder + "character " + std::to_string(i) + ".jpg";
		imwrite(filename, objects[i]);
	}
}

#endif