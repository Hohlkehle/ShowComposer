using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuirkSoft.VirtualVolumeKnob
{
    public class Win32Wrap
    {
        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        /*Usage
                Win32Wrap.POINT p = new Win32Wrap.POINT();
                Win32Wrap.ClientToScreen(this.Handle, ref p);
                Win32Wrap.SetCursorPos(screenX - 10, mouseY);
         */
    }
}
