using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyTimeDatabaseLib
{
    public class BitmapConverter
    {


        public static BitmapImage GetBitmapImage(int[] ImageSrc)
        {
            var wb = new WriteableBitmap(100, 100);
            for (int i = 0; i < wb.Pixels.Length; i++) {
                wb.Pixels[i] = 0xFF3300;
            }
            var bmp = new BitmapImage();
            using (var ms = new MemoryStream()) {
                wb.SaveJpeg(ms, 100, 100, 0, 100);
                bmp.SetSource(ms);
            }

            var bi = new BitmapImage();
            if (ImageSrc != null && ImageSrc.Length >= 0) {
                var ris = new WriteableBitmap(450, 250);

                //get image from database
                for (int i = 0; i < ImageSrc.Length; i++) {
                    ris.Pixels[i] = ImageSrc[i];
                }

                //put the image in a WritableBitmap
                using (var ms = new MemoryStream()) {
                    ris.SaveJpeg(ms, 450, 250, 0, 100);
                    bi.SetSource(ms);
                }

                //crop the image to 100x100 and centered
                var img = new Image {
                    Source = bi,
                    Width = 450,
                    Height = 250
                };
                var wb2 = new WriteableBitmap(100, 100);
                var t = new CompositeTransform {
                    ScaleX = 0.5,
                    ScaleY = 0.5,
                    TranslateX = -((450 / 2) / 2 - 50),
                    TranslateY = -((250 / 2) / 2 - 50)
                };
                wb2.Render(img, t);
                wb2.Invalidate();
                bi = new BitmapImage();
                using (var ms = new MemoryStream()) {
                    wb2.SaveJpeg(ms, 100, 100, 0, 100);
                    bi.SetSource(ms);
                }
                //BitmapImage bi is now cropped
            } else {
                bi = bmp; //Default image.
            }
            return bi;
        }
    }
}
