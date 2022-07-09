using System;

// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  internal static class WiaFormatId
  {
    #region Public Fields

    public static readonly Guid Bmp = new Guid("{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}");

    public static readonly Guid Gif = new Guid("{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}");

    public static readonly Guid Jpeg = new Guid("{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}");

    public static readonly Guid Png = new Guid("{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}");

    public static readonly Guid Tiff = new Guid("{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}");

    #endregion Public Fields
  }
}