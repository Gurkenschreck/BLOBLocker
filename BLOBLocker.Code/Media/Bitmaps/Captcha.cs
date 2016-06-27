using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Media.Bitmaps
{
    public class Captcha : BitmapWrapper
    {
        public string Value { get; set; }
        public string ImageAsBase64
        {
            get
            {
                ImageConverter conv = new ImageConverter();
                return Convert.ToBase64String(conv.ConvertTo(Bitmap, typeof(byte[])) as byte[]);
            }
        }

        public Captcha(string value, Bitmap bitmap, ImageFormat format)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            this.Value = value;
            this.Bitmap = bitmap;
            this.Format = format;
        }
    }
}
