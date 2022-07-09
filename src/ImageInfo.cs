using System;
using System.Drawing;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal class ImageInfo
  {
    #region Private Fields

    private Guid _format;

    private Bitmap _image;

    private int _quality;

    #endregion Private Fields

    #region Public Properties

    public Guid Format
    {
      get => _format;
      set => _format = value;
    }

    public Bitmap Image
    {
      get => _image;
      set => _image = value;
    }

    public int Quality
    {
      get => _quality;
      set => _quality = value;
    }

    #endregion Public Properties
  }
}