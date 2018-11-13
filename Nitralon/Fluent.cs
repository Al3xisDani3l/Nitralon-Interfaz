using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace Nitralon
{
    class Fluent
    {

      public  static void ActivarTransparencia(Window window)
        {
            NativeMethods.EnableBlur(window);
        }

    }

    internal static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hand, ref WindowsCompositionAttributeData data);

        internal enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLED_GRADIENT = 10,
            ACCENT_ENABLED_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLED_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradeintColor;
            public int AnimationId;

        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct WindowsCompositionAttributeData
        {
            public WindowsCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }

        internal enum WindowsCompositionAttribute
        {
            WCA_ACCENT_POLICY = 19
        }

        public static void EnableBlur(Window window)
        {
            var windowHelper = new WindowInteropHelper(window);
            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLED_BLURBEHIND;
            var accentStructSize = Marshal.SizeOf(accent);
            var accentptr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentptr, false);
            var data = new WindowsCompositionAttributeData();
            data.Attribute = WindowsCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentptr;
            SetWindowCompositionAttribute(windowHelper.Handle, ref data);
            Marshal.FreeHGlobal(accentptr);
        }
    }
}
