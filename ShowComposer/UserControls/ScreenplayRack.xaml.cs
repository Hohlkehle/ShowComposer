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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShowComposer.UserControls
{
    /// <summary>
    /// Interaction logic for ScreenplayRack.xaml
    /// </summary>
    public partial class ScreenplayRack : UserControl
    {
        public ScreenplayRack()
        {
            InitializeComponent();
        }

        private void documentviewWord_DragEnter(object sender, DragEventArgs e)
        {
            MessageBox.Show("documentviewWord_DragEnter"); ;
        }


    }
}
