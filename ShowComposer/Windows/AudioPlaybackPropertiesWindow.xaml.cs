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
    /// Interaction logic for AudioPlaybackPropertiesWindow.xaml
    /// </summary>
    public partial class AudioPlaybackPropertiesWindow : Window
    {
        public static string allExtensions = "*.wav;*.aiff;*.mp3;*.aac;*.flac";
        AudioPlaybackControl m_AudioPlaybackControl;
        AudioPlaybackControlBig m_AudioPlaybackControlBig;

        public string AudioFile
        {
            get { return TextBoxAudioFilePath.Text; }
            set { TextBoxAudioFilePath.Text = value; }
        }

        public AudioPlaybackPropertiesWindow()
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
            m_AudioPlaybackControl = target;
            AudioFile = m_AudioPlaybackControl.AudioFile;
        }
        internal void SetTarget(AudioPlaybackControlBig target)
        {
            m_AudioPlaybackControlBig = target;
            AudioFile = m_AudioPlaybackControlBig.AudioFile;
        }

        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.FileName = AudioFile;

            openFileDialog.Filter = String.Format("All Supported Files|{0}|All Files (*.*)|*.*", allExtensions);
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AudioFile = openFileDialog.FileName;
                openFileDialog.FileName = AudioFile;
            }
        }
    }
}
