#ifndef NEURAL_NETWORK_BASE_H
#define NEURAL_NETWORK_BASE_H

#include "Neuron.h"
#include "Constants.h"
#include <vector>
#include <stddef.h>

using std::vector;

class NeuralNetworkBase
{
public:
	NeuralNetworkBase(int numberOfInputNeurons, int numberOfHiddenNeurons, int numberOfOutputNeurons);
	~NeuralNetworkBase();
	void FeedForward(vector<double> pattern);

	vector<Neuron> inputNeurons;
	vector<Neuron> hiddenNeurons;
	vector<Neuron> outputNeurons;

	int numberOfInputNeurons;
	int numberOfHiddenNeurons;
	int numberOfOutputNeurons;

private:
	void InsertCurrentNetworkInput(vector<double> pattern);
	double LogisticFunction(double x);
};

NeuralNetworkBase::NeuralNetworkBase(int numberOfInputNeurons, int numberOfHiddenNeurons, int numberOfOutputNeurons)
{
	this->numberOfInputNeurons = numberOfInputNeurons;
	this->numberOfHiddenNeurons = numberOfHiddenNeurons;
	this->numberOfOutputNeurons = numberOfOutputNeurons;

	for(int i=0; i<this->numberOfInputNeurons; i++)
	{
		Neuron newNeuron;
		for(int j=0; j<this->numberOfHiddenNeurons; j++)
			newNeuron.Weights.push_back(0.0);
		this->inputNeurons.push_back(newNeuron);
	}

	for(int i=0; i<this->numberOfHiddenNeurons; i++)
	{
		Neuron newNeuron;
		for(int j=0; j<this->numberOfOutputNeurons; j++)
			newNeuron.Weights.push_back(0.0);
		this->hiddenNeurons.push_back(newNeuron);
	}

	for(int i=0; i<this->numberOfOutputNeurons; i++)
	{
		Neuron newNeuron;
		this->outputNeurons.push_back(newNeuron);
	}
}

NeuralNetworkBase::~NeuralNetworkBase()
{
	//delete [] this->inputNeurons;
	//delete [] this->hiddenNeurons;
	//delete [] this->outputNeurons;
}

void NeuralNetworkBase::FeedForward(vector<double> pattern)
{
	this->InsertCurrentNetworkInput(pattern);

	double result;
	for(int i=0; i<this->numberOfHiddenNeurons; i++)
	{
		result = 0.0;
		for(int j=0; j<this->numberOfInputNeurons; j++)
			result = result + (this->inputNeurons[j].Value * this->inputNeurons[j].Weights[i]);
		this->hiddenNeurons[i].Value = this->LogisticFunction(result);
	}

	for(int i=0; i<this->numberOfOutputNeurons; i++)
	{
		result = 0.0;
		for(int j=0; j<this->numberOfHiddenNeurons; j++)
			result = result + (this->hiddenNeurons[j].Value * this->hiddenNeurons[j].Weights[i]);
		this->outputNeurons[i].Value = this->LogisticFunction(result);
	}
}

void NeuralNetworkBase::InsertCurrentNetworkInput(vector<double> pattern)
{
	for(int i=0; i<this->numberOfInputNeurons; i++)
		this->inputNeurons[i].Value = pattern[i];
}

double NeuralNetworkBase::LogisticFunction(double x)
{
	double result = 1.0 / (1 + exp(-x));
	return result;
}

#endif