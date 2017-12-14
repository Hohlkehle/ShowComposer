using NAudio.CoreAudioApi;
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
    public class WasapiOutPlugin : IOutputDevicePlugin
    {
        WasapiOutSettingsPanel settingsPanel;
        MMDevice SelectedDevice;
        AudioClientShareMode ShareMode;
        bool UseEventCallback;
        int Latency;

        public WasapiOutPlugin()
        {

        }

        public WasapiOutPlugin(MMDevice selectedDevice, AudioClientShareMode shareMode, bool eventCallback, int latency)
        {
            SelectedDevice = selectedDevice;
            ShareMode = shareMode;
            UseEventCallback = eventCallback;
            Latency = latency;
        }

        public IWavePlayer CreateDevice(int latency)
        {
            var wasapi = new WasapiOut(
                SelectedDevice,
                ShareMode,
                UseEventCallback,
                latency);
            return wasapi;
            //var wasapi = new WasapiOut(
            //    settingsPanel.SelectedDevice,
            //    settingsPanel.ShareMode,
            //    settingsPanel.UseEventCallback,
            //    latency);
            //return wasapi;
            //return null;
        }

        public UserControl CreateSettingsPanel()
        {
            this.settingsPanel = new WasapiOutSettingsPanel();
            return settingsPanel;
            //return null;
        }

        public string Name
        {
            get { return "WasapiOut"; }
        }

        public bool IsAvailable
        {
            // supported on Vista and above
            get { return Environment.OSVersion.Version.Major >= 6; }
        }

        public int Priority
        {
            get { return 3; }
        }
    }
}
