using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using NAudio.Wave;
using ShowComposer.Data;

namespace ShowComposer.NAudioDemo.AudioPlaybackDemo
{
    [Export(typeof(IOutputDevicePlugin))]
    public class NullOutPlugin : IOutputDevicePlugin
    {
        DirectSoundOutSettingsPanel settingsPanel;
        public NAudio.Wave.IWavePlayer CreateDevice(int latency)
        {
            return new DirectSoundOut(new Guid(Preferences.DirectSoundDevice.Value), latency);
        }

        public UserControl CreateSettingsPanel()
        {
            this.settingsPanel = new DirectSoundOutSettingsPanel();
            return this.settingsPanel;
        }

        public string Name
        {
            get { return "NullOut"; }
        }

        public bool IsAvailable
        {
            get { return true; }
        }

        public int Priority
        {
            get { return 0; }
        }
    }
}
