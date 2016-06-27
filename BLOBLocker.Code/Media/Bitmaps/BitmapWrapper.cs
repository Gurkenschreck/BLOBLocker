using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Media.Bitmaps
{
    public abstract class BitmapWrapper
    {
        public ImageFormat Format
        {
            get;
            protected set;
        }
        public Bitmap Bitmap
        {
            get;
            protected set;
        }
    }
}
