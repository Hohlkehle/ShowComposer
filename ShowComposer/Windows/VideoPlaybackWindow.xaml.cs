using ShowComposer.Data;
using ShowComposer.UserControls;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace ShowComposer.Windows
{
    /// <summary>
    /// Interaction logic for VideoPlaybackWindow.xaml
    /// </summary>
    public partial class VideoPlaybackWindow : Window
    {
        public static VideoPlaybackWindow instance;
        public event EventHandler OnVolumeValueChanged;
        private bool m_IsFullScreen;
        private double m_VolumeSens = 0.00025;
        private string m_VideoFile;
        private bool IsCtrlDown;

        public string VideoFile
        {
            get { return m_VideoFile; }
            set
            {
                m_VideoFile = value;
                mePlayer.Source = new Uri(m_VideoFile);
                this.Title = string.Format("{0}", System.IO.Path.GetFileName(m_VideoFile));
                lblFilename.Content = this.Title;
            }
        }

        public double Volume
        {
            set
            {
                if (mePlayer != null)
                    mePlayer.Volume = value;

                ProgressBarVolume.Value = value;
            }
            get { return mePlayer != null ? mePlayer.Volume : 0.5; }
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

        public MediaState CurrentState { get { return GetMediaState(mePlayer); } }

        public VideoPlaybackWindow()
        {
            InitializeComponent();

            instance = this;
            mePlayer.MediaOpened += Player_MediaOpened;
            mePlayer.MediaEnded += Player_MediaEnded;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();

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

        #region Window Drag
        private bool m_IsDragInProgress;
        private System.Windows.Point m_FormMousePosition;
        private bool sliderSeekdragStarted;
        private DateTime sliderSeekMouseDownStart;
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                this.CaptureMouse();
                this.m_IsDragInProgress = true;
                // 
                this.m_FormMousePosition = e.GetPosition((UIElement)this);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            //lblStatus.Content = string.Format("MouseRelative: {0}, MousePos: {1}", e.GetPosition(this), e.GetPosition(null));

            var asize = this.ActualHeight / 100.0 * 10.0;
            var mPos = e.GetPosition(this);
            if (mPos.Y > this.ActualHeight - asize)
            {
                GridControl.Opacity = 1;
            }
            else
            {
                GridControl.Opacity = 0;
            }


            if (this.m_IsDragInProgress)
            {
                System.Drawing.Point screenPos = (System.Drawing.Point)System.Windows.Forms.Cursor.Position;
                double top = (double)screenPos.Y - (double)this.m_FormMousePosition.Y;
                double left = (double)screenPos.X - (double)this.m_FormMousePosition.X;
                this.SetValue(MainWindow.TopProperty, top);
                this.SetValue(MainWindow.LeftProperty, left);
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                this.m_IsDragInProgress = false;
                this.ReleaseMouseCapture();
            }
            base.OnMouseUp(e);
        }

        internal void Play()
        {
            mePlayer.Play();
        }

        #endregion

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                IsCtrlDown = false;
            }

            if (e.Key == Key.Space)
            {
                if (mePlayer != null)
                {
                    if (CurrentState == MediaState.Play)
                        mePlayer.Pause();
                    else if (CurrentState == MediaState.Pause)
                        mePlayer.Play();

                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Right || e.Key == Key.Left)
            {
                if (mePlayer != null)
                {
                    TimeSpan step = new TimeSpan(0, 0, 5);
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        step = new TimeSpan(0, 0, 15);
                    }

                    mePlayer.Pause();

                    if (e.Key == Key.Right)
                        mePlayer.Position = mePlayer.Position += step;
                    if (e.Key == Key.Left)
                        mePlayer.Position = mePlayer.Position -= step;

                    mePlayer.Play();

                    ProgressBarProgress.Value = mePlayer.Position.TotalSeconds;
                }
            } else if (IsCtrlDown)
            {

            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                IsCtrlDown = true;
            }

            if (e.Key == Key.Escape)
            {
                Close();
                e.Handled = true;
            }
        }

        void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            //SetAudioStreamIndex(0);
            sliderPosition.Value = 0;
            sliderPosition.Maximum = mePlayer.NaturalDuration.TimeSpan.TotalSeconds;
            ProgressBarProgress.Value = sliderPosition.Value;
            ProgressBarProgress.Maximum = sliderPosition.Maximum;
        }

        void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            ProgressBarProgress.Value = sliderPosition.Value = 0;
            mePlayer.Stop();
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (mePlayer.Source != null)
            {
                if (mePlayer.NaturalDuration.HasTimeSpan)
                {
                    lblStatus.Content = String.Format("{0} / {1}", mePlayer.Position.ToString(@"mm\:ss"), mePlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                    if (!sliderSeekdragStarted)
                        sliderPosition.Value = mePlayer.Position.TotalSeconds;
                    ProgressBarProgress.Value = sliderPosition.Value;
                }
            }
            else
                lblStatus.Content = "No file selected...";
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy; // Okay
            else
                e.Effects = DragDropEffects.None; // Unknown data, ignore it
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        { }

        private void Grid_Drop(object sender, DragEventArgs e)
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

                    if (DeskLayout.IsVideoFile(fileList[0]))
                    {
                        LoadVideoFileFromDrop(fileList[0]);

                        e.Handled = true;
                    }
                    else
                    {
                        MessageBox.Show("Error occurs, only video files allowed.");
                    }
                }
            }
            else
            { }
        }

        private void LoadVideoFileFromDrop(string file)
        {
            if (mePlayer != null)
            {
                mePlayer.Stop();

                VideoFile = file;

                mePlayer.Play();
            } 
        }

        private MediaState GetMediaState(MediaElement myMedia)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = hlp.GetValue(myMedia);
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)stateField.GetValue(helperObject);
            return state;
        }
        private void SetAudioStreamIndex(int index)
        {
            FieldInfo hlp = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = hlp.GetValue(mePlayer);
            FieldInfo stateField = helperObject.GetType().GetField("_audioStreamIndex", BindingFlags.NonPublic | BindingFlags.Instance);

            //stateField.SetValue(mePlayer, index);
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Play();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Pause();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
        }

        private void VideoPlaybackFullScreen_Click(object sender, RoutedEventArgs e)
        {
            IsFullScreen = !IsFullScreen;
        }

        private void Player_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                IsFullScreen = !IsFullScreen;
            }
        }

        private void VideoPlaybackExit_Click(object sender, RoutedEventArgs e)
        {
            mePlayer.Stop();
            Close();
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var vol = Volume;
            vol += (e.Delta * m_VolumeSens);
            Volume = SmoothStep(Clamp01(vol), 0.0, 1.0, 0.0, 1.0);

            if (OnVolumeValueChanged != null)
                OnVolumeValueChanged(this, EventArgs.Empty);
        }

        private void SliderPosition_ThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (mePlayer != null)
            {
                //mePlayer.Pause();
                //mePlayer.Position = TimeSpan.FromSeconds(mePlayer.NaturalDuration.TimeSpan.TotalSeconds * sliderPosition.Value / 100.0);
                //mePlayer.Play();
            }

            sliderSeekdragStarted = false;
        }

        private void SliderPosition_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mePlayer == null) return;

            if (DateTime.Now - sliderSeekMouseDownStart > TimeSpan.FromMilliseconds(300))
                SliderPosition_ThumbDragCompleted(null, null);
            else
            {

                var pos = e.GetPosition(sliderPosition);

                sliderPosition.Value = SmoothStep(pos.X, 0, sliderPosition.ActualWidth, sliderPosition.Minimum, sliderPosition.Maximum);

                //mePlayer.Position = TimeSpan.FromSeconds(mePlayer.NaturalDuration.TimeSpan.TotalSeconds * sliderPosition.Value / 100.0);

                sliderSeekdragStarted = false;
            }
        }

        private void SliderPosition_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            sliderSeekMouseDownStart = DateTime.Now;
            sliderSeekdragStarted = true;
        }

        private void SliderPosition_ThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            sliderSeekdragStarted = true;
        }

        private void SliderPosition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Приостановка воспроизведения перед переходом в другую позицию
            // исключит "заикания" при слишком быстрых движениях ползунка,
            if (mePlayer != null && sliderSeekdragStarted)
            {
                mePlayer.Pause();
                mePlayer.Position = TimeSpan.FromSeconds(sliderPosition.Value);
                mePlayer.Play();

                ProgressBarProgress.Value = sliderPosition.Value;
            }
        }

        /// <summary>
        /// Interpolates smoothly from c1 to c2 based on x compared to a1 and a2. 
        /// </summary>
        /// <param name="x">value</param>
        /// <param name="a1">min</param>
        /// <param name="a2">max</param>
        /// <param name="c1">from</param>
        /// <param name="c2">to</param>
        /// <returns></returns>
        public static double SmoothStep(double x, double a1, double a2, double c1, double c2)
        {
            return c1 + ((x - a1) / (a2 - a1)) * (c2 - c1) / 1.0f;
        }

        public static double Clamp01(double value)
        {
            if (value < 0f)
            {
                return 0f;
            }
            if (value > 1f)
            {
                return 1f;
            }
            return value;
        }

        private void MenuItemOpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            if(System.IO.File.Exists(VideoFile))
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = System.IO.Path.GetDirectoryName(VideoFile),
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void Player_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mePlayer != null && e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                if (CurrentState == MediaState.Play)
                    mePlayer.Pause();
                else if (CurrentState == MediaState.Pause)
                    mePlayer.Play();

                e.Handled = true;
            }

        }
    }
}
