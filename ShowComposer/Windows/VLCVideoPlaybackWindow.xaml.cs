using gma.System.Windows;
using ShowComposer.Data;
using ShowComposer.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Vlc.DotNet.Core.Interops.Signatures;

namespace ShowComposer.Windows
{
    /// <summary>
    /// Логика взаимодействия для VLCVideoPlaybackWindow.xaml
    /// </summary>
    public partial class VLCVideoPlaybackWindow : Window
    {
        public event EventHandler OnVolumeValueChanged;
        //private bool IsCtrlDown;
        private string m_VideoFile;
        private bool m_IsFullScreen;

        private bool m_SliderSeekdragStarted;
        private DateTime m_SliderSeekMouseDownStart;

        private double m_VolumeSens = 6;
        private int m_Volume = 50;

        private long m_LastClickTime;
        private int m_DoubleClickSensetivity = 2200000;

        //private UserActivityHook m_ActHook;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowThreadProcessId(IntPtr handle, out int processId);

        public double Volume
        {
            set
            {
                m_Volume = (int)value;
            }
            get
            {
                //return MyControl.MediaPlayer.Audio != null  && MyControl.MediaPlayer.Audio.Volume != -1 ? MyControl.MediaPlayer.Audio.Volume : m_Volume; 
                return m_Volume;
            }
        }

        public string VideoFile
        {
            get { return m_VideoFile; }
            set
            {
                m_VideoFile = value;
                //mePlayer.Source = new Uri(m_VideoFile);
                Title = string.Format("{0}", System.IO.Path.GetFileName(m_VideoFile));
                lblFilename.Content = this.Title;
            }
        }

        public bool IsFullScreen
        {
            get { return m_IsFullScreen = this.WindowState == WindowState.Maximized; }
            set
            {
                if (value)
                {
                    this.WindowState = WindowState.Maximized;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                }
                m_IsFullScreen = value;
            }
        }

        public MediaStates CurrentState { get { return MyControl.MediaPlayer.State; } }

        public Action<object, RoutedEventArgs> MediaEnded { get; internal set; }

        public VLCVideoPlaybackWindow()
        {
            InitializeComponent();

            MainWindow.OnApplicationQuit += (object sender, EventArgs e) =>
            {
                Stop();
                Close();
            };

            //var mediaPlayer = new Vlc.DotNet.Core.VlcMediaPlayer(new DirectoryInfo(libDirectory));
            MyControl.MediaPlayer.VlcLibDirectoryNeeded += OnVlcControlNeedsLibDirectory;
            MyControl.MediaPlayer.VlcMediaplayerOptions = new string[] { "-f", "--dummy-quiet", "--ignore-config", "--no-video-title", "--no-sub-autodetect-file" };
            MyControl.MediaPlayer.EndInit();

            // This can also be called before EndInit
            this.MyControl.MediaPlayer.Log += (sender, args) =>
            {
                string message = string.Format("libVlc : {0} {1} @ {2}", args.Level, args.Message, args.Module);
                System.Diagnostics.Debug.WriteLine(message);
            };

            MyControl.MediaPlayer.EndReached += (sender, args) =>
            {
                MediaEnded?.Invoke(this, new RoutedEventArgs());
            };

            MyControl.MediaPlayer.VideoOutChanged += (sender, args) =>
            {
                //MyControl.MediaPlayer.Audio.Volume = (int)Volume;
                var vlc = ((Vlc.DotNet.Forms.VlcControl)sender);
                Dispatcher.Invoke(delegate
                {
                    sliderPosition.Maximum = vlc.Length;
                    ProgressBarProgress.Maximum = vlc.Length;
                    sliderPosition.Value = vlc.Time;
                    ProgressBarProgress.Value = vlc.Time;
                    ProgressBarVolume.Value = Volume;
                    vlc.Audio.Volume = (int)Volume;
                });
            };

            //// crate an instance with global hooks hang on events
            //m_ActHook = new UserActivityHook();
            //m_ActHook.OnMouseActivity += ActHook_OnMouseActivity;
            //m_ActHook.Start();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void ActHook_OnMouseActivity(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!ApplicationIsActivated() || (e.Button == System.Windows.Forms.MouseButtons.None && e.Delta == 0))
                return;

            //var position = System.Windows.Input.Mouse.GetPosition(this);
            var cursorPosition = new Point(e.X, e.Y);

            if (!IsPointInBounds(cursorPosition, MyControl))
                return;

            // WM_MOUSEWHEEL
            if (e.Delta > 0 || e.Delta < 0)
            {
                SetAudioVolume(MyControl.MediaPlayer.Audio.Volume + (int)(Math.Sign(e.Delta) * m_VolumeSens));
                ProgressBarVolume.Value = Volume;
                return;
            }

            // Perform WM_LBUTTONDOWN action 
            if (e.Button == System.Windows.Forms.MouseButtons.Left && !GridContextMenu.IsOpen)
            {
                if (MyControl.MediaPlayer.IsPlaying)
                {
                    MyControl.MediaPlayer.Pause();
                }
                else
                {
                    if (MyControl.MediaPlayer.State == MediaStates.Ended)
                        MyControl.MediaPlayer.Stop();

                    MyControl.MediaPlayer.Play();
                }
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                GridContextMenu.IsOpen = true;
            }

            // DoubleClick
            if (e.Button == System.Windows.Forms.MouseButtons.Left && DateTime.Now.Ticks - m_LastClickTime < m_DoubleClickSensetivity)
            {

                if (WindowState != WindowState.Maximized)
                    this.WindowState = WindowState.Maximized;
                else
                    WindowState = WindowState.Normal;
            }

            m_LastClickTime = DateTime.Now.Ticks;
        }

        private bool IsPointInBounds(Point cursorPosition, FrameworkElement control)
        {
            Point rpt = control.PointFromScreen(cursorPosition);
            return rpt.X > 0 && rpt.Y > 0 && rpt.X < control.ActualWidth && rpt.Y < control.ActualHeight;
        }

        /// <summary>Returns true if the current application has focus, false otherwise</summary>
        public static bool ApplicationIsActivated()
        {
            var activatedHandle = GetForegroundWindow();
            if (activatedHandle == IntPtr.Zero)
            {
                return false;       // No window is currently activated
            }

            var procId = Process.GetCurrentProcess().Id;
            int activeProcId;
            GetWindowThreadProcessId(activatedHandle, out activeProcId);

            return activeProcId == procId;
        }

        private void OnVlcControlNeedsLibDirectory(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            if (IntPtr.Size == 4)
                e.VlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, @"libvlc\x86\"));
            else
                e.VlcLibDirectory = new DirectoryInfo(System.IO.Path.Combine(currentDirectory, @"libvlc\x64\"));
        }

        private void LoadVideoFileFromDrop(string file)
        {
            VideoFile = file;

            if (MyControl.MediaPlayer != null)
            {
                MyControl.MediaPlayer.Stop();
                MyControl.MediaPlayer.SetMedia(new FileInfo(VideoFile));
                MyControl.MediaPlayer.Play();
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (MyControl.MediaPlayer != null && VideoFile != null)
            {
                ProgressBarProgress.Value = MyControl.MediaPlayer.Time;
                /*if (mePlayer.NaturalDuration.HasTimeSpan)
                {
                    lblStatus.Content = String.Format("{0} / {1}", mePlayer.Position.ToString(@"mm\:ss"), mePlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss"));
                    if (!sliderSeekdragStarted)
                        sliderPosition.Value = mePlayer.Position.TotalSeconds;
                    ProgressBarProgress.Value = sliderPosition.Value;
                }*/
            }
            else
                lblStatus.Content = "No file selected...";
        }

        public void Play()
        {
            //MyControl.MediaPlayer.Play(new FileInfo(@"H:\Movie\Мультфильмы\Gravity Falls_(2012)-WEB-DLRip_1s_nnm\Gravity.Falls.S01E06.rus.eng.avi"));
            MyControl.MediaPlayer.Play(new FileInfo(VideoFile));
        }

        public void Stop()
        {
            MyControl.MediaPlayer.Stop();
        }

        public void ShowOnMonitor(int monitor)
        {
            var screen = ScreenHandler.GetScreen(monitor);
            var currentScreen = ScreenHandler.GetCurrentScreen(this);
            WindowState = WindowState.Normal;
            Left = screen.WorkingArea.Left;
            Top = screen.WorkingArea.Top;
            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;
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

        private MouseButton ToMouseButton(System.Windows.Forms.MouseButtons button)
        {
            switch (button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    return MouseButton.Left;
                case System.Windows.Forms.MouseButtons.Right:
                    return MouseButton.Right;
                case System.Windows.Forms.MouseButtons.Middle:
                    return MouseButton.Middle;
                case System.Windows.Forms.MouseButtons.XButton1:
                    return MouseButton.XButton1;
                case System.Windows.Forms.MouseButtons.XButton2:
                    return MouseButton.XButton2;
            }
            throw new InvalidOperationException();
        }

        #region Window Drag
        private bool m_IsDragInProgress;
        private System.Windows.Point m_FormMousePosition;
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                CaptureMouse();
                m_IsDragInProgress = true;
                m_FormMousePosition = e.GetPosition((UIElement)this);
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
            if (e.ChangedButton == MouseButton.Left)
            {
                this.m_IsDragInProgress = false;
                this.ReleaseMouseCapture();
            }
            base.OnMouseUp(e);
        }

        #endregion

        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                Panel.SetZIndex(GridControl, 999999999);
                //Panel.SetZIndex(MyControl, -1);
                //IsCtrlDown = false;
            }

            if (e.Key == Key.Space)
            {
                if (MyControl.MediaPlayer != null)
                {
                    if (CurrentState == MediaStates.Playing)
                        MyControl.MediaPlayer.Pause();
                    else if (CurrentState == MediaStates.Paused || CurrentState == MediaStates.Stopped)
                        MyControl.MediaPlayer.Play();

                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Right || e.Key == Key.Left)
            {
                if (MyControl.MediaPlayer != null)
                {
                    TimeSpan step = new TimeSpan(0, 0, 5);
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        step = new TimeSpan(0, 0, 15);
                    }

                    // MyControl.MediaPlayer.Pause();

                    if (e.Key == Key.Right)
                    {
                        var nt = MyControl.MediaPlayer.Time += (long)step.TotalMilliseconds;
                        if (nt >= MyControl.MediaPlayer.Length)
                            Stop();
                        MyControl.MediaPlayer.Time = nt;
                    }
                    if (e.Key == Key.Left)
                    {
                        var nt = MyControl.MediaPlayer.Time -= (long)step.TotalMilliseconds;
                        if (nt < 0)
                            nt = 0;
                        MyControl.MediaPlayer.Time = nt;
                    }

                    //MyControl.MediaPlayer.Play();
                    sliderPosition.Value = MyControl.MediaPlayer.Position;
                    ProgressBarProgress.Value = MyControl.MediaPlayer.Position;
                }
            }
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftCtrl || e.Key == Key.RightCtrl)
            {
                //IsCtrlDown = true;
            }

            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                IsFullScreen = !IsFullScreen;
            }

            if (e.Key == Key.Down)
            {
                Volume -= 10;
                if (MyControl.MediaPlayer.Audio != null)
                    MyControl.MediaPlayer.Audio.Volume = (int)Volume;

                ProgressBarVolume.Value = Volume;
            }

            if (e.Key == Key.Up)
            {
                Volume += 10;
                if (MyControl.MediaPlayer.Audio != null)
                    MyControl.MediaPlayer.Audio.Volume = (int)Volume;

                ProgressBarVolume.Value = Volume;
            }

            if (e.Key == Key.Escape)
            {
                Stop();
                Close();
                e.Handled = true;
            }
        }

        internal void Pause()
        {
            if (MyControl.MediaPlayer != null)
            {
                MyControl.MediaPlayer.Pause();
            }
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

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (MyControl.MediaPlayer.Audio != null)
                SetAudioVolume(MyControl.MediaPlayer.Audio.Volume + (int)(Math.Sign(e.Delta) * m_VolumeSens));

            ProgressBarVolume.Value = Volume;

            OnVolumeValueChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetAudioVolume(int volume)
        {
            if (volume < 0)
                volume = 0;
            if (volume > 250)
                volume = 250;

            Volume = MyControl.MediaPlayer.Audio.Volume = volume;
        }

        private void SliderPosition_ThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (MyControl.MediaPlayer != null)
            {
                MyControl.MediaPlayer.Time = (long)(MyControl.MediaPlayer.Time * sliderPosition.Value / 100.0);
            }

            m_SliderSeekdragStarted = false;
        }

        private void SliderPosition_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (MyControl.MediaPlayer == null) return;

            if (DateTime.Now - m_SliderSeekMouseDownStart > TimeSpan.FromMilliseconds(300))
                SliderPosition_ThumbDragCompleted(null, null);
            else
            {
                var pos = e.GetPosition(sliderPosition);

                //sliderPosition.Value = SmoothStep(pos.X, 0, sliderPosition.ActualWidth, sliderPosition.Minimum, sliderPosition.Maximum);
                sliderPosition.Value = (long)(MyControl.MediaPlayer.Time * sliderPosition.Value / 100.0);

                m_SliderSeekdragStarted = false;
            }
        }

        private void SliderPosition_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            m_SliderSeekMouseDownStart = DateTime.Now;
            m_SliderSeekdragStarted = true;
        }

        private void SliderPosition_ThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            m_SliderSeekdragStarted = true;
        }

        private void SliderPosition_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Приостановка воспроизведения перед переходом в другую позицию исключит "заикания" при слишком быстрых движениях ползунка.
            if (MyControl.MediaPlayer != null && m_SliderSeekdragStarted)
            {
                MyControl.MediaPlayer.Time = (long)sliderPosition.Value;
                //ProgressBarProgress.Value = sliderPosition.Value;
            }
        }

        private void VideoPlaybackFullScreen_Click(object sender, RoutedEventArgs e)
        {
            IsFullScreen = !IsFullScreen;
        }

        private void MenuItemOpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = System.IO.Path.GetDirectoryName(VideoFile),
                UseShellExecute = true,
                Verb = "open"
            });
        }

        private void VideoPlaybackExit_Click(object sender, RoutedEventArgs e)
        {
            Stop();
            Close();
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            //if (m_ActHook != null)
            //    m_ActHook.Stop();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                MyControl.Margin = new Thickness(0, 0, 0, 0);
            }
            else if (WindowState == WindowState.Normal)
            {
                MyControl.Margin = new Thickness(0, 0, 0, 30);
            }
        }

        private void ProgressBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                var value = (float)(e.GetPosition(ProgressBarProgress).X / ProgressBarProgress.ActualWidth) * MyControl.MediaPlayer.Length;
                MyControl.MediaPlayer.Time = (long)value;
                ProgressBarProgress.Value = value;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Stop();
        }

        private void GridContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            e.Handled = m_IsDragInProgress;
        }
    }
}
