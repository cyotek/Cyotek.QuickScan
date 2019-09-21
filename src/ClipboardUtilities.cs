using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Cyotek.QuickScan
{
  internal static class ClipboardUtilities
  {
    #region Public Fields

    public const string PngFormat = "PNG";

    #endregion Public Fields

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
          data.SetData(PngFormat, false, transparentBitmapStream);

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

    public static Image GetImage()
    {
      Image result;

      // http://csharphelper.com/blog/2014/09/paste-a-png-format-image-with-a-transparent-background-from-the-clipboard-in-c/

      result = null;

      try
      {
        if (Clipboard.ContainsData(PngFormat))
        {
          object data;

          data = Clipboard.GetData(PngFormat);

          if (data != null)
          {
            Stream stream;

            stream = data as MemoryStream;

            if (stream == null)
            {
              byte[] buffer;

              buffer = data as byte[];

              if (buffer != null)
              {
                stream = new MemoryStream(buffer);
              }
            }

            if (stream != null)
            {
              result = Image.FromStream(stream).Copy();

              stream.Dispose();
            }
          }
        }

        if (result == null)
        {
          result = Clipboard.GetImage();
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("Failed to obtain image. {0}", ex.GetBaseException().Message), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      return result;
    }

    #endregion Public Methods
  }
}