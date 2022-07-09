using System.IO;

namespace Cyotek.QuickScan
{
  internal static class FileUtilities
  {
    #region Public Methods

    public static void DeleteFile(string fileName)
    {
      if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName))
      {
        File.Delete(fileName);
      }
    }

    #endregion Public Methods
  }
}