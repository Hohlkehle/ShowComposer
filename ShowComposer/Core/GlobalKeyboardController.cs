using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShowComposer.Core
{
    internal class GlobalKeyboardController : IDisposable
    {

        private GlobalKeyboardHook _globalKeyboardHook;
        public event EventHandler<System.Windows.Forms.KeyEventArgs> OnkeyDownPressed;
        public void SetupKeyboardHooks()
        {
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            //Debug.WriteLine(e.KeyboardData.VirtualCode);
            Keys key = (Keys)e.KeyboardData.VirtualCode;
            if (e.KeyboardData.VirtualCode >= 0x30 && e.KeyboardData.VirtualCode <= 0x39)
            {
                OnkeyDownPressed?.Invoke(this, new System.Windows.Forms.KeyEventArgs(key));
                e.Handled = true;
            }
        }

        public void Dispose()
        {
            _globalKeyboardHook?.Dispose();
        }
    }
}
