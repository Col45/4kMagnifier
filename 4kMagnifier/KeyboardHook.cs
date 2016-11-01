using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;

namespace _4kMagnifier
{
    public class KeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private LowLevelKeyboardProc keyboardProc;
        private IntPtr hookId = IntPtr.Zero;

        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_SHOWWINDOW = 0x0040;

        public Key HideKey { get; set; }
        public Key CloseKey { get; set; }

        public KeyboardHook()
        {
            keyboardProc = HookCallback;
            hookId = SetHook(keyboardProc);
        }

        public void Dispose()
        {
            WindowsServices.UnhookWindowsHookEx(hookId);
        }

        public event EventHandler HideKeyCombinationPressed;
        public event EventHandler CloseKeyCombinationPressed;

        public void OnHideKeyCombinationPressed(EventArgs e)
        {
            EventHandler handler = HideKeyCombinationPressed;
            if (handler != null) handler(null, e);
        }

        public void OnCloseKeyCombinationPressed(EventArgs e)
        {
            EventHandler handler = CloseKeyCombinationPressed;
            if (handler != null) handler(null, e);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, WindowsServices.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                var keyPressed = KeyInterop.KeyFromVirtualKey(vkCode);
                Trace.WriteLine(keyPressed);
                if (keyPressed == HideKey && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    Trace.WriteLine("Triggering Hide Keyboard Hook");
                    OnHideKeyCombinationPressed(new EventArgs());
                }
                else if (keyPressed == CloseKey && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    Trace.WriteLine("Triggering Close Keyboard Hook");
                    OnCloseKeyCombinationPressed(new EventArgs());
                }
            }
            return WindowsServices.CallNextHookEx(hookId, nCode, wParam, lParam);
        }

        public static void ActivateWindow(Window window)
        {
            var interopHelper = new WindowInteropHelper(window);
            var currentForegroundWindow = WindowsServices.GetForegroundWindow();
            var thisWindowThreadId = WindowsServices.GetWindowThreadProcessId(interopHelper.Handle, IntPtr.Zero);
            var currentForegroundWindowThreadId = WindowsServices.GetWindowThreadProcessId(currentForegroundWindow, IntPtr.Zero);
            WindowsServices.AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, true);
            WindowsServices.SetWindowPos(interopHelper.Handle, new IntPtr(0), 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_SHOWWINDOW);
            WindowsServices.AttachThreadInput(currentForegroundWindowThreadId, thisWindowThreadId, false);
            window.Show();
            window.Activate();
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    }
}
