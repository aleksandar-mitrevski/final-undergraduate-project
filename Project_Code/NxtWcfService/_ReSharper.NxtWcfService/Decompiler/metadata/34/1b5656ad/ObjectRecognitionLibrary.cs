// Type: ObjectRecognition.ObjectRecognitionLibrary
// Assembly: ObjectRecognitionLibrary, Version=1.0.4836.14565, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\aleksandar_mitrevski\Documents\Visual Studio 2012\Projects\Bachelor's Thesis\NXT\NxtWcfService\NxtWcfService\bin\ObjectRecognitionLibrary.dll

namespace ObjectRecognition
{
  public class ObjectRecognitionLibrary
  {
    public ObjectRecognitionLibrary(int inputNeurons, int hiddenNeurons, int outputNeurons);
    public void TrainClassifier(double errorThreshold, double learningRate, int numberOfIterations);
    public int[] ClassifyCharacters(string fileName);
  }
}
