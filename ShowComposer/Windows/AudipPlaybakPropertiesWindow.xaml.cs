using ShowComposer.UserControls;
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
using System.Windows.Shapes;

namespace ShowComposer.Windows
{
    /// <summary>
    /// Interaction logic for AudipPlaybakPropertiesWindow.xaml
    /// </summary>
    public partial class AudipPlaybakPropertiesWindow : Window
    {
        public AudipPlaybakPropertiesWindow()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        internal void SetTarget(AudioPlaybackControl target)
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxLatency.Items.Add(25);
            comboBoxLatency.Items.Add(50);
            comboBoxLatency.Items.Add(100);
            comboBoxLatency.Items.Add(150);
            comboBoxLatency.Items.Add(200);
            comboBoxLatency.Items.Add(300);
            comboBoxLatency.Items.Add(400);
            comboBoxLatency.Items.Add(500);
            comboBoxLatency.SelectedIndex = 5;
        }
    }
}
