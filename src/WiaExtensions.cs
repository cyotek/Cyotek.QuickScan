using System.Drawing;
using System.IO;
using System.Text;
using WIA;

namespace Cyotek.QuickScan
{
  internal static class WiaExtensions
  {
    #region Public Methods

    public static Property GetProperty(this WIA.Properties properties, WiaPropertyId id)
    {
      return properties[((int)id).ToString()];
    }

    public static void SetPropertyMaximum(this WIA.Properties properties, WiaPropertyId id)
    {
      Property property;

      property = properties[((int)id).ToString()];

      property.let_Value(property.SubTypeMax);
    }

    public static Bitmap ToBitmap(this ImageFile image)
    {
      byte[] data;

      data = (byte[])image.FileData.get_BinaryData();

      using (MemoryStream stream = new MemoryStream(data))
      {
        return (Bitmap)Image.FromStream(stream);
      }
    }

    public static void SetPropertyMinimum(this WIA.Properties properties, WiaPropertyId id)
    {
      Property property;

      property = properties[((int)id).ToString()];

      property.let_Value(property.SubTypeMin);
    }

    public static void SetPropertyValue<T>(this WIA.Properties properties, WiaPropertyId id, T value)
    {
      Property property;

      property = properties[((int)id).ToString()];

      property.let_Value(value);
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