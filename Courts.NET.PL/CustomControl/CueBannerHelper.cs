using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace STL.PL.CustomControl
{
    public class CueBannerHelper
    {
        #region Win32 API's
        [StructLayout(LayoutKind.Sequential)]
        public struct COMBOBOXINFO
        {
            public int cbSize;
            public RECT rcItem;
            public RECT rcButton;
            public ComboBoxButtonState stateButton;
            public IntPtr hwndCombo;
            public IntPtr hwndItem;
            public IntPtr hwndList;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public enum ComboBoxButtonState
        {
            STATE_SYSTEM_NONE = 0,
            STATE_SYSTEM_INVISIBLE = 0x00008000,
            STATE_SYSTEM_PRESSED = 0x00000008
        }

        /// <summary>
        /// Used to get the current Cue Banner on an edit control.
        /// </summary>
        public const int EM_GETCUEBANNER = 0x1502;
        /// <summary>
        /// Used to set a Cue Banner on an edit control.
        /// </summary>
        public const int EM_SETCUEBANNER = 0x1501; // TextBox and Combobox Integer on Windows 7, Windows Vista and above
        /// <summary>
        /// Used to set a Cue Banner on an edit control on Windows XP.
        /// </summary>
        public const int CB_SETCUEBANNER = 0x1703; //Combobox Integer on Windows XP

        [DllImport("user32.dll")]
        public static extern bool GetComboBoxInfo(IntPtr hwnd, ref COMBOBOXINFO pcbi);

        [DllImport("user32.dll")]
        public static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        #endregion

        #region Method members
        public static void SetCueBanner(Control control, string cueBanner)
        {
            if (control is ComboBox)
            {
                CueBannerHelper.COMBOBOXINFO info = new CueBannerHelper.COMBOBOXINFO();
                info.cbSize = Marshal.SizeOf(info);

                if (CueBannerHelper.GetComboBoxInfo(control.Handle, ref info))
                {
                    if (GetOSVersion() == "Windows XP")
                        CueBannerHelper.SendMessage(info.hwndItem, CueBannerHelper.CB_SETCUEBANNER, 0, cueBanner);
                    else
                        CueBannerHelper.SendMessage(info.hwndItem, CueBannerHelper.EM_SETCUEBANNER, 0, cueBanner);
                }
            }
            else
            {
                CueBannerHelper.SendMessage(control.Handle, CueBannerHelper.EM_SETCUEBANNER, 0, cueBanner);
            }
        }

        public static string GetOSVersion()
        {
            int _MajorVersion = Environment.OSVersion.Version.Major;

            switch (_MajorVersion)
            {
                case 5:
                    return "Windows XP";
                case 6:
                    switch (Environment.OSVersion.Version.Minor)
                    {
                        case 0:
                            return "Windows Vista";
                        case 1:
                            return "Windows 7";
                        default:
                            return "Windows Vista & above";
                    }
                default:
                    return "Unknown";
            }
        }
        #endregion
    }
}
