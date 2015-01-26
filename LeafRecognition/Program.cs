using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using System.IO;
using System.Diagnostics;

namespace LeafRecognition
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //CannyCamera();
            //LeafBorders();
            ShowImage(GetCountours(LoadImage("Resources/maple_leaf.jpg")));
        }


        static IplImage LoadImage(string path)
        {
            return new IplImage(path, LoadMode.Color);
        }

        static void ShowImage(IplImage image)
        {
            using (CvWindow win = new CvWindow("OpenCV Window"))
            {
                win.ShowImage(image);
                CvWindow.WaitKey(0);
            }
            //ShowImages(new IplImage[] { image });
        }
        static void ShowImages(List<IplImage> images)
        {
            ShowImages(images.ToArray());
        }
        static void ShowImages(IplImage[] images)
        {
            var maxWidth = Screen.PrimaryScreen.Bounds.Width;
            var currentWidth = 0;
            var rows = new List<List<IplImage>>();
            var activeRow = new List<IplImage>();
            rows.Add(activeRow);

            foreach (IplImage image in images)
            {
                if (image.Width + currentWidth > maxWidth && activeRow.Count != 0)
                {
                    activeRow = new List<IplImage>();
                    rows.Add(activeRow);
                }
                activeRow.Add(image);
                currentWidth += image.Width;
            }

            IplImage composedImage = new IplImage();
        }



        static void LeafBorders()
        {
            using (CvWindow win2 = new CvWindow("OpenCv Window2"))
            using (CvWindow win = new CvWindow("OpenCv Window"))
            {
                using (IplImage src = Cv.LoadImage("Resources/maple_leaf.jpg", LoadMode.Color))
                {
                    //dst.FindContours()

                    win2.ShowImage(src);
                    win.ShowImage(GetLeafBorders(src));
                }
                CvWindow.WaitKey();
                win.Image.Dispose();
            }
        }

        static IplImage GetThresholdImage(IplImage src)
        {
            IplImage dst = src;
            if (src.ElemChannels != 1)
            {
                dst = new IplImage(src.Size, BitDepth.U8, 1);
                src.CvtColor(dst, ColorConversion.BgrToGray);
            }
            
            dst.Smooth(dst, SmoothType.Gaussian);
            dst.Threshold(dst, 255.0 * 0.9, 255.0, ThresholdType.Binary);

            return dst;
        }

        static IplImage GetLeafBorders(IplImage src)
        {
            IplImage dst = GetThresholdImage(src);

            dst.Laplace(dst); // find edges
            dst.Not(dst); // invert edges

            return dst;
        }

        static IplImage GetCountours(IplImage src)
        {
            CvMemStorage mem = Cv.CreateMemStorage(0);
            CvSeq<CvPoint> firstContour = null;

            IplImage dst = GetThresholdImage(src);
            dst = GetThresholdImage(dst);

            int count = dst.FindContours(mem, out firstContour);

            //src.DrawContours(firstContour, CvColor.Green, CvColor.Blue, 2);
            //src.DrawRect(firstContour.BoundingRect(), CvColor.Red);

            src = src.GetSubImage(firstContour.BoundingRect());

            mem.Dispose();
            firstContour.Dispose();
            dst.Dispose();

            return src;
        }


        static void CannyCamera()
        {
            using (CvWindow win = new CvWindow("Canny"))
            using (CvCapture cap = new CvCapture(0))
            {
                using (IplImage frame = cap.QueryFrame())
                using (IplImage dst = new IplImage(frame.Size, BitDepth.U8, 1))
                {
                    frame.CvtColor(dst, ColorConversion.BgrToGray);

                    dst.Canny(dst, 50.0, 50.0, ApertureSize.Size3);

                    win.Image = dst;
                }

                CvWindow.WaitKey();
            }
        }
    }
}
