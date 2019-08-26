using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace AKBtranslate
{
    public sealed class KeyboardHook : IDisposable
    {

        [DllImport("user32.dll")]

        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]

        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        static extern ushort GetAsyncKeyState(int vKey);

        private class Window : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;

            public Window()
            {
                this.CreateHandle(new CreateParams());
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_HOTKEY)
                {
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);

                    HookModifierKeys modifier = (HookModifierKeys)((int)m.LParam & 0xFFFF);

                    if (KeyPressed != null)

                        KeyPressed(this, new KeyPressedEventArgs(modifier, key));
                }
            }

            public event EventHandler<KeyPressedEventArgs> KeyPressed;

            public void Dispose() { this.DestroyHandle(); }

        }

        private Window _window = new Window();

        private int _currentId;

        public KeyboardHook()
        {
            _window.KeyPressed += delegate(object sender, KeyPressedEventArgs args)
            {
                if (KeyPressed != null)
                    KeyPressed(this, args);
            };

        }

        public void RegisterHotKey(HookModifierKeys modifier, Keys key)
        {
            _currentId = _currentId + 1;

            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
                throw new InvalidOperationException("Couldn’t register the hot key.");

        }

        public void RegisterHotKey(HookModifierKeys modifier, int key)
        {
            _currentId = _currentId + 1;

            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, GetAsyncKeyState(key)))
                throw new InvalidOperationException("Couldn’t register the hot key.");

        }

        public event EventHandler<KeyPressedEventArgs> KeyPressed;

        public void Dispose()
        {
            for (int i = _currentId; i > 0; i--)
            {
                UnregisterHotKey(_window.Handle, i);
            }

            _window.Dispose();
        }

        internal void RegisterHotKey(int p, Keys keys)
        {
            throw new NotImplementedException();
        }
    }

    public class KeyPressedEventArgs : EventArgs
    {

        private HookModifierKeys _modifier;

        private Keys _key;

        internal KeyPressedEventArgs(HookModifierKeys modifier, Keys key)
        {
            _modifier = modifier; _key = key;
        }

        public HookModifierKeys Modifier
        {
            get { return _modifier; }
        }



        public Keys Key
        {
            get { return _key; }
        }
    }

    public enum HookModifierKeys : uint
    {
        Alt = 1, Control = 2, Shift = 4, Win = 8
    }
}

