
// Cyotek Quick Scan
// https://github.com/cyotek/Cyotek.QuickScan

// Copyright © 2022 Cyotek Ltd.

// This work is licensed under the MIT License.
// See LICENSE.TXT for the full text

// Found this code useful?
// https://www.cyotek.com/contribute

namespace Cyotek.QuickScan
{
  // taken from
  // Windows Kits\10\Include\10.0.17763.0\um\gdiplusimaging.h

  internal enum PropertyTag
  {
    PropertyTagExifIFD           = 0x8769,
    PropertyTagGpsIFD            = 0x8825,

    PropertyTagNewSubfileType    = 0x00FE,
    PropertyTagSubfileType       = 0x00FF,
    PropertyTagImageWidth        = 0x0100,
    PropertyTagImageHeight       = 0x0101,
    PropertyTagBitsPerSample     = 0x0102,
    PropertyTagCompression       = 0x0103,
    PropertyTagPhotometricInterp = 0x0106,
    PropertyTagThreshHolding     = 0x0107,
    PropertyTagCellWidth         = 0x0108,
    PropertyTagCellHeight        = 0x0109,
    PropertyTagFillOrder         = 0x010A,
    PropertyTagDocumentName      = 0x010D,
    PropertyTagImageDescription  = 0x010E,
    PropertyTagEquipMake         = 0x010F,
    PropertyTagEquipModel        = 0x0110,
    PropertyTagStripOffsets      = 0x0111,
    PropertyTagOrientation       = 0x0112,
    PropertyTagSamplesPerPixel   = 0x0115,
    PropertyTagRowsPerStrip      = 0x0116,
    PropertyTagStripBytesCount   = 0x0117,
    PropertyTagMinSampleValue    = 0x0118,
    PropertyTagMaxSampleValue    = 0x0119,
    PropertyTagXResolution       = 0x011A,   // Image resolution in width direction
    PropertyTagYResolution       = 0x011B,   // Image resolution in height direction
    PropertyTagPlanarConfig      = 0x011C,   // Image data arrangement
    PropertyTagPageName          = 0x011D,
    PropertyTagXPosition         = 0x011E,
    PropertyTagYPosition         = 0x011F,
    PropertyTagFreeOffset        = 0x0120,
    PropertyTagFreeByteCounts    = 0x0121,
    PropertyTagGrayResponseUnit  = 0x0122,
    PropertyTagGrayResponseCurve = 0x0123,
    PropertyTagT4Option          = 0x0124,
    PropertyTagT6Option          = 0x0125,
    PropertyTagResolutionUnit    = 0x0128,   // Unit of X and Y resolution
    PropertyTagPageNumber        = 0x0129,
    PropertyTagTransferFuncition = 0x012D,
    PropertyTagSoftwareUsed      = 0x0131,
    PropertyTagDateTime          = 0x0132,
    PropertyTagArtist            = 0x013B,
    PropertyTagHostComputer      = 0x013C,
    PropertyTagPredictor         = 0x013D,
    PropertyTagWhitePoint        = 0x013E,
    PropertyTagPrimaryChromaticities =0x013F,
    PropertyTagColorMap          = 0x0140,
    PropertyTagHalftoneHints     = 0x0141,
    PropertyTagTileWidth         = 0x0142,
    PropertyTagTileLength        = 0x0143,
    PropertyTagTileOffset        = 0x0144,
    PropertyTagTileByteCounts    = 0x0145,
    PropertyTagInkSet            = 0x014C,
    PropertyTagInkNames          = 0x014D,
    PropertyTagNumberOfInks      = 0x014E,
    PropertyTagDotRange          = 0x0150,
    PropertyTagTargetPrinter     = 0x0151,
    PropertyTagExtraSamples      = 0x0152,
    PropertyTagSampleFormat      = 0x0153,
    PropertyTagSMinSampleValue   = 0x0154,
    PropertyTagSMaxSampleValue   = 0x0155,
    PropertyTagTransferRange     = 0x0156,

    PropertyTagJPEGProc          = 0x0200,
    PropertyTagJPEGInterFormat   = 0x0201,
    PropertyTagJPEGInterLength   = 0x0202,
    PropertyTagJPEGRestartInterval =0x0203,
    PropertyTagJPEGLosslessPredictors= 0x0205,
    PropertyTagJPEGPointTransforms   = 0x0206,
    PropertyTagJPEGQTables       = 0x0207,
    PropertyTagJPEGDCTables      = 0x0208,
    PropertyTagJPEGACTables      = 0x0209,

    PropertyTagYCbCrCoefficients = 0x0211,
    PropertyTagYCbCrSubsampling  = 0x0212,
    PropertyTagYCbCrPositioning  = 0x0213,
    PropertyTagREFBlackWhite     = 0x0214,

    PropertyTagICCProfile        = 0x8773,   // This TAG is defined by ICC
                                             // for embedded ICC in TIFF
    PropertyTagGamma             = 0x0301,
    PropertyTagICCProfileDescriptor =0x0302,
    PropertyTagSRGBRenderingIntent =0x0303,
                                             
    PropertyTagImageTitle        = 0x0320,
    PropertyTagCopyright         = 0x8298,

    // Extra TAGs (Like Adobe Image Information tags etc.)

    PropertyTagResolutionXUnit         = 0x5001,
    PropertyTagResolutionYUnit         = 0x5002,
    PropertyTagResolutionXLengthUnit   = 0x5003,
    PropertyTagResolutionYLengthUnit   = 0x5004,
    PropertyTagPrintFlags              = 0x5005,
    PropertyTagPrintFlagsVersion       = 0x5006,
    PropertyTagPrintFlagsCrop          = 0x5007,
    PropertyTagPrintFlagsBleedWidth    = 0x5008,
    PropertyTagPrintFlagsBleedWidthScale =0x5009,
    PropertyTagHalftoneLPI             = 0x500A,
    PropertyTagHalftoneLPIUnit         = 0x500B,
    PropertyTagHalftoneDegree          = 0x500C,
    PropertyTagHalftoneShape           = 0x500D,
    PropertyTagHalftoneMisc            = 0x500E,
    PropertyTagHalftoneScreen          = 0x500F,
    PropertyTagJPEGQuality             = 0x5010,
    PropertyTagGridSize                = 0x5011,
    PropertyTagThumbnailFormat         = 0x5012,  // 1 = JPEG, 0 = RAW RGB
    PropertyTagThumbnailWidth          = 0x5013,
    PropertyTagThumbnailHeight         = 0x5014,
    PropertyTagThumbnailColorDepth     = 0x5015,
    PropertyTagThumbnailPlanes         = 0x5016,
    PropertyTagThumbnailRawBytes       = 0x5017,
    PropertyTagThumbnailSize           = 0x5018,
    PropertyTagThumbnailCompressedSize = 0x5019,
    PropertyTagColorTransferFunction   = 0x501A,
    PropertyTagThumbnailData           = 0x501B,// RAW thumbnail bits in
                                                // JPEG format or RGB format
                                                // depends on
                                                // PropertyTagThumbnailFormat

    // Thumbnail related TAGs

    PropertyTagThumbnailImageWidth     = 0x5020,  // Thumbnail width
    PropertyTagThumbnailImageHeight    = 0x5021,  // Thumbnail height
    PropertyTagThumbnailBitsPerSample  = 0x5022,  // Number of bits per
                                                  // component
    PropertyTagThumbnailCompression    = 0x5023,  // Compression Scheme
    PropertyTagThumbnailPhotometricInterp =0x5024, // Pixel composition
    PropertyTagThumbnailImageDescription= 0x5025,  // Image Tile
    PropertyTagThumbnailEquipMake      = 0x5026,  // Manufacturer of Image
                                                  // Input equipment
    PropertyTagThumbnailEquipModel     = 0x5027,  // Model of Image input
                                                  // equipment
    PropertyTagThumbnailStripOffsets   = 0x5028,  // Image data location
    PropertyTagThumbnailOrientation    = 0x5029,  // Orientation of image
    PropertyTagThumbnailSamplesPerPixel= 0x502A,  // Number of components
    PropertyTagThumbnailRowsPerStrip   = 0x502B,  // Number of rows per strip
    PropertyTagThumbnailStripBytesCount= 0x502C,  // Bytes per compressed
                                                  // strip
    PropertyTagThumbnailResolutionX    = 0x502D,  // Resolution in width
                                                  // direction
    PropertyTagThumbnailResolutionY    = 0x502E,  // Resolution in height
                                                  // direction
    PropertyTagThumbnailPlanarConfig   = 0x502F,  // Image data arrangement
    PropertyTagThumbnailResolutionUnit = 0x5030,  // Unit of X and Y
                                                  // Resolution
    PropertyTagThumbnailTransferFunction= 0x5031,  // Transfer function
    PropertyTagThumbnailSoftwareUsed   = 0x5032,  // Software used
    PropertyTagThumbnailDateTime       = 0x5033,  // File change date and
                                                  // time
    PropertyTagThumbnailArtist         = 0x5034,  // Person who created the
                                                  // image
    PropertyTagThumbnailWhitePoint     = 0x5035,  // White point chromaticity
    PropertyTagThumbnailPrimaryChromaticities= 0x5036,
    // Chromaticities of
    // primaries
    PropertyTagThumbnailYCbCrCoefficients= 0x5037, // Color space transforma-
                                                  // tion coefficients
    PropertyTagThumbnailYCbCrSubsampling= 0x5038,  // Subsampling ratio of Y
                                                  // to C
    PropertyTagThumbnailYCbCrPositioning =0x5039,  // Y and C position
    PropertyTagThumbnailRefBlackWhite  = 0x503A,  // Pair of black and white
                                                  // reference values
    PropertyTagThumbnailCopyRight      = 0x503B,  // CopyRight holder

    PropertyTagLuminanceTable          = 0x5090,
    PropertyTagChrominanceTable        = 0x5091,

    PropertyTagFrameDelay              = 0x5100,
    PropertyTagLoopCount               = 0x5101,

        PropertyTagGlobalPalette           = 0x5102,
        PropertyTagIndexBackground         = 0x5103,
        PropertyTagIndexTransparent        = 0x5104,

    PropertyTagPixelUnit       = 0x5110,  // Unit specifier for pixel/unit
    PropertyTagPixelPerUnitX   = 0x5111,  // Pixels per unit in X
    PropertyTagPixelPerUnitY   = 0x5112,  // Pixels per unit in Y
    PropertyTagPaletteHistogram= 0x5113,  // Palette histogram

    // EXIF specific tag

    PropertyTagExifExposureTime= 0x829A,
    PropertyTagExifFNumber     = 0x829D,

    PropertyTagExifExposureProg= 0x8822,
    PropertyTagExifSpectralSense =0x8824,
    PropertyTagExifISOSpeed    = 0x8827,
    PropertyTagExifOECF        = 0x8828,

    PropertyTagExifVer          = 0x9000,
    PropertyTagExifDTOrig       = 0x9003, // Date & time of original
    PropertyTagExifDTDigitized  = 0x9004, // Date & time of digital data generation

    PropertyTagExifCompConfig   = 0x9101,
    PropertyTagExifCompBPP      = 0x9102,

    PropertyTagExifShutterSpeed = 0x9201,
    PropertyTagExifAperture     = 0x9202,
    PropertyTagExifBrightness   = 0x9203,
    PropertyTagExifExposureBias = 0x9204,
    PropertyTagExifMaxAperture  = 0x9205,
    PropertyTagExifSubjectDist  = 0x9206,
    PropertyTagExifMeteringMode = 0x9207,
    PropertyTagExifLightSource  = 0x9208,
    PropertyTagExifFlash        = 0x9209,
    PropertyTagExifFocalLength  = 0x920A,
    PropertyTagExifSubjectArea  = 0x9214,  // exif 2.2 Subject Area
    PropertyTagExifMakerNote    = 0x927C,
    PropertyTagExifUserComment  = 0x9286,
    PropertyTagExifDTSubsec     = 0x9290,  // Date & Time subseconds
    PropertyTagExifDTOrigSS     = 0x9291,  // Date & Time original subseconds
    PropertyTagExifDTDigSS      = 0x9292,  // Date & TIme digitized subseconds

    PropertyTagExifFPXVer       = 0xA000,
    PropertyTagExifColorSpace   = 0xA001,
    PropertyTagExifPixXDim      = 0xA002,
    PropertyTagExifPixYDim      = 0xA003,
    PropertyTagExifRelatedWav   = 0xA004,  // related sound file
    PropertyTagExifInterop      = 0xA005,
    PropertyTagExifFlashEnergy  = 0xA20B,
    PropertyTagExifSpatialFR    = 0xA20C,  // Spatial Frequency Response
    PropertyTagExifFocalXRes    = 0xA20E,  // Focal Plane X Resolution
    PropertyTagExifFocalYRes    = 0xA20F,  // Focal Plane Y Resolution
    PropertyTagExifFocalResUnit = 0xA210,  // Focal Plane Resolution Unit
    PropertyTagExifSubjectLoc   = 0xA214,
    PropertyTagExifExposureIndex= 0xA215,
    PropertyTagExifSensingMethod= 0xA217,
    PropertyTagExifFileSource   = 0xA300,
    PropertyTagExifSceneType    = 0xA301,
    PropertyTagExifCfaPattern   = 0xA302,

    // New EXIF 2.2 properties

    PropertyTagExifCustomRendered         = 0xA401,
    PropertyTagExifExposureMode           = 0xA402,
    PropertyTagExifWhiteBalance           = 0xA403,
    PropertyTagExifDigitalZoomRatio       = 0xA404,
    PropertyTagExifFocalLengthIn35mmFilm  = 0xA405,
    PropertyTagExifSceneCaptureType       = 0xA406,
    PropertyTagExifGainControl            = 0xA407,
    PropertyTagExifContrast               = 0xA408,
    PropertyTagExifSaturation             = 0xA409,
    PropertyTagExifSharpness              = 0xA40A,
    PropertyTagExifDeviceSettingDesc      = 0xA40B,
    PropertyTagExifSubjectDistanceRange   = 0xA40C,
    PropertyTagExifUniqueImageID          = 0xA420,


    PropertyTagGpsVer           = 0x0000,
    PropertyTagGpsLatitudeRef   = 0x0001,
    PropertyTagGpsLatitude      = 0x0002,
    PropertyTagGpsLongitudeRef  = 0x0003,
    PropertyTagGpsLongitude     = 0x0004,
    PropertyTagGpsAltitudeRef   = 0x0005,
    PropertyTagGpsAltitude      = 0x0006,
    PropertyTagGpsGpsTime       = 0x0007,
    PropertyTagGpsGpsSatellites = 0x0008,
    PropertyTagGpsGpsStatus     = 0x0009,
    PropertyTagGpsGpsMeasureMode= 0x00A,
    PropertyTagGpsGpsDop        = 0x000B,  // Measurement precision
    PropertyTagGpsSpeedRef      = 0x000C,
    PropertyTagGpsSpeed         = 0x000D,
    PropertyTagGpsTrackRef      = 0x000E,
    PropertyTagGpsTrack         = 0x000F,
    PropertyTagGpsImgDirRef     = 0x0010,
    PropertyTagGpsImgDir        = 0x0011,
    PropertyTagGpsMapDatum      = 0x0012,
    PropertyTagGpsDestLatRef    = 0x0013,
    PropertyTagGpsDestLat       = 0x0014,
    PropertyTagGpsDestLongRef   = 0x0015,
    PropertyTagGpsDestLong      = 0x0016,
    PropertyTagGpsDestBearRef   = 0x0017,
    PropertyTagGpsDestBear      = 0x0018,
    PropertyTagGpsDestDistRef   = 0x0019,
    PropertyTagGpsDestDist      = 0x001A,
    PropertyTagGpsProcessingMethod =0x001B,
    PropertyTagGpsAreaInformation =0x001C,
    PropertyTagGpsDate          = 0x001D,
    PropertyTagGpsDifferential  = 0x001E,
  }
}
