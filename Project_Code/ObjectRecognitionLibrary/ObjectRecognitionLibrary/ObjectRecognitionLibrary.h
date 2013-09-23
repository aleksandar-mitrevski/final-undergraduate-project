// ObjectRecognitionLibrary.h

#pragma once

#include "NeuralNetworkTrainer.h"
#include "NeuralNetworkClassifier.h"
using namespace System;
using namespace Runtime::InteropServices;

namespace ObjectRecognition
{
	public ref class ObjectRecognitionLibrary
	{
	public:
		ObjectRecognitionLibrary(Int32 inputNeurons, Int32 hiddenNeurons, Int32 outputNeurons)
		{
			this->inputNeurons = inputNeurons;
			this->hiddenNeurons = hiddenNeurons;
			this->outputNeurons = outputNeurons;
		}

		void TrainClassifier(Double errorThreshold, Double learningRate, Int32 numberOfIterations)
		{
			NeuralNetworkInput trainData = readTrainDataFromFile("trainingData.txt", ' ');
			trainData.ErrorThreshold = errorThreshold;
			trainData.LearningRate = learningRate;
			trainData.NumberOfMaximumIterations = numberOfIterations;

			NeuralNetworkTrainer trainer(this->inputNeurons, this->hiddenNeurons, this->outputNeurons);
			trainer.Train(trainData);
		}

		array<Int32>^ ClassifyCharacters(String^ fileName)
		{
			char* filename = (char*)(Marshal::StringToHGlobalAnsi(fileName)).ToPointer();

			vector<vector<double>> testData = readTestDataFromFile(filename, ' ');
			NeuralNetworkClassifier classifier(this->inputNeurons, this->hiddenNeurons, this->outputNeurons);
			vector<int> classes = classifier.Classify(testData);

			array<Int32>^ classesManaged = gcnew array<Int32>(classes.size());
			for(unsigned int i=0; i<classes.size(); i++)
				classesManaged[i] = classes[i];

			Marshal::FreeHGlobal(IntPtr((void*)filename));
			return classesManaged;
		}

	private:
		int inputNeurons;
		int hiddenNeurons;
		int outputNeurons;

		NeuralNetworkInput readTrainDataFromFile(const char* filename, const char delimiter)
		{
			NeuralNetworkInput trainData;

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
				document.exceptions(ifstream::badbit);
				document.open(filename);

				while(getline(document,lineReader))
				{
					dataCounter = 0;
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
					while(dataCounter < this->inputNeurons)
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

			return trainData;
		}

		vector<vector<double>> readTestDataFromFile(const char* filename, const char delimiter)
		{
			vector<vector<double>> testData;

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
				document.exceptions(ifstream::badbit);
				document.open(filename);

				while(getline(document,lineReader))
				{
					dataCounter = 0;
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
					while(dataCounter < this->inputNeurons)
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

					testData.push_back(currentRow);
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

			return testData;
		}
	};
}