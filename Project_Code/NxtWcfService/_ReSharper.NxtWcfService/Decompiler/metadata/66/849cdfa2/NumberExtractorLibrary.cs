// Type: NumberExtractor.NumberExtractorLibrary
// Assembly: NumberExtractorLibrary, Version=1.0.4838.20886, Culture=neutral, PublicKeyToken=null
// Assembly location: C:\Users\aleksandar_mitrevski\Documents\Visual Studio 2012\Projects\Bachelor's Thesis\NXT\NxtWcfService\NxtWcfService\Dlls\NumberExtractorLibrary.dll

using System.Runtime.InteropServices;

namespace NumberExtractor
{
  public class NumberExtractorLibrary
  {
    [return: MarshalAs(UnmanagedType.U1)]
    public bool ProcessImages(string fileName);
  }
}
