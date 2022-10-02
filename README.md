# Cyotek QuickScan

> A simple application that allows for scanning resources from
> any WIA compatible scanner

![A screenshot of the application in use on Windows 11, 200%
display scaling][screenshot2]

The Quick Scan utility does exactly as described - it provides a
(more) convenient work flow for scanning images from flatbed
scanners and saving them to disk with as few user interactions
as possible, with automatic saving and file name generation. It
especially shines at chain-scanning multiple images using the
same parameters.

## A note from updating from 1.0 to 1.1

The original version of this application displayed a modal
confirmation prompt each time it had finished scanning. This
used to frustrate me endlessly, as I had a habit of having this
application on one monitor and my Quick Catalogue application on
another. During scanning I was also adding entries to the
catalogue, and the modal prompt would often rip focus away and
if I was typing out a description and didn't stop fast enough,
invariably I'd press `y` or `n` and trigger a button. Version
1.1 introduces a new "inline prompt" option which both resolves
this and has additional benefit that the image processing tools
can be used when chain scanning. The remainder of this document
assumes that inline prompting is enabled as this is the new
default. If you want to revert to the old modal message, ensure
**Inline Prompt** from the **Options** menu is unchecked.

## Quick Start

1. Select your device from the **Device** group. Only scanners
   will be listed in this field
2. Check **Continuous** if you want to scan multiple items -
   after each scan the application will prompt how to continue
3. Select an appropriate output format from the Output Settings
   group. As JPEG is lossy and GIF limited, neither of these
   formats are recommended - I stick with TIFF, although this
   has significant overhead with file size.
4. (Optional) If using JPEG as the output, set the **Quality**
   as appropriate. Larger values equate to larger file sizes,
   although JPG files should still be smaller than PNG
5. Set the output **Folder**
6. (Optional) Set the base **File name** (without an extension).
   If you leave this blank, the current date and time will be
   used
7. Ensure the **Use Counter** option is set. When this option is
   used, an incrementing number is automatically appended to
   output file names
8. Ensure the **Auto Save** option is set. When this option is
   used, after an image is scanned, it is automatically saved
   using the configured output settings, and the counter
   incremented. If this option isn't set, you can still manually
   save the image - but this tool is all about reducing the
   clicks
9. Click **Scan**, or press _F5_ to initiate the WIA scanning
   dialogue. When you have defined the attributes of the image
   to scan, performed the preview and defined the scan size,
   click **Scan** to scan the image and close the WIA dialogue.
   I always recommend choosing **Custom Settings** and setting
   the DPI to the maximum as the default profiles use quite a
   low value. Note that depending on the image size and DPI,
   values greater than 600 may result in corrupt images.
10. After the image has been scanned and saved, the continuation
   bar will prompt how to progress
      * Choose **Yes** to repeat the scan using the same
        parameters (see known issues below). This is the best
        option when scanning multiple items of the same size and
        resolution
      * Choose **Yes, Customise** to re-open the preview
        dialogue. This is best used when you are doing
        continuous scanning, but the size of the item being
        scanned is different to the previous scan
      * Choose **No** to cancel the scan loop
11. (Optional) Before choosing an option from the continuation
    bar, you can use the imaging tools to manipulate the image
    in basic ways. After performing flip or rotation actions,
    click **Save** to save the image - this will act the same
    way as the automatic save, so a new file name will be
    generated and the counter incremented (if these features are
    enabled)

## Other Features

* Copy - copies the current image to the Windows Clipboard
* Paste - pastes an image from the Windows Clipboard which can
  then be manipulated and saved as if it were any other scan

## Known Issues

* This is only a slight step above an internal tool, so UX may
  not be great and error handling certainly isn't. Please report
  any problems so I can continue to improve this tool.
* If you have multiple scanners, make sure you select the device
  you want to use first before clicking **Scan**. WIA will
  always prompt for a device when there is more than one (even
  if instructed not to!), and if the device you select doesn't
  match the window selection, subsequent scans will fail. Most
  of my scanners are USB based and only appear when plugged in,
  so that part is easy. The scanner on my Brother laser printer
  always appears regardless of if the printer is switched on or
  off. As I rarely use this scanner I disable it (but _not_ the
  printer) in Device Manager to prevent it being seen by WIA
* If (as you probably should) you always choose **Custom
  Settings** from the **Scan** dialogue and you customise those
  settings so the chosen DPI no longer matches the DPI in the
  Quick Scan window, subsequent scans will be wrong. At the
  moment I haven't worked out how to retrieve the parameters
  used for the scan in order to properly update
* Related to the above, depending on the scanner, repeating a
  scan using the same settings may fail. For the CanoScan LiDE
  110 and 220, I haven't had any issues. With the Plustek
  OpticSlim 1180 I _always_ have to display the WIA Scan
  dialogue and reselect all parameters
* Rarely, the program will crash when trying to read WIA
  properties in order to generate EXIF metadata. When this
  happens, I usually just start again from where I left off
  (remembering to manually reset the counter!) as lightening
  generally doesn't hit twice. I have ideas on resolving this in
  a future PR.
* Sometimes, an image is so large the program crashes trying to
  save the modified version. When this happens, Quick Scan will
  instead save the original image from the scanner. This keeps
  the scan, but means it is saved in the scanners native format
  (always BMP in my experience), and with no additional features
  such as EXIF generation or rotation
* Some scanners seem very badly behaved (again, rarely have an
  issue with the CanoScan, but the OpticSlim does this _every
  time_) and sometimes WIA can't connect to the devices when
  they are switched on (if they require separate power), or when
  the OS/scanner resumes from sleep. In this scenario restarting
  the _Windows Image Acquisition (WIA)_ service seems to be the
  solution. Quick Scan _can_ do this, but the current
  implementation isn't appropriate if you scan to a mapped drive
  as the process has to elevate itself - and the mapped drive
  most likely won't be available
* If you are scanning large images (above A4 size) at a high DPI
  (greater than 600) you may end up with "corrupt" images where,
  the bottom of the image is either missing, or is composed of
  bands from other parts of the image. Not sure if this is a
  problem with WIA or the scanner driver. As with most of my
  scanner issues, I get this with the OpticSlim 1180. To resolve
  I gradually reduce the DPI until I get a "clean" image

## Requirements

Pre-built binaries are available from the [releases][ghrel]
page. These are compiled using .NET 4.8 and therefore require
one of the following operating systems

* Windows 11
* Windows 10 Anniversary Update (version 1607) (x86 and x64) (or
  above)
* Windows 8.1 (x86 and x64)
* Windows 7 Service Pack 1 (x86 and x64)
* Windows Server 2016 (version 1709) (or above, e.g. 2019, 2022)
* Windows Server 2012 R2 (x64)
* Windows Server 2012 (x64)
* Windows Server 2008 R2 Service Pack 1 (x64)

If you wish to compile the [source][ghsrc] yourself, the source
code requires .NET Framework 2.0 or above, although it does make
use of some third-party dependencies.

And of course - a WIA compatible scanner. This application
doesn't (yet?) support using the TWAIN interface.

## Tested Scanners and Operating System Combinations

* Brother MFC-L2710DW, Windows 10
* Brother MFC-L2750DW, Windows 10
* CanoScan LiDE 100, Windows 10 ([yes, this works perfectly well][scanblog])
* CanoScan LiDE 100, Windows 11
* CanoScan LiDE 220, Windows 10
* CanoScan LiDE 220, Windows 11
* Plustek OpticSlim 1180, Windows 11 (I don't recommend this
  scanner unless you need A3 scanning on a budget)

## Background, or why I created this tool

![A older screenshot of the application in use on Windows 10][screenshot]

For the past few years, and in a somewhat adhoc fashion, I've
been attempting to catalog the piles of DVDs, music CD's, books,
magazines and all sorts of other clutter I have. In addition to
recording pertinent details in a database, I've either been
scanning the front and back of media such as as books or DVDs
(where they fit in a scanner) and taking digital photographs of
everything else.

However, this sort of scanning is rather tedious and laborious.
If I used a program such as Paint.NET or Paint Shop Pro, I'd be
wasting a large amount of time manually acquiring an image and
then saving it, with lots of time spent navigating menus and
dialogues. I created this tool to simplify the process - if I am
scanning multiple items of the same size, then I only need to
specify the size once, and then just answer a confirmation
prompt to perform a new scan. Images are automatically saved to
a single folder for me to file later. This has turned out to be
quite a massive time-saver - even if each item I'm scanning is
of a different size, I don't need to keep performing the entire
flow to acquire a new image and then save it.

I have made thousands of scans using this tool, tweaking it over
time to add new features such as EXIF meta data and even just
the simple act of playing a sound after automatic saves. It has
worked well for me and I hope it is useful for others.

## Contributing to this code

Contributions accepted!

* Found a problem? [Raise an issue][ghissue]
* Want to improve the code? [Make a pull request][ghpull]

Alternatively, if you make use of this software and it saves you
some time, donations are welcome.

[![PayPal Donation][paypalimg]][paypal]

[![By Me a Coffee Donation][bmacimg]][bmac]

## Acknowledgements

* Scanner icon derived from [Computer Hardware Cute Style vol
  2][1]
* Some toolbar graphics use [Fugue Icons][2]
* The chime sound licensed as CC0 is by [Brandon Morris][chime]

## License

This source is licensed under the MIT license. See `LICENSE.txt`
for the full text.

## References

* [Cyotek ImageBox][6] ([GitHub Project][7])
* [Creating a GroupBox containing an image and a custom display
  rectangle][3]
* [Copy an irregular area from a picture to the clipboard in
  C#][4]
* [Enabling shell styles for the ListView and TreeView controls
  in C#][5]
* [Paste a PNG format image with a transparent background from
  the clipboard in C#][9]
* [CanoScan LiDE 100 Windows 10 Compatibility][scanblog]
* [An introduction to using Windows Image Acquisition (WIA) via
  C#][wiablog]

[1]: https://www.iconfinder.com/icons/2317747/machine_media_multimedia_office_scan_scanner_scanning_icon
[2]: https://p.yusukekamiyamane.com/icons/search/fugue/
[3]: https://www.cyotek.com/blog/creating-a-groupbox-containing-an-image-and-a-custom-display-rectangle
[4]: http://csharphelper.com/blog/2014/09/copy-an-irregular-area-from-one-picture-to-another-in-c/
[5]: https://www.cyotek.com/blog/enabling-shell-styles-for-the-listview-and-treeview-controls-in-csharp
[6]: https://www.cyotek.com/blog/tag/imagebox
[7]: https://github.com/cyotek/Cyotek.Windows.Forms.ImageBox
[9]: http://csharphelper.com/blog/2014/09/paste-a-png-format-image-with-a-transparent-background-from-the-clipboard-in-c/
[scanblog]: https://www.cyotek.com/blog/canoscan-lide-100-windows-10-compatibility
[wiablog]: https://www.cyotek.com/blog/an-introduction-to-using-windows-image-acquisition-wia-via-csharp
[chime]: https://opengameart.org/content/completion-sound

[screenshot]: res/screenshot.png
[screenshot2]: res/screenshot2.png

[ghissue]: https://github.com/cyotek/Cyotek.QuickScan/issues
[ghpull]: https://github.com/cyotek/Cyotek.QuickScan/pulls
[ghrel]: https://github.com/cyotek/Cyotek.QuickScan/releases
[ghsrc]: https://github.com/cyotek/Cyotek.QuickScan

[paypal]: https://www.paypal.me/cyotek
[paypalimg]: https://static.cyotek.com/assets/images/donate.gif
[bmac]: https://www.buymeacoffee.com/cyotek
[bmacimg]: https://static.cyotek.com/assets/images/bmac.png
