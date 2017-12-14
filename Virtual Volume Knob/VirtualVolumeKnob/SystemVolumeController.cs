using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
namespace QuirkSoft.VirtualVolumeKnob
{
    public class SystemVolumeController
    {
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        public MainForm mainForm { get { return MainForm.instance; } }

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);

        public void Mute()
        {
            SendMessageW(mainForm.Handle, WM_APPCOMMAND, mainForm.Handle,
                (IntPtr)APPCOMMAND_VOLUME_MUTE);
        }

        public void VolDown()
        {
            SendMessageW(mainForm.Handle, WM_APPCOMMAND, mainForm.Handle,
                (IntPtr)APPCOMMAND_VOLUME_DOWN);
        }

        public void VolUp()
        {
            SendMessageW(mainForm.Handle, WM_APPCOMMAND, mainForm.Handle,
                (IntPtr)APPCOMMAND_VOLUME_UP);
        }
    }
}
