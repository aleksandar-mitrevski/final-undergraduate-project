#ifndef CONSTANTS_H
#define CONSTANTS_H

#include <string>
using std::string;

class Constants
{
public:
	static int NumberOfImageRows;
	static int NumberOfImageColumns;
	static string CharacterPixelsFile;
	static string CharacterImagesFolder;
};

int Constants::NumberOfImageRows = 16;
int Constants::NumberOfImageColumns = 16;
string Constants::CharacterPixelsFile = "C:\\NxtPath\\classifierData\\characters.txt";
string Constants::CharacterImagesFolder = "C:\\NxtPath\\opencvImages\\";

#endif