using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace LobuliMeasurement
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            if (Config.ConfigExist() == false)
            {
                (new FormMySQL()).ShowDialog();
                ProcessStartInfo proc = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    WorkingDirectory = Environment.CurrentDirectory,
                    FileName = Application.ExecutablePath
                };

                try
                {
                    Process.Start(proc);
                    Environment.Exit(0);
                }
                catch (Exception ex)
                {
                    Environment.Exit(0);
                }
            }
            else
            {
                Config.Load();
            }

#if DEBUG == false
            bool loginsucceed = false;
            using (FormLogin login = new FormLogin())
            {
                DialogResult result = login.ShowDialog();
                
                if(result == DialogResult.OK)
                {
                    loginsucceed = true;
                }               
            }

            if(loginsucceed == false)
            {
                CloseApplication();
                Close();
            }
#endif

            InitializeComponent();
            InitializeText();
        }

        private void InitializeText()
        {
            lblVersion.Text = $"Version: {FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}";
        }

#region ButtonClicks
        private void btnDpMeasSingle_Click(object sender, System.EventArgs e)
        {
            //Hide();
            new FormWorker(WorkerMethod.SingleDatapointsAndMeasurement, this).ShowDialog();
            //CloseApplication();
        }

        private void btnDpMeasMulti_Click(object sender, System.EventArgs e)
        {
            //Hide();
            new FormDownloadData().ShowDialog();
            //CloseApplication();
        }

        private void btnDp_Click(object sender, System.EventArgs e)
        {
            //Hide();
            Process.Start(Config.HomepageBaseUrl);
            //CloseApplication();
        }

        private void btnClose_Click(object sender, System.EventArgs e)
        {
            CloseApplication();
        }
#endregion ButtonClicks

        private void CloseApplication()
        {
            //maybe ask user before close?
            Application.Exit();            
        }

        private void btnMl_Click(object sender, System.EventArgs e)
        {
            MessageBox.Show("This function is not implemented yet.");
            return;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            new FromReanalyzeAll().ShowDialog();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            new FormCalculateRatio().ShowDialog();
        }

        private void button3_Click(object sender, System.EventArgs e)
        {
            new FormCorrectFissurePerimeter().ShowDialog();
        }
    }

    public enum WorkerMethod
    {
        JustDatapoints,
        JustMeasurement,
        SingleDatapointsAndMeasurement,
        MultiDatapointsAndMeasurement,
        ReanalyzeSingleDatapointsAndMeasurement,
    }
}
