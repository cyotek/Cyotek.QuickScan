using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cyotek.QuickScan
{
  internal class ImageInfo
  {
    private Bitmap _image;

    public Bitmap Image
    {
      get { return _image; }
      set { _image = value; }
    }

    private Guid _format;

    public Guid Format
    {
      get { return _format; }
      set { _format = value; }
    }

    private int _quality;

    public int Quality
    {
      get { return _quality; }
      set { _quality = value; }
    }


  }
}
