using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ImageMagick;
using Simple_Scaler_2.Processing;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            List<DateTime> dtest = new List<DateTime> { new DateTime(2017,1,1), new DateTime(2015,1,1), new DateTime(2016,1,1), new DateTime(2018,1,1) };
            dtest = dtest.OrderBy(t => t).ToList();

            var features = MagickNET.Features;
            var magickFormatInfos = MagickNET.SupportedFormats.ToArray();

            string path = Path.GetFullPath("Unbenannt.tif");

            MagickNET.Log += (sender, eventArgs) => Console.WriteLine(eventArgs.Message);
            
            var fac = new MagickFactory();

            var test = MagickImageInfo.ReadCollection(path).First();

            var coll = fac.CreateCollection(path);
            
            var img = coll[0];
            
            //img.Format = MagickFormat.Tiff;
            img.Settings.SetDefine("compress", "None");

            var testcoll = img.GetPixels();
            var testPixel = testcoll.GetPixel(1000, 1000);
            var testColor = testPixel.ToColor();
            var testColor2 = new MagickColor(Color.White);
            if (testColor == testColor2)
            {

            }
            //int width = 975;
            //int height = 330;
            //int x = img.BaseWidth - width * 2;
            //int y = img.BaseHeight - height * 2;
            //MagickGeometry geo = new MagickGeometry
            //{
            //    Height = height,
            //    Width = width,
            //    X = x,
            //    Y = y
            //};

            double Rand1x, Rand1y, Rand2x, Rand2y, Kor1x, Kor1y, Kor2x, Kor2y, Kor3x, Kor3y, Kor4x, Kor4y;

            Rand1x = 10;
            Rand1y = 10;
            Rand2x = -1;
            Rand2y = -1;
            Kor1x = 0;
            Kor1y = -10;
            Kor2x = 0;
            Kor2y = 5;
            Kor3x = -10;
            Kor3y = 10;
            Kor4x = 10;
            Kor4y = -10;

            Transformer testTrans = new Transformer();
            var transformSettings = testTrans.FindSettings(String.Empty, new ImageFileInfo(true, true, true, path, true, false, false), path);

            Rand1x = transformSettings.Rand1X;
            Rand1y = transformSettings.Rand1Y;
            Rand2x = transformSettings.Rand2X;
            Rand2y = transformSettings.Rand2Y;

            double dpi = 360;
            double prevRes = 25.4;
            double prevHmax = 750;

            double imgWpx = img.BaseWidth;
            double imgHpx = img.BaseHeight;
            double imgW = Math.Round(imgWpx / 360 * prevRes, 1);
            double imgH = Math.Round(imgHpx / 360 * prevRes, 1);

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Rand2x == -1)
                Rand2x = imgW - 15;
            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (Rand2y == -1)
                Rand2y = imgH - 15;

            //double prevH;
            //double prevW;
            //double prevHMax = 0;
            //double prevScale;

            //if (imgH > prevHmax)
            //{
            //    prevH = prevHmax;
            //    prevScale = prevHMax / imgH;
            //    prevW = imgW * prevScale;
            //}
            //else
            //{
            //    prevH = imgH;
            //    prevW = imgW;
            //    prevScale = 1.0;
            //}

            double randOpx = Rand1y / 25.4 * 360;
            double randUpx = Rand2y / 25.4 * 360;
            double randLpx = Rand1x / 25.4 * 360;
            double randRpx = Rand2x / 25.4 * 360;

            const double korScale = 1;

            //DrawableRectangle randRectangle = new DrawableRectangle(0,0, img.Width - 1, img.Height - 1);
            //DrawableRectangle rectangle = new DrawableRectangle(Rand1x * 360 / 25.4, Rand1y  * 360 / 25.4, (Rand2x-1) * 360 / 25.4, (Rand2y-1)  * 360 / 25.4);
            DrawableRectangle randRectangle = new DrawableRectangle(0,0, (imgW - 1), (imgH - 1));
            DrawableRectangle rectangle = new DrawableRectangle(Rand1x, Rand1y, (Rand2x - 1), (Rand2y-1));
            DrawableFillColor fillColor = new DrawableFillColor(Color.Transparent);
            DrawableStrokeWidth strokeWidth = new DrawableStrokeWidth(1);
            DrawableStrokeColor red = new DrawableStrokeColor(Color.Red);
            DrawableStrokeColor green = new DrawableStrokeColor(Color.Green);
            DrawableStrokeColor black = new DrawableStrokeColor(Color.Black);
            var checker = new FileInfo("JVS_Checker_1x1.tif");


            double[] previewPerspective = { Rand1x, Rand1y, Rand1x + Kor1x * korScale, Rand1y + Kor1y * korScale, 
                                           Rand2x, Rand1y, Rand2x + Kor2x * korScale, Rand1y + Kor2y * korScale, 
                                           Rand1x, Rand2y, Rand1x + Kor3x * korScale, Rand2y + Kor3y * korScale, 
                                           Rand2x, Rand2y, Rand2x + Kor4x * korScale, Rand2y + Kor4y * korScale };

            using (var prevColl = fac.CreateCollection())
            {
                var prevImg = img.Clone();
                prevImg.ColorSpace = ColorSpace.RGB;
                prevImg.Format = MagickFormat.Png;
                prevImg.Quality = 50;

                //prevImg.Density = new Density(prevImg.Density.X, prevImg.Density.Y, DensityUnit.PixelsPerInch);
                
                prevImg.Resample(prevRes, prevRes);

                int widht = prevImg.Width;
                int height = prevImg.Height;
                
                using (var cimg = fac.CreateImage(checker))
                    prevImg.Tile(cimg, CompositeOperator.Plus);

                prevImg.Draw(black, strokeWidth, fillColor, randRectangle);
                prevImg.Draw(green, strokeWidth,fillColor, rectangle);

                prevColl.Add(prevImg);


                var preCompose = fac.CreateImage();
                
                preCompose.Read(new FileInfo("transparent.tiff"));
                preCompose.Resample(prevRes, prevRes);
                preCompose.Crop(0, 0, widht, height);

                preCompose.Draw(rectangle, red, fillColor);
                preCompose.Interpolate = PixelInterpolateMethod.Nearest;
                preCompose.FilterType = FilterType.Quadratic;
                preCompose.Distort(DistortMethod.BilinearForward, previewPerspective);
                
                prevColl.Add(preCompose);

                using (var prevImgFile = prevColl.Flatten())
                {
                    prevImgFile.Format = MagickFormat.Png;
                    prevImgFile.Write(new FileInfo("prevFile.png"));
                }
            }

            img.Grayscale(PixelIntensityMethod.Average);
            img.Level(new Percentage(99), new Percentage(100));

            double[] perspectiveDistort =
            {
                randLpx, randOpx, randLpx + Kor1x * 1, randOpx + Kor1y * 1, randRpx, randOpx, randRpx + Kor2x * 1, randOpx + Kor2y * 1, randLpx, randUpx, randLpx + Kor3x * 1, 
                randUpx + Kor3y * 1, randRpx, randUpx, randRpx + Kor4x * 1, randUpx + Kor4y * 1
            };

            img.Interpolate = PixelInterpolateMethod.Nearest;
            img.FilterType = FilterType.Point;
            img.Distort(DistortMethod.BilinearForward, perspectiveDistort);

            using (var cimg = fac.CreateImage(checker))
            {
                cimg.Density = new Density(360,360, DensityUnit.PixelsPerInch);
                cimg.FilterType = FilterType.Point;
                img.Tile(cimg, CompositeOperator.Plus);
            }

            img.Write(new FileInfo("New.tiff"));

            img.Dispose();
            coll.Dispose();

            Console.WriteLine(Environment.NewLine + "Fertig");
            Console.ReadKey();
        }
    }
}
