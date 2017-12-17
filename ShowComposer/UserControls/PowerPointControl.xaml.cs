using ShowComposer.Core;
using ShowComposer.DraggableUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace ShowComposer.UserControls
{
    /// <summary>
    /// Interaction logic for powerPointControl.xaml
    /// </summary>
    public partial class PowerPointControl : UserControl, IDraggableUIElement
    {
        public static event EventHandler OnPlay;
        public event EventHandler OnRemove;

        private DispatcherTimer m_DispatcherTimer;

        private string m_PresenterFile;
        //private VideoPlaybackWindow m_VideoPlaybackWindow;
        private bool m_IsRelativePath;

        public bool IsRelativePath
        {
            get { return m_IsRelativePath; }
            set { m_IsRelativePath = value; }
        }

        public string PresenterFile
        {
            get
            {
                var path = m_PresenterFile;
                if (IsRelativePath)
                {
                    path = System.IO.Path.Combine(
                      System.IO.Path.GetDirectoryName(MainWindow.ProjectFileName),
                      m_PresenterFile);
                }
                return path;
            }
            set
            {
                m_PresenterFile = value;
                TextBlockTrackTitle.Text = string.Format("{0}", System.IO.Path.GetFileName(m_PresenterFile));
            }
        }

        public PowerPointControl()
        {
            OnPlay += (object sender, EventArgs e) =>
            {
                //var item = (VideoPlaybackControl)sender;
                //if (item != this && m_VideoPlaybackWindow != null && m_VideoPlaybackWindow.CurrentState == MediaState.Play)
                //{
                //    ButtonStopCommand_Click(null, null);
                //}
            };

            InitializeComponent();

            m_DispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            m_DispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            m_DispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);

            MainWindow.OnUIElementSelected += MainWindow_OnUIElementSelected;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            //if (m_VideoPlaybackWindow.mePlayer.Source != null)
            //    TextBlockTotalTime.Text = (string)m_VideoPlaybackWindow.lblStatus.Content;
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

        private void MainWindow_OnUIElementSelected(object sender, LayoutEditorSelectionEventArgs e)
        {
            if (this.Equals(e.UIElement) || (e.UIElement == null && !e.IsSelected))
                BorderContour.Visibility = e.IsSelected ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
        }
        
        #region Playback
        public void Play()
        {
            if (String.IsNullOrEmpty(PresenterFile) || !System.IO.File.Exists(PresenterFile))
                OnOpenFileClick(null, null);

            if (String.IsNullOrEmpty(PresenterFile))
                return;

            //if (m_VideoPlaybackWindow != null)
            //{
            //    if (m_VideoPlaybackWindow.CurrentState == MediaState.Play)
            //    {
            //        return;
            //    }
            //    else if (m_VideoPlaybackWindow.CurrentState == MediaState.Pause)
            //    {
            //        m_VideoPlaybackWindow.mePlayer.Play();

            //        return;
            //    }
            //}

            try
            {
                Process myProcess = new Process();
                myProcess.StartInfo.FileName = "POWERPNT.exe"; //not the full application path
                myProcess.StartInfo.Arguments = "/s  \"" + PresenterFile + "\"";
                myProcess.Start();



                //ProcessStartInfo startInfo = new ProcessStartInfo(PresenterFile);
                //startInfo.Arguments = "/s";
                //Process.Start(startInfo);

                //System.Diagnostics.Process.Start(PresenterFile, "/s");

                m_DispatcherTimer.Start();
            }
            catch (Exception initException)
            {
                MessageBox.Show(String.Format("initException {0}", initException.Message), "Error Initializing Video Output");
                return;
            }
            /*
             Dispatcher.Invoke(delegate { InvokeSetVolumeDelegate((float)SliderVolume.Value); });
             System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
             {
                 waveOut.Play();
             }));*/
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            ButtonPauseCommand.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPlayCommand.Visibility = System.Windows.Visibility.Visible;


            m_DispatcherTimer.Stop();
        }

        private void VideoPlaybackWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ButtonStopCommand_Click(null, null);
        }

        public bool Stop()
        {
            try
            {
                var POWERPNT = Process.GetProcessesByName("POWERPNT");
                POWERPNT[POWERPNT.Length - 1].CloseMainWindow();
                //POWERPNT
            }
            catch { };
            return true;
        }

        #endregion

        private void OnPlaybackStopped(object sender, EventArgs e)
        {


        }

        private void ButtonPlayCommand_Click(object sender, RoutedEventArgs e)
        {
            Play();

            ButtonPlayCommand.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPauseCommand.Visibility = System.Windows.Visibility.Visible;

            OnPlay?.Invoke(this, EventArgs.Empty);
        }

        private void ButtonPauseCommand_Click(object sender, RoutedEventArgs e)
        {
            ButtonPauseCommand.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPlayCommand.Visibility = System.Windows.Visibility.Visible;

            ButtonStopCommand_Click(null, null);
        }

        private void ButtonStopCommand_Click(object sender, RoutedEventArgs e)
        {
            Stop();

            ButtonPauseCommand.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPlayCommand.Visibility = System.Windows.Visibility.Visible;


            m_DispatcherTimer.Stop();
        }

        private void OnOpenFileClick(object sender, EventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            string allExtensions = "*" + string.Join(";*", CommandHelper.presentExtensions);
            openFileDialog.Filter = String.Format("All Supported Files|{0}|All Files (*.*)|*.*", allExtensions);
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PresenterFile = openFileDialog.FileName;
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
            {
                // Reset the label text.
                //DropLocationLabel.Content = "None";
            }
        }

        private void LoadVideoFileFromDrop(string file)
        {
            ButtonStopCommand_Click(this, null);
            PresenterFile = file;
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

        ~PowerPointControl()
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
    }
}
