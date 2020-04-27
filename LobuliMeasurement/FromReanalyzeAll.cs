using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LobuliMeasurement
{
    public partial class FromReanalyzeAll : Form
    {
        private int firstId;
        private int lastId;
        private MySQL mysql;
        private List<Cut> cutList;

        private int _waitTime = 1;

        public FromReanalyzeAll()
        {
            InitializeComponent();

            mysql = new MySQL();
            mysql._waitTime = _waitTime;
            cutList = mysql.GetCut();

            if (cutList.Count <= 1) return;

            List<Cut> orderedEnumerable = cutList.OrderBy(c => c.Id).ToList();

            firstId = orderedEnumerable.First().Id;
            lastId = orderedEnumerable.Last().Id;

            textBox1.Text = firstId.ToString();
            textBox2.Text = lastId.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            firstId = Convert.ToInt32(textBox1.Text);
            lastId = Convert.ToInt32(textBox2.Text);
            textBox1.Enabled = false;
            textBox2.Enabled = false;
            Reanalyze();
        }

        private void Reanalyze()
        {
            cutList.Clear();
            cutList = mysql.GetCut(firstId, lastId);

            bool filter = false;
            int filterAge = 0;
            string filterGenoType = "";
            string filterLayer = "";

            if (string.IsNullOrEmpty(textBox3.Text) == false)
            {
                if (int.TryParse(textBox3.Text, out filterAge))
                {
                    filter = true;
                }
            }

            int customOffset = 0;
            int customBlurredFactor = 0;
            int customIntersectionPointRestrictedRadius = 0;

            if (string.IsNullOrEmpty(textBox5.Text) == false)
            {
                int.TryParse(textBox5.Text, out customOffset);
            }

            if (string.IsNullOrEmpty(textBox6.Text) == false)
            {
                int.TryParse(textBox6.Text, out customBlurredFactor);
            }

            if (string.IsNullOrEmpty(textBox7.Text) == false)
            {
                int.TryParse(textBox7.Text, out customIntersectionPointRestrictedRadius);
            }

            if (string.IsNullOrEmpty(textBox4.Text) == false)
            {
                if (textBox4.Text.Trim().ToUpper()== "WT")
                {
                    filterGenoType = "WT";
                    filter = true;
                }
                if (textBox4.Text.Trim().ToUpper() == "KO")
                {
                    filterGenoType = "KO";
                    filter = true;
                }
            }

            if (string.IsNullOrEmpty(textBox8.Text) == false)
            {
                filterLayer = textBox8.Text.Trim().ToLower();
                filter = true;
            }

            foreach (Cut cut in cutList)
            {
                try
                {
                    if (filter)
                    {
                        if (filterAge > 0 && cut.Age != filterAge)
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(filterGenoType) == false && cut.Genotype != filterGenoType)
                        {
                            continue;
                        }

                        if (string.IsNullOrEmpty(filterLayer) == false && cut.Layer.ToLower().Contains(filterLayer) == false)
                        {
                            continue;
                        }
                    }

                    FormPleaseWait pleaseWait = new FormPleaseWait("download data");
                    pleaseWait.Owner = this;
                    pleaseWait.StartPosition = FormStartPosition.CenterParent;
                    pleaseWait.Show(this);
                    int x = this.DesktopBounds.Left + (this.Width - pleaseWait.Width) / 2;
                    int y = this.DesktopBounds.Top + (this.Height - pleaseWait.Height) / 2;
                    pleaseWait.SetDesktopLocation(x, y);

                    string fileName =
                        $"P{cut.Age}_{cut.Animal}_{cut.Genotype}_{cut.Method}_{cut.ZoomFactor}x{cut.CutIdentifier}.jpg";
                    string filePath = Path.Combine(Path.GetTempPath(), fileName);
                    mysql.GetImage(cut.Id.ToString(), "marked image", filePath);

                    string filePathOriginal = Path.Combine(Path.GetTempPath(), "original-" + fileName);
                    string filePathMarked = Path.Combine(Path.GetTempPath(), "marked-" + fileName);

                    mysql.GetImage(cut.Id.ToString(), "original image", filePathOriginal);
                    mysql.GetImage(cut.Id.ToString(), "marked image", filePathMarked);

                    filePath = filePathOriginal;

                    List<Point> coordinates = mysql.GetCoordinates(cut.Id.ToString());

                    pleaseWait.Close();
                    Hide();

                    FormWorker formWorker = new FormWorker(WorkerMethod.ReanalyzeSingleDatapointsAndMeasurement, this);

                    bool smthingChanged = false;
                    if (customOffset > 0)
                    {
                        formWorker.DEFAULT_OFFSET = customOffset;
                        formWorker._offsetOuterLine = customOffset;
                        smthingChanged = true;
                    }

                    if (customBlurredFactor > 0)
                    {
                        formWorker.DEFAULT_BLURRED_FACTOR = customBlurredFactor;
                        formWorker._blurredFactor = customBlurredFactor;
                        smthingChanged = true;
                    }

                    if (customIntersectionPointRestrictedRadius > 0)
                    {
                        formWorker.DEFAULT_INTERSECTION_POINT_RESTRICTED_RADIUS = customIntersectionPointRestrictedRadius;
                        formWorker._intersectionPointRestrictedRadius = customIntersectionPointRestrictedRadius;
                        smthingChanged = true;
                    }

                    if (smthingChanged)
                    {
                        formWorker.FillOptionsWithDefaultValues();
                    }

                    formWorker._waitTime = _waitTime;
                    formWorker._filePath = filePath;
                    formWorker._coordinates = coordinates;
                    formWorker._note = cut.Note;
                    formWorker._layer = cut.Layer;
                    formWorker._originalPictureOVERWRITE = new Bitmap(filePathOriginal);
                    formWorker._markedPictureOVERWRITE = new Bitmap(filePathMarked);
                    formWorker.ShowDialog();
                }
                catch(Exception ex)
                {
                    //just for debug
                }
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) //&&
                //(e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = true;
            //}
        }
    }
}
