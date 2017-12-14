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
using ShowComposer.DraggableUI;
using ShowComposer.Data;
using gma.System.Windows;
using System.Windows.Forms;
using Utilities;
using ShowComposer.Core;

namespace ShowComposer.UserControls
{
    /// <summary>
    /// Логика взаимодействия для DeskLayoutGrid.xaml
    /// </summary>
    public partial class DeskLayoutGrid : System.Windows.Controls.UserControl
    {
        private DeskLayout m_DeskLayout;
        private UIElement m_SelectedElement;
        private Dictionary<int, string> m_AudioPlaybackControls = new Dictionary<int, string>();
        private int m_GridHeight = 56;
        private int m_KeyPressed = 1;
        private int m_KeyPressDelay = 3000000;
        private long m_KeyDownTime;
        private static readonly UserActivityHook actHook = new UserActivityHook();
        //private static readonly globalKeyboardHook gkh = new globalKeyboardHook();
        //private GlobalKeyboardController m_GlobalKeyboardController;

        public DeskLayout DeskLayout
        {
            get => m_DeskLayout;
            set
            {
                m_DeskLayout = value;
                DeskLayout.OnUIElementEvent += DeskLayout_OnUIElementEvent;
                DeskLayout.OnDeskLayoutLoaded += DeskLayout_OnDeskLayoutLoaded;

            }
        }

        private void DeskLayout_OnDeskLayoutLoaded(object sender, DeskLayoutEventArgs e)
        {
            if (m_DeskLayout == null || sender != m_DeskLayout)
                return;

            RebindKeys();
        }

        private void DeskLayout_OnUIElementEvent(object sender, DeskLayoutEventArgs e)
        {
            if (m_DeskLayout == null || sender != m_DeskLayout)
                return;

            m_SelectedElement = e.UIElement;

            switch (e.UIElementEventType)
            {
                case UIElementEventType.Move:
                    var top = Canvas.GetTop(m_SelectedElement);
                    var hheight = ((FrameworkElement)m_SelectedElement).ActualHeight;
                    Canvas.SetTop(m_SelectedElement, (top - (top % m_GridHeight)));
                    m_SelectedElement = e.UIElement;
                    break;
                case UIElementEventType.StopDragging:
                    if (m_SelectedElement != null)
                    {
                        var top1 = Canvas.GetTop(m_SelectedElement);
                        var topMargin = 0;

                        //var top = Canvas.GetTop(m_SelectedElement);
                        //RowDefinition clossest = MyGrid.RowDefinitions[0];
                        //var top1 = Canvas.GetTop(clossest);

                        //foreach(var row in MyGrid.RowDefinitions)
                        var mp = Mouse.GetPosition(this); //Application.Current.MainWindow
                                                          //UIElement container = VisualTreeHelper.GetParent(control) as UIElement;
                                                          //Point relativeLocation = this.TranslatePoint(new Point(0, top + (((FrameworkElement)m_SelectedElement).ActualHeight)), MyGrid);
                        Point relativeLocation = this.TranslatePoint(new Point(0, mp.Y + m_GridHeight + topMargin), MyGrid);

                        var p = MyGrid.GetColumnRow(new Point(0, mp.Y));
                        Canvas.SetTop(m_SelectedElement, p.Y * m_GridHeight);
                        Canvas.SetLeft(m_SelectedElement, 30);

                        RebindKeys();
                    }

                    break;
            }
        }

        public DeskLayoutGrid()
        {
            InitializeComponent();
        }

        private void BindGlobalKeysHook()
        {
            //actHook = new UserActivityHook(); // crate an instance with global hooks
            // hang on events

            actHook.KeyUp += new System.Windows.Forms.KeyEventHandler(MyKeyDown);
            actHook.Start();
            //actHook.KeyUp += new KeyEventHandler(MyKeyUp);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            m_GridHeight = (int)MyGrid.RowDefinitions.ElementAt(0).ActualHeight;
        }

        public void RebindKeys()
        {
            m_AudioPlaybackControls.Clear();
            //AudioPlaybackControl[] apcs = m_DeskLayout.AudioControls.ToArray();
            //for (var i = 0; i < apcs.Length; i++)
            //{
            //    var top = Canvas.GetTop((UIElement)apcs[i]);
            //    var p = MyGrid.GetColumnRow(new Point(0, top + m_GridHeight));

            //    if ((int)p.Y == 9)
            //    {
            //        m_AudioPlaybackControls.Add(0, apcs[i]);
            //        break;
            //    }
            //    else
            //    {
            //        m_AudioPlaybackControls.Add((int)p.Y + 1, apcs[i]);
            //    }
            //}

            foreach (var ac in m_DeskLayout.AudioControls.OrderBy(c => Canvas.GetTop(c)))
            {
                var top = Canvas.GetTop((UIElement)ac);
                var p = MyGrid.GetColumnRow(new Point(0, top + m_GridHeight));

                if ((int)p.Y == 9)
                {
                    m_AudioPlaybackControls.Add(0, ac.AudioFile);
                    break;
                }
                else
                {
                    if (m_AudioPlaybackControls.ContainsKey((int)p.Y + 1))
                    {
                        System.Windows.MessageBox.Show("An item with the same key already added to collection! Layout will be cleared!", "Warning", MessageBoxButton.OK);
                        m_AudioPlaybackControls.Clear();
                        m_DeskLayout.ClearLayout();
                        break;
                    } else 
                        m_AudioPlaybackControls.Add((int)p.Y + 1, ac.AudioFile);
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            m_KeyDownTime = DateTime.Now.Ticks;
            BindGlobalKeysHook();
        }

        private void MyKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //if (DateTime.Now.Ticks > m_KeyDownTime + m_KeyPressDelay)
            {
                int key = ((int)e.KeyCode) - 48;

                if (key == -1 || key > 9)
                    return;
                
                m_KeyPressed = key;
                foreach (var ac in m_DeskLayout.AudioControls.Where(i => i.AudioFile == m_AudioPlaybackControls[m_KeyPressed]))
                {
                    if(ac.IsPlaying)
                        ac.TogglePlayback();

                    else if(DateTime.Now.Ticks > m_KeyDownTime + m_KeyPressDelay)
                    {
                        ac.TogglePlayback();
                    }

                    m_KeyDownTime = DateTime.Now.Ticks;
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (actHook != null)
                actHook.Stop();

            //if(gkh != null)
            //    gkh.unhook();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RebindKeys();
        }
    }
}
