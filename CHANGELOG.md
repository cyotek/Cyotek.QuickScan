# Cyotek Quick Scan Change Log

## 1.4 (28Mar2024)

### Added

* Added a new experimental per-machine "Maximise Scan Window"
  option that will attempt to maximise the WIA scan dialogue so
  that the preview area is the largest possible size, making it
  easier to make exact selections.
  
  > Note: On non-English versions of Windows this setting
  > probably won't work out of the box, you would also need to
  > change the `ScanDialogTitle` value in `ctkqscan.ini` to
  > match the dialogue title of your system.

### Fixed

* Scan origin was being reset to 0, 0 for subsequent scans,
  causing them to be invalid if the original scan was offset.
* The UI now updates the DPI field based on the original scan,
  preventing invalid or failed scans if the DPI was changed from
  within the WIA dialog.
* If multiple devices are present, pressing Scan no longer
  prompts for a device first, but correctly honours the UI
  setting.

## 1.3 (07Oct2023)

### Added

* Added new Show Progress option. If disabled, the transfer
  progress dialog when chain scanning is no longer displayed.

## 1.2.1 (08Oct2022)

### Fixed

* Metadata tokens are now evaluated on demand, preventing a
  fatal crash which sometimes occurred when saving an image and
  trying to read all WIA device properties.
* Timestamp saved into images is now the timestamp of the scan,
  not that of the time the image was saved.

## 1.2 (04Oct2022)

### Changed

* If you use the **Restart WIA Service** option found in the
  **Tools** menu, this now delegates to an external program for
  requesting elevation and then restarting the service, avoiding
  the core Quick Scan requiring elevation.

## 1.1 (02Oct2022)

### Breaking

* Now requires .NET Framework 4.8
* Due to the switch in the ini parser, it is almost certain that
  when upgrading from 1.0 to 1.1 the _Output Folder_ setting
  will be corrupted and will need to be re-defined.

### Added

* The confirmation prompt displayed at the conclusion of an
  scan-and-save sequence is now inline and means imagine editing
  tools can be used when chain scanning. If you prefer the
  original modal prompt, this can be re-enabled from the
  **Options** menu.
* Added `ctkqscan.default.ini`, a commented ini file with all
  settings documented (including ones which can't be currently
  set in the UI). If the main `ctkqscan.ini` is missing, the
  default file will be automatically copied and used.
* Added ability to save metadata (e.g. EXIF tag) on scanned
  images, if supported by the chosen output format. This feature
  is not currently configurable via the UI, see the comments in
  the default ini file for details.
* Added an option to play a sound at the prompt for continuing a
  chain scan. The choice of sound is configurable too, but is
  not currently configurable via the UI, see the comments in the
  default ini file for details.

### Changed

* Simplified control panel and menus
* Sometimes trying to save an image fails, typically when doing
  large high DPI scans. When this happens, the raw WIA image is
  saved instead with no conversion or customisation. This is
  typically a _very_ large bitmap.
* Ini file parsing is now handled by [Cyotek.Data.Ini][ini].
* The window and splitter position is now preserved between
  sessions.
* Output parameters are now validated before scanning starts, if
  auto save is enabled.
* File preview are now generated from the scanned image saved to
  a temporary file, not an in-memory copy.

### Fixed

* The UI now works properly with high DPI displays
* Fixed a crash trying to do an immediate scan (not preview) and
  no previous scan settings were saved.
* Fixed a crash that could occur when trying to calculate file
  sizes.
* The `WIA_ERROR_OFFLINE` error is gracefully handled.
* The _Save Settings on Exit_ setting is now preserved between
  sessions.
* Added error traps for various conditions, usually caused by
  scanning large high DPI images, or working with temperamental
  scanners.

## 1.0 (10Oct2021)

* Initial Release.

[ini]: https://github.com/cyotek/Cyotek.Data.Ini
