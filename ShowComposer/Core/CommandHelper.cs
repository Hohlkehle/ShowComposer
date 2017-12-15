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

        public static void Log(string message)
        {
            if (!Properties.Settings.Default.LoggingEnabled)
                return;

            if (LoggingWindowInstance == null)
            {
                LoggingWindowInstance = new LoggingWindow();
                LoggingWindowInstance.Log(message);
                LoggingWindowInstance.Show();

            } else
            {
                if (!LoggingWindowInstance.IsLoaded)
                    LoggingWindowInstance = new LoggingWindow();

                LoggingWindowInstance.Log(message);
                
                LoggingWindowInstance.Show();
            }
        }

        
    }
}
