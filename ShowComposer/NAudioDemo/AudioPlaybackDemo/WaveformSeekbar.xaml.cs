using NAudio.Flac;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ShowComposer.Data;
using ShowComposer.NAudioDemo.Gui;
using ShowComposer.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
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

namespace ShowComposer.NAudioDemo.AudioPlaybackDemo
{
    /// <summary>
    /// Interaction logic for WaveformSeekbar.xaml
    /// </summary>
    public partial class WaveformSeekbar : UserControl
    {
        public string AudioFile { set; get; }
        private AudioPlaybackControl m_AudioPlaybackControl;
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        private bool mouseDown;
        private float[] samples, lsamples, rsamples;
        private int saplesCount;

        public AudioPlaybackControl AudioPlaybackControl
        {
            get { return m_AudioPlaybackControl; }
            set
            {
                m_AudioPlaybackControl = value;

                if (AudioPlaybackControl != null && AudioPlaybackControl.AudioFileReader != null)
                {
                    m_AudioPlaybackControl.WaveOutPlayer.PlaybackStopped += WaveOut_PlaybackStopped;

                    dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                    dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
                    dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 5);
                    dispatcherTimer.Start();
                }
            }
        }

        public WaveformSeekbar()
        {
            CheckDatabase();
            InitializeComponent();
        }
        
        void WaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (AudioPlaybackControl.WaveOutPlayer == null)
                return;

            var currentTime = (AudioPlaybackControl.WaveOutPlayer.PlaybackState == PlaybackState.Stopped)
                ? TimeSpan.Zero
                : AudioPlaybackControl.AudioFileReader.CurrentTime;

            //var pos = SmoothStep(currentTime.TotalSeconds, 0, AudioPlaybackControl.AudioFileReader.TotalTime.TotalSeconds, 0, ActualWidth);
            var position = Math.Min(
                ActualWidth,
                (int)(ActualWidth * currentTime.TotalSeconds / AudioPlaybackControl.AudioFileReader.TotalTime.TotalSeconds));

            Canvas.SetLeft(SeekPosition, position - 2);
            //SeekPosition.SetValue(Canvas.LeftProperty, position);
        }

        public void LoadAudio(string audioFile)
        {
            AudioFile = audioFile;
            //CheckDatabase();

            // Check if waveform is already exists in database
            using (var connection = new SQLiteConnection("Data Source=" + Properties.Settings.Default.WaveformDatabase + ";Version=3"))
            using (var command = new SQLiteCommand("SELECT fid FROM file WHERE location= @Location", connection))
            {
                command.Parameters.AddWithValue("Location", AudioFile);
                int fid = -1;
                connection.Open();
                try
                {
                    //fid = (int)command.ExecuteScalar();
                    var r = command.ExecuteReader();
                    while (r.Read())
                    {
                        if (r["fid"] != null)
                        {
                            var res = r["fid"];
                            fid = (int)(Int64)res;
                        }
                    }
                    r.Close();


                    if (fid == -1)
                    {
                        // Read from file
                        ReadAudioFile();

                        // Update painters
                        InitializePainer(WaveformPainterLeft, lsamples);
                        InitializePainer(WaveformPainterRight, rsamples);

                        // Store to database
                        command.CommandText = "INSERT INTO file (location, subsong) VALUES (@Location,@Subsong )";
                        {
                            command.Parameters.AddWithValue("Location", AudioFile);
                            command.Parameters.AddWithValue("Subsong", "");
                            int lres = command.ExecuteNonQuery();
                            command.CommandText = @"select last_insert_rowid()";
                            var datar = command.ExecuteScalar(); ;
                            fid = (int)(Int64)datar;

                            List<float> allMinSampless = new List<float>();
                            List<float> allMaxSampless = new List<float>();
                            for (var i = 0; i < WaveformPainterLeft.NegativeSamples.Count; i++)
                            {
                                allMinSampless.Add(WaveformPainterLeft.NegativeSamples[i]);
                                allMinSampless.Add(WaveformPainterRight.NegativeSamples[i]);
                            }
                            for (var i = 0; i < WaveformPainterLeft.PositiveSamples.Count; i++)
                            {
                                allMaxSampless.Add(WaveformPainterLeft.PositiveSamples[i]);
                                allMaxSampless.Add(WaveformPainterRight.PositiveSamples[i]);
                            }

                            var dataMin = new byte[allMinSampless.Count * 4];
                            Buffer.BlockCopy(allMinSampless.ToArray(), 0, dataMin, 0, dataMin.Length);
                            var dataMax = new byte[allMaxSampless.Count * 4];
                            Buffer.BlockCopy(allMaxSampless.ToArray(), 0, dataMax, 0, dataMax.Length);

                            try
                            {
                                command.CommandText = "INSERT INTO wave (fid,min,max,channels,compression)" +
                                                      "VALUES (@fid,@min,@max,@channels,@compression)";

                                command.Parameters.Add("@fid", DbType.Int32).Value = fid;
                                command.Parameters.Add("@min", DbType.Binary).Value = dataMin;
                                command.Parameters.Add("@max", DbType.Binary).Value = dataMax;
                                command.Parameters.Add("@channels", DbType.Int32).Value = 2;
                                command.Parameters.Add("@compression", DbType.Int32).Value = 1;
                                command.ExecuteNonQuery();
                            }
                            catch (SQLiteException sQLiteException)
                            {
                                MessageBox.Show(String.Format("LoadAudio from file sQLiteException {0}", sQLiteException.Message), "Error reading from database");
                            }
                        }
                    }
                    else
                    {
                        ReadFromDatabase(command, fid);

                        // Update painters
                        WaveformPainterLeft.Invalidate();
                        WaveformPainterRight.Invalidate();
                    }
                    connection.Close();
                }
                catch (SQLiteException sQLiteException)
                {
                    MessageBox.Show(String.Format("LoadAudio sQLiteException {0}", sQLiteException.Message), "Error Initializing database");
                }
            }
        }

        private void ReadFromDatabase(SQLiteCommand command, int fid)
        {
            try
            {
                command.CommandText = "SELECT min, max FROM wave WHERE fid = " + fid;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        byte[] min = SQLiteHelper.GetBytes(reader, 0);
                        byte[] max = SQLiteHelper.GetBytes(reader, 1);

                        // create a second float array and copy the bytes into it...
                        var floatArray1 = new float[min.Length / 4];
                        Buffer.BlockCopy(min, 0, floatArray1, 0, min.Length);

                        // create a second float array and copy the bytes into it...
                        var floatArray2 = new float[max.Length / 4];
                        Buffer.BlockCopy(max, 0, floatArray2, 0, max.Length);

                        for (int i = 0, j = 0; i < floatArray1.Length; i += 2)
                        {
                            WaveformPainterLeft.NegativeSamples[j] = floatArray1[i];
                            WaveformPainterRight.NegativeSamples[j] = floatArray1[i + 1];
                            j++;
                        }
                        for (int i = 0, j = 0; i < floatArray2.Length; i += 2)
                        {
                            WaveformPainterLeft.PositiveSamples[j] = floatArray2[i];
                            WaveformPainterRight.PositiveSamples[j] = floatArray2[i + 1];
                            j++;
                        }
                    }
                }
            }
            catch (SQLiteException sQLiteException)
            {
                MessageBox.Show(String.Format("ReadFromDatabase sQLiteException {0}", sQLiteException.Message), "Error Initializing database");
            }
        }

        void ReadAudioFile()
        {
            WaveStream _audioFileReader;
            if (System.IO.Path.GetExtension(AudioFile).ToUpper() == ".FLAC")
            {
                _audioFileReader = new FlacReader(AudioFile);
            }
            else
                _audioFileReader = new AudioFileReader(AudioFile);

            var sampleChannel = new SampleChannel(_audioFileReader, true);
            var memoryStream = new System.IO.MemoryStream();

            saplesCount = (int)(_audioFileReader.TotalTime.TotalSeconds * sampleChannel.WaveFormat.SampleRate * sampleChannel.WaveFormat.Channels);
            samples = new float[saplesCount];
            lsamples = new float[saplesCount / 2 + 1];
            rsamples = new float[saplesCount / 2 + 1];

            sampleChannel.Read(samples, 0, samples.Length);

            // Separate channels
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

            //System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
            //{

            //    Dispatcher.Invoke(delegate
            //    {
            //        // Update painters
            //        InitializePainer(WaveformPainterLeft, lsamples);
            //        InitializePainer(WaveformPainterRight, rsamples);
            //    });

            //    StoreToDatabase();

            //}));

        }

        private void StoreToDatabase()
        {
            using (var connection = new SQLiteConnection("Data Source=" + Properties.Settings.Default.WaveformDatabase + ";Version=3"))
            using (var command = new SQLiteCommand("INSERT INTO file (location, subsong) VALUES (@Location, @Subsong)", connection))
            {
                command.Parameters.AddWithValue("Location", AudioFile);
                command.Parameters.AddWithValue("Subsong", "0");
                //command.Parameters.AddWithValue("Location", AudioFile);

                int fid = -1;
                connection.Open();
                try
                {

                    // Store to database

                    int lres = command.ExecuteNonQuery();
                    command.CommandText = @"select last_insert_rowid()";
                    var datar = command.ExecuteScalar(); ;
                    fid = (int)(Int64)datar;

                    List<float> allMinSampless = new List<float>();
                    List<float> allMaxSampless = new List<float>();
                    for (var i = 0; i < WaveformPainterLeft.NegativeSamples.Count; i++)
                    {
                        allMinSampless.Add(WaveformPainterLeft.NegativeSamples[i]);
                        allMinSampless.Add(WaveformPainterRight.NegativeSamples[i]);
                    }
                    for (var i = 0; i < WaveformPainterLeft.PositiveSamples.Count; i++)
                    {
                        allMaxSampless.Add(WaveformPainterLeft.PositiveSamples[i]);
                        allMaxSampless.Add(WaveformPainterRight.PositiveSamples[i]);
                    }

                    var dataMin = new byte[allMinSampless.Count * 4];
                    Buffer.BlockCopy(allMinSampless.ToArray(), 0, dataMin, 0, dataMin.Length);
                    var dataMax = new byte[allMaxSampless.Count * 4];
                    Buffer.BlockCopy(allMaxSampless.ToArray(), 0, dataMax, 0, dataMax.Length);

                    try
                    {
                        command.CommandText = "INSERT INTO wave (fid,min,max,channels,compression)" +
                                              "VALUES (@fid,@min,@max,@channels,@compression)";

                        command.Parameters.Add("@fid", DbType.Int32).Value = fid;
                        command.Parameters.Add("@min", DbType.Binary).Value = dataMin;
                        command.Parameters.Add("@max", DbType.Binary).Value = dataMax;
                        command.Parameters.Add("@channels", DbType.Int32).Value = 2;
                        command.Parameters.Add("@compression", DbType.Int32).Value = 1;
                        command.ExecuteNonQuery();
                    }
                    catch (SQLiteException sQLiteException)
                    {
                        MessageBox.Show(String.Format("LoadAudio from file sQLiteException {0}", sQLiteException.Message), "Error reading from database");
                    }

                    connection.Close();
                }
                catch (SQLiteException sQLiteException)
                {
                    MessageBox.Show(String.Format("UpdatePainters sQLiteException {0}", sQLiteException.Message), "Error Initializing database");
                }
            }
        }

        private void CheckDatabase()
        {
            SQLiteHelper.InitializeDatabase(Properties.Settings.Default.WaveformDatabase);
        }

        void InitializePainer(WaveformPainter waveformPainter, float[] samples)
        {
            waveformPainter.ClearSamples();
            int step = samples.Length / waveformPainter.MaxSamples;
            int cnt = 0;
            for (var i = 0; i < waveformPainter.MaxSamples; i++)
            {
                var avgs = new List<float>();
                var avgNeg = new List<float>();
                var avgPos = new List<float>();
                for (var j = 0; j < step; j++, cnt++)
                {
                    if (cnt >= samples.Length)
                        break;

                    if (samples[cnt] > 0)
                    {
                        avgPos.Add(samples[cnt]);
                        avgNeg.Add(0);
                    }
                    if (samples[cnt] <= 0)
                    {
                        avgNeg.Add(samples[cnt]);
                        avgPos.Add(0);
                    }
                    avgs.Add(samples[cnt]);
                }

                if (avgPos.Count > 0)
                    waveformPainter.PositiveSamples[i] = (avgPos.Max());
                if (avgNeg.Count > 0)
                    waveformPainter.NegativeSamples[i] = (avgNeg.Min());
                if (avgs.Count > 0)
                    waveformPainter.AverageSamples.Add(avgs.Average());
            }

            waveformPainter.Invalidate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var pos = new Random().Next(0, (int)ActualWidth);
            Canvas.SetLeft(SeekPosition, pos);
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition((UIElement)sender);

            AudioPlaybackControl.AudioFileReader.CurrentTime = TimeSpan.FromSeconds(
                AudioPlaybackControl.AudioFileReader.TotalTime.TotalSeconds * pos.X / ActualWidth);

            mouseDown = false;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return;
            //CheckDatabase();
            try
            {
                using (var connection = new SQLiteConnection("Data Source=" + Properties.Settings.Default.WaveformDatabase + ";Version=3"))
                using (var command = new SQLiteCommand("SELECT fid FROM file WHERE location=@Location", connection))
                {
                    command.Parameters.AddWithValue("Location", AudioFile);
                    int fid = -1;
                    connection.Open();

                    SQLiteDataReader r = command.ExecuteReader();
                    while (r.Read())
                    {
                        if (r["fid"] != null)
                        {
                            var res = r["fid"];
                            fid = (int)(Int64)res;
                        }
                    }
                    r.Close();

                    if (fid != -1)
                    {
                        ReadFromDatabase(command, fid);

                        // Update painters
                        WaveformPainterLeft.Invalidate();
                        WaveformPainterRight.Invalidate();
                    }
                    else
                    {
                        if (samples == null)
                            return;

                        InitializePainer(WaveformPainterLeft, lsamples);
                        InitializePainer(WaveformPainterRight, rsamples);
                    }
                    connection.Close();
                }
            }
            catch (SQLiteException sQLiteException)
            {
                MessageBox.Show(String.Format("Grid_SizeChanged sQLiteException {0}", sQLiteException.Message), "Error Initializing database");
                return;
            }
            catch (Exception exception)
            {
                MessageBox.Show(String.Format("Grid_SizeChanged Exception {0}", exception.Message), "Error Initializing database");
                return;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            mouseDown = true;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                var pos = e.GetPosition((UIElement)sender);
                Canvas.SetLeft(SeekPosition, pos.X);
            }
        }

        public static double SmoothStep(double x, double a1, double a2, double c1, double c2)
        {
            return c1 + ((x - a1) / (a2 - a1)) * (c2 - c1) / 1.0f;
        }

        private void MenuItemReload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItemClose_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
