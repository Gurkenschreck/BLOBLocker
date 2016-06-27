using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOBLocker.Code.Media.Bitmaps
{
    public class CaptchaFactory : BitmapFactory<Captcha>
    {
        public CaptchaFactory()
        {
            Format = ImageFormat.Gif;
        }
        
        public override Captcha CreateNew(int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);

            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);

            Random rdm = new Random();
            graphics.DrawLine(Pens.Black, rdm.Next(0, width), rdm.Next(0, height), rdm.Next(0, width), rdm.Next(0, height));
            graphics.DrawRectangle(Pens.Blue, rdm.Next(0, width), rdm.Next(0, height), rdm.Next(0, width), rdm.Next(0, height));
            graphics.DrawLine(Pens.Blue, rdm.Next(0, width), rdm.Next(0, height), rdm.Next(0, width), rdm.Next(0, height));

            Brush brush = default(Brush);

            HatchStyle[] aHatchStyles = new HatchStyle[]  
            {  
                HatchStyle.BackwardDiagonal, HatchStyle.Cross, HatchStyle.DashedDownwardDiagonal, HatchStyle.DashedHorizontal, HatchStyle.DashedUpwardDiagonal, HatchStyle.DashedVertical,  
                    HatchStyle.DiagonalBrick, HatchStyle.DiagonalCross, HatchStyle.Divot, HatchStyle.DottedDiamond, HatchStyle.DottedGrid, HatchStyle.ForwardDiagonal, HatchStyle.Horizontal,  
                    HatchStyle.HorizontalBrick, HatchStyle.LargeCheckerBoard, HatchStyle.LargeConfetti, HatchStyle.LargeGrid, HatchStyle.LightDownwardDiagonal, HatchStyle.LightHorizontal  
            };

            RectangleF rectF = new RectangleF(0, 0, width, height);

            brush = new HatchBrush(aHatchStyles[rdm.Next(aHatchStyles.Length - 3)],
                Color.FromArgb((rdm.Next(100, 255)), (rdm.Next(100, 255)), (rdm.Next(100, 255))), Color.White);
            
            graphics.FillRectangle(brush, rectF);

            Font font = new Font("Courier New", rdm.Next(20, 28), FontStyle.Bold);
            string captchaText = string.Format("{0:X}", rdm.Next(1000000, 9999999));

            graphics.DrawString(captchaText, font, Brushes.Black, 20f, 20f);

            Captcha captcha = new Captcha(captchaText, bitmap, Format);

            return captcha;
        }
    }
}
