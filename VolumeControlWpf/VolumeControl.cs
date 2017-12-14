using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuirkSoft;

namespace VolumeControlWpf.UserControls
{
    /// <summary>
    /// Interaction logic for VolumeControl.xaml
    /// </summary>
    public class VolumeControl : WindowsFormsHost
    {
        public QuirkSoft.VolumeController VolumeController { get; private set; }

        public VolumeControl()
        { 
            VolumeController = new QuirkSoft.VolumeController();

            this.Child = VolumeController;
        }

        public void Stop()
        {
            VolumeController?.Stop();
        }
    }
}
