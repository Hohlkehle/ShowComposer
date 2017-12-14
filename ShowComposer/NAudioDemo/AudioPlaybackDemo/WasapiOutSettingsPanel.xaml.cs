using NAudio.CoreAudioApi;
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
    /// Interaction logic for WasapiOutSettingsPanel.xaml
    /// </summary>
    public partial class WasapiOutSettingsPanel : UserControl
    {
        public WasapiOutSettingsPanel()
        {
            InitializeComponent();
            InitialiseWasapiControls();
        }


        class WasapiDeviceComboItem
        {
            public string Description { get; set; }
            public MMDevice Device { get; set; }
        }

        private void InitialiseWasapiControls()
        {
            var enumerator = new MMDeviceEnumerator();
            var endPoints = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            var comboItems = new List<WasapiDeviceComboItem>();
            foreach (var endPoint in endPoints)
            {
                var comboItem = new WasapiDeviceComboItem();
                comboItem.Description = string.Format("{0} ({1})", endPoint.FriendlyName, endPoint.DeviceFriendlyName);
                comboItem.Device = endPoint;
                comboItems.Add(comboItem);
            }
            comboBoxWaspai.DisplayMemberPath = "Description";
            comboBoxWaspai.SelectedValuePath = "Device";
            comboBoxWaspai.ItemsSource = comboItems;
            comboBoxWaspai.SelectedIndex = Preferences.WasapiOutDevice.Value;

            checkBoxWasapiEventCallback.IsChecked = bool.Parse(Preferences.WasapiOutIsEventCallback);
            checkBoxWasapiExclusiveMode.IsChecked = bool.Parse(Preferences.WasapiOutExclusiveMode);
        }

        private void comboBoxWaspai_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Preferences.WasapiOutDevice.Value = comboBoxWaspai.SelectedIndex;
        }

        public MMDevice SelectedDevice { get { return (MMDevice)comboBoxWaspai.SelectedValue; } }

        public AudioClientShareMode ShareMode
        {
            get
            {
                return (bool)checkBoxWasapiExclusiveMode.IsChecked ?
                    AudioClientShareMode.Exclusive :
                    AudioClientShareMode.Shared;
            }
        }

        public bool UseEventCallback { get { return (bool)checkBoxWasapiEventCallback.IsChecked; } }

        private void checkBoxWasapiEventCallback_Unchecked(object sender, RoutedEventArgs e)
        {
            Preferences.WasapiOutIsEventCallback.Value = checkBoxWasapiEventCallback.IsChecked.ToString();
            Preferences.WasapiOutExclusiveMode.Value = checkBoxWasapiExclusiveMode.IsChecked.ToString();
        }

        private void checkBoxWasapiExclusiveMode_Checked(object sender, RoutedEventArgs e)
        {
            Preferences.WasapiOutIsEventCallback.Value = checkBoxWasapiEventCallback.IsChecked.ToString();
            Preferences.WasapiOutExclusiveMode.Value = checkBoxWasapiExclusiveMode.IsChecked.ToString();
        }
    }
}
