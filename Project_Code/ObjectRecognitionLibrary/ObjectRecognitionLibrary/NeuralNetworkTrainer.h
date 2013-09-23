#ifndef NEURAL_NETWORK_H
#define NEURAL_NETWORK_H

#include "NeuralNetworkBase.h"
#include "NeuralNetworkInput.h"
#include <vector>
#include <ctime>
#include <fstream>

using std::vector;
using std::ofstream;

class NeuralNetworkTrainer : public NeuralNetworkBase
{
public:
	NeuralNetworkTrainer(int numberOfInputNeurons, int numberOfHiddenNeurons, int numberOfOutputNeurons);
	void Train(NeuralNetworkInput trainData);

private:
	void InitializeWeights();
	double Backpropagate(vector<double> expectedOutput, double learningRate);
	void SaveWeightsToFile();
};


NeuralNetworkTrainer::NeuralNetworkTrainer(int numberOfInputNeurons, int numberOfHiddenNeurons, int numberOfOutputNeurons)
	: NeuralNetworkBase(numberOfInputNeurons, numberOfHiddenNeurons, numberOfOutputNeurons){ }

void NeuralNetworkTrainer::InitializeWeights()
{
	double randomNumber = 0.0;
	srand((unsigned)time(NULL));
	for(int i=0; i<this->numberOfInputNeurons; i++)
	{
		for(int j=0; j<this->numberOfHiddenNeurons; j++)
		{
			randomNumber = ((double)rand() / (double)RAND_MAX);
			this->inputNeurons[i].Weights[j] = randomNumber;
		}
	}

	for(int i=0; i<this->numberOfHiddenNeurons; i++)
	{
		for(int j=0; j<this->numberOfOutputNeurons; j++)
		{
			randomNumber = ((double)rand() / (double)RAND_MAX);
			this->hiddenNeurons[i].Weights[j] = randomNumber;
		}
	}
}

void NeuralNetworkTrainer::Train(NeuralNetworkInput trainData)
{
	unsigned int numberOfTrainingPatterns = trainData.Data.size();
	int numberOfIterations = 0;
	double error = 10000000;

	this->InitializeWeights();

	while(error > trainData.ErrorThreshold && numberOfIterations < trainData.NumberOfMaximumIterations)
	{
		error = 0.0;

		for(unsigned int i=0; i<numberOfTrainingPatterns; i++)
		{
			this->FeedForward(trainData.Data[i]);
			error += this->Backpropagate(trainData.ExpectedOutputs[i], trainData.LearningRate);
		}

		error /= 2.0;
		numberOfIterations++;
	}

	this->SaveWeightsToFile();
}

double NeuralNetworkTrainer::Backpropagate(vector<double> expectedOutput, double learningRate)
{
	//will be used for backpropagation
	double *hiddenToOutputDeltas = new double[this->numberOfOutputNeurons];
	double *inputToHiddenDeltas = new double[this->numberOfHiddenNeurons];

	double totalError = 0.0;
	for(int i=0; i<this->numberOfOutputNeurons; i++)
	{
		double difference = expectedOutput[i] - this->outputNeurons[i].Value;
		totalError = totalError + (difference * difference);
		hiddenToOutputDeltas[i] = this->outputNeurons[i].Value * (1 - this->outputNeurons[i].Value) * difference;
	}

	for(int i=0; i<this->numberOfHiddenNeurons; i++)
	{
		double derivative = this->hiddenNeurons[i].Value * (1 - this->hiddenNeurons[i].Value);
		double correction = 0.0;
		for(int j=0; j<this->numberOfOutputNeurons; j++)
			correction = correction + (this->hiddenNeurons[i].Weights[j] * hiddenToOutputDeltas[j]);

		correction *= derivative;
		inputToHiddenDeltas[i] = correction;
	}

	//we correct the weights from the hidden to the output layer
	for(int i=0; i<this->numberOfHiddenNeurons; i++)
	{
		for(int j=0; j<this->numberOfOutputNeurons; j++)
		{
			double weightCorrection = learningRate * hiddenToOutputDeltas[j] * this->hiddenNeurons[i].Value;
			this->hiddenNeurons[i].Weights[j] = this->hiddenNeurons[i].Weights[j] + weightCorrection;
		}
	}

	//we correct the weights from the input to the hidden layer
	for(int i=0; i<this->numberOfInputNeurons; i++)
	{
		for(int j=0; j<this->numberOfHiddenNeurons; j++)
		{
			double weightCorrection = learningRate * inputToHiddenDeltas[j] * this->inputNeurons[i].Value;
			this->inputNeurons[i].Weights[j] = this->inputNeurons[i].Weights[j] + weightCorrection;
		}
	}

	delete [] hiddenToOutputDeltas;
	delete [] inputToHiddenDeltas;

	return totalError;
}

void NeuralNetworkTrainer::SaveWeightsToFile()
{
	ofstream outFile;
	outFile.open(Constants::HiddenWeightsFilename);
	for(int i=0; i<this->numberOfInputNeurons - 1; i++)
	{
		for(int j=0; j<this->numberOfHiddenNeurons - 1; j++)
			outFile << this->inputNeurons[i].Weights[j] << ",";
		outFile << this->inputNeurons[i].Weights[this->numberOfHiddenNeurons-1] << "\n";
	}

	for(int j=0; j<this->numberOfHiddenNeurons - 1; j++)
		outFile << this->inputNeurons[this->numberOfInputNeurons-1].Weights[j] << ",";
	outFile << this->inputNeurons[this->numberOfInputNeurons-1].Weights[this->numberOfHiddenNeurons-1];

	outFile.close();
	outFile.clear();

	outFile.open(Constants::OutputWeightsFilename);
	for(int i=0; i<this->numberOfHiddenNeurons - 1; i++)
	{
		for(int j=0; j<this->numberOfOutputNeurons - 1; j++)
			outFile << this->hiddenNeurons[i].Weights[j] << ",";
		outFile << this->hiddenNeurons[i].Weights[this->numberOfOutputNeurons-1] << "\n";
	}

	for(int j=0; j<this->numberOfOutputNeurons - 1; j++)
		outFile << this->hiddenNeurons[this->numberOfHiddenNeurons-1].Weights[j] << ",";
	outFile << this->hiddenNeurons[this->numberOfHiddenNeurons-1].Weights[this->numberOfOutputNeurons-1];

	outFile.close();
}

#endif