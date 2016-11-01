using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace _4kMagnifier
{
    public class ClickOnPointTool
    {
        public static void ClickOnPoint(IntPtr wndHandle, System.Drawing.Point clientPoint)
        {
            int IParam = (clientPoint.Y << 16) + clientPoint.X;
            WindowsServices.SendMessage(wndHandle, 0x0201, (IntPtr)0,(IntPtr)IParam);
            WindowsServices.SendMessage(wndHandle, 0x0202, (IntPtr)0, (IntPtr)IParam);
        }

    }
}
