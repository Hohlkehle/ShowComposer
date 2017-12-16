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
    /// Interaction logic for LoggingWindow.xaml
    /// </summary>
    public partial class LoggingWindow : Window
    {
        public static LoggingWindow instance;
        public LoggingWindow()
        {
            instance = this;
            InitializeComponent();
            Initialize();
            Log("\n---------------------------------------");
        }

        private void Initialize()
        {
            MainWindow.OnApplicationQuit += (object sender, EventArgs e) =>
            {
                Quit();
            };

            AudioPlaybackControl.OnPlay += (object sender, EventArgs e) =>
            {
                AudioPlaybackControl apc = (AudioPlaybackControl)sender;
                Log("Now playing {0}", apc.AudioTrackTitle);
            };
            AudioPlaybackControl.OnStop += (object sender, EventArgs e) =>
            {
                AudioPlaybackControl apc = (AudioPlaybackControl)sender;
                Log("{0} STOPPED", apc.AudioTrackTitle);
            };
        }

        public static void LogStatic(string message)
        {
            if (instance == null)
                return;

            instance.Dispatcher.Invoke(delegate {
                instance.RichTextBox1.AppendText(message);
                instance.RichTextBox1.AppendText("\n");
            });
        }

        public void Log(string message)
        {
            Dispatcher.Invoke(delegate {
                RichTextBox1.AppendText(message);
                RichTextBox1.AppendText("\n");
            });
        }

        public void Log(string format, params string[] message)
        {
            Dispatcher.Invoke(delegate {
                RichTextBox1.AppendText(string.Format(format, message));
                RichTextBox1.AppendText("\n");
            });
        }

        public static void Quit()
        {
            if(instance != null)
            {
                instance.Close();
            }
        }
    }
}
