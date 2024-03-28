// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2024 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

// WIA_IPA_DATATYPE constants in WiaDef.h

namespace Cyotek.QuickScan
{
  internal enum WiaDataType
  {
    WIA_DATA_THRESHOLD = 0,

    WIA_DATA_DITHER = 1,

    WIA_DATA_GRAYSCALE = 2,

    WIA_DATA_COLOR = 3,

    WIA_DATA_COLOR_THRESHOLD = 4,

    WIA_DATA_COLOR_DITHER = 5,
  }
}