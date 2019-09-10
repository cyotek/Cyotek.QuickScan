using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Cyotek.QuickScan
{
  internal static class ClipboardUtilities
  {
    #region Public Methods

    public static bool CopyImage(Image image)
    {
      bool result;

      // http://csharphelper.com/blog/2014/09/copy-an-irregular-area-from-one-picture-to-another-in-c/

      try
      {
        IDataObject data;
        Bitmap opaqueBitmap;
        Bitmap transparentBitmap;
        MemoryStream transparentBitmapStream;

        data = new DataObject();
        opaqueBitmap = null;
        transparentBitmap = null;
        transparentBitmapStream = null;

        try
        {
          opaqueBitmap = image.Copy(Color.White);
          transparentBitmap = image.Copy(Color.Transparent);

          transparentBitmapStream = new MemoryStream();
          transparentBitmap.Save(transparentBitmapStream, ImageFormat.Png);

          data.SetData(DataFormats.Bitmap, opaqueBitmap);
          data.SetData("PNG", false, transparentBitmapStream);

          Clipboard.Clear();
          Clipboard.SetDataObject(data, true);
        }
        finally
        {
          opaqueBitmap?.Dispose();
          transparentBitmapStream?.Dispose();
          transparentBitmap?.Dispose();
        }

        result = true;
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("Failed to copy image. {0}", ex.GetBaseException().Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);

        result = false;
      }

      return result;
    }

    #endregion Public Methods
  }
}