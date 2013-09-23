// NumberExtractorLibrary.h

#pragma once

#include "ObjectExtractorLibrary.h"
using namespace System;
using namespace Runtime::InteropServices;

namespace NumberExtractor
{
	public ref class NumberExtractorLibrary
	{
	public:
		bool ProcessImages(System::String ^ fileName)
		{
			char* filename;

			try
			{
				filename = (char*)(Marshal::StringToHGlobalAnsi(fileName)).ToPointer();

				ObjectExtractorLibrary library(filename);
				library.ExtractAndSaveObjects();

				return true;
			}
			catch(...)
			{
				return false;
			}
			finally
			{
				Marshal::FreeHGlobal(IntPtr((void*)filename));
			}
		}
	};
}