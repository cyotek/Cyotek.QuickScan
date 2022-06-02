using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text;

namespace Cyotek.QuickScan
{
  internal static class ImageExtensions
  {
    #region Public Methods

    public static Bitmap Copy(this Image image)
    {
      return image.Copy(Color.Transparent);
    }

    public static Bitmap Copy(this Image image, Color transparentColor)
    {
      Bitmap copy;

      copy = new Bitmap(image.Size.Width, image.Size.Height, PixelFormat.Format32bppArgb);

      using (Graphics g = Graphics.FromImage(copy))
      {
        g.Clear(transparentColor);
        g.PageUnit = GraphicsUnit.Pixel;
        g.DrawImage(image, new Rectangle(Point.Empty, image.Size));
      }

      return copy;
    }

    public static void SetPropertyItem(this Image image, PropertyTag tag, DateTime value)
    {
      image.SetPropertyItemImpl(tag, Encoding.ASCII.GetBytes(value.ToString("s") + "\0"), PropertyTagType.PropertyTagTypeASCII);
    }

    public static void SetPropertyItem(this Image image, PropertyTag tag, string value)
    {
      image.SetPropertyItemImpl(tag, Encoding.ASCII.GetBytes(value + "\0"), PropertyTagType.PropertyTagTypeASCII);
    }

    public static void SetPropertyItem(this Image image, PropertyTag tag, PropertyTagType type, string value)
    {
      byte[] valueBytes;

      valueBytes = type == PropertyTagType.PropertyTagTypeASCII
        ? Encoding.ASCII.GetBytes(value + "\0")
        : Encoding.ASCII.GetBytes(value);

      image.SetPropertyItemImpl(tag, valueBytes, type);
    }

    #endregion Public Methods

    #region Private Methods

    private static void SetPropertyItemImpl(this Image image, PropertyTag tag, byte[] value, PropertyTagType type)
    {
      try
      {
        PropertyItem item;

        item = (PropertyItem)typeof(PropertyItem).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], new ParameterModifier[0]).Invoke(null);
        item.Id = (int)tag;
        item.Type = (short)type;
        item.Len = value.Length;
        item.Value = value;

        image.SetPropertyItem(item);
      }
      catch
      {
        // ignore
      }
    }

    #endregion Private Methods
  }
}