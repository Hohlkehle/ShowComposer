using NAudio.Wave;
using ShowComposer.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShowComposer.NAudioDemo.AudioPlaybackDemo
{
    /// <summary>
    /// Interaction logic for WaveOutSettingsPanel.xaml
    /// </summary>
    public partial class WaveOutSettingsPanel : UserControl
    {
        public WaveOutSettingsPanel()
        {
            InitializeComponent();

            InitialiseDeviceCombo();
            InitialiseStrategyCombo();
        }

        class CallbackComboItem
        {
            public CallbackComboItem(string text, WaveCallbackStrategy strategy)
            {
                this.Text = text;
                this.Strategy = strategy;
            }
            public string Text { get; private set; }
            public WaveCallbackStrategy Strategy { get; private set; }
        }

        private void InitialiseDeviceCombo()
        {
            for (int deviceId = 0; deviceId < WaveOut.DeviceCount; deviceId++)
            {
                var capabilities = WaveOut.GetCapabilities(deviceId);
                comboBoxWaveOutDevice.Items.Add(String.Format("Device {0} ({1})", deviceId, capabilities.ProductName));

                if (deviceId == Preferences.WaveOutDevice.Value)
                    comboBoxWaveOutDevice.SelectedIndex = deviceId;
            }

            if (comboBoxWaveOutDevice.SelectedItem == null)
                comboBoxWaveOutDevice.SelectedIndex = 0;
        }

        private void comboBoxWaveOutDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Preferences.WaveOutDevice.Value = SelectedDeviceNumber;
        }

        private void InitialiseStrategyCombo()
        {
            comboBoxCallback.DisplayMemberPath = "Text";
            comboBoxCallback.SelectedValuePath = "Strategy";
            comboBoxCallback.Items.Add(new CallbackComboItem("Window", WaveCallbackStrategy.NewWindow));
            comboBoxCallback.Items.Add(new CallbackComboItem("Function", WaveCallbackStrategy.FunctionCallback));
            comboBoxCallback.Items.Add(new CallbackComboItem("Event", WaveCallbackStrategy.Event));

            for (var i = 0; i < comboBoxCallback.Items.Count; i++)
                if (Preferences.WaveOutCallback.Value == ((CallbackComboItem)comboBoxCallback.Items[i]).Text)
                    comboBoxCallback.SelectedIndex = i;
        }

        private void comboBoxCallback_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Preferences.WaveOutCallback.Value = ((CallbackComboItem)comboBoxCallback.SelectedItem).Text;
        }

        public int SelectedDeviceNumber { get { return comboBoxWaveOutDevice.SelectedIndex; } }

        public WaveCallbackStrategy CallbackStrategy { get { return ((CallbackComboItem)comboBoxCallback.SelectedItem).Strategy; } }


      
    }
}
