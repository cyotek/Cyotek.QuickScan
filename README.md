# Cyotek QuickScan

[![PayPal Donation][paypalimg]][paypal] [![By Me a Coffee Donation][bmacimg]][bmac]

![A screenshot of the application in use][screenshot]

For the past few years, and in a somewhat adhoc fashion, I've
been attempting to catalog the piles of DVDs, music CD's, books,
managed and all sorts of other clutter I have. In addition to
record pertinent details in a database, I've either been
scanning the front and back of media such as as books or DVDs
(where they fit in a scanner) and taking digital photographs of
everything else.

However, this sort of scanning is rather tedious and laborious.
If I used a program such as Paint.NET or Paint Shop Pro, I'd be
wasting a large amount of time manually acquiring an image and
then saving it . I created this tool to simplify the process -
if I am scanning multiple items of the same size, then I only
need to specify the size once, and then just answer a
confirmation prompt to perform a new scan. Images are
automatically saved to a single folder for me to file later.
This has turned out to be quite a massive time-saver - even if
each item I'm scanning is of a different size, I don't need to
keep request to acquire an image and then saving it when done.

## Quick Start

1. Select your device from the **Device** group. Only scanners
   will be listed in this field
2. Ensure **Prompt for device** is unchecked
3. Ensure **Continuous** is checked
4. Select an appropriate output format from the Output Settings group
5. (Optional) If using JPEG as the output, set the **Quality** as
   appropriate. Larger values equate to larger file sizes,
   although JPG files should still be smaller than PNG
6. Set the output **Folder**
7. (Optional) Set the base **File name**. If you leave this
   blank, the current date and time will be used
8. Ensure the **Use Counter** option is set. When this option is
   used, an incrementing number is automatically appended to
   output file names
9. Ensure the **Auto Save** option is set. When this option is
   used, after an image is scanned, it is automatically saved
   using the configured output settings - and the counter
   incremented
10. Click Preview to initiate the WIA scanning dialog. When you
    have defined the attributes of the image to scan, performed
    the preview and defined the scan size, click Scan to scan
   image
11. After the image has been scanned and saved, a confirmation
    prompt will be displayed
      * Choose **Yes** to repeat the scan using the same
        parameters - but make sure you change the media in the
        scanner first
      * Choose **No** to re-open the preview dialog - this is
        best used when you are doing continuous scanning, but
        the size of the item being scanned is different to the
        previous scan
      * Choose **Cancel** to cancel the scan loop

## Other Features

Although I ended up not using them at all after implementing
continuos scanning with automatic saving and automatic file name
generation, there are some basic controls to manipulate an image
(e.g. rotation) prior to saving, but as I've mentioned to me the
value of this tool was in the ability to chain scan so these
other options fell by the wayside.

## Known Issues

* This is only a slight step above and internal tool, so UX may
  not be great and error handling certainly isn't
* If you have multiple scanners, make sure you select the device
  you want to use first before clicking Preview. WIA will still
  prompt for a device when there is more than one, and if the
  device you select doesn't match the window selection,
  subsequent scans will fail

## Requirements

Pre-built binaries are available from the [releases][ghrel]
page. These are compiled using .NET 4.7.2 and therefore require
one of the following operating systems

* Windows Server 2016 (version 1709)
* Windows 10 Anniversary Update (version 1607) (x86 and x64)
* Windows 10 Creators Update (version 1703) (x86 and x64)
* Windows 10 Fall Creators Update (version 1709) (x86 and x64)
* Windows Server 2012 R2 (x64)
* Windows 8.1 (x86 and x64)
* Windows Server 2012 (x64)
* Windows Server 2008 R2 Service Pack 1 (x64)
* Windows 7 Service Pack 1 (x86 and x64)

If you wish to compile the [source][ghsrc] yourself, the source
code requires .NET Framework 2.0 or above, although it does make
use of some third-party dependencies.

## Contributing to this code

Contributions accepted!

* Found a problem? [Raise an issue][ghissue]
* Want to improve the code? [Make a pull request][ghpull]

Alternatively, if you make use of this software and it saves you
some time, donations are welcome.

[![PayPal Donation][paypalimg]][paypal]

[![By Me a Coffee Donation][bmacimg]][bmac]

## Acknowledgements

* Scanner icon derived from [Computer Hardware Cute Style vol 2][1]
* Some toolbar graphics use [Fugue Icons][2]

## License

This source is licensed under the MIT license. See `LICENSE.txt`
for the full text.

## References

* [Cyotek ImageBox][6] ([GitHub Project][7])
* [Creating a GroupBox containing an image and a custom display rectangle][3]
* [Copy an irregular area from a picture to the clipboard in C#][4]
* [Enabling shell styles for the ListView and TreeView controls in C#][5]
* [Paste a PNG format image with a transparent background from the clipboard in C#][9]
* [CanoScan LiDE 100 Windows 10 Compatibility][scanblog]
* [An introduction to using Windows Image Acquisition (WIA) via C#][wiablog]

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

[screenshot]: res/screenshot.png

[ghissue]: https://github.com/cyotek/Cyotek.QuickScan/issues
[ghpull]: https://github.com/cyotek/Cyotek.QuickScan/pulls
[ghrel]: https://github.com/cyotek/Cyotek.QuickScan/releases
[ghsrc]: https://github.com/cyotek/Cyotek.QuickScan

[paypal]: https://www.paypal.me/cyotek
[paypalimg]: https://static.cyotek.com/assets/images/donate.gif
[bmac]: https://www.buymeacoffee.com/cyotek
[bmacimg]: https://static.cyotek.com/assets/images/bmac.png
