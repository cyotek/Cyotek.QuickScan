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
  internal partial class ImagePreviewWindow : BaseForm
  {
    #region Private Fields

    private readonly Image _image;

    #endregion Private Fields

    #region Public Constructors

    public ImagePreviewWindow()
    {
      this.InitializeComponent();
    }

    public ImagePreviewWindow(Image image)
      : this()
    {
      _image = image;
    }

    #endregion Public Constructors

    #region Public Methods

    public static void ShowImagePreview(Image image)
    {
      using (ImagePreviewWindow window = new ImagePreviewWindow(image))
      {
        window.ShowDialog();
      }
    }

    #endregion Public Methods

    #region Protected Methods

    protected override void OnShown(EventArgs e)
    {
      base.OnShown(e);

      previewImageBox.Image = _image;
      previewImageBox.ZoomToFit();
    }

    #endregion Protected Methods
  }
}