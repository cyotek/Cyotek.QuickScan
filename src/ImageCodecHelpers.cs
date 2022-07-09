using System;
using System.Drawing.Imaging;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal static class ImageCodecHelpers
  {
    #region Public Methods

    public static EncoderParameters GetEncoderParameters(Guid formatId, int quality)
    {
      ImageCodecInfo codec;

      codec = ImageCodecHelpers.GetImageCodec(formatId);

      return ImageCodecHelpers.GetEncoderParameters(codec, quality);
    }

    public static EncoderParameters GetEncoderParameters(ImageCodecInfo codec, int quality)
    {
      Encoder qualityEncoder;
      EncoderParameters encoderParameters;

      if (codec.FormatID == ImageFormat.Jpeg.Guid)
      {
        qualityEncoder = Encoder.Quality;
        encoderParameters = new EncoderParameters(1);
        encoderParameters.Param[0] = new EncoderParameter(qualityEncoder, quality);
      }
      else
      {
        encoderParameters = null;
      }

      return encoderParameters;
    }

    public static ImageCodecInfo GetImageCodec(Guid formatId)
    {
      ImageCodecInfo[] codecs;
      ImageCodecInfo result;

      codecs = ImageCodecInfo.GetImageEncoders();
      result = null;

      for (int i = 0; i < codecs.Length; i++)
      {
        ImageCodecInfo codec;

        codec = codecs[i];

        if (codec.FormatID == formatId)
        {
          result = codec;
          break;
        }
      }

      return result;
    }

    public static string GetSuggestedExtension(ImageCodecInfo codecInfo)
    {
      return GetSuggestedExtension(codecInfo.FilenameExtension);
    }

    public static string GetSuggestedExtension(string extensions)
    {
      int position;
      string extension;

      position = extensions.IndexOf(';');

      extension = position == -1 ? extensions : extensions.Substring(0, position);

      if (extension.Length > 1 && extension[0] == '*')
      {
        extension = extension.Remove(0, 1);
      }

      return extension;
    }

    #endregion Public Methods
  }
}