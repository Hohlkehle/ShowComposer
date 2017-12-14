using ShowComposer.Data;
using ShowComposer.DraggableUI;
using ShowComposer.Windows;
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
using System.Windows.Threading;
using ShowComposer.Core;

namespace ShowComposer.UserControls
{
    /// <summary>
    /// Interaction logic for VideoPlaybackControl.xaml
    /// </summary>
    public partial class VideoPlaybackControl : UserControl, IDraggableUIElement
    {
        public static event EventHandler OnPlay, OnStop;
        public event EventHandler OnRemove;

        private DispatcherTimer dispatcherTimer;

        //private bool sliderSeekdragStarted;
        //private DateTime sliderSeekMouseDownStart;
        private string m_VideoFile;
        private VLCVideoPlaybackWindow m_VideoPlaybackWindow;
        private bool m_IsRelativePath;

        public bool IsRelativePath
        {
            get { return m_IsRelativePath; }
            set { m_IsRelativePath = value; }
        }

        public string VideoFile
        {
            get
            {
                var path = m_VideoFile;
                if (IsRelativePath)
                {
                    path = System.IO.Path.Combine(
                      System.IO.Path.GetDirectoryName(MainWindow.ProjectFileName),
                      m_VideoFile);
                }
                return path;
            }
            set { m_VideoFile = value; LoadTrackTags(); }
        }

        public VideoPlaybackControl()
        {
            OnPlay += (object sender, EventArgs e) =>
            {
                var item = (VideoPlaybackControl)sender;
                if (item != this && m_VideoPlaybackWindow != null && m_VideoPlaybackWindow.CurrentState == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                {
                    ButtonStopCommand_Click(null, null);
                }
            };

            InitializeComponent();

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);

            MainWindow.OnUIElementSelected += MainWindow_OnUIElementSelected;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (m_VideoPlaybackWindow != null && m_VideoPlaybackWindow.VideoFile != null)
                TextBlockTotalTime.Text = (string)m_VideoPlaybackWindow.lblStatus.Content;
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                && BorderContour.Visibility == System.Windows.Visibility.Hidden)
                BorderContour.Visibility = System.Windows.Visibility.Visible;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                && BorderContour.Visibility == System.Windows.Visibility.Visible)
                BorderContour.Visibility = System.Windows.Visibility.Hidden;
        }

        void MainWindow_OnUIElementSelected(object sender, LayoutEditorSelectionEventArgs e)
        {
            if (this.Equals(e.UIElement) || (e.UIElement == null && !e.IsSelected))
                BorderContour.Visibility = e.IsSelected ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }



        #region Playback
        public void Play()
        {
            if (String.IsNullOrEmpty(VideoFile) || !System.IO.File.Exists(VideoFile))
                OnOpenFileClick(null, null);

            if (String.IsNullOrEmpty(VideoFile))
                return;

            int externalPlayer = MainWindow.iniFile.GetInt32("Externals", "ExternalVideoPlayback", 0);

            //if (externalPlayer == 1)
            if (Preferences.OutputDevice == "NullOut")
            {
                StartBuildinVLCPlayer();
                return;
            }

            if ((VideoPlaybackOptions)Preferences.VideoPlaybackDevice.Value == VideoPlaybackOptions.BuildinVLCPlayer)
            {
                StartBuildinVLCPlayer();
            }
            else if ((VideoPlaybackOptions)Preferences.VideoPlaybackDevice.Value == VideoPlaybackOptions.InternalVLCRenderer)
            {
                StartInternalVLCRenderer();
            }
            else if ((VideoPlaybackOptions)Preferences.VideoPlaybackDevice.Value == VideoPlaybackOptions.EmbededPlayer)
            {
                StartEmbededPlayer();
            }
            else if ((VideoPlaybackOptions)Preferences.VideoPlaybackDevice.Value == VideoPlaybackOptions.InstalledVLCPlayer)
            {
                StartInstalledVLCPlayer();
            }
            else if ((VideoPlaybackOptions)Preferences.VideoPlaybackDevice.Value == VideoPlaybackOptions.WindowMediaPlayer)
            {
                StartWindowMediaPlayer();
            }



            /*
             Dispatcher.Invoke(delegate { InvokeSetVolumeDelegate((float)SliderVolume.Value); });
             System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
             {
                 waveOut.Play();
             }));*/
        }

        private void StartEmbededPlayer()
        {
            var wnd = new VideoPlaybackWindow();
            wnd.Show();
            if (ScreenHandler.AllScreens > 1)
            {
                wnd.ShowOnMonitor(1);
            }
            wnd.VideoFile = VideoFile;
            wnd.Volume = SoundVolume * 100;
            wnd.Play();
        }

        private void StartInternalVLCRenderer()
        {
            if (m_VideoPlaybackWindow != null)
            {
                if (m_VideoPlaybackWindow.CurrentState == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Playing)
                {
                    return;
                }
                else if (m_VideoPlaybackWindow.CurrentState == Vlc.DotNet.Core.Interops.Signatures.MediaStates.Paused)
                {
                    m_VideoPlaybackWindow.Play();

                    return;
                }
            }

            try
            {
                m_VideoPlaybackWindow = new VLCVideoPlaybackWindow();
                m_VideoPlaybackWindow.VideoFile = VideoFile;
                m_VideoPlaybackWindow.Volume = SoundVolume * 100;
                m_VideoPlaybackWindow.Show();
                if (ScreenHandler.AllScreens > 1)
                {
                    m_VideoPlaybackWindow.ShowOnMonitor(1);
                }
                m_VideoPlaybackWindow.WindowState = WindowState.Maximized;
                m_VideoPlaybackWindow.Play();

                m_VideoPlaybackWindow.Closing += m_VideoPlaybackWindow_Closing;
                m_VideoPlaybackWindow.OnVolumeValueChanged += m_VideoPlaybackWindow_OnVolumeValueChanged;
                m_VideoPlaybackWindow.MediaEnded += mePlayer_MediaEnded;
                dispatcherTimer.Start();
            }
            catch (Exception initException)
            {
                MessageBox.Show(String.Format("initException {0}", initException.Message), "Error Initializing Video Output");
                return;
            }
        }

        private void StartBuildinVLCPlayer()
        {
            string output;
            var vlc = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "vlc-" + Properties.Settings.Default.VLCVersion, "vlc.exe");
            if (System.IO.File.Exists(vlc))
                FileExecutionHelper.ExecutionHelper.RunCmdCommand(
                           "\"" + vlc + "\" " +
                           "\"" + VideoFile + "\"" +
                           " --fullscreen --qt-fullscreen-screennumber=2", out output, false, 1251);
            else if (System.IO.File.Exists(Properties.Settings.Default.VLCSystemPath))
            {
                StartInstalledVLCPlayer();
            }
            else
            {
                StartWindowMediaPlayer();
            }
        }

        private void StartWindowMediaPlayer()
        {
            string output;
            string player = System.IO.Path.Combine(System.IO.Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), Properties.Settings.Default.WMPSystemPath);

            FileExecutionHelper.ExecutionHelper.RunCmdCommand(
                   "\"" + player + "\" " +
                   "\"" + VideoFile + "\"" +
                   " /fullscreen /display:2", out output, false, 1251);
            return;
        }

        private void StartInstalledVLCPlayer()
        {
            //if (externalPlayer == 1)
            string output;
            string player = MainWindow.iniFile.GetString("Externals", "VideoPlayer", System.IO.Path.Combine(System.IO.Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), Properties.Settings.Default.VLCSystemPath));

            FileExecutionHelper.ExecutionHelper.RunCmdCommand(
                   "\"" + player + "\" " +
                   "\"" + VideoFile + "\"", out output, false, 1251);
            return;
        }

        void mePlayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                ButtonPauseCommand.Visibility = System.Windows.Visibility.Collapsed;
                ButtonPlayCommand.Visibility = System.Windows.Visibility.Visible;
            });

            dispatcherTimer.Stop();
        }

        void m_VideoPlaybackWindow_OnVolumeValueChanged(object sender, EventArgs e)
        {
            SliderVolume.Value = m_VideoPlaybackWindow.Volume;
            ProgressBarVolume.Value = SliderVolume.Value;
        }

        void m_VideoPlaybackWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ButtonStopCommand_Click(null, null);
        }


        void LoadTrackTags()
        {
            if (String.IsNullOrEmpty(VideoFile) || !System.IO.File.Exists(VideoFile))
                return;

            TagLib.File file = TagLib.File.Create(VideoFile);
            string title = file.Tag.Title;
            string performers = string.Join(", ", file.Tag.Performers);
            if (string.IsNullOrEmpty(title))
            {
                TextBlockTrackTitle.Text = string.Format("{0}", System.IO.Path.GetFileName(file.Name));
            }
            else
                TextBlockTrackTitle.Text = string.Format("{0} - {1}", string.Join(", ", file.Tag.Performers), title);

            var image = file as TagLib.Image.File;
            if (image != null)
            { }
        }

        public bool Stop()
        {
            try
            {
                m_VideoPlaybackWindow.Close();
            }
            catch { };
            return true;
        }

        #endregion

        void OnPlaybackStopped(object sender, EventArgs e)
        {


        }

        private void ButtonPlayCommand_Click(object sender, RoutedEventArgs e)
        {
            Play();

            ButtonPlayCommand.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPauseCommand.Visibility = System.Windows.Visibility.Visible;


            if (OnPlay != null)
                OnPlay(this, EventArgs.Empty);
        }

        private void ButtonPauseCommand_Click(object sender, RoutedEventArgs e)
        {
            ButtonPauseCommand.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPlayCommand.Visibility = System.Windows.Visibility.Visible;

            if (m_VideoPlaybackWindow != null)
            {
                m_VideoPlaybackWindow.Pause();
            }
        }

        private void ButtonStopCommand_Click(object sender, RoutedEventArgs e)
        {
            Stop();

            ButtonPauseCommand.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPlayCommand.Visibility = System.Windows.Visibility.Visible;


            dispatcherTimer.Stop();
        }

        private void OnOpenFileClick(object sender, EventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            string allExtensions = "*.AVI;*.MP4;*.DIVX;*.WMV;*.MKV";
            openFileDialog.Filter = String.Format("All Supported Files|{0}|All Files (*.*)|*.*", allExtensions);
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                VideoFile = openFileDialog.FileName;
            }
        }

        private void myGrid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.Copy; // Okay
            else
                e.Effects = DragDropEffects.None; // Unknown data, ignore it
        }

        private void myGrid_DragLeave(object sender, DragEventArgs e)
        { }

        private void myGrid_Drop(object sender, DragEventArgs e)
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
            {
                // Reset the label text.
                //DropLocationLabel.Content = "None";
            }
        }

        private void LoadVideoFileFromDrop(string file)
        {
            ButtonStopCommand_Click(this, null);
            VideoFile = file;
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_VideoPlaybackWindow != null)
            {
                m_VideoPlaybackWindow.Volume = SliderVolume.Value;
            }

            if (ProgressBarVolume != null)
                ProgressBarVolume.Value = SliderVolume.Value;
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




        ~VideoPlaybackControl()
        {
            MainWindow.OnUIElementSelected -= MainWindow_OnUIElementSelected;
        }

        public bool ClutchElement()
        {
            return true;
        }

        private void AudioPlaybackShowProperties_Click(object sender, RoutedEventArgs e)
        {

        }



        public double SoundVolume { get { return SliderVolume.Value; } set { SliderVolume.Value = value; ProgressBarVolume.Value = value; } }

        private void ButtonCloseCommand_Click(object sender, RoutedEventArgs e)
        {
            if (OnRemove != null && MessageBox.Show("Delete slected item?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Stop();
                OnRemove(this, e);
            }
        }


        private void AudioPlaybackExclusive_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemOpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            if (System.IO.File.Exists(VideoFile))
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
                {
                    FileName = System.IO.Path.GetDirectoryName(VideoFile),
                    UseShellExecute = true,
                    Verb = "open"
                });
        }
    }
}
