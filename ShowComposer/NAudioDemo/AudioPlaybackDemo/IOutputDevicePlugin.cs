using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ShowComposer.NAudioDemo.AudioPlaybackDemo
{
    public interface IOutputDevicePlugin
    {
        IWavePlayer CreateDevice(int latency);
        UserControl CreateSettingsPanel();
        string Name { get; }
        bool IsAvailable { get; }
        int Priority { get; }
    }
}
