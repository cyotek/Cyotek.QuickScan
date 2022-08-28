using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using WIA;
using WiaProperties = WIA.Properties;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal static class WiaExtensions
  {
    #region Private Fields

    private static readonly WiaPropertyId[] _ignoreCopyProperties = { WiaPropertyId.WIA_IPA_ITEM_NAME, WiaPropertyId.WIA_IPA_FULL_ITEM_NAME, WiaPropertyId.WIA_IPA_ITEM_FLAGS, WiaPropertyId.WIA_IPA_ACCESS_RIGHTS, WiaPropertyId.WIA_IPA_PREFERRED_FORMAT, WiaPropertyId.WIA_IPA_CHANNELS_PER_PIXEL, WiaPropertyId.WIA_IPA_BITS_PER_CHANNEL, WiaPropertyId.WIA_IPA_PLANAR, WiaPropertyId.WIA_IPA_PIXELS_PER_LINE, WiaPropertyId.WIA_IPA_BYTES_PER_LINE, WiaPropertyId.WIA_IPA_NUMBER_OF_LINES, WiaPropertyId.WIA_IPA_ITEM_SIZE, WiaPropertyId.WIA_IPA_MIN_BUFFER_SIZE, WiaPropertyId.WIA_IPA_FILENAME_EXTENSION };

    #endregion Private Fields

    #region Public Methods

    public static void CopyFrom(this WiaProperties properties, WiaProperties dst)
    {
      for (int i = 0; i < properties.Count; i++)
      {
        Property property;
        WiaPropertyId propertyId;

        property = properties[i + 1];
        propertyId = (WiaPropertyId)property.PropertyID;

        if (Array.IndexOf(_ignoreCopyProperties, propertyId) == -1)
        {
          try
          {
            dst.SetPropertyValue(propertyId, (object)property.get_Value());
          }
          catch (UnauthorizedAccessException)
          {
            // can't copy this property
          }
        }
      }
    }

    public static void CopyPropertyValueFrom(this WiaProperties properties, WiaPropertyId id, WiaProperties copy)
    {
      Property property;
      Property copyFromProperty;

      property = properties[((int)id).ToString()];
      copyFromProperty = copy.GetProperty(id);

      property.let_Value((object)copyFromProperty.get_Value());
    }

    public static void Dump(this WiaProperties properties)
    {
      for (int i = 0; i < properties.Count; i++)
      {
        Property property;

        property = properties[i + 1];

        Trace.WriteLine(property.Name + " = " + property.GetValueString());
      }
    }

    public static Property GetProperty(this WiaProperties properties, WiaPropertyId id)
    {
      return properties[((int)id).ToString()];
    }

    [HandleProcessCorruptedStateExceptions]
    public static string GetValueString(this Property property)
    {
      string value;

      // HACK: Sometimes AccessViolationException is thrown trying to get the value, hence testing HandleProcessCorruptedStateExceptions

      try
      {
        if (property.IsVector)
        {
          value = ((Vector)property.get_Value()).ToSeparatedString();
        }
        else
        {
          switch ((WiaPropertyType)property.Type)
          {
            case WiaPropertyType.ClassIDPropertyType:
            case WiaPropertyType.StringPropertyType: // string
              value = (string)property.get_Value();
              break;

            case WiaPropertyType.LongPropertyType:
              value = ((int)property.get_Value()).ToString();
              break;

            default:
              value = property.get_Value().ToString();
              break;
          }
        }
      }
      catch
      {
        value = null;
      }

      return value;
    }

    public static void SaveFileEx(this ImageFile imageFile, string fileName)
    {
      try
      {
        FileUtilities.DeleteFile(fileName);

        imageFile.SaveFile(fileName);
      }
      catch (Exception ex)
      {
        UiHelpers.ShowError("Failed to save image.", ex);
      }
    }

    public static void SetPropertyMaximum(this WiaProperties properties, WiaPropertyId id)
    {
      try
      {
        Property property;

        property = properties[((int)id).ToString()];

        property.let_Value(property.SubTypeMax);
      }
      catch (Exception ex)
      {
        UiHelpers.ShowError(string.Format("Failed to update property {0}.", id), ex);
      }

    }

    public static void SetPropertyMinimum(this WiaProperties properties, WiaPropertyId id)
    {
      try
      {
        Property property;

        property = properties[((int)id).ToString()];

        property.let_Value(property.SubTypeMin);
      }
      catch (Exception ex)
      {
        UiHelpers.ShowError(string.Format("Failed to update property {0}.", id), ex);
      }
    }

    public static void SetPropertyValue<T>(this WiaProperties properties, WiaPropertyId id, T value)
    {
      try
      {
        Property property;

        property = properties[((int)id).ToString()];

        property.let_Value(value);
      }
      catch (Exception ex)
      {
        UiHelpers.ShowError(string.Format("Failed to update property {0}.", id), ex);
      }
    }

    public static void SetPropertyValue(this WiaProperties properties, WiaPropertyId id, int value)
    {
      try
      {
        Property property;

        property = properties[((int)id).ToString()];

        if (value >= property.SubTypeMin && value <= property.SubTypeMax)
        {
          property.let_Value(value);
        }
        else
        {
          UiHelpers.ShowWarning(string.Format("Unable to set property {0}, value {3} must be between {1} and {2}.", id, property.SubTypeMin, property.SubTypeMax, value));
        }
      }
      catch (Exception ex)
      {
        UiHelpers.ShowError(string.Format("Failed to update property {0}.", id), ex);
      }
    }

    public static Bitmap ToBitmap(this ImageFile image)
    {
      Bitmap result;
      byte[] data;

      data = (byte[])image.FileData.get_BinaryData();

      using (MemoryStream stream = new MemoryStream(data))
      {
        Image scannedImage;

        scannedImage = Image.FromStream(stream);

        try
        {
          result = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);

          //result.SetResolution((float)image.HorizontalResolution, (float)image.VerticalResolution);

          using (Graphics g = Graphics.FromImage(result))
          {
            g.Clear(Color.Transparent);
            g.PageUnit = GraphicsUnit.Pixel;
            g.DrawImage(scannedImage, new Rectangle(0, 0, image.Width, image.Height));
          }
        }
        catch (COMException)
        {
          // most likely because the image is too large; so don't create a copy
          result = (Bitmap)scannedImage;
        }
      }

      return result;
    }

    public static string ToSeparatedString(this Vector vector)
    {
      StringBuilder sb;

      sb = new StringBuilder();

      for (int i = 0; i < vector.Count; i++)
      {
        if (i > 0)
        {
          sb.Append(',').Append(' ');
        }

        sb.Append(vector.get_Item(i + 1).ToString());
      }

      return sb.ToString();
    }

    #endregion Public Methods
  }
}