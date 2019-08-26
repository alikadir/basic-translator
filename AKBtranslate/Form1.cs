using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Runtime.CompilerServices;

namespace AKBtranslate
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();  
            
            KeyboardHook dinle = new KeyboardHook();
            dinle.KeyPressed += new EventHandler<KeyPressedEventArgs>(dinle_KeyPressed);
            dinle.RegisterHotKey(HookModifierKeys.Control, 0x02);


          


        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            FormKapat();
          
        }


        [DllImport("user32.dll")]

        static extern int GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(int hWnd, StringBuilder text, int count);

        private void GetActiveWindow()
        {

            const int nChars = 256;
            int handle = 0;
            StringBuilder Buff = new StringBuilder(nChars);

            handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                label1.Text = Buff.ToString();
                label2.Text = handle.ToString();
            }

        }

        [DllImport("user32.dll")]
        static extern ushort GetAsyncKeyState(int vKey);


        [DllImport("user32.dll")]
        public static extern int SendMessage(
              int hWnd,      // handle to destination window
              uint Msg,       // message
              long wParam,  // first message parameter
              long lParam   // second message parameter
              );



        [MethodImpl(MethodImplOptions.Synchronized)]
        private string getSelectedTextHACK()
        {
            object restorePoint = Clipboard.GetData(DataFormats.UnicodeText);

            var hWnd = GetForegroundWindow();
          
          //  SendMessage(hWnd, 0x301, 0, 0);
          

            
            
            SendKeys.SendWait("^c");
            

            string result = Convert.ToString(Clipboard.GetText());
            Clipboard.SetData(DataFormats.UnicodeText, restorePoint);

            return result;
        }




        void dinle_KeyPressed(object sender, KeyPressedEventArgs e)
        {



            GetActiveWindow();
            label3.Text = getSelectedTextHACK();
            Application.DoEvents();

            FormAc();
           // MessageBox.Show("bass");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnHide_Click(object sender, EventArgs e)
        {
            FormKapat();
        }

        void FormAc()
        {
            this.Opacity = 100;
            this.ShowInTaskbar = true;
            this.Activate();
            this.Focus();
        }
        void FormKapat()
        {
            this.Opacity = 0;
            this.ShowInTaskbar = false;

        }



       


    }
}
