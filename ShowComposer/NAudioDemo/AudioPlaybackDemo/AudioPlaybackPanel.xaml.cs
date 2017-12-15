using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;

namespace ShowComposer.NAudioDemo.AudioPlaybackDemo
{
    /// <summary>
    /// Interaction logic for AudioPlaybackPanel.xaml
    /// </summary>
    public partial class AudioPlaybackPanel : UserControl
    {
        //private IWavePlayer waveOut;
        //private string fileName = null;
        //private AudioFileReader audioFileReader;
        //private Action<float> setVolumeDelegate;

        private IOutputDevicePlugin SelectedOutputDevicePlugin
        {
            get { return (IOutputDevicePlugin)comboBoxOutputDevice.SelectedItem; }
        }

        [ImportingConstructor]
        public AudioPlaybackPanel([ImportMany]IEnumerable<IOutputDevicePlugin> outputDevicePlugins)
        {
            InitializeComponent();
            LoadOutputDevicePlugins(outputDevicePlugins);
        }

        private void LoadOutputDevicePlugins(IEnumerable<IOutputDevicePlugin> outputDevicePlugins)
        {
            //comboBoxOutputDevice.DisplayMember = "Name";
            comboBoxOutputDevice.SelectionChanged += comboBoxOutputDevice_SelectedIndexChanged;
            foreach (var outputDevicePlugin in outputDevicePlugins.OrderBy(p => p.Priority))
            {
                comboBoxOutputDevice.Items.Add(outputDevicePlugin);
            }
            comboBoxOutputDevice.SelectedIndex = 0;
        }

        void comboBoxOutputDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            //panelOutputDeviceSettings.Controls.Clear();
            Control settingsPanel;
            if (SelectedOutputDevicePlugin.IsAvailable)
            {
                settingsPanel = SelectedOutputDevicePlugin.CreateSettingsPanel();
            }
            else
            {
                //settingsPanel = new Label() { Text = "This output device is unavailable on your system", Dock = DockStyle.Fill };
            }
            //panelOutputDeviceSettings.Controls.Add(settingsPanel);
        }
    }
}
