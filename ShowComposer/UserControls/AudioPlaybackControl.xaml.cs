using NAudio.CoreAudioApi;
using NAudio.Flac;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ShowComposer.Data;
using ShowComposer.DraggableUI;
using ShowComposer.NAudioDemo.AudioPlaybackDemo;
using ShowComposer.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for AudioPlaybackControl.xaml
    /// </summary>
    public partial class AudioPlaybackControl : UserControl, IDraggableUIElement
    {
        public static event EventHandler OnTryPlay, OnPlay, OnStop;
        public event EventHandler OnRemove;
        private IWavePlayer waveOut;

      
        private string m_AudioFile = null;
        private WaveStream audioFileReader;

      
        private Action<float> InvokeSetVolumeDelegate;
        // Create a Object of BackgroundWorker Class
        private BackgroundWorker WorkerThread = new BackgroundWorker();
        DispatcherTimer dispatcherTimer;
        IOutputDevicePlugin SelectedOutputDevicePlugin;
        bool sliderSeekdragStarted;
        DateTime sliderSeekMouseDownStart;
        bool m_IsExclusivePlayback = false;
        bool m_IsRelativePath;

        public IWavePlayer WaveOutPlayer
        {
            get { return waveOut; }
            set { waveOut = value; }
        }

        public WaveStream AudioFileReader
        {
            get { return audioFileReader; }
            set { audioFileReader = value; }
        }

        public bool IsRelativePath
        {
            get { return m_IsRelativePath; }
            set { m_IsRelativePath = value; }
        }

        public bool IsExclusivePlayback
        {
            get { return m_IsExclusivePlayback; }
            set
            {
                m_IsExclusivePlayback = value;
                BorderBackground.Background = IsExclusivePlayback
                    ? new SolidColorBrush(Color.FromArgb(200, 47, 76, 48))
                    : new SolidColorBrush(Color.FromArgb(200, 20, 20, 24));
            }
        }
        public ICommand PlayCommand { get; private set; }

        public ICommand StopCommand { get; private set; }

        public bool IsPlaying { get { return (waveOut != null && waveOut.PlaybackState == PlaybackState.Playing); } }

        public string AudioFile
        {
            get
            {
                var path = m_AudioFile;
                if (IsRelativePath)
                {
                    path = System.IO.Path.Combine(
                      System.IO.Path.GetDirectoryName(MainWindow.ProjectFileName),
                      m_AudioFile);
                }
                return path;
            }
            set { m_AudioFile = value; LoadTrackTags(); }
        }

        public AudioPlaybackControl()
        {

            OnPlay += (object sender, EventArgs e) =>
            {
                var apcItem = (AudioPlaybackControl)sender;
                if (!IsExclusivePlayback && !apcItem.IsExclusivePlayback && apcItem != this && waveOut != null && waveOut.PlaybackState == PlaybackState.Playing)
                {
                    ButtonStopCommand_Click(null, null);
                }

                if(apcItem == this)
                {
                    BorderContour.Visibility = Visibility.Visible;
                    //BorderBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CC002db3"));
                }
            };

            OnStop += (object sender, EventArgs e) =>
            {
                var apcItem = (AudioPlaybackControl)sender;
                
                if (apcItem == this)
                {
                    BorderContour.Visibility = Visibility.Collapsed;
                    //BorderBackground.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E1141418"));
                }
            };
            ////Yes you can. Create the directory as normal then just set the attributes on it. E.g.

            ////DirectoryInfo di = new DirectoryInfo(@"C:\SomeDirectory");

            ////See if directory has hidden flag, if not, make hidden
            //if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            //{   
            //     //Add Hidden flag    
            //     di.Attributes |= FileAttributes.Hidden;    
            //}



            InitializeComponent();

            //InitializeBackgroundWorker();

            //PlayCommand = new DelegateCommand(Play);
            //StopCommand = new DelegateCommand(Stop);

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);

            MainWindow.OnUIElementSelected += MainWindow_OnUIElementSelected;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (waveOut != null && audioFileReader != null)
            {
                TimeSpan currentTime = (waveOut.PlaybackState == PlaybackState.Stopped) ? TimeSpan.Zero : audioFileReader.CurrentTime;

                if (!sliderSeekdragStarted)
                    SliderSeek.Value = Math.Min(SliderSeek.Maximum, (int)(100 * currentTime.TotalSeconds / audioFileReader.TotalTime.TotalSeconds));
                ProgressBarPosition.Value = SliderSeek.Value;

                TextBlockPlayedTime.Text = String.Format("{0:00}:{1:00}", (int)currentTime.TotalMinutes, currentTime.Seconds);
                TextBlockTotalTime.Text = String.Format("{0:00}:{1:00}", (int)audioFileReader.TotalTime.TotalMinutes, audioFileReader.TotalTime.Seconds);

                if (waveOut.PlaybackState == PlaybackState.Stopped)
                {

                }
            }
            else
            {
                SliderSeek.Value = ProgressBarPosition.Value = 0;
            }
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

        #region InitializeBackgroundWorker
        /// <summary>
        /// Initialize Background Worker
        /// </summary>
        private void InitializeBackgroundWorker()
        {
            try
            {
                WorkerThread.WorkerReportsProgress = true;
                WorkerThread.DoWork += new DoWorkEventHandler(WorkerThread_DoWork);
                WorkerThread.ProgressChanged += new ProgressChangedEventHandler(WorkerThread_ProgressChanged);
                WorkerThread.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerThread_RunWorkerCompleted);
                WorkerThread.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        void WorkerThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ProgressBarPosition.Value = ProgressBarPosition.Maximum;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " WorkerThread_RunWorkerCompleted");
            }
        }

        void WorkerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                ProgressBarPosition.Value = e.ProgressPercentage;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " WorkerThread_ProgressChanged");
            }
        }

        void WorkerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                for (var i = 0; i < 100; i++)
                {
                    Thread.Sleep(300);
                    WorkerThread.ReportProgress(i);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region Playback
        public void Play()
        {
            if (String.IsNullOrEmpty(AudioFile) || !System.IO.File.Exists(AudioFile))
                OnOpenFileClick(null, null);

            if (String.IsNullOrEmpty(AudioFile))
                return;

            switch (Preferences.OutputDevice)
            {
                case "WaveOut":
                    for (int deviceId = 0; deviceId < WaveOut.DeviceCount; deviceId++)
                    {
                        var capabilities = WaveOut.GetCapabilities(deviceId);
                    }

                    WaveCallbackStrategy strategy = WaveCallbackStrategy.NewWindow;
                    if (Preferences.WaveOutCallback == "Function")
                        strategy = WaveCallbackStrategy.FunctionCallback;
                    if (Preferences.WaveOutCallback == "Window")
                        strategy = WaveCallbackStrategy.NewWindow;
                    if (Preferences.WaveOutCallback == "Event")
                        strategy = WaveCallbackStrategy.Event;

                    SelectedOutputDevicePlugin = new WaveOutPlugin(strategy, Preferences.WaveOutDevice);

                    break;
                case "WasapiOut":
                    var endPoints = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
                    SelectedOutputDevicePlugin = new WasapiOutPlugin(
                        endPoints[Preferences.WasapiOutDevice],
                        Preferences.WasapiOutExclusiveMode == "True" ? AudioClientShareMode.Exclusive : AudioClientShareMode.Shared,
                        Preferences.WasapiOutIsEventCallback == "True",
                        Preferences.RequestedLatency);

                    break;
                  
                case "NullOut":
                    string output;
                    string player = MainWindow.iniFile.GetString("Externals", "AudioPlayer", "C:\\Program Files (x86)\\AIMP2\\AIMP2.exe");
                    if (!player.Contains(":"))
                    {
                        //player = System.IO.Path.Combine(Environment.CurrentDirectory, player);
                    }
                    FileExecutionHelper.ExecutionHelper.RunCmdCommand(
                        "\"" + player +"\" " +
                        "\"" + AudioFile +"\"", out output, false, 1251);
                    return;
                    //break;
                case "DirectSound":
                default:
                    SelectedOutputDevicePlugin = new DirectSoundOutPlugin();
                    break;
            }




            if (!SelectedOutputDevicePlugin.IsAvailable)
            {
                MessageBox.Show("The selected output driver is not available on this system");
                return;
            }

            if (waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    return;
                }
                else if (waveOut.PlaybackState == PlaybackState.Paused)
                {
                    System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
                    {
                        waveOut.Play();
                    }));

                    return;
                }
            }

            // we are in a stopped state
            try
            {
                CreateWaveOut();

                dispatcherTimer.Start();

                Dispatcher.Invoke(delegate { LoadTrackTags(); });

            }
            catch (TagLib.CorruptFileException tagLibCorruptFileException)
            {
                Dispatcher.Invoke(delegate { TextBlockTrackTitle.Text = string.Format("{0}  {1}", System.IO.Path.GetFileName(AudioFile), tagLibCorruptFileException.Message); });
            }
            catch (Exception driverCreateException)
            {
                MessageBox.Show(String.Format("driverCreateException {0}", driverCreateException.Message));
                return;
            }

            ISampleProvider sampleProvider = null;
            try
            {
                sampleProvider = CreateInputStream(AudioFile);
            }
            catch (Exception createException)
            {
                MessageBox.Show(String.Format("createException {0}", createException.Message), "Error Loading File");
                return;
            }

            try
            {
                //if (m_FadeInOutSampleProvider != null && !(audioFileReader is FlacReader))
                //    waveOut.Init(m_FadeInOutSampleProvider);
                //else
                waveOut.Init(sampleProvider);
            }
            catch (Exception initException)
            {
                MessageBox.Show(String.Format("initException {0}", initException.Message), "Error Initializing Output");
                return;
            }


            Dispatcher.Invoke(delegate { InvokeSetVolumeDelegate((float)SliderVolume.Value); });
            System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
            {
                waveOut.Play();
            }));

            //try
            //{
            //    System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
            //    {
            //        InitWaveformPainter(sampleProvider);
            //    }));

            //}
            //catch (Exception waveFormException)
            //{
            //    MessageBox.Show(String.Format("waveFormException {0}", waveFormException.Message), "Error Initializing Output");
            //    return;
            //}

        }

        void LoadTrackTags()
        {
            if (String.IsNullOrEmpty(AudioFile) || !System.IO.File.Exists(AudioFile))
                return;

            TagLib.File file = TagLib.File.Create(AudioFile);
            TextBlockTrackTitle.Text = string.Format("{0}", System.IO.Path.GetFileName(file.Name));
            
            //string title = file.Tag.Title;
            //string performers = string.Join(", ", file.Tag.Performers);
            //if (string.IsNullOrEmpty(title))
            //{
            //    TextBlockTrackTitle.Text = string.Format("{0}", System.IO.Path.GetFileName(file.Name));
            //}
            //else
            //    TextBlockTrackTitle.Text = string.Format("{0} - {1}", string.Join(", ", file.Tag.Performers), title);

            //var image = file as TagLib.Image.File;
            //if (image != null)
            //{ }
        }

        public bool Stop()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
            }

            OnStop?.Invoke(this, EventArgs.Empty);

            SliderSeek.Value = ProgressBarPosition.Value = 0;
            //m_AudioFile = "";

            return true;
        }

        public void TogglePlayback()
        {
            if (IsPlaying)
            {
                ButtonStopCommand_Click(this, null);
            }
            else
            {
                ButtonPlayCommand_Click(this, null);
            }
        }
        #endregion

        private ISampleProvider CreateInputStream(string fileName)
        {
            if (System.IO.Path.GetExtension(fileName).ToUpper() == ".FLAC")
            {
                this.audioFileReader = new FlacReader(fileName);
            }
            else
                this.audioFileReader = new AudioFileReader(fileName);

            if (!(audioFileReader is FlacReader))
            {
                m_FadeInOutSampleProvider = new FadeInOutSampleProvider((ISampleProvider)audioFileReader, false);

            }

            var sampleChannel = new SampleChannel(audioFileReader, true);
            sampleChannel.PreVolumeMeter += OnPreVolumeMeter;
            this.InvokeSetVolumeDelegate = (vol) =>
            {
                sampleChannel.Volume = vol;
                if (audioFileReader is AudioFileReader)
                    ((AudioFileReader)audioFileReader).Volume = vol;

            };
            var postVolumeMeter = new MeteringSampleProvider(((audioFileReader is FlacReader)) ? sampleChannel : (ISampleProvider)m_FadeInOutSampleProvider);
            postVolumeMeter.StreamVolume += OnPostVolumeMeter;

            return postVolumeMeter;
        }

        void OnPreVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            // we know it is stereo
            //waveformPainter1.AddMax(e.MaxSampleValues[0]);
            // waveformPainter2.AddMax(e.MaxSampleValues[1]);
        }

        void OnPostVolumeMeter(object sender, StreamVolumeEventArgs e)
        {
            // we know it is stereo
            System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
            {
                Dispatcher.Invoke(delegate
                {
                    // Update meter async
                    volumeMeter1.Amplitude = e.MaxSampleValues[0];
                });
            }));
            
            //volumeMeter2.Amplitude = e.MaxSampleValues[1];
        }

        private void InitWaveformPainter(ISampleProvider sampleProvider)
        {/*
            WaveStream _audioFileReader;
            if (System.IO.Path.GetExtension(AudioFile).ToUpper() == ".FLAC")
            {
                _audioFileReader = new FlacReader(AudioFile);
            }
            else
                _audioFileReader = new AudioFileReader(AudioFile);

            var sampleChannel = new SampleChannel(_audioFileReader, true);


            byte[] buffer1 = new byte[4096];
            int reader = 0;
            var memoryStream = new System.IO.MemoryStream();
            //while ((reader = _audioFileReader.Read(buffer1, 0, buffer1.Length)) != 0)
            //    memoryStream.Write(buffer1, 0, reader);
            int saplesCount = (int)(_audioFileReader.TotalTime.TotalSeconds * sampleChannel.WaveFormat.SampleRate * sampleChannel.WaveFormat.Channels);
            float[] samples = new float[saplesCount];
            float[] lsamples = new float[saplesCount / 2+1];
            float[] rsamples = new float[saplesCount / 2+1];

            sampleChannel.Read(samples, 0, samples.Length);

            byte[] buffer = memoryStream.ToArray();

            //byte[] buffer = new byte[_audioFileReader.Length];
            byte[] lbuffer = new byte[buffer.Length / 2];
            //int read = _audioFileReader.Read(buffer, 0, buffer.Length);

            //for (int i = 0, j = 0; i < lbuffer.Length; i += 2, j++)
            //{
            //    lbuffer[j] = buffer[i];
            //}

            for (int i = 0, j = 0, k = 0; i < samples.Length; i++)
            {
                if (i % 2 == 0)
                {
                    lsamples[j] = samples[i];
                    j++;
                }
                else
                {
                    rsamples[k] = samples[i];
                    k++;
                }
            }

            //var sc = new SamplesConverter();
            //sc.Data = buffer;
            //var samples = sc.getSamples(_audioFileReader, _audioFileReader.WaveFormat.BitsPerSample);

            //sampleProvider.WaveFormat.BitsPerSample
            WaveformPainter1.ClearSamples();
            int step = rsamples.Length / WaveformPainter1.MaxSamples;
            int cnt = 0;
            for (var i = 0; i < WaveformPainter1.MaxSamples;i++ )
            {
                float[] avgs = new float[step];
                var avgNeg = new List<float>();
                var avgPos = new List<float>();
                for (var j = 0; j < step; j++, cnt++)
                {
                    if (cnt >= rsamples.Length)
                        break;

                    if (rsamples[cnt] > 0)
                    {
                        avgPos.Add(rsamples[cnt]);
                        avgNeg.Add(0);
                    }
                    if (rsamples[cnt] <= 0)
                    {
                        avgNeg.Add(rsamples[cnt]);
                        avgPos.Add(0);
                    }
                    avgs[j] = rsamples[cnt];
                    
                }

         

                //WaveformPainter1.AddMax(avgNeg.Min());
             
                if (avgPos.Count > 0)
                    WaveformPainter1.PositiveSamples.Add(avgs.Max());
                if (avgNeg.Count > 0)
                    WaveformPainter1.NegativeSamples.Add(avgs.Min());

                //WaveformPainter1.AddMax(avg);
            }

            //for (var i = 0; i < lsamples.Length; i += step)
            //{
            //    WaveformPainter1.AddMax(lsamples[i]);
            //}
            WaveformPainter1.Invalidate();
            */


            /*
           
                 var audiobyteLen = _audioFileReader.Length;
                 var sampleChannel = new SampleChannel(_audioFileReader, true);

                 //var smplLng = audiobyteLen;// / sampleProvider.WaveFormat.BitsPerSample;
                 //float[] buffer = new float[smplLng];
                 //float[] lbuffer = new float[smplLng / 2];
                 //float[] rbuffer = new float[smplLng / 2];
                 //var length = sampleChannel.Read(buffer, 0, (int)smplLng/4);

                 //for (int i = 0, j = 0; i < lbuffer.Length; i += 2, j++)
                 //{
                 //    lbuffer[j] = buffer[i];
                 //}

                 //for (int i = 1, j = 0; i < rbuffer.Length; i += 2, j++)
                 //{
                 //    rbuffer[j] = buffer[i];
                 //}

                 int samplesDesired = (int)(audiobyteLen / 4 / 8);
                 byte[] buffer = new byte[samplesDesired * 4];
                 int[] left = new int[samplesDesired];
                 int[] right = new int[samplesDesired];
                 int bytesRead = _audioFileReader.Read(buffer, 0, (int)audiobyteLen/8);
                 _audioFileReader.Position = 0;
                 int index = 0;
                 for (int sample = 0; sample < bytesRead / 4; sample++)
                 {
                     left[sample] = BitConverter.ToInt16(buffer, index);
                     index += 2;
                     right[sample] = BitConverter.ToInt16(buffer, index);
                     index += 2;
                 }


                 int step = left.Length / WaveformPainter1.MaxSamples;

                 for (var i = 0; i < left.Length; i += step)
                 {
                     WaveformPainter1.AddMax(left[i]);
                 }
                 WaveformPainter1.Invalidate();
                 return;

 


                 int samplesDesired = (int)(audiobyteLen / 2);
                 byte[] buffer = new byte[samplesDesired * 4];
                 int[] left = new int[samplesDesired];
                 int[] right = new int[samplesDesired];
                 int bytesRead = _audioFileReader.Read(buffer, 0, (int)audiobyteLen);
                 _audioFileReader.Position = 0;
                 int index = 0;
                 for (int sample = 0; sample < bytesRead / 4; sample++)
                 {
                     left[sample] = BitConverter.ToInt16(buffer, index);
                     index += 2;
                     right[sample] = BitConverter.ToInt16(buffer, index);
                     index += 2;
                 }

                 var sc = new SamplesConverter();
                 sc.Amplitudes = left;
                 var samples = sc.getSamples(_audioFileReader, 24);
                 //sampleProvider.WaveFormat.BitsPerSample
                 int step = left.Length / WaveformPainter1.MaxSamples;
                 for (var i = 0; i < samples.Length; i += step)
                 {
                     WaveformPainter1.AddMax(samples[i]);
                 }

                 //foreach (var s in samples)
                 //{
                 //    WaveformPainter1.AddMax(s);
                 //}
                 WaveformPainter1.Invalidate();

            

            
            

                 WaveStream _audioFileReader;
                 if (System.IO.Path.GetExtension(AudioFile).ToUpper() == ".FLAC")
                 {
                     _audioFileReader = new FlacReader(AudioFile);
                 }
                 else
                     _audioFileReader = new AudioFileReader(AudioFile);


           



           
                 int size = sampleProvider.WaveFormat.BitsPerSample / 8;
                 var offset = 0;
                 var data = new byte[audiobyteLen / sampleProvider.WaveFormat.Channels];
                 var dataLen = data.Length;
                 // Use data from left channel.
                 while (offset < dataLen)
                 {
                     _audioFileReader.Read(data, offset, size);
                     //_audioFileReader.Seek(size, System.IO.SeekOrigin.Current);
                     offset += size;
                 }

                 _audioFileReader.Position = 0;

                 //float[] buffer = new float[smplLng];
                 //var length = sampleProvider.Read(buffer, 0, (int)smplLng);



          

                 var sc = new SamplesConverter();
                 sc.Data = data;
                 var samples = sc.getSamples(_audioFileReader, 24);
                 //sampleProvider.WaveFormat.BitsPerSample

                 for (var i = 0; i < samples.Length; i += 32)
                 {
                     WaveformPainter1.AddMax(samples[i]);
                 }
           
                 //foreach (var s in samples)
                 //{
                 //    WaveformPainter1.AddMax(s);
                 //}
                 WaveformPainter1.Invalidate();

                 //var size = sampleProvider.WaveFormat.ConvertLatencyToByteSize((int)audioFileReader.CurrentTime.TotalMilliseconds);
                 //float[] buffer = new float[size];
                 ////var length = sampleProvider.Read(buffer, 0);

                 //DbProviderFactory fact = DbProviderFactories.GetFactory("System.Data.SQLite");
                 //using (DbConnection cnn = fact.CreateConnection())
                 //{
                 //    cnn.ConnectionString = "Data Source=test.db3";
                 //    cnn.Open();
                 //}


                 //audioFileReader.*/

        }

        private void CreateWaveOut()
        {
            CloseWaveOut();
            var latency = Preferences.RequestedLatency;//(int)comboBoxLatency.SelectedItem;
            waveOut = SelectedOutputDevicePlugin.CreateDevice(latency);
            waveOut.PlaybackStopped += OnPlaybackStopped;
        }

        void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            //groupBoxDriverModel.Enabled = true;
            if (e.Exception != null)
            {
                MessageBox.Show(e.Exception.Message, "Playback Device Error");
            }
            if (audioFileReader != null)
            {
                audioFileReader.Position = 0;
            }

            ButtonPauseCommand.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPlayCommand.Visibility = System.Windows.Visibility.Visible;
            if (m_FadeInOutSampleProvider != null)
            {
                ButtonPlay1Command.Visibility = System.Windows.Visibility.Collapsed;
                ButtonPause1Command.Visibility = System.Windows.Visibility.Collapsed;
            }

            dispatcherTimer.Stop();

            SliderSeek.Value = ProgressBarPosition.Value = 0;
        }

        public void CloseWaveOut()
        {
            if (waveOut != null)
            {
                waveOut.Stop();
            }
            if (audioFileReader != null)
            {
                // this one really closes the file and ACM conversion
                audioFileReader.Dispose();
                InvokeSetVolumeDelegate = null;
                audioFileReader = null;
            }
            if (waveOut != null)
            {
                waveOut.Dispose();
                waveOut = null;
            }
        }

        private void ButtonPlayCommand_Click(object sender, RoutedEventArgs e)
        {
            OnTryPlay?.Invoke(this, EventArgs.Empty);

            Play();

            if (this.m_FadeInOutSampleProvider != null && !(audioFileReader is FlacReader))
            {
                this.m_FadeInOutSampleProvider.BeginFadeIn(100);
            }
            
            
            ButtonPlayCommand.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPauseCommand.Visibility = System.Windows.Visibility.Visible;
            if (m_FadeInOutSampleProvider != null && !(audioFileReader is FlacReader))
            {
                ButtonPlay1Command.Visibility = System.Windows.Visibility.Collapsed;
                ButtonPause1Command.Visibility = System.Windows.Visibility.Visible;
            }

            if (OnPlay != null && !IsExclusivePlayback)
                OnPlay(this, EventArgs.Empty);
        }

        private void ButtonPauseCommand_Click(object sender, RoutedEventArgs e)
        {
            
            ButtonPauseCommand.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPlayCommand.Visibility = System.Windows.Visibility.Visible;
            if (m_FadeInOutSampleProvider != null && !(audioFileReader is FlacReader))
            {
                ButtonPlay1Command.Visibility = System.Windows.Visibility.Visible;
                ButtonPause1Command.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    waveOut.Pause();
                }
            }
        }

        private void ButtonPlay1Command_Click(object sender, RoutedEventArgs e)
        {
            Play();

            ButtonPlayCommand.Visibility = System.Windows.Visibility.Collapsed;
            ButtonPauseCommand.Visibility = System.Windows.Visibility.Visible;
            if (m_FadeInOutSampleProvider != null && !(audioFileReader is FlacReader))
            {
                ButtonPlay1Command.Visibility = System.Windows.Visibility.Collapsed;
                ButtonPause1Command.Visibility = System.Windows.Visibility.Visible;
            }

            if (this.m_FadeInOutSampleProvider != null && !(audioFileReader is FlacReader))
            {
                this.m_FadeInOutSampleProvider.BeginFadeIn(2000);
            }

            if (OnPlay != null)
                OnPlay(this, EventArgs.Empty);
        }

        private void ButtonPause1Command_Click(object sender, RoutedEventArgs e)
        {
            ButtonPauseCommand.Visibility = System.Windows.Visibility.Visible;
            ButtonPlayCommand.Visibility = System.Windows.Visibility.Collapsed;
            if (m_FadeInOutSampleProvider != null && !(audioFileReader is FlacReader))
            {
                ButtonPlay1Command.Visibility = System.Windows.Visibility.Visible;
                ButtonPause1Command.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (waveOut != null)
            {
                if (waveOut.PlaybackState == PlaybackState.Playing)
                {
                    if (this.m_FadeInOutSampleProvider != null)
                    {
                        this.m_FadeInOutSampleProvider.BeginFadeOut(1600);
                    }
                    else
                        waveOut.Pause();
                }
            }
        }

        private void ButtonStopCommand_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void PBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }



        private void OnOpenFileClick(object sender, EventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog();
            string allExtensions = "*.wav;*.aiff;*.mp3;*.aac;*.flac";
            openFileDialog.Filter = String.Format("All Supported Files|{0}|All Files (*.*)|*.*", allExtensions);
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                AudioFile = openFileDialog.FileName;
            }
        }


        private void myGrid_DragEnter(object sender, DragEventArgs e)
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

        private void myGrid_DragLeave(object sender, DragEventArgs e)
        {
            //myScrollViewer.AllowDrop = false;
            //myScrollViewer.IsHitTestVisible = false;
        }

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

                    if (DeskLayout.IsAudioFile(fileList[0]))
                    {
                        LoadAudioFileFromDrop(fileList[0]);

                        e.Handled = true;
                    }
                    else
                    {
                        MessageBox.Show("Error occurs, only audio files allowed.");
                    }
                }

            }
            else
            {
                // Reset the label text.
                //DropLocationLabel.Content = "None";
            }
        }

        private void LoadAudioFileFromDrop(string file)
        {
            ButtonStopCommand_Click(this, null);
            AudioFile = file;
            //ButtonPlayCommand_Click(this, null);
        }


        private void SliderSeek_ThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (waveOut != null)
            {
                audioFileReader.CurrentTime = TimeSpan.FromSeconds(audioFileReader.TotalTime.TotalSeconds * SliderSeek.Value / 100.0);
            }

            sliderSeekdragStarted = false;
        }

        private void SliderSeek_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {

            if (e.ChangedButton != MouseButton.Left)
                return;

            if (waveOut == null) return;

            if (DateTime.Now - sliderSeekMouseDownStart > TimeSpan.FromMilliseconds(300))
                SliderSeek_ThumbDragCompleted(null, null);
            else
            {

                var pos = e.GetPosition(SliderSeek);

                SliderSeek.Value = SmoothStep(pos.X, 0, SliderSeek.ActualWidth, SliderSeek.Minimum, SliderSeek.Maximum);

                audioFileReader.CurrentTime = TimeSpan.FromSeconds(audioFileReader.TotalTime.TotalSeconds * SliderSeek.Value / 100.0);

                sliderSeekdragStarted = false;
            }
        }

        private void SliderSeek_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            sliderSeekMouseDownStart = DateTime.Now;
            sliderSeekdragStarted = true;
        }

        private void SliderSeek_ThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            sliderSeekdragStarted = true;
        }

        private void SliderSeek_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (waveOut != null && sliderSeekdragStarted)
            {
                audioFileReader.CurrentTime = TimeSpan.FromSeconds(audioFileReader.TotalTime.TotalSeconds * SliderSeek.Value / 100.0);
            }
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (InvokeSetVolumeDelegate != null)
            {
                InvokeSetVolumeDelegate((float)SliderVolume.Value);
                ProgressBarVolume.Value = SliderVolume.Value;
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




        ~AudioPlaybackControl()
        {
            MainWindow.OnUIElementSelected -= MainWindow_OnUIElementSelected;
        }

        public bool ClutchElement()
        {
            return true;
        }

        private void AudioPlaybackShowProperties_Click(object sender, RoutedEventArgs e)
        {
            var window = new AudioPlaybackPropertiesWindow();
            window.SetTarget(this);
            //window.SetTarget(CaptionInfo.Convert(new UIElement[] { SpanContent }).ElementAt<CaptionInfo>(0), this);

            if (window.ShowDialog() == true)
            {
                ButtonStopCommand_Click(this, null);
                AudioFile = window.AudioFile;
                ButtonPlayCommand_Click(this, null);
                //ActionHistory.Push(new TextSpanPropertiesCommand(this));

                //Canvas.SetLeft(this, window.Vector2DPosition.ValueInPixel.X);
                //Canvas.SetTop(this, window.Vector2DPosition.ValueInPixel.Y);
                //SpanContent.Text = window.TextCaption;
                //XlsColumn = (string)window.XLSColumn.XLSColumsList.SelectedValue;

                //SpanFontFamily = window.FontChooser.SelectedFontFamily;
                //SpanFontSize = Helper.ToEmSize(window.FontChooser.SelectedFontSize, MainWindow.DpiY);
                //SpanFontWeight = window.FontChooser.SelectedFontWeight;
                //SpanFontStyle = window.FontChooser.SelectedFontStyle;

                //LayoutWindow.instance.IsChanged = true;
            }
        }

        public FadeInOutSampleProvider m_FadeInOutSampleProvider { get; set; }

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
            IsExclusivePlayback = !IsExclusivePlayback;

            AudioPlaybackExclusive.IsChecked = IsExclusivePlayback;
        }

        private void AudioPlaybackExclusive_Unchecked(object sender, RoutedEventArgs e)
        {
            //AudioPlaybackExclusive.IsChecked = IsExclusivePlayback = false;

            //BorderBackground.Background = new SolidColorBrush(Color.FromArgb(200, 20, 20, 24));
        }

        private void MenuItemOpenContainingFolder_Click(object sender, RoutedEventArgs e)
        {
            if (!System.IO.File.Exists(AudioFile))
            {
                System.Windows.MessageBox.Show(String.Format("systemIOException {0}", "File not found!"), "IO Error");
                return;
            }
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = System.IO.Path.GetDirectoryName(AudioFile),//"%systemroot%\\explorer.exe",// + AudioFile,
                Arguments = System.IO.Path.GetFileName(AudioFile),
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }

    class DelegateCommand : ICommand
    {
        private readonly Action action;
        private bool isEnabled;

        public DelegateCommand(Action action)
        {
            this.action = action;
            isEnabled = true;
        }

        public void Execute(object parameter)
        {
            action();
        }

        public bool CanExecute(object parameter)
        {
            return isEnabled;
        }

        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    OnCanExecuteChanged();
                }
            }
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}
