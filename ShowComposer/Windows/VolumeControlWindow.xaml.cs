﻿using System;
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
    /// Interaction logic for VolumeControlWindow.xaml
    /// </summary>
    public partial class VolumeControlWindow : Window
    {
        public VolumeControlWindow()
        {
            InitializeComponent();

            MainWindow.OnApplicationQuit += (object sender, EventArgs e) =>
            {
                Close();
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VolumeControl.Stop();
        }
    }
}
