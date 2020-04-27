using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace LobuliMeasurement
{
    public partial class FormCollectInfo : Form
    {
        public string Age;
        public string AnimalNumber;
        public string Genotype;
        public string Method;
        public string ZoomFactor;
        public string CutIdentifier;
        public string Layer;
        public DateTime DateStaining;
        private BackgroundWorker closeTimer;
        private string btnMessage = "Continue";
        private string btnFormat = "{0} ({1})";
        private bool stoppedWorker = false;
        private bool considerInput = false;
        public int _waitTime;

        public FormCollectInfo(string filePath, int waitTime = 0, string layer = "")
        {
            if (waitTime > 0) _waitTime = waitTime;
            InitializeComponent();
            Layer = layer;
            CollectInfoFromFileName(filePath);
            if(_waitTime > 0) StartCloseTimer();
            considerInput = true;
        }

        private void StopCloseTimer()
        {
            try
            {
                stoppedWorker = true;
                button1.Text = btnMessage ?? string.Empty;
                if (closeTimer != null && closeTimer.WorkerSupportsCancellation)
                {
                    closeTimer.CancelAsync();
                }
            }
            catch (Exception ex)
            {
                //just for debug
            }
        }

        private void StartCloseTimer()
        {
            try
            {
                button1.Text = string.Format(btnFormat, btnMessage ?? string.Empty, _waitTime);
                closeTimer = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true
                };
                closeTimer.DoWork += StartTimer;
                closeTimer.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                //just for debug
            }
        }

        private void StartTimer(object sender, DoWorkEventArgs e)
        {
            try
            {
                while (_waitTime > 0)
                {
                    RefreshButtonText(_waitTime--);
                    Thread.Sleep(1000);
                    if (stoppedWorker) return;
                }

                if (stoppedWorker == false)
                {
                    DoClosing();
                }
            }
            catch (Exception ex)
            {
                //just for debug
            }
        }

        private void RefreshButtonText(int seconds)
        {
            try
            {
                if (InvokeRequired)
                {
                    Invoke(new Action<int>(RefreshButtonText), new object[] {seconds});
                    return;
                }

                button1.Text = string.Format(btnFormat, btnMessage ?? string.Empty, seconds);
                Refresh();
            }
            catch (Exception ex)
            {
                //just for debug
            }
        }

        private void CollectInfoFromFileName(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) == false)
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    //example: P10_2_KO_nissl_2,5xc
                    Regex regex = new Regex(@"[P](\d*)_(\d[a-zA-Z]*)_([WTKO]*)_([a-zA-Z]*)_([0-9,]*)x(.*)");
                    Match match = regex.Match(Path.GetFileNameWithoutExtension(fileName));
                    if (match.Success)
                    {
                        if (match.Groups.Count - 1 >= 1 && !string.IsNullOrEmpty(match.Groups[1].Value))
                        {
                            Age = match.Groups[1].Value;
                        }

                        if (match.Groups.Count - 1 >= 2 && !string.IsNullOrEmpty(match.Groups[2].Value))
                        {
                            AnimalNumber = match.Groups[2].Value;
                        }

                        if (match.Groups.Count - 1 >= 3 && !string.IsNullOrEmpty(match.Groups[3].Value))
                        {
                            Genotype = match.Groups[3].Value;
                        }

                        if (match.Groups.Count - 1 >= 4 && !string.IsNullOrEmpty(match.Groups[4].Value))
                        {
                            Method = match.Groups[4].Value;
                        }

                        if (match.Groups.Count - 1 >= 5 && !string.IsNullOrEmpty(match.Groups[5].Value))
                        {
                            ZoomFactor = match.Groups[5].Value;
                        }

                        if (match.Groups.Count - 1 >= 6 && !string.IsNullOrEmpty(match.Groups[6].Value))
                        {
                            CutIdentifier = match.Groups[6].Value;
                        }
                    }

                    DateTime dtCreation = DateTime.Now.AddYears(5);
                    DateTime dtModification = DateTime.Now.AddYears(5);
                    try
                    {
                        dtCreation = File.GetCreationTime(filePath);
                    }
                    catch (Exception ex)
                    {

                    }

                    try
                    {
                        dtModification = File.GetLastWriteTime(filePath);
                    }
                    catch (Exception ex)
                    {

                    }

                    if (dtCreation < dtModification)
                    {
                        dtpStaining.Value = dtCreation;
                    }
                    else
                    {
                        dtpStaining.Value = dtModification;
                    }

                    if (dtpStaining.Value > DateTime.Now) dtpStaining.Value = DateTime.Now;
                }

                txbAge.Text = Age;
                txbAnimalNumber.Text = AnimalNumber;
                txbGenotype.Text = Genotype;
                txbMethod.Text = Method;
                txbZoomFactor.Text = ZoomFactor;
                txbCutIfentifier.Text = CutIdentifier;
                txbLayer.Text = Layer;
            }
            catch (Exception ex)
            {
                //just for debug
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DoClosing();
        }

        private void DoClosing()
        {
            try
            {
                if (Disposing) return;

                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)delegate { DoClosing(); });
                    //Invoke(new Action<bool>(CloseForm), message);
                    return;
                }

                Age = txbAge.Text ?? string.Empty;
                AnimalNumber = txbAnimalNumber.Text ?? string.Empty;
                Genotype = txbGenotype.Text ?? string.Empty;
                Method = txbMethod.Text ?? string.Empty;
                ZoomFactor = txbZoomFactor.Text ?? string.Empty;
                CutIdentifier = txbCutIfentifier.Text ?? string.Empty;
                Layer = txbLayer.Text ?? string.Empty;
                DateStaining = dtpStaining.Value;

                Close();
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex);
            }
        }

        private void txbAge_TextChanged(object sender, EventArgs e)
        {
            if (considerInput)
            {
                StopCloseTimer();
            }
        }

        private void txbGenotype_TextChanged(object sender, EventArgs e)
        {
            if (considerInput)
            {
                StopCloseTimer();
            }
        }

        private void txbAnimalNumber_TextChanged(object sender, EventArgs e)
        {
            if (considerInput)
            {
                StopCloseTimer();
            }
        }

        private void txbMethod_TextChanged(object sender, EventArgs e)
        {
            if (considerInput)
            {
                StopCloseTimer();
            }
        }

        private void txbZoomFactor_TextChanged(object sender, EventArgs e)
        {
            if (considerInput)
            {
                StopCloseTimer();
            }
        }

        private void txbCutIfentifier_TextChanged(object sender, EventArgs e)
        {
            if (considerInput)
            {
                StopCloseTimer();
            }
        }

        private void txbLayer_TextChanged(object sender, EventArgs e)
        {
            if (considerInput)
            {
                StopCloseTimer();
            }
        }

        private void dtpStaining_ValueChanged(object sender, EventArgs e)
        {
            if (considerInput)
            {
                StopCloseTimer();
            }
        }
    }
}
