using ShowComposer.Core;
using ShowComposer.Data;
using ShowComposer.NAudioDemo.AudioPlaybackDemo;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using System.Windows.Shapes;

namespace ShowComposer.Windows
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>

    [Export]
    public partial class OptionsWindow : Window
    {
        [ImportingConstructor]
        public OptionsWindow([ImportMany]IEnumerable<IOutputDevicePlugin> outputDevicePlugins)
        {
            InitializeComponent();
            LoadVideoDevices();
            LoadOutputDevicePlugins(outputDevicePlugins);
        }

        private void LoadOutputDevicePlugins(IEnumerable<IOutputDevicePlugin> outputDevicePlugins)
        {
            comboBoxOutputDevice.DisplayMemberPath = "Name";
            //comboBoxOutputDevice.SelectionChanged +=comboBoxOutputDevice_SelectionChanged;
            foreach (var outputDevicePlugin in outputDevicePlugins.OrderBy(p => p.Priority))
            {
                comboBoxOutputDevice.Items.Add(outputDevicePlugin);

                if (outputDevicePlugin.Name == Preferences.OutputDevice.Value)
                    comboBoxOutputDevice.SelectedItem = outputDevicePlugin;
            }

            if (comboBoxOutputDevice.SelectedItem == null)
                comboBoxOutputDevice.SelectedIndex = 0;
        }

        private void LoadVideoDevices()
        {
            comboBoxVideoDevice.DisplayMemberPath = "Value";
            comboBoxVideoDevice.SelectedValuePath = "Key";
            comboBoxVideoDevice.Items.Add(new KeyValuePair<int, string>((int)VideoPlaybackOptions.EmbededPlayer, "Embeded Player"));
            comboBoxVideoDevice.Items.Add(new KeyValuePair<int, string>((int)VideoPlaybackOptions.InternalVLCRenderer, "Internal VLC renderer"));
            comboBoxVideoDevice.Items.Add(new KeyValuePair<int, string>((int)VideoPlaybackOptions.BuildinVLCPlayer, "Buildin VLC Player 2.2.6"));
            comboBoxVideoDevice.Items.Add(new KeyValuePair<int, string>((int)VideoPlaybackOptions.InstalledVLCPlayer, "Installed VLC Player"));
            comboBoxVideoDevice.Items.Add(new KeyValuePair<int, string>((int)VideoPlaybackOptions.WindowMediaPlayer, "Window Media Player"));

            comboBoxVideoDevice.SelectedIndex = Preferences.VideoPlaybackDevice.Value;
        }

        private IOutputDevicePlugin SelectedOutputDevicePlugin
        {
            get { return (IOutputDevicePlugin)comboBoxOutputDevice.SelectedItem; }
        }

        private void comboBoxOutputDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            panelOutputDeviceSettings.Children.Clear();

            UIElement settingsPanel;
            if (SelectedOutputDevicePlugin.IsAvailable)
            {
                settingsPanel = SelectedOutputDevicePlugin.CreateSettingsPanel();
                Preferences.OutputDevice.Value = SelectedOutputDevicePlugin.Name;
            }
            else
            {
                settingsPanel = new Label() { Content = "This output device is unavailable on your system" };
            }
            panelOutputDeviceSettings.Children.Add(settingsPanel);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int[] lt = new int[] { 25, 50, 100, 150, 200, 300, 400, 500 };


            for (var i = 0; i < lt.Length; i++)
            {
                comboBoxLatency.Items.Add(lt[i]);

                if (lt[i] == Preferences.RequestedLatency.Value)
                    comboBoxLatency.SelectedIndex = i;
            }

            if (comboBoxLatency.SelectedItem == null)
                comboBoxLatency.SelectedIndex = 0;

            comboBoxVideoDevice.SelectedIndex = Preferences.VideoPlaybackDevice.Value;
        }

        private void comboBoxLatency_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Preferences.RequestedLatency.Value = (int)comboBoxLatency.SelectedItem;
        }

        private void comboBoxVideoDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("Not Implemented!", "Info", MessageBoxButton.OK);
            //Preferences.VideoPlaybackDevice.Value = ((KeyValuePair<int, string>)comboBoxVideoDevice.SelectedItem).Key;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Preferences.VideoPlaybackDevice.Value = ((KeyValuePair<int, string>)comboBoxVideoDevice.SelectedItem).Key;
        }
    }
}
