#ifndef CONSTANTS_H
#define CONSTANTS_H

#include <string>
using std::string;

class Constants
{
public:
	static string HiddenWeightsFilename;
	static string OutputWeightsFilename;
};

string Constants::HiddenWeightsFilename = "C:\\NxtPath\\classifierData\\Hidden Weights.txt";
string Constants::OutputWeightsFilename = "C:\\NxtPath\\classifierData\\Output Weights.txt";

#endif