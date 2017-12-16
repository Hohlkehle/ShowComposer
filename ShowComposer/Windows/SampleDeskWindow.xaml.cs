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
    /// Interaction logic for SampleDeskWindow.xaml
    /// </summary>
    public partial class SampleDeskWindow : Window
    {
        private string m_FileName;

        public string ProjectFileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        public SampleDeskWindow()
        {
            InitializeComponent();

            MainWindow.OnApplicationQuit += (object sender, EventArgs e) =>
            {
                Close();
            };

            MyDeskLayoutGrid.DeskLayout = MyDeskLayout;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //AudioPlaybackControl[] apcs = MyDeskLayout.AudioControls.ToArray();
            //for (var i = 0; i < apcs.Length; i++)
            //{
            //    apcs[i].CloseWaveOut();
            //}

            MyDeskLayout.AudioControls.All((i) => i.Stop());
            MyDeskLayout.VideoControls.All((i) => i.Stop());

            MyDeskLayout.SaveProject(ProjectFileName);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //MyDeskLayoutGrid.RebindKeys();
        }
    }
}
