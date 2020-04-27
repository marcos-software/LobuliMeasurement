using CryptSharp;
using System;
using System.Windows.Forms;

namespace LobuliMeasurement
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            this.DialogResult = DialogResult.No;
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start($"{Config.HomepageBaseUrl}/php/register.php");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            checkLogin();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                checkLogin();
            }
        }

        public void checkLogin()
        {
            MySQL mysql = new MySQL();

            if (string.IsNullOrEmpty(textBox1.Text.Trim()) || string.IsNullOrEmpty(textBox2.Text.Trim()))
            {

                MessageBox.Show("Mail or Password is empty!");
                return;
            }

            string passwordHash = mysql.GetUserPasswordHash(textBox1.Text.Trim());

            if (string.IsNullOrEmpty(passwordHash))
            {
                MessageBox.Show("Mail is unknown!");
                return;
            }

            if (Crypter.CheckPassword(textBox2.Text.Trim(), passwordHash))
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Password is wrong!");
                return;
            }
        }        
    }
}
