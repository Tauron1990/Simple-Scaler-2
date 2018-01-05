using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ImageMagick;
using JetBrains.Annotations;
using Simple_Scaler_2.Properties;
using Color = System.Drawing.Color;

namespace Simple_Scaler_2.Processing
{
    [PublicAPI]
    public sealed class Transformer
    {
        private readonly MagickFactory _magickFactory = new MagickFactory();

        public static bool CheckFolderPermission(string folderPath)
        {
            var dirInfo = new DirectoryInfo(folderPath);
            try
            {
                var unused = dirInfo.GetAccessControl(AccessControlSections.Access);
                return true;
            }
            catch (PrivilegeNotHeldException)
            {
                return false;
            }
        }

        public event EventHandler<PreviewGeneratedEventArgs> PreviewGeneratedEvent;

        private Result Protect(Func<Result> action)
        {
            try
            {
                return action();
            }
            catch (Exception e)
            {
                if (e is StackOverflowException || e is OutOfMemoryException || e is ThreadAbortException
                 || e is SecurityException)
                    throw;

                return new ExceptionResult(e);
            }
        }

        private int Iterate(int first, int second, MagickColor baseColor, Func<int, int, Pixel> getPixel, bool revers = false)
        {
            if (!revers)
                for (var i = 0; i < first; i += 3)
                {
                    for (var j = 0; j < second; j += 3)
                    {
                        if (getPixel(i, j).ToColor() == baseColor) continue;

                        return i;
                    }
                }
            else
                for (var i = first; i >= 0; i -= 3)
                {
                    for (var j = 0; j < second; j += 3)
                    {
                        if (getPixel(i, j).ToColor() == baseColor) continue;

                        return i;
                    }
                }

            return -1;
        }

        private ImageSource CreateImageSource(byte[] imageBytes)
        {
            return Application.Current.Dispatcher.Invoke(() =>
                                                         {
                                                             var stream = new MemoryStream(imageBytes);
                                                             var img    = new BitmapImage();

                                                             img.BeginInit();
                                                             img.StreamSource = stream;
                                                             img.EndInit();

                                                             img.Freeze();
                                                             return img;
                                                         });
        }

        public TransformSettings FindSettings(string basePath, ImageFileInfo info, string realPath)
        {
            var settingsPath = Path.Combine(basePath, info.FilePath.TrimStart('\\') + ".jvs");
            if (File.Exists(settingsPath))
                try
                {
                    return TransformSettings.FromSettings(settingsPath);
                }
                catch (InvalidOperationException)
                {
                }

            var newSettings = new TransformSettings {FilePath = settingsPath};

            using (var image = _magickFactory.CreateImage(realPath))
            {
                // ReSharper disable AccessToDisposedClosure
                using (var pcoll = image.GetPixelsUnsafe())
                {
                    var appSet  = Settings.Default;
                    var dpiFull = appSet.Resolution;
                    var dpiPrev = appSet.PrevResolution;
                    var imgW    = image.Width;
                    var imgH    = image.Height;

                    var baseColor = new MagickColor(Color.White);

                    Task one   = Task.Run(() => newSettings.Rand1X = (int) Math.Round(Iterate(imgH, imgW, baseColor, (i, j) => pcoll.GetPixel(i, j)) / dpiFull * dpiPrev, 1));
                    Task two   = Task.Run(() => newSettings.Rand1Y = (int) Math.Round(Iterate(imgW, imgH, baseColor, (i, j) => pcoll.GetPixel(j, i)) / dpiFull * dpiPrev, 1));
                    Task three = Task.Run(() => newSettings.Rand2X = (int) Math.Round(Iterate(imgW, imgH, baseColor, (i, j) => pcoll.GetPixel(i, j), true) / dpiFull * dpiPrev, 1));
                    Task four  = Task.Run(() => newSettings.Rand2Y = (int) Math.Round(Iterate(imgH, imgW, baseColor, (i, j) => pcoll.GetPixel(j, i), true) / dpiFull * dpiPrev, 1));

                    Task.WaitAll(one, two, three, four);
                }
                // ReSharper restore AccessToDisposedClosure
            }

            newSettings.Save(settingsPath);

            return newSettings;
        }

        public Result GetInfo(string basePath, string relativePath)
        {
            return Protect(() =>
                           {
                               var info = GetInfoImpl(basePath, relativePath);
                               return Result.Create(info, info.IsAccesible);
                           });
        }

        private ImageFileInfo GetInfoImpl(string basePath, string relativePath)
        {
            var path     = Path.Combine(basePath, relativePath.TrimStart('\\'));
            var settings = Settings.Default;

            var isSingleLayer = false;

            var isAccesible = CheckFolderPermission(Path.GetDirectoryName(path));
            if (isAccesible)
                isAccesible = File.Exists(path);

            if (!isAccesible) return new ImageFileInfo(false, false, false, relativePath, false, false, false);

            var infos = new List<IMagickImageInfo>(MagickImageInfo.ReadCollection(path));

            if (infos.Count == 1)
                isSingleLayer = true;

            var des = infos[0].Density;
            // ReSharper disable CompareOfFloatsByEqualityOperator
            var isCorrectResolution  = des.X == settings.Resolution && des.Y == settings.Resolution;
            var isResolutionUnKnowen = !isCorrectResolution && des.X == 0d && des.Y == 0d;
            // ReSharper restore CompareOfFloatsByEqualityOperator

            var isCorrectType = infos[0].Format == MagickFormat.Tiff || infos[0].Format == MagickFormat.Tif;
            var isGrayScale   = infos[0].ColorSpace == ColorSpace.Gray;

            return new ImageFileInfo(isSingleLayer, isCorrectResolution, isCorrectType, relativePath, true, isResolutionUnKnowen, isGrayScale);
        }

        public string GetPreparationPath(string basePath, ImageFileInfo info)
        {
            
            return Path.Combine(basePath, Path.GetDirectoryName(info.FilePath) ?? throw new InvalidOperationException(),
                                        "Prep", Path.GetFileName(info.FilePath) + ".tiff");
        }

        public Result PrepareFile(string basePath, ImageFileInfo info)
        {
            return Protect(() =>
                           {
                               info = GetInfoImpl(basePath, info.FilePath);
                               var realPath = Path.Combine(basePath, info.FilePath.TrimStart('\\'));
                               var resolution = Settings.Default.Resolution;

                               if (info.IsCorrectResolution && info.IsCorrectType && info.IsSingleLayer)
                                   return Result.Create(new PreparedImageFileInfo(info.FilePath, info, FindSettings(basePath, info, info.FilePath)), true);

                               var preparationPath = GetPreparationPath(basePath, info);
                               var folder = Path.GetDirectoryName(preparationPath) ?? throw new InvalidOperationException();
                               if (!Directory.Exists(folder))
                                   Directory.CreateDirectory(folder);

                               var writeDate = File.GetLastWriteTime(realPath);
                               var writeFile = preparationPath + ".prep";

                               if (File.Exists(writeFile))
                               {
                                   try
                                   {
                                       using (var reader = new BinaryReader(new FileStream(writeFile, FileMode.Open)))
                                       {
                                           var oldTime = new DateTime(reader.ReadInt64());
                                           if(writeDate == oldTime && File.Exists(preparationPath))
                                               return Result.Create(new PreparedImageFileInfo(preparationPath.Replace(basePath, string.Empty), info, FindSettings(basePath, info, preparationPath)), true);
                                       }
                                   }
                                   catch (Exception e) when(e is IOException)
                                   {
                                      
                                   }
                               }

                               IMagickImage img;
                               var          setting = info.IsResolutionUnKnowen ? new MagickReadSettings {Density = new Density(360, DensityUnit.PixelsPerInch)} : null;

                               if (info.IsSingleLayer)
                                   img = _magickFactory.CreateImage(info.FilePath, setting);
                               else
                                   using (var coll = _magickFactory.CreateCollection(info.FilePath, setting))
                                       img = coll.Flatten();

                               using (img)
                               {
                                   if (!info.IsCorrectType)
                                       img.Format = MagickFormat.Tiff;
                                   if (!info.IsCorrectResolution && !info.IsResolutionUnKnowen)
                                       img.Resample(resolution, resolution);
                                   if (!info.IsGreyScale)
                                   {
                                       img.Grayscale(PixelIntensityMethod.Average);
                                       img.Level(new Percentage(60), new Percentage(61));
                                   }


                                   img.Settings.Compression = Compression.LZW; //.SetDefine("compress", "LZW");
                                   img.Write(preparationPath);
                               }

                               using (var file = new BinaryWriter(new FileStream(writeFile, FileMode.Create)))
                                   file.Write(writeDate.Ticks);

                               return Result.Create(new PreparedImageFileInfo(preparationPath.Replace(basePath, string.Empty), info, FindSettings(basePath, info, preparationPath)), true);
                           });
        }

        public Result GeneratePreview(string basePath, string path, TransformSettings settings) => Transform(basePath, path, null, settings, true);

        public Result Transform(string basePath, string relativePath, string target, TransformSettings settings, bool generateOnlyPreview = false)
        {
            return Protect(() =>
                           {
                               var path = Path.Combine(basePath, relativePath.TrimStart('\\'));
                               var appSet = Settings.Default;

                               var checker = new Func<IMagickImage>(() => _magickFactory.CreateImage(Resources.JVS_Checker_1x1));

                               using (var img = _magickFactory.CreateImage(path))
                               {
                                   try
                                   {
                                       var fullRes = appSet.Resolution;
                                       var prevRes = appSet.PrevResolution;

                                       double imgWpx = img.BaseWidth;
                                       double imgHpx = img.BaseHeight;
                                       var    imgW   = Math.Round(imgWpx / fullRes * prevRes, 1);
                                       var    imgH   = Math.Round(imgHpx / fullRes * prevRes, 1);

                                       // ReSharper disable once CompareOfFloatsByEqualityOperator
                                       if (settings.Rand2X == -1)
                                           settings.Rand2X = (int) imgW - 15;
                                       // ReSharper disable once CompareOfFloatsByEqualityOperator
                                       if (settings.Rand2Y == -1)
                                           settings.Rand2Y = (int) (imgH - 15);

                                       settings.Save();

                                       double rand1X = settings.Rand1X;
                                       double rand1Y = settings.Rand1Y;
                                       double rand2X = settings.Rand2X;
                                       double rand2Y = settings.Rand2Y;
                                       double kor1X  = settings.Kor1X;
                                       double kor1Y  = settings.Kor1Y;
                                       double kor2X  = settings.Kor2X;
                                       double kor2Y  = settings.Kor2Y;
                                       double kor3X  = settings.Kor3X;
                                       double kor3Y  = settings.Kor3Y;
                                       double kor4X  = settings.Kor4X;
                                       double kor4Y  = settings.Kor4Y;

                                       const double korScale = 1;

                                       double[] previewPerspective =
                                       {
                                           rand1X, rand1Y, rand1X + kor1X * korScale, rand1Y + kor1Y * korScale,
                                           rand2X, rand1Y, rand2X + kor2X * korScale, rand1Y + kor2Y * korScale,
                                           rand1X, rand2Y, rand1X + kor3X * korScale, rand2Y + kor3Y * korScale,
                                           rand2X, rand2Y, rand2X + kor4X * korScale, rand2Y + kor4Y * korScale
                                       };

                                       var imgClone = img.Clone();
                                       // ReSharper disable once AccessToDisposedClosure
                                       var previewGen = Task.Run(() => GeneratePreview(imgW, imgH, rand1X, rand1Y, rand2X, rand2Y, imgClone, prevRes, checker, settings, previewPerspective))
                                                            .ContinueWith(t => imgClone.Dispose());

                                       if (generateOnlyPreview)
                                       {
                                           previewGen.Wait();
                                           return Result.Create(true, true);
                                       }

                                       previewGen.Wait();
                                       var imgGen = Task.Run(() =>
                                                             {
                                                                 // ReSharper disable once AccessToDisposedClosure
                                                                 var innerImg = img;

                                                                 var randOpx = rand1Y / prevRes * fullRes;
                                                                 var randUpx = rand2Y / prevRes * fullRes;
                                                                 var randLpx = rand1X / prevRes * fullRes;
                                                                 var randRpx = rand2X / prevRes * fullRes;

                                                                 double[] perspectiveDistort =
                                                                 {
                                                                     randLpx, randOpx, randLpx + kor1X * 1, randOpx + kor1Y * 1, randRpx, randOpx, randRpx + kor2X * 1, randOpx + kor2Y * 1, randLpx, randUpx, randLpx + kor3X * 1,
                                                                     randUpx + kor3Y * 1, randRpx, randUpx, randRpx + kor4X * 1, randUpx + kor4Y * 1
                                                                 };

                                                                 innerImg.Interpolate = PixelInterpolateMethod.Nearest;
                                                                 innerImg.FilterType  = FilterType.Point;
                                                                 innerImg.Distort(DistortMethod.BilinearForward, perspectiveDistort);

                                                                 if(settings.Checker)
                                                                 {
                                                                     using (var cimg = checker())
                                                                     {
                                                                         cimg.Density    = new Density(fullRes, fullRes, DensityUnit.PixelsPerInch);
                                                                         cimg.FilterType = FilterType.Point;
                                                                         innerImg.Tile(cimg, CompositeOperator.Plus);
                                                                     }
                                                                 }

                                                                 innerImg.Settings.Compression = Compression.NoCompression;
                                                                 innerImg.Write(target);
                                                             });

                                       previewGen.Wait();
                                       imgGen.Wait();
                                   }
                                   catch
                                   {
                                       var previewSource1 = CreateImageSource(Resources.Error);
                                       var previewSource2 = CreateImageSource(Resources.Error);

                                       OnPreviewGeneratedEvent(new PreviewGeneratedEventArgs(previewSource1, previewSource2));

                                       throw;
                                   }
                               }

                               return Result.Create(true, true);
                           });
        }

        private void GeneratePreview(double       imgW, double    imgH, double                rand1X, double             rand1Y, double rand2X, double rand2Y,
                                     IMagickImage prevImg, double prevRes, Func<IMagickImage> checker, TransformSettings settings,
                                     double[]     previewPerspective)
        {
            var randRectangle = new DrawableRectangle(0, 0, imgW - 1, imgH - 1);
            var rectangle     = new DrawableRectangle(rand1X, rand1Y, rand2X - 1, rand2Y - 1);
            var fillColor     = new DrawableFillColor(Color.Transparent);
            var strokeWidth   = new DrawableStrokeWidth(1);
            var red           = new DrawableStrokeColor(Color.Red);
            var green         = new DrawableStrokeColor(Color.Green);
            var black         = new DrawableStrokeColor(Color.Black);

            using (var prevColl = _magickFactory.CreateCollection())
            {
                var widht  = 0;
                var height = 0;

                var prevImgTask = Task.Run(() =>
                                           {
                                               widht  = prevImg.Width;
                                               height = prevImg.Height;

                                               prevImg.Resample(prevRes, prevRes);

                                               prevImg.ColorSpace = ColorSpace.RGB;
                                               prevImg.Format     = MagickFormat.Png;
                                               prevImg.Quality    = 50;

                                               return prevImg;
                                           });

                var prevImgLeft = Task.Run(() =>
                                           {
                                               var prevImgl = prevImgTask.Result;

                                               if (settings.Checker)
                                                   using (var cimg = checker())
                                                       prevImgl.Tile(cimg, CompositeOperator.Plus);

                                               prevImgl.Draw(black, strokeWidth, fillColor, randRectangle);
                                               prevImgl.Draw(green, strokeWidth, fillColor, rectangle);

                                               return prevImgl.ToByteArray();
                                           });

                var prevImgRight = Task.Run(() =>
                                            {
                                                var preCompose = _magickFactory.CreateImage(Resources.Transparent);

                                                preCompose.Resample(prevRes, prevRes);
                                                preCompose.Crop(0, 0, widht, height);

                                                preCompose.Draw(rectangle, red, fillColor);
                                                preCompose.Interpolate = PixelInterpolateMethod.Nearest;
                                                preCompose.FilterType  = FilterType.Quadratic;
                                                preCompose.Distort(DistortMethod.BilinearForward, previewPerspective);

                                                return preCompose;
                                            });

                var         left = CreateImageSource(prevImgLeft.Result);
                ImageSource right;
                prevColl.Add(prevImgTask.Result);
                prevColl.Add(prevImgRight.Result);

                using (var prevImgFile = prevColl.Flatten())
                {
                    prevImgFile.Format = MagickFormat.Png;
                    right              = CreateImageSource(prevImgFile.ToByteArray());
                }

                OnPreviewGeneratedEvent(new PreviewGeneratedEventArgs(left, right));
            }
        }

        private void OnPreviewGeneratedEvent(PreviewGeneratedEventArgs e)
        {
            PreviewGeneratedEvent?.Invoke(this, e);
        }
    }
}