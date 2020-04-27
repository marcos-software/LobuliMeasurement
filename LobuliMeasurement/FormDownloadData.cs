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
    public partial class FormDownloadData : Form
    {
        public const int waitTime = 0;
        public FormDownloadData()
        {
            InitializeComponent();

            MySQL mysql = new MySQL();
            List<Cut> cutList = mysql.GetCut();

            dataGridView1.Rows.Clear();

            foreach (Cut cut in cutList)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                row.Cells[0].Value = cut.Id;
                row.Cells[1].Value = cut.Age;
                row.Cells[2].Value = cut.Genotype;
                row.Cells[3].Value = cut.Animal;
                row.Cells[4].Value = cut.CutIdentifier;
                row.Cells[5].Value = cut.Method;
                row.Cells[6].Value = cut.DateMeasurement.ToString("dd.MM.yyyy");
                row.Cells[7].Value = cut.DateStaining.ToString("dd.MM.yyyy");
                row.Cells[8].Value = cut.ZoomFactor;
                row.Cells[9].Value = cut.Layer;
                row.Cells[10].Value = cut.Note;
                row.Cells[11].Value = "Reanalyze now";
                dataGridView1.Rows.Add(row);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                //if(MessageBoxAutocloseWithButtons.Show(5, true, "Yes", "No", $"Include X Y coordinates?") == DialogResult.OK)
                if (MessageBox.Show("Include X Y coordinates?", "Include coordinates", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    FormPleaseWait pleaseWait = new FormPleaseWait("download data");
                    pleaseWait.Owner = this;
                    pleaseWait.StartPosition = FormStartPosition.CenterParent;
                    pleaseWait.Show(this);
                    int x = this.DesktopBounds.Left + (this.Width - pleaseWait.Width) / 2;
                    int y = this.DesktopBounds.Top + (this.Height - pleaseWait.Height) / 2;
                    pleaseWait.SetDesktopLocation(x, y);

                    string id = senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString();

                    MySQL mySql = new MySQL();

                    string fileName = $"P{senderGrid.Rows[e.RowIndex].Cells[1].Value}_{senderGrid.Rows[e.RowIndex].Cells[3].Value}_{senderGrid.Rows[e.RowIndex].Cells[2].Value}_{senderGrid.Rows[e.RowIndex].Cells[5].Value}_{senderGrid.Rows[e.RowIndex].Cells[8].Value}x{senderGrid.Rows[e.RowIndex].Cells[4].Value}.jpg";
                    string filePath = Path.Combine(Path.GetTempPath(), fileName);
                    mySql.GetImage(id, "marked image", filePath);

                    string filePathOriginal = Path.Combine(Path.GetTempPath(), "original-"+fileName);
                    string filePathMarked = Path.Combine(Path.GetTempPath(), "marked-"+fileName);

                    mySql.GetImage(id, "original image", filePathOriginal);
                    mySql.GetImage(id, "marked image", filePathMarked);

                    filePath = filePathOriginal;

                    List<Point> coordinates = mySql.GetCoordinates(id);

                    pleaseWait.Close();
                    Hide();

                    int blurredFactor = FormWorker.fallbackBlurredFactor;
                    string blurredFactorString = blurredFactor.ToString();
                    if (MessageBoxCustom.InputBox("Setting: Min Dist", "minimal distance between 2 datapoints:", ref blurredFactorString) == DialogResult.OK)
                    {
                        int.TryParse(blurredFactorString, out blurredFactor);
                    }

                    int offset = FormWorker.fallbackOffset;
                    string offsetString = offset.ToString();
                    if (MessageBoxCustom.InputBox("Setting: Offset", "offset:", ref offsetString) == DialogResult.OK)
                    {
                        int.TryParse(offsetString, out offset);
                    }

                    int restArea = FormWorker.fallbackIntersectionPointRestrictedRadius;
                    string restAreaString = restArea.ToString();
                    if (MessageBoxCustom.InputBox("Setting: Restricted Area", "intersection point restriced area:", ref restAreaString) == DialogResult.OK)
                    {
                        int.TryParse(restAreaString, out restArea);
                    }

                    FormWorker formWorker = new FormWorker(WorkerMethod.ReanalyzeSingleDatapointsAndMeasurement, this);


                    bool smthingChanged = false;
                    if (offset > 0)
                    {
                        formWorker.DEFAULT_OFFSET = offset;
                        formWorker._offsetOuterLine = offset;
                        smthingChanged = true;
                    }

                    if (blurredFactor > 0)
                    {
                        formWorker.DEFAULT_BLURRED_FACTOR = blurredFactor;
                        formWorker._blurredFactor = blurredFactor;
                        smthingChanged = true;
                    }

                    if (restArea > 0)
                    {
                        formWorker.DEFAULT_INTERSECTION_POINT_RESTRICTED_RADIUS = restArea;
                        formWorker._intersectionPointRestrictedRadius = restArea;
                        smthingChanged = true;
                    }

                    if (smthingChanged)
                    {
                        formWorker.FillOptionsWithDefaultValues();
                    }

                    formWorker._waitTime = waitTime;
                    formWorker._filePath = filePath;
                    formWorker._coordinates = coordinates;
                    formWorker._note = senderGrid.Rows[e.RowIndex].Cells[10].Value.ToString();
                    formWorker._layer = senderGrid.Rows[e.RowIndex].Cells[9].Value.ToString();

                    if (File.Exists(filePathOriginal))
                    {
                        formWorker._originalPictureOVERWRITE = new Bitmap(filePathOriginal);
                    }
                    else
                    {
                        formWorker._originalPictureOVERWRITE = new Bitmap(1024,768);
                    }

                    if (File.Exists(filePathMarked))
                    {
                        formWorker._markedPictureOVERWRITE = new Bitmap(filePathMarked);
                    }
                    else
                    {
                        formWorker._markedPictureOVERWRITE = new Bitmap(1024, 768);
                    }

                    formWorker.ShowDialog();
                }
                else
                {
                    FormPleaseWait pleaseWait = new FormPleaseWait("download data");
                    pleaseWait.Owner = this;
                    pleaseWait.StartPosition = FormStartPosition.CenterParent;
                    pleaseWait.Show(this);
                    int x = this.DesktopBounds.Left + (this.Width - pleaseWait.Width) / 2;
                    int y = this.DesktopBounds.Top + (this.Height - pleaseWait.Height) / 2;
                    pleaseWait.SetDesktopLocation(x, y);

                    string id = senderGrid.Rows[e.RowIndex].Cells[0].Value.ToString();

                    MySQL mySql = new MySQL();

                    string fileName =
                        $"P{senderGrid.Rows[e.RowIndex].Cells[1].Value}_{senderGrid.Rows[e.RowIndex].Cells[3].Value}_{senderGrid.Rows[e.RowIndex].Cells[2].Value}_{senderGrid.Rows[e.RowIndex].Cells[5].Value}_{senderGrid.Rows[e.RowIndex].Cells[8].Value}x{senderGrid.Rows[e.RowIndex].Cells[4].Value}.jpg";
                    string filePath = Path.Combine(Path.GetTempPath(), fileName);
                    mySql.GetImage(id, "original image", filePath);

                    string filePathOriginal = Path.Combine(Path.GetTempPath(), "original-" + fileName);
                    string filePathMarked = Path.Combine(Path.GetTempPath(), "marked-" + fileName);

                    mySql.GetImage(id, "original image", filePathOriginal);
                    mySql.GetImage(id, "marked image", filePathMarked);

                    pleaseWait.Close();
                    Hide();

                    int offset = FormWorker.fallbackOffset;
                    string offsetString = offset.ToString();
                    if (MessageBoxCustom.InputBox("Setting: Offset", "offset:", ref offsetString) == DialogResult.OK)
                    {
                        int.TryParse(offsetString, out offset);
                    }

                    int blurredFactor = FormWorker.fallbackBlurredFactor;
                    string blurredFactorString = blurredFactor.ToString();
                    if (MessageBoxCustom.InputBox("Setting: Min Dist", "minimal distance between 2 datapoints:", ref blurredFactorString) == DialogResult.OK)
                    {
                        int.TryParse(blurredFactorString, out blurredFactor);
                    }

                    int restArea = FormWorker.fallbackIntersectionPointRestrictedRadius;
                    string restAreaString = restArea.ToString();
                    if (MessageBoxCustom.InputBox("Setting: Restricted Area", "intersection point restriced area:", ref restAreaString) == DialogResult.OK)
                    {
                        int.TryParse(restAreaString, out restArea);
                    }

                    FormWorker formWorker = new FormWorker(WorkerMethod.ReanalyzeSingleDatapointsAndMeasurement, this);


                    bool smthingChanged = false;
                    if (offset > 0)
                    {
                        formWorker.DEFAULT_OFFSET = offset;
                        formWorker._offsetOuterLine = offset;
                        smthingChanged = true;
                    }

                    if (blurredFactor > 0)
                    {
                        formWorker.DEFAULT_BLURRED_FACTOR = blurredFactor;
                        formWorker._blurredFactor = blurredFactor;
                        smthingChanged = true;
                    }

                    if (restArea > 0)
                    {
                        formWorker.DEFAULT_INTERSECTION_POINT_RESTRICTED_RADIUS = restArea;
                        formWorker._intersectionPointRestrictedRadius = restArea;
                        smthingChanged = true;
                    }

                    if (smthingChanged)
                    {
                        formWorker.FillOptionsWithDefaultValues();
                    }

                    formWorker._waitTime = waitTime;
                    formWorker._filePath = filePath;
                    formWorker._layer = senderGrid.Rows[e.RowIndex].Cells[9].Value.ToString();
                    
                    if (File.Exists(filePathOriginal))
                    {
                        formWorker._originalPictureOVERWRITE = new Bitmap(filePathOriginal);
                    }
                    else
                    {
                        formWorker._originalPictureOVERWRITE = new Bitmap(1024, 768);
                    }

                    if (File.Exists(filePathMarked))
                    {
                        formWorker._markedPictureOVERWRITE = new Bitmap(filePathMarked);
                    }
                    else
                    {
                        formWorker._markedPictureOVERWRITE = new Bitmap(1024, 768);
                    }

                    formWorker.ShowDialog();
                }
            }
        }
    }
}
