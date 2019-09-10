using System;
using System.Globalization;

namespace Cyotek
{
  internal static class LongExtensions
  {
    #region Constants

    private const long _gigabyte = 1024 * _megabyte;

    private const long _kilobyte = 1024;

    private const long _megabyte = 1024 * _kilobyte;

    private const long _petabyte = 1024 * _terabyte;

    private const long _terabyte = 1024 * _gigabyte;

    #endregion

    #region Static Methods

    public static string ToFileSizeString(this long fileSize)
    {
      return ((ulong)fileSize).ToFileSizeString();
    }

    public static string ToFileSizeString(this ulong fileSize)
    {
      string result;

      if (fileSize > _petabyte)
      {
        result = (fileSize / (double)_petabyte).ToString("0.00 PB");
      }
      else if (fileSize > _terabyte)
      {
        result = (fileSize / (double)_terabyte).ToString("0.00 TB");
      }
      else if (fileSize > _gigabyte)
      {
        result = (fileSize / (double)_gigabyte).ToString("0.00 GB");
      }
      else if (fileSize > _megabyte)
      {
        result = (fileSize / (double)_megabyte).ToString("0.00 MB");
      }
      else if (fileSize > _kilobyte)
      {
        result = (fileSize / (double)_kilobyte).ToString("0.00 KB");
      }
      else
      {
        result = fileSize.ToString(CultureInfo.InvariantCulture) + " bytes";
      }

      if (result.Length > 6)
      {
        int length;

        length = result.Length;

        if (result[length - 6] == '.' && result[length - 5] == '0' && result[length - 4] == '0' && result[length - 3] == ' ')
        {
          result = result.Substring(0, length - 6) + result.Substring(length - 3);
        }
      }

      return result;
    }

    #endregion
  }
}
