using ShowComposer.UserControls;
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
    public class PowerPointInfo:CanvasItemInfo
    { private double m_Width = 100;

        [XmlAttribute]
        public double Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        [XmlAttribute]
        public string PresenterFile { set; get; }



        [XmlAttribute]
        public bool IsRelativePath { set; get; }

        public PowerPointInfo()
        { }

        /// <param name="left">Left position in millimeters</param>
        /// <param name="top">Top position in millimeters</param>
        public PowerPointInfo(double left, double top, string file)
            : base(left, top)
        {
            PresenterFile = file;
        }

        public static IEnumerable<PowerPointInfo> Convert(UIElement[] elements)
        {
            List<PowerPointInfo> cic = new List<PowerPointInfo>();
            var textBlock = (PowerPointControl[])elements;
            foreach (var tb in textBlock)
            {
                var c = new PowerPointInfo(Canvas.GetLeft(tb), Canvas.GetTop(tb), tb.PresenterFile);
                c.Width = tb.ActualWidth;

                cic.Add(c);
            }
            return cic;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(PresenterFile))
                return base.ToString();
            return PresenterFile;
        }
    }
}
