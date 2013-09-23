#include "ObjectExtractorLibrary.h"
#include <vector>
#include <opencv\cv.h>
#include <opencv\highgui.h>

using namespace cv;
using std::vector;

int main()
{
	ObjectExtractorLibrary objectExtractor("characterImage.jpg");
	objectExtractor.ExtractAndSaveObjects();

	return 0;
}