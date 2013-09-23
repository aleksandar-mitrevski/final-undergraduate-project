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

string Constants::HiddenWeightsFilename = "Hidden Weights.txt";
string Constants::OutputWeightsFilename = "Output Weights.txt";

#endif