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
using System.Windows.Shapes;

namespace ShowComposer.Windows
{
    /// <summary>
    /// Interaction logic for BlackScreenWindow.xaml
    /// </summary>
    public partial class BlackScreenWindow : Window
    {
        private bool m_IsFullScreen;
        public BlackScreenWindow()
        {
            InitializeComponent();
        }
        public bool IsFullScreen
        {
            get { return m_IsFullScreen = this.WindowState == System.Windows.WindowState.Maximized; }
            set
            {
                if (value)
                {
                    this.WindowState = System.Windows.WindowState.Maximized;
                }
                else
                {
                    this.WindowState = System.Windows.WindowState.Normal;
                }
                m_IsFullScreen = value;
            }
        }
        public void ShowOnMonitor(int monitor)
        {
            var screen = ScreenHandler.GetScreen(monitor);
            var currentScreen = ScreenHandler.GetCurrentScreen(this);
            this.WindowState = WindowState.Normal;
            this.Left = screen.WorkingArea.Left;
            this.Top = screen.WorkingArea.Top;
            this.Width = screen.WorkingArea.Width;
            this.Height = screen.WorkingArea.Height;
        }
   
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                IsFullScreen = !IsFullScreen;
            }
        }
    }
}
