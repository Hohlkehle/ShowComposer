using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowComposer.Data
{
    public class Preferences
    {
        public class Property
        {
            protected string m_Key = "key1";
            protected string m_Section = "section1";
            public string Key
            {
                get { return m_Key; }
                set { m_Key = value; }
            }

            public string Section
            {
                get { return m_Section; }
                set { m_Section = value; }
            }

            public Property(string section, string key)
            {
                Key = key;
                Section = section;
            }
        }

        public class IntProperty : Property
        {
            private int m_DefaultValue = 0;
            private int m_Value = 0;

            public int DefaultValue { get { return m_DefaultValue; } }

            public int Value
            {
                get { return m_Value; }
                set { m_Value = value; }
            }

            public IntProperty(string section, string key, int defaultValue)
                : base(section, key)
            {
                m_DefaultValue = defaultValue;
            }

            public static implicit operator int(IntProperty p)
            {
                return p.Value;
            }
        }

        public class StringProperty : Property
        {
            private String m_DefaultValue = "";
            private String m_Value = "";

            public String DefaultValue { get { return m_DefaultValue; } }

            public String Value
            {
                get { return m_Value; }
                set { m_Value = value; }
            }

            public StringProperty(string section, string key, string defaultValue)
                : base(section, key)
            {
                m_DefaultValue = defaultValue;
            }

            public static implicit operator string(StringProperty p)
            {
                return p.Value;
            }
        }

        public static IntProperty RequestedLatency { set; get; }
        public static StringProperty OutputDevice { set; get; }
        public static StringProperty WaveOutCallback { set; get; }
        public static IntProperty WaveOutDevice { set; get; }
        public static StringProperty DirectSoundDevice { set; get; }
        public static IntProperty WasapiOutDevice { set; get; }
        public static StringProperty WasapiOutIsEventCallback { set; get; }
        public static StringProperty WasapiOutExclusiveMode { set; get; }
        public static IntProperty VideoPlaybackDevice { set; get; }

        Utilities.IniFile m_iniFile;

        public Preferences(Utilities.IniFile iniFile)
        {
            m_iniFile = iniFile;

            RequestedLatency = new IntProperty("Audio", "RequestedLatency", 300);
            OutputDevice = new StringProperty("Audio", "OutputDevice", "DirectSound");

            WaveOutCallback = new StringProperty("Audio", "WaveOutCallback", "Window");
            WaveOutDevice = new IntProperty("Audio", "WaveOutDevice", 0);

            DirectSoundDevice = new StringProperty("Audio", "DirectSoundDevice", "00000000-0000-0000-0000-000000000000");

            WasapiOutDevice = new IntProperty("Audio", "WasapiOutDevice", 0);
            WasapiOutIsEventCallback = new StringProperty("Audio", "WasapiOutIsEventCallback", "False");
            WasapiOutExclusiveMode = new StringProperty("Audio", "WasapiOutExclusiveMode", "False");

            VideoPlaybackDevice = new IntProperty("Video", "VideoPlaybackDevice", 1);
            Load();
        }

        public void Load()
        {
            RequestedLatency.Value = m_iniFile.GetInt32(RequestedLatency.Section, RequestedLatency.Key, RequestedLatency.DefaultValue);
            OutputDevice.Value = m_iniFile.GetString(OutputDevice.Section, OutputDevice.Key, OutputDevice.DefaultValue);

            WaveOutCallback.Value = m_iniFile.GetString(WaveOutCallback.Section, WaveOutCallback.Key, WaveOutCallback.DefaultValue);
            WaveOutDevice.Value = m_iniFile.GetInt32(WaveOutDevice.Section, WaveOutDevice.Key, WaveOutDevice.DefaultValue);

            DirectSoundDevice.Value = m_iniFile.GetString(DirectSoundDevice.Section, DirectSoundDevice.Key, DirectSoundDevice.DefaultValue);

            WasapiOutDevice.Value = m_iniFile.GetInt32(WasapiOutDevice.Section, WasapiOutDevice.Key, WasapiOutDevice.DefaultValue);
            WasapiOutIsEventCallback.Value = m_iniFile.GetString(WasapiOutIsEventCallback.Section, WasapiOutIsEventCallback.Key, WasapiOutIsEventCallback.DefaultValue);
            WasapiOutExclusiveMode.Value = m_iniFile.GetString(WasapiOutExclusiveMode.Section, WasapiOutExclusiveMode.Key, WasapiOutExclusiveMode.DefaultValue);

            VideoPlaybackDevice.Value = m_iniFile.GetInt32(VideoPlaybackDevice.Section, VideoPlaybackDevice.Key, VideoPlaybackDevice.DefaultValue);
        }

        public void Save()
        {
            m_iniFile.WriteValue(RequestedLatency.Section, RequestedLatency.Key, RequestedLatency.Value);
            m_iniFile.WriteValue(OutputDevice.Section, OutputDevice.Key, OutputDevice.Value);

            m_iniFile.WriteValue(WaveOutCallback.Section, WaveOutCallback.Key, WaveOutCallback.Value);
            m_iniFile.WriteValue(WaveOutDevice.Section, WaveOutDevice.Key, WaveOutDevice.Value);

            m_iniFile.WriteValue(DirectSoundDevice.Section, DirectSoundDevice.Key, DirectSoundDevice.Value);

            m_iniFile.WriteValue(WasapiOutDevice.Section, WasapiOutDevice.Key, WasapiOutDevice.Value);
            m_iniFile.WriteValue(WasapiOutIsEventCallback.Section, WasapiOutIsEventCallback.Key, WasapiOutIsEventCallback.Value);
            m_iniFile.WriteValue(WasapiOutExclusiveMode.Section, WasapiOutExclusiveMode.Key, WasapiOutExclusiveMode.Value);

            m_iniFile.WriteValue(VideoPlaybackDevice.Section, VideoPlaybackDevice.Key, VideoPlaybackDevice.Value);
        }
    }
}
