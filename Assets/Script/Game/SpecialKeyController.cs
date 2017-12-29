using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

namespace NTUT.CSIE.GameDev.Game
{
    public class SpecialKeyController
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true, CallingConvention = CallingConvention.Winapi)] private static extern short GetKeyState(int keyCode);
        [DllImport("user32.dll")] private static extern int GetKeyboardState(byte[] lpKeyState);
        [DllImport("user32.dll", EntryPoint = "keybd_event")] private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public const byte VK_NUMLOCK = 0x90;
        public const byte VK_SCROLL = 0x91;
        private const uint KEYEVENTF_EXTENDEDKEY = 1;
        private const int KEYEVENTF_KEYUP = 0x2;
        private const int KEYEVENTF_KEYDOWN = 0x0;

        public static bool GetKeyState(byte vk)
        {
            return (((ushort)GetKeyState((int)vk)) & 0xffff) != 0;
        }

        public static void GetKeyStatus(byte vk, bool bState)
        {
            if (GetKeyState(vk) != bState)
            {
                keybd_event(vk, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0);
                keybd_event(vk, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }
    }
}
