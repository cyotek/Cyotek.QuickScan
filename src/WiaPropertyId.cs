﻿// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2019-2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

// Values taken from WiaDef.h

namespace Cyotek.QuickScan
{
  internal enum WiaPropertyId
  {
    WIA_IPA_ITEM_NAME = 4098,

    WIA_IPA_FULL_ITEM_NAME = 4099,

    WIA_IPA_ITEM_FLAGS = 4101,

    WIA_IPA_ACCESS_RIGHTS = 4102,

    WIA_IPA_DATATYPE = 4103,

    WIA_IPA_DEPTH = 4104,

    WIA_IPA_PREFERRED_FORMAT = 4105,

    WIA_IPA_FORMAT = 4106,

    WIA_IPA_CHANNELS_PER_PIXEL = 4109,

    WIA_IPA_BITS_PER_CHANNEL = 4110,

    WIA_IPA_PLANAR = 4111,

    WIA_IPA_PIXELS_PER_LINE = 4112,

    WIA_IPA_BYTES_PER_LINE = 4113,

    WIA_IPA_NUMBER_OF_LINES = 4114,

    WIA_IPA_ITEM_SIZE = 4116,

    WIA_IPA_MIN_BUFFER_SIZE = 4118,

    WIA_IPA_FILENAME_EXTENSION = 4123,

    WIA_IPS_CUR_INTENT = 6146,

    WIA_IPS_XRES = 6147,

    WIA_IPS_YRES = 6148,

    WIA_IPS_XPOS = 6149,

    WIA_IPS_YPOS = 6150,

    WIA_IPS_XEXTENT = 6151,

    WIA_IPS_YEXTENT = 6152
  }
}