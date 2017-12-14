using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShowComposer.NAudioDemo.Gui
{
    /// <summary>
    /// Interaction logic for WaveformPainter.xaml
    /// </summary>
    public partial class WaveformPainter : UserControl
    {
        const int RESOLUTION = 1920;
        Pen foregroundPen;
        List<float> samples = new List<float>(RESOLUTION);
        public List<float> PositiveSamples = new List<float>(RESOLUTION);
        public List<float> NegativeSamples = new List<float>(RESOLUTION);
        public List<float> AverageSamples = new List<float>(RESOLUTION);
        int maxSamples;

        public int MaxSamples
        {
            get { return maxSamples; }
            set { maxSamples = value; }
        }
        int insertPos;
        private int bytesPerSample;
        private int samplesPerPixel = 128;

        bool hasSamples { get { return false; } }

        public WaveformPainter()
        {
            foregroundPen = new Pen(Brushes.Black, 1);

            //this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |ControlStyles.OptimizedDoubleBuffer, true);

            InitializeComponent();

            PositiveSamples = Enumerable.Repeat(0.0f, RESOLUTION).ToList();
            NegativeSamples = Enumerable.Repeat(0.0f, RESOLUTION).ToList();

            maxSamples = RESOLUTION;//(int)this.ActualWidth;

            //bytesPerSample = (waveStream.WaveFormat.BitsPerSample / 8) * waveStream.WaveFormat.Channels;
            bytesPerSample = (24 / 8) * 2;

            //OnForeColorChanged(EventArgs.Empty);
            // OnResize(EventArgs.Empty);
        }

        public void ClearSamples()
        {
            samples.Clear();
            PositiveSamples = Enumerable.Repeat(0.0f, RESOLUTION).ToList();
            NegativeSamples = Enumerable.Repeat(0.0f, RESOLUTION).ToList();
        }
        /// <summary>
        /// On Resize
        /// </summary>
        //protected override void OnResize(EventArgs e)
        //{
        //    maxSamples = this.Width;
        //    base.OnResize(e);
        //}

        /// <summary>
        /// On ForeColor Changed
        /// </summary>
        /// <param name="e"></param>
        //protected override void OnForeColorChanged(EventArgs e)
        //{
        //    foregroundPen = new Pen(ForeColor);
        //    base.OnForeColorChanged(e);
        //}

        /// <summary>
        /// Add Max Value
        /// </summary>
        /// <param name="maxSample"></param>
        public void AddMax(float maxSample)
        {
            if (maxSamples == 0)
            {
                // sometimes when you minimise, max samples can be set to 0
                return;
            }
            if (samples.Count <= maxSamples)
            {
                samples.Add(maxSample);
            }
            else if (insertPos < maxSamples)
            {
                //samples[insertPos] = maxSample;
            }
            insertPos++;
            insertPos %= maxSamples;

            //Dispatcher.Invoke(delegate { InvalidateVisual(); });
        }

        public void Invalidate()
        {
            //Dispatcher.Invoke(delegate { InvalidateVisual(); });
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                //
                // ... draw on the drawingContext
                //
                var height = ActualHeight;
                var med = height / 2;
                StreamGeometry streamGeometry = new StreamGeometry();
                //RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

                drawingContext.DrawLine(
                    new Pen(new SolidColorBrush(Color.FromArgb(255, 192, 192, 192)), 1),
                    new Point(0, med - med / 2),
                    new Point(ActualWidth, med - med / 2));

                drawingContext.DrawLine(
                   new Pen(new SolidColorBrush(Color.FromArgb(255, 192, 192, 192)), 1),
                   new Point(0, med + med / 2),
                   new Point(ActualWidth, med + med / 2));

                drawingContext.DrawLine(
                    new Pen(new SolidColorBrush(Color.FromArgb(255, 0, 0, 235)), 1),
                    new Point(0, med),
                    new Point(ActualWidth, med));

                if (PositiveSamples.Count > 0)
                    using (StreamGeometryContext geometryContext = streamGeometry.Open())
                    {
                        PointCollection pcn = new PointCollection(), pcp = new PointCollection();
                        double deltaX = (double)ActualWidth / NegativeSamples.Count;
                        double normalizeFactor = NegativeSamples.Max(a => Math.Abs(a)) / ((double)height / 2);
                        normalizeFactor *= 1.2;
                        geometryContext.BeginFigure(new Point(0, med), false, false);
                        for (int x = 0, n = PositiveSamples.Count; x < n; x++)
                        {
                            //geometryContext.LineTo(new Point(x * deltaX, med), true, true);
                            geometryContext.LineTo(new Point(x * deltaX, med - (PositiveSamples[x] / normalizeFactor)), true, true);

                            //geometryContext.LineTo(new Point(x * deltaX, med), true, true);
                            geometryContext.LineTo(new Point(x * deltaX, med - (NegativeSamples[x] / normalizeFactor)), true, true);
                        }
                    }

                drawingContext.DrawGeometry(
                            new SolidColorBrush(Color.FromArgb(255, 215, 215, 215)),
                            new Pen(new SolidColorBrush(Color.FromArgb(255, 0, 0, 90)), 1),
                            streamGeometry);
             
                /* if (PositiveSamples.Count > 0)
                     using (StreamGeometryContext geometryContext = streamGeometry.Open())
                     {
                         PointCollection pcn = new PointCollection(), pcp = new PointCollection();
                         double i = 0.0;
                         double step = ((int)RESOLUTION / ActualWidth);
                         step = step < 0 ? 1 : step;
                         //bool isNeg = false;
                         //double epsilon = 0.01;
                         double deltaX = (double)ActualWidth / NegativeSamples.Count;
                         double normalizeFactor = NegativeSamples.Max(a => Math.Abs(a)) / ((double)height / 2);

                         geometryContext.BeginFigure(new Point(0, med), false, false);

                         for (int x = 0, n = NegativeSamples.Count; x < n; x++)
                         {
                             geometryContext.LineTo(new Point(x * deltaX, med), true, true);
                             geometryContext.LineTo(new Point(x * deltaX, med - (NegativeSamples[x] / normalizeFactor)), true, true);
                         }
                         for (int x = 0, n = PositiveSamples.Count; x < n; x++)
                         {
                             geometryContext.LineTo(new Point(x * deltaX, med), true, true);
                             geometryContext.LineTo(new Point(x * deltaX, med - (PositiveSamples[x] / normalizeFactor)), true, true);
                         }
                         for (int x = 1; x < ActualWidth; x++)
                         {
                             i += step;
                             int j = (int)i;
                             if (j >= NegativeSamples.Count)
                                 j = NegativeSamples.Count - 1;


                             geometryContext.LineTo(new Point(x, (NegativeSamples[j]) * -(med - med/100*20) + med), true, true);
                             geometryContext.LineTo(new Point(x, (PositiveSamples[j]) * -(med - med / 100 * 20) + med), true, true);
                        
                             //if (isNeg)
                             //{
                             //    pcn.Add(new Point(x, (NegativeSamples[j] * 0.85) * -med + med));
                             //    geometryContext.LineTo(new Point(x, (NegativeSamples[j] * 0.85) * -med + med), true, true);
                             //    isNeg = Math.Abs(NegativeSamples[j]) < epsilon;
                             //}
                             //else
                             //{
                             //    pcn.Add(new Point(x, (PositiveSamples[j] * 0.85) * -med + med));
                             //    geometryContext.LineTo(new Point(x, (PositiveSamples[j] * 0.85) * -med + med), true, true);
                             //    isNeg = Math.Abs(PositiveSamples[j]) > epsilon;
                             //}
                            
                             ////pcn.Add(new Point(x - 1, (NegativeSamples[j - 1] * 0.85) * -med + (med)));
                             //pcn.Add(new Point(x, (NegativeSamples[j] * 0.85) * -med + med));
                             //pcn.Add(new Point(x, (PositiveSamples[j] * 0.85) * -med + med));
                             ////pcn.Add(new Point(x - 1, (PositiveSamples[j - 1] * 0.85) * -med + (med)));

                         }

                        // drawingContext.DrawGeometry(Brushes.LightGreen, new Pen(Brushes.White, 1), streamGeometry);
                        // geometryContext.BeginFigure(new Point(0, med), false, false);
                        // geometryContext.PolyLineTo(pcn, true, false);
                        
                         drawingContext.DrawGeometry(
                             new SolidColorBrush(Color.FromArgb(255, 215, 215, 215)),
                             new Pen(new SolidColorBrush(Color.FromArgb(255, 0, 0, 90)), 1),
                             streamGeometry);

                         //var image = BitmapToImageSource(GetSignalImage(PositiveSamples.ToArray(), (int)ActualWidth, (int)ActualHeight));

                         //ImageVisual.Source = (ImageSource)image;

                     }
 */


                drawingContext.Close();

                var bmp = GetImage(drawingVisual);
                ImageVisual.Source = (ImageSource)bmp;




                //var tmpfile = System.IO.Path.GetTempFileName();
                //using (var pngFile = System.IO.File.OpenWrite(tmpfile))
                //    SaveAsPng(bmp, pngFile);

                //var bitmapImage = new BitmapImage();
                //var bitmapEncoder = new PngBitmapEncoder();
                //bitmapEncoder.Frames.Add(BitmapFrame.Create(bmp));
                //using (var stream = new MemoryStream())
                //{
                //    bitmapEncoder.Save(stream);
                //    stream.Seek(0, SeekOrigin.Begin);

                //    bitmapImage.BeginInit();
                //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                //    bitmapImage.StreamSource = stream;
                //    bitmapImage.EndInit();
                //}
            }


        }

        public System.Drawing.Bitmap GetSignalImage(float[] data, int width, int height)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height);
            System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(image);

            //FillBackgroundColor(width, height, graphics, Color.Black);
            //DrawGridlines(width, height, graphics);

            int center = height / 2;
            // Draw lines
            using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.MediumSpringGreen, 1))
            {
                // Find delta X, by which the lines will be drawn
                double deltaX = (double)width / data.Length;
                double normalizeFactor = data.Max(a => Math.Abs(a)) / ((double)height / 2);
                for (int i = 0, n = data.Length; i < n; i++)
                {
                    graphics.DrawLine(
                        pen,
                        (float)(i * deltaX),
                        center,
                        (float)(i * deltaX),
                        (float)(center - (data[i] / normalizeFactor)));
                }
            }

            // Draw center line
            using (System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.DarkGreen, 1))
            {
                graphics.DrawLine(pen, 0, center, width, center);
            }


            return image;
        }

        public RenderTargetBitmap GetImage(DrawingVisual view)
        {
            PresentationSource source = PresentationSource.FromVisual(view);

            double dpiX = 96.0, dpiY = 96.0;
            if (source != null)
            {
                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }

            RenderTargetBitmap result = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, dpiX, dpiY, PixelFormats.Default);

            //DrawingVisual drawingvisual = new DrawingVisual();
            //using (DrawingContext context = drawingvisual.RenderOpen())
            //{
            //    context.DrawRectangle(new VisualBrush(view), null, new Rect(new Point(), new Size(ActualWidth, ActualHeight)));
            //    context.Close();
            //}

            result.Render(view);
            return result;
        }

        public static void SaveAsPng(RenderTargetBitmap src, System.IO.Stream outputStream)
        {
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(src));

            encoder.Save(outputStream);
        }

        BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        /// <summary>
        /// OnRender
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            //maxSamples = (int)this.ActualWidth;
            base.OnRender(drawingContext);
            return;


            var height = ActualHeight;
            var med = height / 2;
            StreamGeometry streamGeometry = new StreamGeometry();
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            drawingContext.DrawLine(
                new Pen(new SolidColorBrush(Color.FromArgb(255, 192, 192, 192)), 1),
                new Point(0, med - med / 2),
                new Point(ActualWidth, med - med / 2));

            drawingContext.DrawLine(
               new Pen(new SolidColorBrush(Color.FromArgb(255, 192, 192, 192)), 1),
               new Point(0, med + med / 2),
               new Point(ActualWidth, med + med / 2));

            drawingContext.DrawLine(
                new Pen(new SolidColorBrush(Color.FromArgb(255, 0, 0, 235)), 1),
                new Point(0, med),
                new Point(ActualWidth, med));



            if (NegativeSamples.Count > 0)
                using (StreamGeometryContext geometryContext = streamGeometry.Open())
                {
                    PointCollection pc = new PointCollection(), pcn = new PointCollection(), pcp = new PointCollection();
                    var points = new Point[(int)this.ActualWidth];

                    for (int x = 1; x < ActualWidth; x++)
                    {

                        //pcn.Add(new Point(x-1, NegativeSamples[x-1] * med + med));
                        //pcn.Add(new Point(x, NegativeSamples[x] * med + med));

                        //pcn.Add(new Point(x-1, PositiveSamples[x-1] * med + (med)));
                        //pcn.Add(new Point(x, PositiveSamples[x] * med + (med)));

                        pcn.Add(new Point(x - 1, NegativeSamples[x - 1] * med + (med)));
                        pcn.Add(new Point(x, NegativeSamples[x] * med + med));
                        pcn.Add(new Point(x - 1, PositiveSamples[x - 1] * med + (med)));
                        pcn.Add(new Point(x, PositiveSamples[x] * med + med));








                        //
                        //if (x + 1 < ActualWidth)


                        // geometryContext.BeginFigure(new Point(x - 1, NegativeSamples[x - 1] * med + med), false, false);
                        // geometryContext.LineTo(new Point(x - 1, NegativeSamples[x - 1] * med + med), false, false);
                        //geometryContext.LineTo(new Point(x, PositiveSamples[x] * med + med), false, false);



                        //drawingContext.DrawLine(
                        //    foregroundPen,
                        //    new Point(x - 1, NegativeSamples[x - 1] *   med+med),
                        //    new Point(x, PositiveSamples[x] * med + med));



                        //drawingContext.DrawLine(
                        //    foregroundPen, 
                        //    new Point(x - 1, NegativeSamples[x - 1] * ActualHeight + med), 
                        //    new Point(x, NegativeSamples[x] * ActualHeight + med));


                    }
                    // drawingContext.DrawGeometry(Brushes.LightGreen, new Pen(Brushes.White, 1), streamGeometry);
                    geometryContext.BeginFigure(new Point(0, med), false, false);
                    geometryContext.PolyLineTo(pcn, true, false);

                    drawingContext.DrawGeometry(
                        new SolidColorBrush(Color.FromArgb(255, 215, 215, 215)),
                        new Pen(new SolidColorBrush(Color.FromArgb(255, 0, 0, 90)), 1),
                        streamGeometry);

                    pcn.Clear();

                    DrawingVisual drawingVisual = new DrawingVisual();
                    using (DrawingContext dc = drawingVisual.RenderOpen())
                    {
                        //
                        // ... draw on the drawingContext
                        //
                        dc.DrawGeometry(Brushes.LightGreen, new Pen(Brushes.White, 2), streamGeometry);

                        RenderTargetBitmap bmp = new RenderTargetBitmap((int)ActualWidth, (int)ActualHeight, 72, 72, PixelFormats.Default);
                        bmp.Render(drawingVisual);
                        ImageVisual.Source = bmp;
                    }

                    //return;
                    //for (int x = 0; x < this.ActualWidth; x++)
                    //{
                    //    //var y = SmoothStep(NegativeSamples[x], -1, 0, 0, med);
                    //    var y = NegativeSamples[x] * height + med;
                    //    pcn.Add(new Point(x, y));
                    //    //var y2 = SmoothStep(PositiveSamples[x], 0, 1, 0, ActualHeight);
                    //    var y2 = PositiveSamples[x] * height + med;

                    //    pcp.Add(new Point(x, y2));
                    //}

                    //geometryContext.BeginFigure(new Point(0, med + 1), true, false);
                    //geometryContext.PolyLineTo(pcn, false, false);

                    //geometryContext.BeginFigure(new Point(0, med + 1), true, false);
                    //geometryContext.PolyLineTo(pcp, false, false);

                }


            //drawingContext.DrawGeometry(Brushes.LightGreen, new Pen(Brushes.White, 2), streamGeometry);



            /*return;

            float min = 0.0f, max = 0.0f; ;
            if (samples.Count > 0)
            {
                min = samples.Min();
                max = samples.Max();
            }

            for (int x = 0; x < this.ActualWidth; x++)
            {
                //if (x % 2 == 0)
                //{
                //    drawingContext.DrawLine(foregroundPen, new Point(x, 0), new Point(x, this.ActualHeight));
                //}
                //else
                //{
                //    drawingContext.DrawLine(new Pen(Brushes.Bisque, 1), new Point(x, 0), new Point(x, this.ActualHeight));
                //}
                if (samples.Count == 0)
                    return;

                var amp = samples[x] / max;

                //if (x % 2 == 0)
                //{
                //    amp = 1;

                //    foregroundPen = new Pen(Brushes.Bisque, 1);
                //}
                //else
                //{
                //    amp = 0.5f;
                //    foregroundPen = new Pen(Brushes.Black, 1);
                //}


                var height1 = this.ActualHeight * amp;
                var outer = this.ActualHeight - height1;

                drawingContext.DrawLine(foregroundPen, new Point(x, outer / 2), new Point(x, outer / 2 + height));


            }

            return;
            for (int x = 0; x < this.ActualWidth; x++)
            {
                var index = (int)(x - this.ActualWidth + insertPos);
                if (index < 0)
                    index += maxSamples;
                float lineHeight = (float)(this.ActualHeight * GetSample((int)(x - this.ActualWidth + insertPos)));
                float y1 = (float)(this.ActualHeight - lineHeight) / 2.0f;

                drawingContext.DrawLine(foregroundPen, new Point(x, y1), new Point(x, y1 + lineHeight));
            }*/
        }
        /// <summary>
        /// Interpolates smoothly from c1 to c2 based on x compared to a1 and a2. 
        /// </summary>
        /// <param name="x">value</param>
        /// <param name="a1">min</param>
        /// <param name="a2">max</param>
        /// <param name="c1">from</param>
        /// <param name="c2">to</param>
        /// <returns></returns>
        public static double SmoothStep(double x, double a1, double a2, double c1, double c2)
        {
            return c1 + ((x - a1) / (a2 - a1)) * (c2 - c1) / 1.0f;
        }
        // Cosine interpolation 
        // http://paulbourke.net/miscellaneous/interpolation/
        public static double Cerp(double y1, double y2, double mu)
        {
            double mu2;
            mu2 = (1f - (double)Math.Cos(mu * Math.PI)) / 2f;
            return (y1 * (1f - mu2) + y2 * mu2);
        }

        public static double Lerp(double a, double b, double t)
        {
            return a + (b - a) * t;
        }

        float GetSample(int index)
        {
            if (index < 0)
                index += maxSamples;
            if (index >= 0 & index < samples.Count)
                return samples[index];
            return 0;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InvalidateVisual();
        }

    }
}
