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
    class WaveOutPlugin : IOutputDevicePlugin
    {
        private WaveOutSettingsPanel waveOutSettingsPanel;
        public int SelectedDeviceNumber;
        public WaveCallbackStrategy CallbackStrategy;

        public WaveOutPlugin()
        {

        }

        public WaveOutPlugin(WaveCallbackStrategy strategy, int selectedDevice)
        {
            SelectedDeviceNumber = selectedDevice;
            CallbackStrategy = strategy;
        }

        public IWavePlayer CreateDevice(int latency)
        {
            IWavePlayer device;
            WaveCallbackStrategy strategy = CallbackStrategy;// waveOutSettingsPanel.CallbackStrategy;
            if (strategy == WaveCallbackStrategy.Event)
            {
                var waveOut = new WaveOutEvent();
                waveOut.DeviceNumber = SelectedDeviceNumber;// waveOutSettingsPanel.SelectedDeviceNumber;
                waveOut.DesiredLatency = latency;
                device = waveOut;
            }
            else
            {
                WaveCallbackInfo callbackInfo = strategy == WaveCallbackStrategy.NewWindow ? WaveCallbackInfo.NewWindow() : WaveCallbackInfo.FunctionCallback();
                WaveOut outputDevice = new WaveOut(callbackInfo);
                outputDevice.DeviceNumber = SelectedDeviceNumber;// waveOutSettingsPanel.SelectedDeviceNumber;
                outputDevice.DesiredLatency = latency;
                device = outputDevice;
            }
            // TODO: configurable number of buffers

            return device;
        }

        public UserControl CreateSettingsPanel()
        {
            this.waveOutSettingsPanel = new WaveOutSettingsPanel();
            return waveOutSettingsPanel;
        }

        public string Name
        {
            get { return "WaveOut"; }
        }

        public bool IsAvailable
        {
            get { return WaveOut.DeviceCount > 0; }
        }

        public int Priority
        {
            get { return 1; }
        }
    }
}
