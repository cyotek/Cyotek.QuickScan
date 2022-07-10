# Cyotek Quick Scan Change Log

## 1.1 (WIP)

### Breaking Change

* Due to the switch in the ini parser, it is almost certain that
  when upgrading from 1.0 to 1.1 the _Output Folder_ setting
  will be corrupted and will need to be re-defined

### Added

* Added `ctkqscan.default.ini`, a commented ini file with all
  settings documented (including ones which can't be currently
  set in the UI). If the main `ctkqscan.ini` is missing, the
  default file will be automatically copied and used
* Added ability to save metadata (e.g. EXIF tag) on scanned
  images, if supported by the chosen output format. This feature
  is not currently configurable via the UI, see the comments in
  the default ini file for details
* Added an option to play a sound at the prompt for continuing a
  chain scan. The choice of sound is configurable too, but is
  not currently configurable via the UI, see the comments in the
  default ini file for details

### Changed

* Sometimes trying to save an image fails, typically when doing
  large high DPI scans. When this happens, the raw WIA image is
  saved instead with no conversion or customisation. This is
  typically a _very_ large bitmap
* Ini file parsing is now handled by [Cyotek.Data.Ini][ini]
* The window and splitter position is now preserved between
  sessions
* Output parameters are now validated before scanning starts, if
  auto save is enabled
* File preview are now generated from the scanned image saved to
  a temporary file, not an in-memory copy

### Fixed

* The UI now works properly with high DPI displays
* Fixed a crash trying to do an immediate scan (not preview) and
  no previous scan settings were saved
* Fixed a crash that occur occur when trying to calculate file
  sizes
* The `WIA_ERROR_OFFLINE` error is gracefully handled
* The _Save Settings on Exit_ setting is now preserved between
  sessions
* Added error traps for various conditions, usually caused by
  scanning large high DPI images, or working with temperamental
  scanners

## 1.0 (10Oct2021)

* Initial Release

[ini]: https://github.com/cyotek/Cyotek.Data.Ini
