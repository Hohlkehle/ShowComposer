//using Microsoft.Office.Interop.Word;
using DocxReaderApplication;
using LayoutFileSystem;
using Microsoft.Win32;
using ShowComposer.Data;
//using Word = Microsoft.Office.Interop.Word;
using ShowComposer.DraggableUI;
using ShowComposer.NAudioDemo.AudioPlaybackDemo;
using ShowComposer.UserControls;
using ShowComposer.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Xps.Packaging;
using System.Xml.Serialization;
using Utilities;
using SysIO = System.IO;

namespace ShowComposer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        public static IniFile iniFile;

        public static event EventHandler<LayoutEditorSelectionEventArgs> OnUIElementSelected = (Object sender, LayoutEditorSelectionEventArgs e) => { };

        private string m_IniPath = System.IO.Path.Combine(Environment.CurrentDirectory, "settings.ini");
        private bool m_IsSelected = false;
        private bool m_IsCtrlDown;

        private UIElement m_SelectedElement = null;
        private Layout m_Layout;
        private static string m_FileName = @"scenario 1.scomp";
        private List<UIElement> m_LoadedCanvasItems = new List<UIElement>();
        private LayoutProperties m_LayoutProperties;
        private LayoutProperties LayoutProperties { set { m_LayoutProperties = value; MyDeskLayout.LayoutProperties = value; } get { return m_LayoutProperties; } }
        private bool m_IsNewProject;
        private ComposerLayout m_ComposerLayout;
        private SampleDeskWindow m_SampleDeskWindow;
        private VolumeControlWindow m_VolumeControlWindow;
        private Preferences m_Preferences;

        public static bool IsProjectLoaded { get; set; }

        public static string ProjectFileName
        {
            get { return m_FileName; }
            set { m_FileName = value; }
        }

        public string WordDocument
        {
            get
            {
                var path = m_ComposerLayout.FrontSideLayout.WordDocument;
                if (m_ComposerLayout.FrontSideLayout.IsRelativePath)
                {
                    path = System.IO.Path.Combine(
                      System.IO.Path.GetDirectoryName(MainWindow.ProjectFileName),
                      m_ComposerLayout.FrontSideLayout.WordDocument);
                }
                return path;
            }
            set { m_ComposerLayout.FrontSideLayout.WordDocument = value; }
        }

        public bool IsCtrlDown
        {
            get { return m_IsCtrlDown; }
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

        public MainWindow()
        {
            InitializeComponent();

            IniFileInit();
            LoadPreferences();

            m_ComposerLayout = new ComposerLayout();

            MyDeskLayout.myScrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
            MyDeskLayout.ComposerLayout = m_ComposerLayout;

            SetProgressBarValue(ProgressBarLoading.Maximum, null);

            AudioPlaybackControl.OnTryPlay += (object sender, EventArgs e) =>
            {
                if (Preferences.OutputDevice == "NullOut")
                    return;

                try
                {
                    //MyDeskLayout.AudioControls.Where(i=>!((AudioPlaybackControl)i).IsExclusivePlayback).All((i) => i.Stop());
                }
                catch (Exception)
                {
                    
                }
            };

            AudioPlaybackControl.OnPlay += (object sender, EventArgs e) =>
            {
                if (Preferences.OutputDevice == "NullOut")
                    return;

                try
                {
                    WaveformSeekbar1.AudioPlaybackControl = (AudioPlaybackControl)sender;

                    WaveformSeekbar1.LoadAudio(((AudioPlaybackControl)sender).AudioFile);

                    System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
                    {
                        Dispatcher.Invoke(delegate {
                            WaveformSeekbar1.LoadAudio(((AudioPlaybackControl)sender).AudioFile);

                        });

                        
                    }));

                }
                catch (Exception waveFormException)
                {
                    MessageBox.Show(String.Format("waveFormException {0}", waveFormException.Message), "Error Initializing Output");
                    return;
                }
            };
            //SV.ScrollChanged += ScrollChangedEventHandler;
            //SV..ScrollToVerticalOffset(SV.ScrollableHeight / 2);

            //System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
            //{
            //    System.Threading.Thread.Sleep(2000);
            //    Dispatcher.Invoke(delegate { ((Image)(myScrollViewer.Content)).Height = dvScrollViewer.ExtentHeight; });
            //    // ((Image)(myScrollViewer.Content)).Height = FindVisualChild<ScrollViewer>(screenplayRack.documentviewWord).ExtentHeight;
            //}));

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var plr in MyDeskLayout.AudioControls)
            {
                plr.Stop();
            }

            if (m_SampleDeskWindow != null)
                m_SampleDeskWindow.Close();

            if (VideoPlaybackWindow.instance != null)
                VideoPlaybackWindow.instance.Close();


            m_VolumeControlWindow?.Close();
            LoggingWindow.Quit();
            SavePreferences();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            //myScrollViewer.ScrollToVerticalOffset(e.VerticalOffset);
            //FindVisualChild<ScrollViewer>(documentviewWord).ScrollToVerticalOffset(e.VerticalOffset);
            FindVisualChild<ScrollViewer>(flowDocumentReader).ScrollToVerticalOffset(e.VerticalOffset);
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

                        MyDeskLayout.RemoveControl(m_SelectedElement);
                        e.Handled = true;
                    }
                }
            }

            if(IsCtrlDown && e.Key == Key.O)
            {
                MenuItemOpenProject_Click(this, null);
            }

            if (IsCtrlDown && e.Key == Key.D)
            {
                MenuItemDeskWindow_Click(this, null);
            }

            if (IsCtrlDown && e.Key == Key.B)
            {
                MenuItemVideotWindow_Click(this, null);
            }

            if (IsCtrlDown && e.Key == Key.V)
            {
                var files = Clipboard.GetFileDropList();
                if (files.Count == 1 && !string.IsNullOrEmpty(files[0]) && DeskLayout.IsAcceptableFile(files[0]))
                    MyDeskLayout.LoadFileToCanvas(files[0]);
            }
        }

        private void MenuItemSaveProject_Click(object sender, RoutedEventArgs e)
        {
            MyDeskLayout.SaveProject(ProjectFileName);
        }

        private void MenuItemCreateLayout_Click(object sender, RoutedEventArgs e)
        {
            MyDeskLayout.ClearLayout();

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "My Scenario"; // Default file name
            dlg.DefaultExt = ".scomp"; // Default file extension
            dlg.Filter = "Scenario documents (.scomp)|*.scomp"; // Filter files by extension

            // Process save file dialog box results
            if (dlg.ShowDialog() == true)
            {
                ProjectFileName = dlg.FileName;
                m_Layout = new Layout();
                m_Layout.Save(ProjectFileName);
                using (new WaitCursor())
                {
                    m_ComposerLayout.FrontSideLayout = new LayoutProperties();
                    m_ComposerLayout.RearSideLayout = new LayoutProperties();
                    m_ComposerLayout.FrontSideLayout.m_AudioPlayerInfo = new List<AudioPlayerInfo>();


                    WordDocument = "";
                    m_Layout.Data1 = LayoutFileReader.GetBytes(SerializeToString(m_ComposerLayout.FrontSideLayout));
                    m_Layout.Data2 = LayoutFileReader.GetBytes(SerializeToString(m_ComposerLayout.RearSideLayout));
                    m_Layout.Save(ProjectFileName);
                }
                m_IsNewProject = true;
                LoadProjectFileAsync(ProjectFileName);
            }
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItemOpenProject_Click(object sender, RoutedEventArgs e)
        {
            // Initialize an OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter and RestoreDirectory
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "Showcomposer Project Files (.scomp)|*.scomp|All Files (*.*)|*.*";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                if (openFileDialog.FileName.Length > 0)
                {
                    LoadProjectFileAsync(openFileDialog.FileName);
                }
            }
        }

        private void MenuItemCloseProject_Click(object sender, RoutedEventArgs e)
        {
            MyDeskLayout.ClearLayout();
            if (m_SampleDeskWindow != null)
                m_SampleDeskWindow.MyDeskLayout.ClearLayout();

            string tmp = string.Concat(System.IO.Path.GetTempPath(), "\\", Guid.NewGuid().ToString(), ".docx");
            ByteArrayToFile(tmp, Properties.Resources.scenario);
            ReadDocx(tmp);
            //LoadWordDocToView(tmp);

            IsProjectLoaded = false;
        }

        private void MenuItemDeskWindow_Click(object sender, RoutedEventArgs e)
        {
            if (m_SampleDeskWindow == null || !m_SampleDeskWindow.IsLoaded)
            {
                m_SampleDeskWindow = new SampleDeskWindow();
                m_SampleDeskWindow.ProjectFileName = ProjectFileName;
                m_SampleDeskWindow.Show();
                m_SampleDeskWindow.MyDeskLayout.ComposerLayout = m_ComposerLayout;
                m_SampleDeskWindow.MyDeskLayout.LoadLayout(m_ComposerLayout.RearSideLayout);
            }
            else if (m_SampleDeskWindow.IsLoaded)
            {
                m_SampleDeskWindow.Focus();
            }
        }

        private void MenuItemVideotWindow_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new VideoPlaybackWindow();
            wnd.Show();
        }

        private void MenuItemOptionsWindow_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new OptionsWindow(new List<IOutputDevicePlugin>() {
                new WasapiOutPlugin(),
                new WaveOutPlugin(),
                new DirectSoundOutPlugin(),
                new NullOutPlugin()
            });

            wnd.ShowDialog();
        }

        private void MenuItemConsolidateProject_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("All media files will be moved on the one folder. Continue?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ConsolidateProject();
            }
        }

        private void MenuItemResetWorkspace_Click(object sender, RoutedEventArgs e)
        {
            MenuItemCloseProject_Click(sender, e);
        }

        private void MenuItemVolumeControlWindow_Click(object sender, RoutedEventArgs e)
        {
            if (m_VolumeControlWindow == null || !m_VolumeControlWindow.IsLoaded)
            {
                m_VolumeControlWindow = new VolumeControlWindow();
                m_VolumeControlWindow.Show();
                m_VolumeControlWindow.Focus();
            }
        }

        private void ButtonOpenScenario_Click(object sender, RoutedEventArgs e)
        {
            if (m_ComposerLayout.FrontSideLayout == null)
            {
                //MessageBox.Show("Please create new project and try again.");
                MenuItemCreateLayout_Click(null, null);
                return;
            }
            // Initialize an OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter and RestoreDirectory
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "Word documents(*.docx)|*.docx";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                if (openFileDialog.FileName.Length > 0)
                {
                    WordDocument = openFileDialog.FileName;
                    Dispatcher.Invoke(new EventHandler(SetProgressBarValue), new object[] { 20.0, null });

                    Dispatcher.Invoke(delegate
                    {
                        ReadDocx(WordDocument);
                        /*var lwdtvRes = LoadWordDocToView(WordDocument);
                        if (lwdtvRes != 0)
                        {
                            string tmp = string.Concat(System.IO.Path.GetTempPath(), "\\", Guid.NewGuid().ToString(), ".docx");
                            if (lwdtvRes == 2)
                                ByteArrayToFile(tmp, Properties.Resources.error);
                            if (lwdtvRes == 3)
                                ByteArrayToFile(tmp, Properties.Resources.scenario);
                            LoadWordDocToView(tmp);
                        }*/

                        SetProgressBarValue(80.0, null);

                        System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
                        {
                            Thread.Sleep(1000);
                            Dispatcher.Invoke(delegate
                            {
                                //var dvScrollViewer = FindVisualChild<ScrollViewer>(documentviewWord);
                                var dvScrollViewer = FindVisualChild<ScrollViewer>(flowDocumentReader); ;
                                MyDeskLayout.ExpandedHeight = dvScrollViewer.ExtentHeight;
                                SetProgressBarValue(ProgressBarLoading.Maximum, null);
                            });
                        }));
                    });
                }
            }
        }

        private void SetProgressBarValue(object sender, EventArgs e)
        {
            var value = (double)sender;

            ProgressBarLoading.Value = value;

            if ((BusyIndicatorSheetLoading.IsBusy && (value >= ProgressBarLoading.Maximum || value == 0)) || value == ProgressBarLoading.Maximum)
                BusyIndicatorSheetLoading.IsBusy = false;
            else if (!BusyIndicatorSheetLoading.IsBusy)
                BusyIndicatorSheetLoading.IsBusy = true;
        }


       

        private void ReadDocx(string path)
        {
            if (!File.Exists(path))
                path = "scenario.docx";
            if (File.Exists(path))
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var flowDocumentConverter = new DocxToFlowDocumentConverter(stream);
                    flowDocumentConverter.Read();
                    this.flowDocumentReader.Document = flowDocumentConverter.Document;
                    this.Title = System.IO.Path.GetFileName(path);
                }
        }

       
        
        private void LoadProjectFileAsync(string p)
        {
            ProjectFileName = p;
            System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
            {
                Dispatcher.Invoke(new EventHandler(SetProgressBarValue), new object[] { 10.0, null });

                m_ComposerLayout.LoadFile(ProjectFileName);

                //LayoutProperties = ReadLayoutXml(LayoutFileReader.GetString(m_Layout.Data1));
                LayoutProperties = m_ComposerLayout.FrontSideLayout;

                Dispatcher.Invoke(new EventHandler(SetProgressBarValue), new object[] { 20.0, null });

                Dispatcher.Invoke(delegate
                {
                    ReadDocx(WordDocument);
                    /*var lwdtvRes = LoadWordDocToView(WordDocument);
                    if (lwdtvRes != 0)
                    {
                        string tmp = string.Concat(System.IO.Path.GetTempPath(), "\\", Guid.NewGuid().ToString(), ".docx");
                        if (lwdtvRes == 2)
                            ByteArrayToFile(tmp, Properties.Resources.error);
                        if (lwdtvRes == 3)
                            ByteArrayToFile(tmp, Properties.Resources.scenario);

                        LoadWordDocToView(tmp);
                    }*/

                    SetProgressBarValue(50.0, null);

                    MyDeskLayout.LoadLayout(m_ComposerLayout.FrontSideLayout);
                    if (m_SampleDeskWindow != null)
                        m_SampleDeskWindow.MyDeskLayout.LoadLayout(m_ComposerLayout.RearSideLayout);

                    SetProgressBarValue(85.0, null);

                    System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
                    {
                        Thread.Sleep(1000);
                        Dispatcher.Invoke(delegate
                        {
                            //var dvScrollViewer = FindVisualChild<ScrollViewer>(documentviewWord);
                            var dvScrollViewer = FindVisualChild<ScrollViewer>(flowDocumentReader);
                            MyDeskLayout.ExpandedHeight = dvScrollViewer.ExtentHeight;

                            SetProgressBarValue(ProgressBarLoading.Maximum, null);

                            IsProjectLoaded = true;
                        });
                    }));
                });
            }));

            //if (OnSupplementLayoutLoaded != null)
            //    OnSupplementLayoutLoaded(this, EventArgs.Empty);
        }
        
        #region Helpers
        public static bool InDesignMode()
        {
            return !(System.Windows.Application.Current is App);
        }
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

        private string SerializeToString(LayoutProperties prop)
        {
            prop.Check();

            var xml = "";
            using (var writer = new StringWriter())
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(LayoutProperties));
                    serializer.Serialize(writer, prop);
                    xml = writer.GetStringBuilder().ToString();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "SupplementLayout.SerializeToString()", MessageBoxButton.OK);
                }
            }
            return xml;
        }

        public bool ByteArrayToFile(string _FileName, byte[] _ByteArray)
        {
            try
            {
                // Open file for reading
                System.IO.FileStream _FileStream =
                   new System.IO.FileStream(_FileName, System.IO.FileMode.Create,
                                            System.IO.FileAccess.Write);
                // Writes a block of bytes to this stream using data from
                // a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}",
                                  _Exception.ToString());
            }

            // error occured, return false
            return false;
        }

  


        #endregion




        private void IniFileInit()
        {
            if (!File.Exists(m_IniPath))
            {
                var fstreem = File.Create(m_IniPath);
                fstreem.Close();

                iniFile = new IniFile(m_IniPath);
                SavePreferences();
            }
            else
            {
                iniFile = new IniFile(m_IniPath);
            }
        }

        private void LoadPreferences()
        {
            m_Preferences = new Preferences(iniFile);
        }

        private void SavePreferences()
        {
            try
            {
                m_Preferences.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения настроек. " + ex.Message, "Ошибка");
            }
        }
        
        

        private void ConsolidateProject()
        {
            using (new WaitCursor())
            {
                Dispatcher.Invoke(new EventHandler(SetProgressBarValue), new object[] { 10.0, null });

                if (m_SampleDeskWindow != null)
                    m_SampleDeskWindow.Close();

                MenuItemSaveProject_Click(null, null);

                try
                {
                    SysIO.File.Copy(
                        ProjectFileName,
                        SysIO.Path.Combine(
                            SysIO.Path.GetDirectoryName(ProjectFileName),
                            SysIO.Path.GetFileNameWithoutExtension(ProjectFileName) + "_bak" + SysIO.Path.GetExtension(ProjectFileName)));
                }
                catch { };

                var mediaFolder = SysIO.Path.Combine(
                    SysIO.Path.GetDirectoryName(ProjectFileName),
                     SysIO.Path.GetFileNameWithoutExtension(ProjectFileName));

                var di = SysIO.Directory.CreateDirectory(mediaFolder);

                if (!di.Exists)
                {
                    MessageBox.Show("Error occurs. Can't create directory " + mediaFolder, "Ошибка");
                    return;
                }

                for (var i = 0; i < m_ComposerLayout.FrontSideLayout.m_AudioPlayerInfo.Count; i++)
                {
                    var nf = SysIO.Path.Combine(mediaFolder, SysIO.Path.GetFileName(m_ComposerLayout.FrontSideLayout.m_AudioPlayerInfo[i].AudioFile));
                    try
                    {
                        SysIO.File.Copy(m_ComposerLayout.FrontSideLayout.m_AudioPlayerInfo[i].AudioFile, nf);
                    }
                    catch { };
                    m_ComposerLayout.FrontSideLayout.m_AudioPlayerInfo[i].AudioFile = SysIO.Path.Combine(SysIO.Path.GetFileNameWithoutExtension(ProjectFileName), SysIO.Path.GetFileName(m_ComposerLayout.FrontSideLayout.m_AudioPlayerInfo[i].AudioFile));
                    m_ComposerLayout.FrontSideLayout.m_AudioPlayerInfo[i].IsRelativePath = true;
                }

                Dispatcher.Invoke(new EventHandler(SetProgressBarValue), new object[] { 30.0, null });

                for (var i = 0; i < m_ComposerLayout.FrontSideLayout.m_VideoPlayerInfo.Count; i++)
                {
                    var nf = SysIO.Path.Combine(mediaFolder, SysIO.Path.GetFileName(m_ComposerLayout.FrontSideLayout.m_VideoPlayerInfo[i].VideoFile));
                    try
                    {
                        SysIO.File.Copy(m_ComposerLayout.FrontSideLayout.m_VideoPlayerInfo[i].VideoFile, nf);
                    }
                    catch { };
                    m_ComposerLayout.FrontSideLayout.m_VideoPlayerInfo[i].VideoFile = SysIO.Path.Combine(SysIO.Path.GetFileNameWithoutExtension(ProjectFileName), SysIO.Path.GetFileName(m_ComposerLayout.FrontSideLayout.m_VideoPlayerInfo[i].VideoFile));
                    m_ComposerLayout.FrontSideLayout.m_VideoPlayerInfo[i].IsRelativePath = true;
                }

                for (var i = 0; i < m_ComposerLayout.FrontSideLayout.m_PowerPointInfo.Count; i++)
                {
                    var nf = SysIO.Path.Combine(mediaFolder, SysIO.Path.GetFileName(m_ComposerLayout.FrontSideLayout.m_PowerPointInfo[i].PresenterFile));
                    try
                    {
                        SysIO.File.Copy(m_ComposerLayout.FrontSideLayout.m_PowerPointInfo[i].PresenterFile, nf);
                    }
                    catch { };
                    m_ComposerLayout.FrontSideLayout.m_PowerPointInfo[i].PresenterFile = SysIO.Path.Combine(SysIO.Path.GetFileNameWithoutExtension(ProjectFileName), SysIO.Path.GetFileName(m_ComposerLayout.FrontSideLayout.m_PowerPointInfo[i].PresenterFile));
                    m_ComposerLayout.FrontSideLayout.m_PowerPointInfo[i].IsRelativePath = true;
                }

                Dispatcher.Invoke(new EventHandler(SetProgressBarValue), new object[] { 50.0, null });

                for (var i = 0; i < m_ComposerLayout.RearSideLayout.m_AudioPlayerInfo.Count; i++)
                {
                    var nf = SysIO.Path.Combine(mediaFolder, SysIO.Path.GetFileName(m_ComposerLayout.RearSideLayout.m_AudioPlayerInfo[i].AudioFile));
                    try
                    {
                        SysIO.File.Copy(m_ComposerLayout.RearSideLayout.m_AudioPlayerInfo[i].AudioFile, nf);
                    }
                    catch { };
                    m_ComposerLayout.RearSideLayout.m_AudioPlayerInfo[i].AudioFile = SysIO.Path.Combine(SysIO.Path.GetFileNameWithoutExtension(ProjectFileName), SysIO.Path.GetFileName(m_ComposerLayout.RearSideLayout.m_AudioPlayerInfo[i].AudioFile));
                    m_ComposerLayout.RearSideLayout.m_AudioPlayerInfo[i].IsRelativePath = true;
                }

                Dispatcher.Invoke(new EventHandler(SetProgressBarValue), new object[] { 65.0, null });

                for (var i = 0; i < m_ComposerLayout.RearSideLayout.m_VideoPlayerInfo.Count; i++)
                {
                    var nf = SysIO.Path.Combine(mediaFolder, SysIO.Path.GetFileName(m_ComposerLayout.RearSideLayout.m_VideoPlayerInfo[i].VideoFile));
                    try
                    {
                        SysIO.File.Copy(m_ComposerLayout.RearSideLayout.m_VideoPlayerInfo[i].VideoFile, nf);
                    }
                    catch { };
                    m_ComposerLayout.RearSideLayout.m_VideoPlayerInfo[i].VideoFile = SysIO.Path.Combine(SysIO.Path.GetFileNameWithoutExtension(ProjectFileName), SysIO.Path.GetFileName(m_ComposerLayout.RearSideLayout.m_VideoPlayerInfo[i].VideoFile));
                    m_ComposerLayout.RearSideLayout.m_VideoPlayerInfo[i].IsRelativePath = true;
                }

                for (var i = 0; i < m_ComposerLayout.RearSideLayout.m_PowerPointInfo.Count; i++)
                {
                    var nf = SysIO.Path.Combine(mediaFolder, SysIO.Path.GetFileName(m_ComposerLayout.RearSideLayout.m_PowerPointInfo[i].PresenterFile));
                    try
                    {
                        SysIO.File.Copy(m_ComposerLayout.RearSideLayout.m_PowerPointInfo[i].PresenterFile, nf);
                    }
                    catch { };
                    m_ComposerLayout.RearSideLayout.m_PowerPointInfo[i].PresenterFile = SysIO.Path.Combine(SysIO.Path.GetFileNameWithoutExtension(ProjectFileName), SysIO.Path.GetFileName(m_ComposerLayout.RearSideLayout.m_PowerPointInfo[i].PresenterFile));
                    m_ComposerLayout.RearSideLayout.m_PowerPointInfo[i].IsRelativePath = true;
                }

                Dispatcher.Invoke(new EventHandler(SetProgressBarValue), new object[] { 80.0, null });



                m_ComposerLayout.Save();

                MenuItemCloseProject_Click(null, null);

                var ndf = SysIO.Path.Combine(mediaFolder, SysIO.Path.GetFileName(WordDocument));
                if (SysIO.File.Exists(ndf))
                {
                    try { SysIO.File.Delete(ndf); }
                    catch { };
                    if (SysIO.File.Exists(ndf))
                    {
                        try { SysIO.File.Move(ndf, SysIO.Path.Combine(mediaFolder, SysIO.Path.GetFileName(WordDocument), "_renamed")); }
                        catch { };
                    }
                }
                try
                {
                    SysIO.File.Copy(WordDocument, ndf, true);
                    WordDocument = SysIO.Path.Combine(SysIO.Path.GetFileNameWithoutExtension(ProjectFileName), SysIO.Path.GetFileName(WordDocument));
                    m_ComposerLayout.FrontSideLayout.IsRelativePath = true;
                }
                catch { }


                Dispatcher.Invoke(new EventHandler(SetProgressBarValue), new object[] { 85.0, null });

                LoadProjectFileAsync(ProjectFileName);
            }
        }

        #region Obsolete
        private void OnDocumentLoaded(DocumentViewer documentViewer)
        {
            //firstScrollAfterContenChanged = true;
            //var height = screenplayRack.documentviewWord.Document.DocumentPaginator.PageSize.Height;

            var dvScrollViewer = FindVisualChild<ScrollViewer>(documentviewWord); ;

            //dvScrollViewer.ScrollChanged -= dvScrollViewer_ScrollChanged;
            //dvScrollViewer.ScrollChanged += dvScrollViewer_ScrollChanged;

            //myScrollViewer.ScrollChanged -= dvScrollViewer_ScrollChanged;
            //myScrollViewer.ScrollChanged += dvScrollViewer_ScrollChanged;

            System.Threading.Tasks.Task.Factory.StartNew(new Action(() =>
            {
                System.Threading.Thread.Sleep(2000);
                //Dispatcher.Invoke(delegate { ((Image)(ImageSpacer)).Height = dvScrollViewer.ExtentHeight; });
            }));


            //ImageSpacer.Height = dvScrollViewer.ActualHeight;
            //ImageSpacer.Width = dvScrollViewer.ActualWidth;

            //var vp = FindVisualChild<ScrollViewer>(screenplayRack.documentviewWord);
            //var ccs = vp.ExtentHeight;
            //var ViewportHeight = vp.VerticalOffset;
            ////vp.CanContentScroll = false;
            ////myScrollViewer.SetCurrentValue(ScrollViewer.ViewportHeightProperty, ViewportHeight);
            ////((Image)(myScrollViewer.Content)).Height = ViewportHeight;
        }
        private void btnSelectWord_Click(object sender, RoutedEventArgs e)
        {
            // Initialize an OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // Set filter and RestoreDirectory
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Filter = "Word documents(*.doc;*.docx)|*.doc;*.docx";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                if (openFileDialog.FileName.Length > 0)
                {
                    txbSelectedWordFile.Text = openFileDialog.FileName;
                }
            }
        }

        public float SetVerticalOffset
        {
            set
            {
                //var border = VisualTreeHelper.GetChild(screenplayRack.documentviewWord, 0) as Decorator;
                //var scrollViewer = border.Child as ScrollViewer;

                var dvScrollViewer = FindVisualChild<ScrollViewer>(documentviewWord); ;
                //var height = documentviewWord.Document.DocumentPaginator.PageSize.Height;


                //var cnt = dvScrollViewer.Content;
                //var cntType = cnt.GetType();
                //var cntContent = cntType.GetProperty("Content");
                //var paginator = cntContent.GetValue(cnt);
                //var paginatorType = paginator.GetType();
                //var pageSize = paginatorType.GetProperty("PageSize");

                //var size = pageSize.GetValue(paginatorType); ;
                //PageSize = {816;1056}
                //MS.Internal.Documents.FixedDocumentSequencePaginator
                //scrollViewer.ScrollToBottom();
                //myScrollViewer.SetCurrentValue(ScrollViewer.ViewportHeightProperty, dvScrollViewer.ExtentHeight);



                //((Image)(ImageSpacer)).Height = dvScrollViewer.ExtentHeight;
                //myCanvas.Height = dvScrollViewer.ExtentHeight;

                //myScrollViewer.ScrollToVerticalOffset(dvScrollViewer.VerticalOffset);



                //var dvScrollViewer = FindVisualChild<ScrollViewer>(screenplayRack.documentviewWord); ;
                //((Image)(myScrollViewer.Content)).Height = dvScrollViewer.ExtentHeight;

            }
        }

        private void ButtonPrintInfo_Click(object sender, RoutedEventArgs e)
        {
            SetVerticalOffset = 1;

            MenuItemOpenProject_Click(null, null);
            MyDeskLayout.LoadLayout(LayoutProperties);

            #region MyRegion
            //var dvScrollViewer = FindVisualChild<ScrollViewer>(screenplayRack.documentviewWord);

            //var dvsl = Canvas.GetLeft(screenplayRack.documentviewWord);   NaN
            //var dvsr = Canvas.GetRight(screenplayRack.documentviewWord);
            //Canvas.SetLeft(dvScrollViewer, dvsl);
            //Canvas.SetRight(dvScrollViewer, dvsr);



            //var height = screenplayRack.documentviewWord.Document.DocumentPaginator.PageSize.Height;

            //var dvScrollViewer = FindVisualChild<ScrollViewer>(screenplayRack.documentviewWord); ;

            //dvScrollViewer.ScrollChanged += dvScrollViewer_ScrollChanged;
            ////((Image)(myScrollViewer.Content)).Height = dvScrollViewer.ExtentHeight;
            //((Image)(ImageSpacer)).Height = dvScrollViewer.ExtentHeight;

            //Canvas.SetLeft(dvScrollViewer, Canvas.GetLeft(screenplayRack.documentviewWord));
            //Canvas.SetRight(dvScrollViewer, Canvas.GetRight(screenplayRack.documentviewWord));

            //dvScrollViewer.Height = screenplayRack.documentviewWord.ActualHeight;
            //dvScrollViewer.Width = screenplayRack.documentviewWord.ActualWidth;

            ////ImageSpacer.Height = dvScrollViewer.ActualHeight;
            ////ImageSpacer.Width = dvScrollViewer.ActualWidth;


            //var vp = FindVisualChild<ScrollViewer>(screenplayRack.documentviewWord);
            //var ccs = vp.ExtentHeight;
            //var ViewportHeight = vp.VerticalOffset;
            ////vp.CanContentScroll = false;
            ////myScrollViewer.SetCurrentValue(ScrollViewer.ViewportHeightProperty, ViewportHeight);
            ////((Image)(myScrollViewer.Content)).Height = ViewportHeight;


            //RichTextBoxInfo.AppendText("ViewportHeight: " + screenplayRack.documentviewWord.ViewportHeight); RichTextBoxInfo.AppendText("\n");
            //RichTextBoxInfo.AppendText("HorizontalOffset: " + screenplayRack.documentviewWord.HorizontalOffset); RichTextBoxInfo.AppendText("\n");
            //RichTextBoxInfo.AppendText("Height " + screenplayRack.documentviewWord.Height); RichTextBoxInfo.AppendText("\n");
            //RichTextBoxInfo.AppendText("ActualHeight: " + screenplayRack.documentviewWord.ActualHeight); RichTextBoxInfo.AppendText("\n");
            //RichTextBoxInfo.AppendText("DesiredSize: " + screenplayRack.documentviewWord.DesiredSize); RichTextBoxInfo.AppendText("\n");
            //RichTextBoxInfo.AppendText("RenderSize: " + screenplayRack.documentviewWord.RenderSize); RichTextBoxInfo.AppendText("\n");
            //RichTextBoxInfo.AppendText("VerticalOffset: " + screenplayRack.documentviewWord.VerticalOffset); RichTextBoxInfo.AppendText("\n");
            //RichTextBoxInfo.AppendText("VerticalPageSpacing: " + screenplayRack.documentviewWord.VerticalPageSpacing); RichTextBoxInfo.AppendText("\n");
            //RichTextBoxInfo.AppendText("\n");
            //RichTextBoxInfo.AppendText("ExtentWidth: " + screenplayRack.documentviewWord.ViewportWidth);
            //RichTextBoxInfo.AppendText("\n"); 
            #endregion
        }

        private void ButtonAddGuideLine_Click(object sender, RoutedEventArgs e)
        {
            //AddGuigeLine(new WindowsPoint(10, 5), 3, 300, new WindowsPoint());

            m_Layout = new Layout();
            m_Layout.Save(ProjectFileName);
            //SaveLayout();

        }

        /// <summary>
        ///  Convert the word document to xps document
        /// </summary>
        /// <param name="wordFilename">Word document Path</param>
        /// <param name="xpsFilename">Xps document Path</param>
        /// <returns></returns>
        [Obsolete]
        private XpsDocument ConvertWordToXps(string wordFilename, string xpsFilename)
        {
            // Create a WordApplication and host word document
            /*Word.Application wordApp = new Microsoft.Office.Interop.Word.Application();
            try
            {
                wordApp.Documents.Open(wordFilename);

                // To Invisible the word document
                wordApp.Application.Visible = false;

                // Minimize the opened word document
                wordApp.WindowState = WdWindowState.wdWindowStateMinimize;

                Document doc = wordApp.ActiveDocument;

                doc.SaveAs(xpsFilename, WdSaveFormat.wdFormatXPS);

                XpsDocument xpsDocument = new XpsDocument(xpsFilename, FileAccess.Read);
                return xpsDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurs, The error message is  " + ex.ToString());
                return null;
            }
            finally
            {
                try
                {
                    wordApp.Documents.Close();
                    ((_Application)wordApp).Quit(WdSaveOptions.wdDoNotSaveChanges);
                }
                catch { };
            }*/
            return null;
        }

        [Obsolete]
        private int LoadWordDocToView(string wordDocument)
        {
            if (m_IsNewProject)
            {
                m_IsNewProject = false;
                return 3;
            }
            if ((string.IsNullOrEmpty(wordDocument) || !File.Exists(wordDocument)) && !m_IsNewProject)
            {
                MessageBox.Show("The file (" + wordDocument + ") is invalid. Please select an existing file again.");
                return 2;
            }
            else
            {
                string convertedXpsDoc = string.Concat(System.IO.Path.GetTempPath(), "\\", Guid.NewGuid().ToString(), ".xps");
                XpsDocument xpsDocument = ConvertWordToXps(wordDocument, convertedXpsDoc);
                if (xpsDocument == null)
                {
                    MessageBox.Show("Fatal error! Convertation Word document to XpsDocument failed!");
                    return 1;
                }

                documentviewWord.Document = xpsDocument.GetFixedDocumentSequence();
                xpsDocument.Close();

            }
            return 0;
        }

        [Obsolete]
        public void SaveLayout()
        {
            using (new WaitCursor())
            {
                LayoutProperties = new LayoutProperties();
                LayoutProperties.m_AudioPlayerInfo = new List<AudioPlayerInfo>();

                //LayoutProperties.m_AudioPlayerInfo.AddRange(AudioPlayerInfo.Convert(myCanvas.Children.OfType<AudioPlaybackControl>().ToArray()));

                //Update();
                LayoutProperties.WordDocument = txbSelectedWordFile.Text;
                m_Layout.Data1 = LayoutFileReader.GetBytes(SerializeToString(LayoutProperties));
                m_Layout.Save(ProjectFileName);
            }
        }

        [Obsolete]
        public static EventHandler GetEventHandler(object classInstance, string eventName)
        {
            Type classType = classInstance.GetType();
            var flds = classType.GetFields(BindingFlags.GetField
                                                               | BindingFlags.NonPublic
                                                               | BindingFlags.Instance);
            FieldInfo eventField = classType.GetField(eventName, BindingFlags.GetField
                                                               | BindingFlags.NonPublic
                                                               | BindingFlags.Instance);

            EventHandler eventDelegate = (EventHandler)eventField.GetValue(classInstance);

            // eventDelegate will be null if no listeners are attached to the event
            if (eventDelegate == null)
            {
                return null;
            }

            return eventDelegate;
        }
        #endregion
    }
}
