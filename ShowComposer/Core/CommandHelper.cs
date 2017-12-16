using ShowComposer.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShowComposer.Core
{
    public class CommandHelper
    {
        public static LoggingWindow LoggingWindowInstance;

        public static string[] audioExtensions = { ".WAV", ".MID", ".MIDI", ".WMA", ".MP3", ".OGG", ".RMA", ".FLAC" };
        public static string[] videoExtensions = { ".AVI", ".MP4", ".DIVX", ".WMV", ".MKV" };
        public static string[] imageExtensions = { ".PNG", ".JPG", ".JPEG", ".BMP", ".GIF" };
        public static string[] presentExtensions = { ".ppt", ".pptx", ".pptm", ".potx", ".potm", ".pot", ".ppsx", ".ppsm", ".pps", ".ppam", ".ppa", ".odp" };

        public static void Log(string message)
        {
            if (!Properties.Settings.Default.LoggingEnabled)
                return;

            ShowLoggingWindow();

            LoggingWindowInstance.Log(message);
        }

        public static void LogNotify(string message, string title = "Notification")
        {
            if (Properties.Settings.Default.LoggingEnabled)
            {
                ShowLoggingWindow();
                LoggingWindowInstance.Log(message);
            }

            System.Windows.MessageBox.Show(message, title);
        }

        public static void ShowLoggingWindow()
        {
            if (LoggingWindowInstance == null)
            {
                LoggingWindowInstance = new LoggingWindow();
                LoggingWindowInstance.Show();
            }
            else
            {
                if (!LoggingWindowInstance.IsLoaded)
                    LoggingWindowInstance = new LoggingWindow();

                LoggingWindowInstance.Show();
            }
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

        public static int IndexOf<T>(IEnumerable<T> source, T value)
        {
            int index = 0;
            var comparer = EqualityComparer<T>.Default; // or pass in as a parameter
            foreach (T item in source)
            {
                if (comparer.Equals(item, value)) return index;
                index++;
            }
            return -1;
        }
    }
}
