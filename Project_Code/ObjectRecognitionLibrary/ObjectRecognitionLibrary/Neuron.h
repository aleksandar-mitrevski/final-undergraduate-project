#ifndef NEURON_H
#define NEURON_H

#include <vector>
using std::vector;

struct Neuron
{
	~Neuron()
	{
		//delete this->Weights;
	}

	double Value;
	vector<double> Weights;
};

#endif