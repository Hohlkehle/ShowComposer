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
    /// Interaction logic for DirectSoundOutSettingsPanel.xaml
    /// </summary>
    public partial class DirectSoundOutSettingsPanel : UserControl
    {
        public DirectSoundOutSettingsPanel()
        {
            InitializeComponent();
            InitialiseDirectSoundControls();
        }

        private void InitialiseDirectSoundControls()
        {
            comboBoxDirectSound.DisplayMemberPath = "Description";
            comboBoxDirectSound.SelectedValuePath = "Guid";
            comboBoxDirectSound.ItemsSource = DirectSoundOut.Devices;

            for (var i = 0; i < DirectSoundOut.Devices.Count(); i++)
            {
                if (((DirectSoundDeviceInfo)comboBoxDirectSound.Items[i]).Guid.ToString() == Preferences.DirectSoundDevice.Value)
                    comboBoxDirectSound.SelectedIndex = i;
            }
        }

        public Guid SelectedDevice
        {
            get { return (Guid)comboBoxDirectSound.SelectedValue; }
        }

        private void comboBoxDirectSound_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Preferences.DirectSoundDevice.Value = SelectedDevice.ToString();
        }
    }
}
