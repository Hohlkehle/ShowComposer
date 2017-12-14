using ShowComposer.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using IOFile = System.IO.File;

namespace ShowComposer
{
    [Serializable]
    public class LayoutProperties
    {
        public List<AudioPlayerInfo> m_AudioPlayerInfo;
        public List<VideoPlayerInfo> m_VideoPlayerInfo;
        public List<PowerPointInfo> m_PowerPointInfo;

        [XmlAttribute]
        public string Name { set; get; }
        [XmlAttribute]
        public string WordDocument { set; get; }
         [XmlAttribute]
        public bool IsRelativePath { set; get; }
        public Point BackgroundOffset { set; get; }
        public Point Size { set; get; }


        public LayoutProperties() { }

        internal void Check()
        {
            //m_CaptionInfo = m_CaptionInfo.Where((e) => { if (e.CaptionText == "") e.CaptionText = "corrupted"; return true; }).ToList<CaptionInfo>();

            //foreach (var e in m_CaptionInfo)
            //{
            //    if (e.CaptionText == "")
            //        e.CaptionText = "corrupted";
            //}

            //m_CaptionInfo.RemoveAll((e) => e.CaptionText == "");
        }

        public void Save(string filename)
        {
            IOFile.Delete(filename);

            using (Stream writer = new FileStream(filename, FileMode.OpenOrCreate))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(LayoutProperties));
                    serializer.Serialize(writer, this);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
