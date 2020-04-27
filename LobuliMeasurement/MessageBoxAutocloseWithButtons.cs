using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace LobuliMeasurement
{
    public partial class MessageBoxAutocloseWithButtons : Form
    {
        private const int CS_DROPSHADOW = 0x00020000;
        private BackgroundWorker closeTimer;
        private string btnMessage = "Ok";
        private string btnFormat = "{0} ({1})";
        private int closeTime = 5;
        private int originalHeight = 214;
        private int originalWidth = 406;

        public static DialogResult Show(bool onTop, string textBtn1, string textBtn2, string message)
        {
            using (MessageBoxAutocloseWithButtons form = new MessageBoxAutocloseWithButtons(5, textBtn1, textBtn2, message))
            {
                form.TopMost = onTop;
                return form.ShowDialog();
            }
        }

        public static DialogResult Show(bool onTop, string textBtn1, string textBtn2, string message, params object[] args)
        {
            using (MessageBoxAutocloseWithButtons form = new MessageBoxAutocloseWithButtons(5, textBtn1, textBtn2, string.Format(message, args)))
            {
                form.TopMost = onTop;
                return form.ShowDialog();
            }
        }

        public static DialogResult Show(string textBtn1, string textBtn2, string message)
        {
            using (MessageBoxAutocloseWithButtons form = new MessageBoxAutocloseWithButtons(5, textBtn1, textBtn2, message))
            {
                return form.ShowDialog();
            }
        }

        public static DialogResult Show(string textBtn1, string textBtn2, string message, params object[] args)
        {
            using (MessageBoxAutocloseWithButtons form = new MessageBoxAutocloseWithButtons(5, textBtn1, textBtn2, string.Format(message, args)))
            {
                return form.ShowDialog();
            }
        }

        public static DialogResult Show(int closeTime, bool onTop, string textBtn1, string textBtn2, string message)
        {
            using (MessageBoxAutocloseWithButtons form = new MessageBoxAutocloseWithButtons(closeTime, textBtn1, textBtn2, message))
            {
                form.TopMost = onTop;
                return form.ShowDialog();
            }
        }

        public static DialogResult Show(int closeTime, bool onTop, string textBtn1, string textBtn2, string message, params object[] args)
        {
            using (MessageBoxAutocloseWithButtons form = new MessageBoxAutocloseWithButtons(closeTime, textBtn1, textBtn2, string.Format(message, args)))
            {
                form.TopMost = onTop;
                return form.ShowDialog();
            }
        }

        public static DialogResult Show(int closeTime, string textBtn1, string textBtn2, string message)
        {
            using (MessageBoxAutocloseWithButtons form = new MessageBoxAutocloseWithButtons(closeTime, textBtn1, textBtn2, message))
            {
                return form.ShowDialog();
            }
        }

        public static DialogResult Show(int closeTime, string textBtn1, string textBtn2, string message, params object[] args)
        {
            using (MessageBoxAutocloseWithButtons form = new MessageBoxAutocloseWithButtons(closeTime, textBtn1, textBtn2, string.Format(message, args)))
            {
                return form.ShowDialog();
            }
        }

        private MessageBoxAutocloseWithButtons(int closeTime, string textBtn1, string textBtn2, string message)
        {
            this.closeTime = closeTime;

            try
            {
                InitializeComponent();
                InitializeLanguage();
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex);
            }

            btnMessage = textBtn1;
            btnCancel.Text = textBtn2;
            textBox1.Text = message;
            btnClose.Text = btnMessage;

            try
            {
                if (closeTime > 0) StartCloseTimer();
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex);
            }

            lblMessageBoxUnicornTitel.BackColor = Color.Transparent;
            lblMessageBoxUnicornTitel.Parent = pictureBox1;
        }

        private void InitializeLanguage()
        {
            btnClose.Text = "Ok";
            btnMessage = "Ok";
            lblMessageBoxUnicornTitel.Text = "Information";
            Text = "Hinweis";
        }

        private void StartCloseTimer()
        {
            btnClose.Text = string.Format(btnFormat, "Ok", closeTime);
            closeTimer = new BackgroundWorker { WorkerSupportsCancellation = true };
            closeTimer.DoWork += StartTimer;
            closeTimer.RunWorkerAsync();
        }

        private void RefreshButtonText(int seconds)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<int>(RefreshButtonText), new object[] { seconds });
                return;
            }
            btnClose.Text = string.Format(btnFormat, btnMessage, seconds);
            Refresh();
        }

        private void StartTimer(object sender, DoWorkEventArgs e)
        {
            while (closeTime > 0)
            {
                RefreshButtonText(closeTime--);
                Thread.Sleep(1000);
            }
            DialogResult = DialogResult.OK;
            CloseForm();
        }

        private void CloseForm(bool message = false)
        {
            try
            {
                if (Disposing) return;

                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate { CloseForm(message); });
                    //Invoke(new Action<bool>(CloseForm), message);
                    return;
                }

                if (message) MessageBox.Show("Form is closing");
                Close();
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle |= CS_DROPSHADOW;
                return cp;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            CloseForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            CloseForm();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            //dont do this!
            return;
            Size size = TextRenderer.MeasureText(textBox1.Text, textBox1.Font);
            Width = Width + size.Width - textBox1.Width;
            Height = Height + size.Height - textBox1.Height;
            textBox1.Width = size.Width > originalWidth ? size.Width : originalWidth;
            textBox1.Height = size.Height > originalHeight ? size.Height : originalHeight;
        }
    }
}
