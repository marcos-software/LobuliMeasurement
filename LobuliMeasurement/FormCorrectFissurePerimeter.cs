using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LobuliMeasurement
{
    public partial class FormCorrectFissurePerimeter : Form
    {
        private Cut cut;
        private int fissureCount = 0;
        private double fissurePeriSum = 0;

        double fisPeri_1_old = -1;
        double fisPeri_2_old = -1;
        double fisPeri_3_old = -1;
        double fisPeri_4_old = -1;
        double fisPeri_5_old = -1;
        double fisPeri_6_old = -1;
        double fisPeri_7_old = -1;
        double fisPeri_8_old = -1;
        double fisPeri_9_old = -1;
        double fisPeri_10_old = -1;

        double fisPeri_1_new = -1;
        double fisPeri_2_new = -1;
        double fisPeri_3_new = -1;
        double fisPeri_4_new = -1;
        double fisPeri_5_new = -1;
        double fisPeri_6_new = -1;
        double fisPeri_7_new = -1;
        double fisPeri_8_new = -1;
        double fisPeri_9_new = -1;
        double fisPeri_10_new = -1;

        public FormCorrectFissurePerimeter()
        {
            InitializeComponent();
            btnFix.Enabled = false;
            btnUpload.Enabled = false;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            btnFix.Enabled = false;
            btnUpload.Enabled = false;

            cut = null;
            fissureCount = 0;
            fissurePeriSum = 0;

            fisPeri_1_old = -1;
            fisPeri_2_old = -1;
            fisPeri_3_old = -1;
            fisPeri_4_old = -1;
            fisPeri_5_old = -1;
            fisPeri_6_old = -1;
            fisPeri_7_old = -1;
            fisPeri_8_old = -1;
            fisPeri_9_old = -1;
            fisPeri_10_old = -1;

            fisPeri_1_new = -1;
            fisPeri_2_new = -1;
            fisPeri_3_new = -1;
            fisPeri_4_new = -1;
            fisPeri_5_new = -1;
            fisPeri_6_new = -1;
            fisPeri_7_new = -1;
            fisPeri_8_new = -1;
            fisPeri_9_new = -1;
            fisPeri_10_new = -1;

            MySQL mysql = new MySQL();

            cut = mysql.GetCut(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text);

            if (cut == null)
            {
                MessageBox.Show("No cut for this data found!");
                return;
            }

            List<Result> results = mysql.GetResults(cut.Id.ToString());

            if (results == null || results.Count == 0)
            {
                MessageBox.Show("No results for this cut found!");
                return;
            }
            
            string fisPeri_1_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "1" && res.Key == "perimeter")?.Value;
            if (string.IsNullOrEmpty(fisPeri_1_string) == false)
            {
                Double.TryParse(fisPeri_1_string, out fisPeri_1_old);
                fissureCount++;
            }

            string fisPeri_2_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "2" && res.Key == "perimeter")?.Value;
            if (string.IsNullOrEmpty(fisPeri_2_string) == false)
            {
                Double.TryParse(fisPeri_2_string, out fisPeri_2_old);
                fissureCount++;
            }

            string fisPeri_3_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "3" && res.Key == "perimeter")?.Value;
            if (string.IsNullOrEmpty(fisPeri_3_string) == false)
            {
                Double.TryParse(fisPeri_3_string, out fisPeri_3_old);
                fissureCount++;
            }

            string fisPeri_4_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "4" && res.Key == "perimeter")?.Value;
            if (string.IsNullOrEmpty(fisPeri_4_string) == false)
            {
                Double.TryParse(fisPeri_4_string, out fisPeri_4_old);
                fissureCount++;
            }

            string fisPeri_5_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "5" && res.Key == "perimeter")?.Value;
            if (string.IsNullOrEmpty(fisPeri_5_string) == false)
            {
                Double.TryParse(fisPeri_5_string, out fisPeri_5_old);
                fissureCount++;
            }

            string fisPeri_6_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "6" && res.Key == "perimeter")?.Value;
            if (string.IsNullOrEmpty(fisPeri_6_string) == false)
            {
                Double.TryParse(fisPeri_6_string, out fisPeri_6_old);
                fissureCount++;
            }

            string fisPeri_7_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "7" && res.Key == "perimeter")?.Value;
            if (string.IsNullOrEmpty(fisPeri_7_string) == false)
            {
                Double.TryParse(fisPeri_7_string, out fisPeri_7_old);
                fissureCount++;
            }

            string fisPeri_8_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "8" && res.Key == "perimeter")?.Value;
            if (string.IsNullOrEmpty(fisPeri_8_string) == false)
            {
                Double.TryParse(fisPeri_8_string, out fisPeri_8_old);
                fissureCount++;
            }

            string fisPeri_9_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "9" && res.Key == "perimeter")?.Value;
            if (string.IsNullOrEmpty(fisPeri_9_string) == false)
            {
                Double.TryParse(fisPeri_9_string, out fisPeri_9_old);
                fissureCount++;
            }

            string fisPeri_10_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "10" && res.Key == "perimeter")?.Value;
            if (string.IsNullOrEmpty(fisPeri_10_string) == false)
            {
                Double.TryParse(fisPeri_10_string, out fisPeri_10_old);
                fissureCount++;
            }

            AddOldRow("1", fisPeri_1_old);
            AddOldRow("2", fisPeri_2_old);
            AddOldRow("3", fisPeri_3_old);
            AddOldRow("4", fisPeri_4_old);
            AddOldRow("5", fisPeri_5_old);
            AddOldRow("6", fisPeri_6_old);
            AddOldRow("7", fisPeri_7_old);
            AddOldRow("8", fisPeri_8_old);
            AddOldRow("9", fisPeri_9_old);
            AddOldRow("10", fisPeri_10_old);

            btnFix.Enabled = true;
        }

        private void AddOldRow(string id, double value)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell idCell = new DataGridViewTextBoxCell();
            idCell.Value = id;
            DataGridViewTextBoxCell perimeter = new DataGridViewTextBoxCell();
            if (value < 0)
            {
                perimeter.Value = "not existent";
            }
            else
            {
                perimeter.Value = value.ToString(FormWorker.FLOAT_ACCURACY);
            }
            row.Cells.Add(idCell);
            row.Cells.Add(perimeter);
            dgvOld.Rows.Add(row);
        }

        private void AddNewRow(string id, double value)
        {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell idCell = new DataGridViewTextBoxCell();
            idCell.Value = id;
            DataGridViewTextBoxCell perimeter = new DataGridViewTextBoxCell();
            if (value < 0)
            {
                perimeter.Value = "not existent";
            }
            else
            {
                perimeter.Value = value.ToString(FormWorker.FLOAT_ACCURACY);
            }
            row.Cells.Add(idCell);
            row.Cells.Add(perimeter);
            dgvNew.Rows.Add(row);
        }

        private void btnFix_Click(object sender, EventArgs e)
        {
            
            if (fisPeri_1_old >= 0)
            {
                fisPeri_1_new = Fix(fissureCount, 1, fisPeri_1_old);
            }

            if (fisPeri_2_old >= 0)
            {
                fisPeri_2_new = Fix(fissureCount, 2, fisPeri_2_old);
            }

            if (fisPeri_3_old >= 0)
            {
                fisPeri_3_new = Fix(fissureCount, 3, fisPeri_3_old);
            }

            if (fisPeri_4_old >= 0)
            {
                fisPeri_4_new = Fix(fissureCount, 4, fisPeri_4_old);
            }

            if (fisPeri_5_old >= 0)
            {
                fisPeri_5_new = Fix(fissureCount, 5, fisPeri_5_old);
            }

            if (fisPeri_6_old >= 0)
            {
                fisPeri_6_new = Fix(fissureCount, 6, fisPeri_6_old);
            }

            if (fisPeri_7_old >= 0)
            {
                fisPeri_7_new = Fix(fissureCount, 7, fisPeri_7_old);
            }

            if (fisPeri_8_old >= 0)
            {
                fisPeri_8_new = Fix(fissureCount, 8, fisPeri_8_old);
            }

            if (fisPeri_9_old >= 0)
            {
                fisPeri_9_new = Fix(fissureCount, 9, fisPeri_9_old);
            }

            if (fisPeri_10_old >= 0)
            {
                fisPeri_10_new = Fix(fissureCount, 10, fisPeri_10_old);
            }

            AddNewRow("1", fisPeri_1_new);
            AddNewRow("2", fisPeri_2_new);
            AddNewRow("3", fisPeri_3_new);
            AddNewRow("4", fisPeri_4_new);
            AddNewRow("5", fisPeri_5_new);
            AddNewRow("6", fisPeri_6_new);
            AddNewRow("7", fisPeri_7_new);
            AddNewRow("8", fisPeri_8_new);
            AddNewRow("9", fisPeri_9_new);
            AddNewRow("10", fisPeri_10_new);

            btnUpload.Enabled = true;
        }

        private double Fix(int fissureCount, int position, double oldValue)
        {
            return oldValue / (fissureCount - (position - 1));
        }
        
        private void btnUpload_Click(object sender, EventArgs e)
        {
            MySQL mysql = new MySQL();

            Result result_fisPer_1 = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "1",
                Key = "perimeter",
                Unit = "µm",
                Value = fisPeri_1_new.ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_1);

            Result result_fisPer_2 = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "2",
                Key = "perimeter",
                Unit = "µm",
                Value = fisPeri_2_new.ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_2);

            Result result_fisPer_3 = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "3",
                Key = "perimeter",
                Unit = "µm",
                Value = fisPeri_3_new.ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_3);

            Result result_fisPer_4 = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "4",
                Key = "perimeter",
                Unit = "µm",
                Value = fisPeri_4_new.ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_4);

            Result result_fisPer_5 = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "5",
                Key = "perimeter",
                Unit = "µm",
                Value = fisPeri_5_new.ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_5);

            Result result_fisPer_6 = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "6",
                Key = "perimeter",
                Unit = "µm",
                Value = fisPeri_6_new.ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_6);

            Result result_fisPer_7 = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "7",
                Key = "perimeter",
                Unit = "µm",
                Value = fisPeri_7_new.ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_7);

            Result result_fisPer_8 = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "8",
                Key = "perimeter",
                Unit = "µm",
                Value = fisPeri_8_new.ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_8);

            Result result_fisPer_9 = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "9",
                Key = "perimeter",
                Unit = "µm",
                Value = fisPeri_9_new.ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_9);

            Result result_fisPer_10 = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "10",
                Key = "perimeter",
                Unit = "µm",
                Value = fisPeri_10_new.ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_10);

            if (fisPeri_1_new > 0)
            {
                fissurePeriSum += fisPeri_1_new;
            }

            if (fisPeri_2_new > 0)
            {
                fissurePeriSum += fisPeri_2_new;
            }

            if (fisPeri_3_new > 0)
            {
                fissurePeriSum += fisPeri_3_new;
            }

            if (fisPeri_4_new > 0)
            {
                fissurePeriSum += fisPeri_4_new;
            }

            if (fisPeri_5_new > 0)
            {
                fissurePeriSum += fisPeri_5_new;
            }

            if (fisPeri_6_new > 0)
            {
                fissurePeriSum += fisPeri_6_new;
            }

            if (fisPeri_7_new > 0)
            {
                fissurePeriSum += fisPeri_7_new;
            }

            if (fisPeri_8_new > 0)
            {
                fissurePeriSum += fisPeri_8_new;
            }

            if (fisPeri_9_new > 0)
            {
                fissurePeriSum += fisPeri_9_new;
            }

            if (fisPeri_10_new > 0)
            {
                fissurePeriSum += fisPeri_10_new;
            }

            Result result_fisPer_avg = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "Ø",
                Key = "perimeter",
                Unit = "µm",
                Value = (fissurePeriSum / fissureCount).ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_avg);

            Result result_fisPer_sum = new Result
            {
                CutId = cut.Id,
                ResultType = "fissure",
                Identifier = "Σ",
                Key = "perimeter",
                Unit = "µm",
                Value = fissurePeriSum.ToString(FormWorker.FLOAT_ACCURACY)
            };
            mysql.CreateResultOnlyValid(result_fisPer_sum);



            if (MessageBoxAutocloseWithButtons.Show(0, true, "No", "Yes",
                    $"Show result online?") == DialogResult.Cancel)
                //if (MessageBox.Show("Show result online?", "Result", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start($"{Config.HomepageBaseUrl}/php/details.php?age={cut.Age}&genotype={cut.Genotype}&animal={cut.Animal}&cutidentifier={cut.CutIdentifier}");
            }


            Close();
        }
    }
}
