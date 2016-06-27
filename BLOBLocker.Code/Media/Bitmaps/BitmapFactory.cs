using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;


namespace BLOBLocker.Code.Media.Bitmaps
{
    public abstract class BitmapFactory<T>
        where T : BitmapWrapper
    {
        public ImageFormat Format { get; set; }
        
        public abstract T CreateNew(int width, int height);
    }
}
