using LayoutFileSystem;
using ShowComposer.Core;
using ShowComposer.Data;
using ShowComposer.DraggableUI;
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
using System.Xml.Serialization;
using WindowsPoint = System.Windows.Point;

namespace ShowComposer.UserControls
{
    /// <summary>
    /// Interaction logic for DeskLayout.xaml
    /// </summary>
    public partial class DeskLayout : UserControl
    {
        public static event EventHandler<LayoutEditorSelectionEventArgs> OnUIElementSelected = (Object sender, LayoutEditorSelectionEventArgs e) => { };
        public static event EventHandler<DeskLayoutEventArgs> OnUIElementEvent = (Object sender, DeskLayoutEventArgs e) => { };
        public static event EventHandler<DeskLayoutEventArgs> OnDeskLayoutLoaded = (Object sender, DeskLayoutEventArgs e) => { };

        private double m_OriginalLeft;
        private double m_OriginalTop;
        private double m_SnapSize = 10;
        private bool m_IsDown;
        private bool m_IsDragging;
        private bool m_IsSelected = false;
        private bool m_IsCtrlDown;

        private GuideLine[] m_GuideLines;
        private UIElement m_SelectedElement = null;
        private WindowsPoint m_StartPoint;
        private int m_DragDelta = 10;

        //private Layout m_Layout;
        //private string m_FileName = @"scenario 1.scomp";
        private List<UIElement> m_LoadedCanvasItems = new List<UIElement>();
        public LayoutProperties LayoutProperties;
        //private bool m_IsNewProject;
        private ComposerLayout m_ComposerLayout;

        public ComposerLayout ComposerLayout
        {
            get { return m_ComposerLayout; }
            set { m_ComposerLayout = value; }
        }
        public bool IsCtrlDown
        {
            //TODO
            get { return Properties.Settings.Default.UseDraggingModifierKey ? m_IsCtrlDown : true; }
            set
            {
                m_IsCtrlDown = value;
                //if (value)
                //{
                //    foreach (var el in myCanvas.Children.OfType<IDraggableUIElement>())
                //        if (((UIElement)el).IsMouseOver)
                //        { Mouse.OverrideCursor = Cursors.SizeAll; }
                //        else
                //        {
                //            if (Mouse.OverrideCursor == Cursors.SizeAll)
                //                Mouse.OverrideCursor = Cursors.Arrow;
                //            break;
                //        }
                //}
                //else if (Mouse.OverrideCursor == Cursors.SizeAll) Mouse.OverrideCursor = Cursors.Arrow;
            }
        }

        double m_ExpandedHeight;
        public double ExpandedHeight
        {
            set
            {
                m_ExpandedHeight = value;
                myCanvas.Height = m_ExpandedHeight;
                ((Image)(ImageSpacer)).Height = m_ExpandedHeight;
            }
            get { return m_ExpandedHeight; }
        }

        public IEnumerable<AudioPlaybackControl> AudioControls { get { return myCanvas.Children.OfType<ShowComposer.UserControls.AudioPlaybackControl>(); } }
        public IEnumerable<VideoPlaybackControl> VideoControls { get { return myCanvas.Children.OfType<ShowComposer.UserControls.VideoPlaybackControl>(); } }

        public DeskLayout()
        {
            InitializeComponent();
        }

        #region Drag elements on canvas

        /// <summary>
        /// Handler for drag stopping on leaving the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Window1_MouseLeave(object sender, MouseEventArgs e)
        {
            StopDragging();
            //e.Handled = true;
        }

        /// <summary>
        /// Handler for drag stopping on user choise
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DragFinishedMouseHandler(object sender, MouseButtonEventArgs e)
        {
            var thr = m_StartPoint - Mouse.GetPosition(myCanvas);
            if (Math.Sqrt(thr.X * thr.X + thr.Y * thr.Y) < m_DragDelta)
            {
                e.Handled = false;
            } else
                OnUIElementEvent(this, new DeskLayoutEventArgs(m_SelectedElement) { UIElementEventType = UIElementEventType.StopDragging });
            StopDragging();
            //e.Handled = true;
        }

        /// <summary>
        /// Method for stopping dragging
        /// </summary>
        private void StopDragging()
        {
            if (m_IsDown)
            {
                m_IsDown = false;
                m_IsDragging = false;
            }
        }

        /// <summary>
        /// Hanler for providing drag operation with selected element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Window1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_IsDown)
            {
                var thr = m_StartPoint - Mouse.GetPosition(myCanvas);
                if(Math.Sqrt(thr.X * thr.X + thr.Y * thr.Y) > m_DragDelta)

                if ((m_IsDragging == false) &&
                    ((Math.Abs(e.GetPosition(myCanvas).X - m_StartPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    (Math.Abs(e.GetPosition(myCanvas).Y - m_StartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    m_IsDragging = true;
                e.Handled = true;
                if (m_IsDragging && m_SelectedElement != null)
                {
                    var position = Mouse.GetPosition(myCanvas);
                    var elPos = new WindowsPoint(position.X - (m_StartPoint.X - m_OriginalLeft), position.Y - (m_StartPoint.Y - m_OriginalTop));

                    if (m_SelectedElement.GetType() != typeof(GuideLine))
                    {
                        var height = ((FrameworkElement)m_SelectedElement).Height;
                        var width = ((FrameworkElement)m_SelectedElement).Width;
                        foreach (var gl in m_GuideLines)
                        {
                            if (position.X > gl.Left && position.X < gl.RightEdge && Math.Abs(gl.Top - (elPos.Y + height)) < m_SnapSize)
                            {
                                elPos.Y = gl.Top - height;
                                elPos.Y = gl.Left - height;
                                break;
                            }
                        }
                    }

                    if (elPos.X < 20)
                        elPos.X = 4;

                    Canvas.SetTop(m_SelectedElement, elPos.Y);
                    Canvas.SetLeft(m_SelectedElement, elPos.X);

                    OnUIElementEvent(this, new DeskLayoutEventArgs(m_SelectedElement) { UIElementEventType = UIElementEventType.Move });
                }
            }
        }

        /// <summary>
        /// Handler for clearing element selection, adorner removal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Window1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (m_IsSelected)
            {
                m_IsSelected = false;
                if (m_SelectedElement != null)
                {
                    //m_ALayer.Remove(m_ALayer.GetAdorners(m_SelectedElement)[0]);
                    m_SelectedElement = null;
                }
            }
        }

        /// <summary>
        /// Handler for element selection on the canvas providing resizing adorner
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MyCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (m_IsSelected)
            {
                m_IsSelected = false;
                if (m_SelectedElement != null && ((FrameworkElement)m_SelectedElement).Name != "ImageBackground" /*&& selectedElement.Uid != m_CanvasImageUID*/)
                {
                    // Remove the adorner from the selected element
                    try
                    {
                        //m_ALayer.Remove(m_ALayer.GetAdorners(m_SelectedElement)[0]);
                    }
                    catch (NullReferenceException) { }
                    m_SelectedElement = null;

                    OnUIElementSelected(this, new LayoutEditorSelectionEventArgs(null, false));
                }
            }

            if (e.Source is IDraggableUIElement && ((IDraggableUIElement)e.Source).ClutchElement())
            {
                var iscl = ((IDraggableUIElement)e.Source).ClutchElement();
                m_IsDown = true;
                var rfocused = this.Focus();
            }

            // If any element except canvas is clicked, 
            // assign the selected element and add the adorner
            if (e.Source is IDraggableUIElement && ((IDraggableUIElement)e.Source).ClutchElement() && IsCtrlDown)
            {
                m_IsDown = true;
                m_StartPoint = e.GetPosition(myCanvas);

                m_SelectedElement = e.Source as UIElement;

                m_OriginalLeft = Canvas.GetLeft(m_SelectedElement);
                m_OriginalTop = Canvas.GetTop(m_SelectedElement);

                //m_ALayer = System.Windows.Documents.AdornerLayer.GetAdornerLayer(m_SelectedElement);
                //m_ALayer.Add(new ResizingAdorner(m_SelectedElement));

                m_IsSelected = true;
                //e.Handled = true;

                m_GuideLines = myCanvas.Children.OfType<GuideLine>().ToArray();
                //PushCommandToHistory(new MoveCommand(m_SelectedElement));

                OnUIElementSelected(this, new LayoutEditorSelectionEventArgs(m_SelectedElement, true));
                
            }
        }

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                IsCtrlDown = false;
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                IsCtrlDown = true;
            }

            if (m_IsSelected)
            {
                if (e.Key == Key.Delete)
                {
                    if (MessageBox.Show("Delete slected item?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (m_SelectedElement is AudioPlaybackControl)
                            ((AudioPlaybackControl)m_SelectedElement).Stop();

                        RemoveCanvasElement(m_SelectedElement);
                        e.Handled = true;
                    }
                }
            }
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.MouseLeftButtonDown += new MouseButtonEventHandler(Window1_MouseLeftButtonDown);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(DragFinishedMouseHandler);
            this.MouseMove += new MouseEventHandler(Window1_MouseMove);
            this.MouseLeave += new MouseEventHandler(Window1_MouseLeave);

            myCanvas.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(MyCanvas_PreviewMouseLeftButtonDown);
            myCanvas.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(DragFinishedMouseHandler);
        }

        public void ToggleControlPlaybackState()
        {

        }

        #region Drag&Drop

        private void MyCanvas_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //myScrollViewer.AllowDrop = true;
                //myScrollViewer.IsHitTestVisible = true;
            }

            // Check if the Dataformat of the data can be accepted
            // (we only accept file drops from Explorer, etc.)
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy; // Okay
            else
                e.Effects = DragDropEffects.None; // Unknown data, ignore it
        }

        private void MyCanvas_DragLeave(object sender, DragEventArgs e)
        {
            //myScrollViewer.AllowDrop = false;
            //myScrollViewer.IsHitTestVisible = false;
            if (!e.Handled)
            {
                //TODO
                //HandleDragDrop(e);

                e.Handled = true;
            }
        }

        private void MyCanvas_Drop(object sender, DragEventArgs e)
        {
            HandleDragDrop(e);

            e.Handled = true;
        }

        private void HandleDragDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Object item = (object)e.Data.GetData(DataFormats.FileDrop);

                // Perform drag-and-drop, depending upon the effect.
                if (((e.Effects & DragDropEffects.Copy) == DragDropEffects.Copy) ||
                   ((e.Effects & DragDropEffects.Move) == DragDropEffects.Move))
                {

                    // Extract the data from the DataObject-Container into a string list
                    string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

                    LoadFileToCanvas(fileList[0]);
                }
            }
            else
            {
                // Reset the label text.
                //DropLocationLabel.Content = "None";
            }
        }

        #endregion

        #region Layout
        void AudioPlaybackControl_OnRemove(object sender, EventArgs e)
        {
            RemoveCanvasElement((UIElement)sender);
        }

        private void VideoPlaybackControl_OnRemove(object sender, EventArgs e)
        {
            RemoveCanvasElement((UIElement)sender);
        }

        void DeskLayout_OnRemove(object sender, EventArgs e)
        {
            RemoveCanvasElement((UIElement)sender);
        }

        private void AddAudioPlaybackControl(WindowsPoint location, string audioFile)
        {
            var control = new ShowComposer.UserControls.AudioPlaybackControl();
            control.IsRelativePath = false;
            control.AudioFile = audioFile;
            control.SoundVolume = 0.5;
            control.IsExclusivePlayback = false;
            
            ((AudioPlaybackControl)control).OnRemove += AudioPlaybackControl_OnRemove;

            Canvas.SetTop(control, location.Y);
            Canvas.SetLeft(control, location.X);

            AddCanvasElement(control, new WindowsPoint());
        }

     
        private void AddAudioPlaybackControl(AudioPlayerInfo info)
        {
            if (Properties.Settings.Default.AudioPlaybackOneOutput)
                AudioControls.All((i) => i.Stop());

            var control = new ShowComposer.UserControls.AudioPlaybackControl();
            control.IsRelativePath = info.IsRelativePath;
            control.AudioFile = info.AudioFile;
            control.SoundVolume = info.AudioVolume;
            control.IsExclusivePlayback = info.IsExclusivePlayback;
           
            ((AudioPlaybackControl)control).OnRemove += AudioPlaybackControl_OnRemove;

            Canvas.SetTop(control, info.Top);
            Canvas.SetLeft(control, info.Left);

            AddCanvasElement(control, new WindowsPoint());
        }

        private void AddVideoPlaybackControl(WindowsPoint location, string videoFile)
        {
            var control = new ShowComposer.UserControls.VideoPlaybackControl();
            control.IsRelativePath = false;
            control.VideoFile = videoFile;
            control.SoundVolume = 0.5;
            
            ((VideoPlaybackControl)control).OnRemove += VideoPlaybackControl_OnRemove;

            Canvas.SetTop(control, location.Y);
            Canvas.SetLeft(control, location.X);

            AddCanvasElement(control, new WindowsPoint());
        }

        private void AddVideoPlaybackControl(VideoPlayerInfo info)
        {
            if (Properties.Settings.Default.AudioPlaybackOneOutput)
                AudioControls.All((i) => i.Stop());

            var control = new ShowComposer.UserControls.VideoPlaybackControl();
            control.IsRelativePath = info.IsRelativePath;
            control.VideoFile = info.VideoFile;
            control.SoundVolume = info.AudioVolume;
            
            ((VideoPlaybackControl)control).OnRemove += VideoPlaybackControl_OnRemove;

            Canvas.SetTop(control, info.Top);
            Canvas.SetLeft(control, info.Left);

            AddCanvasElement(control, new WindowsPoint());
        }

        private void AddPowerPointControl(WindowsPoint location, string presenterFile)
        {
            var control = new ShowComposer.UserControls.PowerPointControl();
            control.IsRelativePath = false;
            control.PresenterFile = presenterFile;

            ((PowerPointControl)control).OnRemove += DeskLayout_OnRemove;

            Canvas.SetTop(control, location.Y);
            Canvas.SetLeft(control, location.X);

            AddCanvasElement(control, new WindowsPoint());
        }

        private void AddPowerPointControl(PowerPointInfo info)
        {
           // if (Properties.Settings.Default.AudioPlaybackOneOutput)
            //    AudioControls.All((i) => i.Stop());

            var control = new ShowComposer.UserControls.PowerPointControl();
            control.IsRelativePath = info.IsRelativePath;
            control.PresenterFile = info.PresenterFile;

            ((PowerPointControl)control).OnRemove += DeskLayout_OnRemove;

            Canvas.SetTop(control, info.Top);
            Canvas.SetLeft(control, info.Left);

            AddCanvasElement(control, new WindowsPoint());
        }

        private void AddGuigeLine(WindowsPoint location, double width, double height, WindowsPoint mediaOffset)
        {
            var control = new GuideLine();
            Canvas.SetTop(control, location.Y);
            Canvas.SetLeft(control, location.X);

            control.Width = width;
            control.Height = height;

            AddCanvasElement(control, mediaOffset);
        }

        public void AddCanvasElement(UIElement element, WindowsPoint mediaOffset)
        {
            myCanvas.Children.Add(element);

            double l = Canvas.GetLeft(element), t = Canvas.GetTop(element);
            Canvas.SetLeft(element, l + mediaOffset.X);
            Canvas.SetTop(element, t + mediaOffset.Y);

            m_LoadedCanvasItems.Add(element);
        }

        private void RemoveCanvasElement(UIElement element)
        {
            myCanvas.Children.Remove(element);
            m_LoadedCanvasItems.Remove(element);

            if (element.Equals(m_SelectedElement))
                m_SelectedElement = null;

            //UpdateLayoutProperties();
        }

        public void LoadFileToCanvas(string file)
        {
            if (!MainWindow.IsProjectLoaded)
                return;

            Mouse.Capture(myCanvas);

            var position = Mouse.GetPosition(myCanvas);
             
            Mouse.Capture(null);

            if(position.X < 0 || position.Y < 0)
            {
                position = Mouse.GetPosition(null);

                if (position.X < 0 || position.Y < 0)
                    position = new WindowsPoint(50, 50);
            }

            if (IsAudioFile(file))
            {
                AddAudioPlaybackControl(position, file);
            }
            else if (IsVideoFile(file))
            {
                AddVideoPlaybackControl(position, file);
            }
            else if (IsPresenterFile(file))
            {
                AddPowerPointControl(Mouse.GetPosition(myCanvas), file);
            }

            CommandHelper.Log(string.Format("Calculated position for new element: {0}", position));
        }

      

        public void ClearLayout()
        {
            AudioControls.All((i) => i.Stop());
            VideoControls.All((i) => i.Stop());
            foreach (var el in m_LoadedCanvasItems)
            {
                myCanvas.Children.Remove(el);
            }
            //documentviewWord.Document = new FixedDocument();
        }

        public void LoadLayout(LayoutProperties prop)
        {
            LayoutProperties = prop;
            using (new WaitCursor())
            {
                if (prop != null)
                {
                    ClearLayout();

                    foreach (var ci in prop.m_AudioPlayerInfo)
                    {
                        AddAudioPlaybackControl(ci);
                    }

                    foreach (var ci in prop.m_VideoPlayerInfo)
                    {
                        AddVideoPlaybackControl(ci);
                    }

                    foreach (var ci in prop.m_PowerPointInfo)
                    {
                        AddPowerPointControl(ci);
                    }
                }
            }
            OnDeskLayoutLoaded(this, new DeskLayoutEventArgs(null) { UIElementEventType = UIElementEventType.Loaded });
        }
        #endregion

        public void SaveProject(string projectFileName)
        {
            if (LayoutProperties == null)
                return;

            using (new WaitCursor())
            {
                LayoutProperties.m_AudioPlayerInfo = new List<AudioPlayerInfo>();
                LayoutProperties.m_AudioPlayerInfo.AddRange(AudioPlayerInfo.Convert(myCanvas.Children.OfType<AudioPlaybackControl>().ToArray()));

                LayoutProperties.m_VideoPlayerInfo = new List<VideoPlayerInfo>();
                LayoutProperties.m_VideoPlayerInfo.AddRange(VideoPlayerInfo.Convert(myCanvas.Children.OfType<VideoPlaybackControl>().ToArray()));

                LayoutProperties.m_PowerPointInfo = new List<PowerPointInfo>();
                LayoutProperties.m_PowerPointInfo.AddRange(PowerPointInfo.Convert(myCanvas.Children.OfType<PowerPointControl>().ToArray()));


                ComposerLayout.Save();

                //LayoutProperties.WordDocument = "";

                //m_Layout.Data1 = LayoutFileReader.GetBytes(SerializeToString(LayoutProperties));
               // m_Layout.Save(projectFileName);

                //MenuItemSaveProject.IsEnabled = false;  
            }
        }

        #region Helpers
        private childItem FindVisualChild<childItem>(DependencyObject obj) where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        public static string[] audioExtensions = { ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA", ".FLAC" };
        public static string[] videoExtensions = { ".AVI", ".MP4", ".DIVX", ".WMV", ".MKV" };
        public static string[] imageExtensions = { ".PNG", ".JPG", ".JPEG", ".BMP", ".GIF" };
        public static string[] presentExtensions = { ".ppt", ".pptx", ".pptm", ".potx", ".potm", ".pot", ".ppsx", ".ppsm", ".pps", ".ppam", ".ppa", ".odp" };

        public static bool IsAcceptableFile(string filePath)
        {
            return System.IO.File.Exists(filePath) && (IsAudioFile(filePath) || IsVideoFile(filePath) || IsPresenterFile(filePath));
        }

        public static bool IsAudioFile(string path)
        {
            return audioExtensions.Contains(System.IO.Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsVideoFile(string path)
        {
            return videoExtensions.Contains(System.IO.Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsImageFile(string path)
        {
            return imageExtensions.Contains(System.IO.Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }

        public static bool IsPresenterFile(string path)
        {
            return presentExtensions.Contains(System.IO.Path.GetExtension(path), StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        [Obsolete]
        private void DvScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //myScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            // FindVisualChild<ScrollViewer>(documentviewWord).ScrollToVerticalOffset(e.VerticalOffset);
        }

        internal void RemoveControl(UIElement selectedElement)
        {
            RemoveCanvasElement(selectedElement);
        }

        private void myCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            //OnDeskLayoutLoaded(this, new DeskLayoutEventArgs(null) { UIElementEventType = UIElementEventType.Loaded });
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            ClearLayout();
        }
    }
}
