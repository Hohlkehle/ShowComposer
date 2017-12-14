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
    public class VideoPlayerInfo : CanvasItemInfo
    {
        private double m_Width = 100;

        [XmlAttribute]
        public double Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        [XmlAttribute]
        public string VideoFile { set; get; }

        [XmlAttribute]
        public double AudioVolume { set; get; }

        [XmlAttribute]
        public bool IsRelativePath { set; get; }

        public VideoPlayerInfo()
        {
            AudioVolume = 1;
        }

        /// <param name="left">Left position in millimeters</param>
        /// <param name="top">Top position in millimeters</param>
        public VideoPlayerInfo(double left, double top, string file, double volume)
            : base(left, top)
        {
            VideoFile = file;
            AudioVolume = volume;
        }

        public static IEnumerable<VideoPlayerInfo> Convert(UIElement[] elements)
        {
            List<VideoPlayerInfo> cic = new List<VideoPlayerInfo>();
            var textBlock = (VideoPlaybackControl[])elements;
            foreach (var tb in textBlock)
            {
                var c = new VideoPlayerInfo(Canvas.GetLeft(tb), Canvas.GetTop(tb), tb.VideoFile, tb.SoundVolume);
                c.Width = tb.ActualWidth;

                cic.Add(c);
            }
            return cic;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(VideoFile))
                return base.ToString();
            return VideoFile;
        }
    }
}
