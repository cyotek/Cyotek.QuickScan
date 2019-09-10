using System;
using System.Drawing;

namespace Cyotek.QuickScan
{
  internal partial class ImagePreviewWindow : BaseForm
  {
    #region Private Fields

    private readonly Bitmap _image;

    #endregion Private Fields

    #region Public Constructors

    public ImagePreviewWindow()
    {
      this.InitializeComponent();
    }

    public ImagePreviewWindow(Bitmap image)
      : this()
    {
      _image = image;
    }

    #endregion Public Constructors

    #region Public Methods

    public static void ShowImagePreview(Bitmap image)
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