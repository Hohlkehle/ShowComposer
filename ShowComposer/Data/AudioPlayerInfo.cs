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
    public class AudioPlayerInfo : CanvasItemInfo
    {
        private double m_Width = 100;

        [XmlAttribute]
        public double Width
        {
            get { return m_Width; }
            set { m_Width = value; }
        }

        [XmlAttribute]
        public string AudioFile { set; get; }

        [XmlAttribute]
        public double AudioVolume { set; get; }

        [XmlAttribute]
        public bool IsExclusivePlayback { set; get; }

        [XmlAttribute]
        public bool IsRelativePath { set; get; }

        public AudioPlayerInfo()
        {
            AudioVolume = 1;
        }

        /// <param name="left">Left position in millimeters</param>
        /// <param name="top">Top position in millimeters</param>
        public AudioPlayerInfo(double left, double top, string file, double volume)
            : base(left, top)
        {
            AudioFile = file;
            AudioVolume = volume;
        }

        public static IEnumerable<AudioPlayerInfo> Convert(UIElement[] elements)
        {
            List<AudioPlayerInfo> cic = new List<AudioPlayerInfo>();
            var textBlock = (AudioPlaybackControl[])elements;
            foreach (var tb in textBlock)
            {
                var c = new AudioPlayerInfo(Canvas.GetLeft(tb), Canvas.GetTop(tb), tb.AudioFile, tb.SoundVolume);
                c.Width = tb.ActualWidth;
                c.IsExclusivePlayback = tb.IsExclusivePlayback;
                cic.Add(c);
            }
            return cic;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(AudioFile))
                return base.ToString();
            return AudioFile;
        }
    }
}
