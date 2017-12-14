using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ShowComposer.NAudioDemo.AudioPlaybackDemo
{
    [Export(typeof(IOutputDevicePlugin))]
    class AsioOutPlugin : IOutputDevicePlugin
    {
        //AsioOutSettingsPanel settingsPanel;

        public IWavePlayer CreateDevice(int latency)
        {
            //return new AsioOut(settingsPanel.SelectedDeviceName);
            return null;
        }

        public UserControl CreateSettingsPanel()
        {
            //this.settingsPanel = new AsioOutSettingsPanel();
            //return settingsPanel;
            return null;
        }

        public string Name
        {
            get { return "AsioOut"; }
        }

        public bool IsAvailable
        {
            get { return AsioOut.isSupported(); }
        }

        public int Priority
        {
            get { return 4; }
        }
    }
}
