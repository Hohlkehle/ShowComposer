using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuirkSoft
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
        }


        public IntProperty Sensitivity { set; get; }
        public IntProperty Axis { set; get; }  
        public IntProperty Modifier { set; get; }

        Utilities.IniFile m_iniFile;

        public Preferences(Utilities.IniFile iniFile)
        {
            m_iniFile = iniFile;

            Sensitivity = new IntProperty("global", "Sensitivity", 50);
            Axis = new IntProperty("global", "Axis", 0);
            Modifier = new IntProperty("global", "Modifier", 131072);

            Load();
        }

        public void Load()
        {
            Sensitivity.Value = m_iniFile.GetInt32(Sensitivity.Section, Sensitivity.Key, Sensitivity.DefaultValue);
            Axis.Value = m_iniFile.GetInt32(Axis.Section, Axis.Key, Axis.DefaultValue);
            Modifier.Value = m_iniFile.GetInt32(Modifier.Section, Modifier.Key, Modifier.DefaultValue);
        }

        internal void Save()
        {
            m_iniFile.WriteValue(Sensitivity.Section, Sensitivity.Key, Sensitivity.Value);
            m_iniFile.WriteValue(Axis.Section, Axis.Key, Axis.Value);
            m_iniFile.WriteValue(Modifier.Section, Modifier.Key, Modifier.Value);
        }
    }
}
