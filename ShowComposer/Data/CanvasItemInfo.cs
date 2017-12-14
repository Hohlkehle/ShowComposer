using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace ShowComposer.Data
{
    [Serializable]
    public class CanvasItemInfo
    {
        [XmlAttribute]
        public double Left { set; get; }
        [XmlAttribute]
        public double Top { set; get; }

        public CanvasItemInfo()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left">Left position in pixels</param>
        /// <param name="top">Top position in pixels</param>
        public CanvasItemInfo(double left, double top)
        {
            Left = left;
            Top = top;
            //Left = Math.Ceiling(left / MainWindow.DpiX * 2.54d * 100d);
            //Top = Math.Ceiling(top / MainWindow.DpiY * 2.54d * 100d);
            //Left = (left / MainWindow.DpiX * 2.54d * 100d);
            //Top = (top / MainWindow.DpiY * 2.54d * 100d);
        }

        /// <summary>
        /// Get location in pixels
        /// </summary>
        /// <returns></returns>
        public Point Location()
        {
            //double Xpixel = Left * MainWindow.DpiX / 2.54d / 100d;
            //double Ypixel = Top * MainWindow.DpiY / 2.54d / 100d;

            return new Point(Left, Top);
        }

        public static IEnumerable<CanvasItemInfo> Convert(UIElement[] elements, Point mediaOffset)
        {
            var en = new List<CanvasItemInfo>();
            foreach (var tl in elements)
            {
                var c = new CanvasItemInfo(Canvas.GetLeft(tl), Canvas.GetTop(tl));
                en.Add(c);
            }
            return en;
        }
    }
}
