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

namespace ShowComposer.DraggableUI
{
    /// <summary>
    /// Interaction logic for GuigeLine.xaml
    /// </summary>
    public partial class GuideLine : UserControl, IDraggableUIElement
    {
        public double Left { get { return Canvas.GetLeft(this); } }
        public double Top { get { return Canvas.GetTop(this); } }
        public double RightEdge { get { return Left + ActualWidth; } }
        public GuideLine()
        {
            InitializeComponent();
        }

        public bool ClutchElement()
        {
            return true;
        }
    }
}
