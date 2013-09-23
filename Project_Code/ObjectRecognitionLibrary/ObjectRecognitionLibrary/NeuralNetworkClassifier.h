#ifndef NEURAL_NETWORK_CLASSIFIER_H
#define NEURAL_NETWORK_CLASSIFIER_H

#include "NeuralNetworkBase.h"
#include <fstream>
#include <string>
#include <vector>
#include <sstream>

using std::vector;
using std::ifstream;
using std::string;
using std::stringstream;

class NeuralNetworkClassifier : public NeuralNetworkBase
{
public:
	NeuralNetworkClassifier(int numberOfInputNeurons, int numberOfHiddenNeurons, int numberOfOutputNeurons);
	int Classify(vector<double> pattern);
	vector<int> Classify(vector<vector<double>> patterns);

private:
	void LoadHiddenWeights();
	void LoadOutputWeights();
};


NeuralNetworkClassifier::NeuralNetworkClassifier(int numberOfInputNeurons, int numberOfHiddenNeurons, int numberOfOutputNeurons)
	: NeuralNetworkBase(numberOfInputNeurons, numberOfHiddenNeurons, numberOfOutputNeurons)
{
	this->LoadHiddenWeights();
	this->LoadOutputWeights();
}


int NeuralNetworkClassifier::Classify(vector<double> pattern)
{
	this->FeedForward(pattern);

	int maxClass = 0;
	double maxValue = this->outputNeurons[0].Value;

	for(int i=1; i<this->numberOfOutputNeurons; i++)
	{
		if(maxValue < this->outputNeurons[i].Value)
		{
			maxValue = this->outputNeurons[i].Value;
			maxClass = i;
		}
	}

	return maxClass;
}

vector<int> NeuralNetworkClassifier::Classify(vector<vector<double>> patterns)
{
	unsigned int numberOfPatterns = patterns.size();
	vector<int> classes;

	for(unsigned int i=0; i<numberOfPatterns; i++)
	{
		this->FeedForward(patterns[i]);

		int maxClass = 0;
		double maxValue = this->outputNeurons[0].Value;

		for(int j=1; j<this->numberOfOutputNeurons; j++)
		{
			if(maxValue < this->outputNeurons[j].Value)
			{
				maxValue = this->outputNeurons[j].Value;
				maxClass = j;
			}
		}

		classes.push_back(maxClass);
	}

	return classes;
}

void NeuralNetworkClassifier::LoadHiddenWeights()
{
	//stream for reading data from the file
	ifstream document;

	//string for storing a line from the file
	string lineReader;

	//stream for converting strings to numbers
	stringstream converter;

	//variable for storing a converted string to number
	double tempNumber;

	int neuronCounter = 0, weightCounter;

	try
	{
		document.exceptions(ifstream::badbit);
		document.open(Constants::HiddenWeightsFilename);

		while(getline(document,lineReader))
		{
			weightCounter = 0;

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
				if(lineReader[i] == ',')
				{
					converter << lineReader.substr(delimiterIndex, i-delimiterIndex);
					converter >> tempNumber;
					this->inputNeurons[neuronCounter].Weights[weightCounter] = tempNumber;
					weightCounter++;

					delimiterIndex = i+1;

					//we check for repetitive occurences of a delimiter
					while(lineReader[delimiterIndex] == ',')
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
			converter.clear();
			this->inputNeurons[neuronCounter].Weights[weightCounter] = tempNumber;

			neuronCounter++;
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

void NeuralNetworkClassifier::LoadOutputWeights()
{
	//stream for reading data from the file
	ifstream document;

	//string for storing a line from the file
	string lineReader;

	//stream for converting strings to numbers
	stringstream converter;

	//variable for storing a converted string to number
	double tempNumber;

	int neuronCounter = 0, weightCounter;

	try
	{
		document.exceptions(ifstream::badbit);
		document.open(Constants::OutputWeightsFilename);

		while(getline(document,lineReader))
		{
			weightCounter = 0;

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
				if(lineReader[i] == ',')
				{
					converter << lineReader.substr(delimiterIndex, i-delimiterIndex);
					converter >> tempNumber;
					this->hiddenNeurons[neuronCounter].Weights[weightCounter] = tempNumber;
					weightCounter++;

					delimiterIndex = i+1;

					//we check for repetitive occurences of a delimiter
					while(lineReader[delimiterIndex] == ',')
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
			converter.clear();
			this->hiddenNeurons[neuronCounter].Weights[weightCounter] = tempNumber;

			neuronCounter++;
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

#endif