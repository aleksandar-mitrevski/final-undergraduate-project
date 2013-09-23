#ifndef NEURAL_NETWORK_INPUT_H
#define NEURAL_NETWORK_INPUT_H

#include <vector>
using std::vector;

struct NeuralNetworkInput
{
	vector<vector<double>> Data;
	vector<vector<double>> ExpectedOutputs;
	double LearningRate;
	double ErrorThreshold;
	int NumberOfMaximumIterations;
};

#endif