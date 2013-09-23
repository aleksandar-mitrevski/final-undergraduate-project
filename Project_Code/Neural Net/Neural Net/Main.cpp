#include "NeuralNetworkTrainer.h"
#include "NeuralNetworkClassifier.h"
#include <fstream>
#include <vector>
#include <string>
#include <sstream>
#include <iostream>

using std::cout;
using std::ifstream;
using std::vector;
using std::string;
using std::stringstream;

const int NUMBER_OF_INPUT_NEURONS = 256;
const int NUMBER_OF_HIDDEN_NEURONS = 40;
const int NUMBER_OF_OUTPUT_NEURONS = 12;

NeuralNetworkInput trainData;

void readDataFromFile(const char* filename, const char delimiter);

int main()
{
	readDataFromFile("trainingData.txt", ' ');
	NeuralNetworkTrainer neuralNetwork(NUMBER_OF_INPUT_NEURONS, NUMBER_OF_HIDDEN_NEURONS, NUMBER_OF_OUTPUT_NEURONS);
	trainData.ErrorThreshold = 0.05;
	trainData.LearningRate = 0.1;
	trainData.NumberOfMaximumIterations = 5000;

	cout << "Training started...\n";
	neuralNetwork.Train(trainData);
	cout << "Training finished";

	getchar();

	//NeuralNetworkClassifier classifier(NUMBER_OF_INPUT_NEURONS, NUMBER_OF_HIDDEN_NEURONS, NUMBER_OF_OUTPUT_NEURONS);
	//double correctlyClassified = 0;
	//for(int i=0; i<trainData.Data.size(); i++)
	//{
	//	int maxClass = classifier.Classify(trainData.Data[i]);
	//	int desiredClass = -1;

	//	for(int j=0; j<trainData.ExpectedOutputs[i].size(); j++)
	//	{
	//		if(abs(trainData.ExpectedOutputs[i][j] - 1.0) < 0.005)
	//		{
	//			desiredClass = j;
	//			break;
	//		}
	//	}

	//	if(desiredClass == maxClass)
	//		correctlyClassified += 1.0;
	//}

	//double accuracy = correctlyClassified / trainData.Data.size();
	//cout << "training set accuracy = " << accuracy;

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
	int dataCounter;

	//used for looping through the file line
	//which was read in the current iteration
	unsigned int i = 0;

	try
	{
		document.exceptions(ifstream::badbit | ifstream::failbit);
		document.open(filename);

		while(!document.eof())
		{
			dataCounter = 0;
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

			i = 0;

			//we loop through the file line and extract numbers
			while(dataCounter < NUMBER_OF_INPUT_NEURONS)
			{
				//if we find a delimiter, we extract a number,
				//convert it to 'float' and then store it in the data matrix
				if(lineReader[i] == delimiter)
				{
					converter << lineReader.substr(delimiterIndex, i-delimiterIndex);
					converter >> tempNumber;
					currentRow.push_back(tempNumber);

					delimiterIndex = i+1;
					dataCounter++;

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

			vector<double> currentExpectedOutput;
			int classCounter = 0;
			while(i<lineReader.size())
			{
				//if we find a delimiter, we extract a number,
				//convert it to 'float' and then store it in the data matrix
				if(lineReader[i] == delimiter)
				{
					converter << lineReader.substr(delimiterIndex, i-delimiterIndex);
					converter >> tempNumber;
					currentExpectedOutput.push_back(tempNumber);

					delimiterIndex = i+1;
					dataCounter++;

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

			trainData.Data.push_back(currentRow);
			trainData.ExpectedOutputs.push_back(currentExpectedOutput);
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