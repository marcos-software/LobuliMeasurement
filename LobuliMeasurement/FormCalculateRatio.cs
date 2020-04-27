using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LobuliMeasurement
{
    public partial class FormCalculateRatio : Form
    {
        public FormCalculateRatio()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            AddLogMessage("Get cuts");
            MySQL mysqlOuter = new MySQL();
            List<Cut> cutList = mysqlOuter.GetCut();

            AddLogMessage($"Got {cutList.Count} cuts");
            AddLogMessage($"=======================================");

            progressBar1.Maximum = cutList.Count;
            progressBar1.Step = 1;

            foreach (Cut cut in cutList)
            {
                AddLogMessage($" ", 2);
                AddLogMessage($"START OF CUT {cut.Id}", 1);

                MySQL mysql = new MySQL();
                List<Result> results = mysql.GetResults(cut.Id.ToString());

                string cerebellumPerimeterString = results.First(res => res.Key == "cerebellum perimeter")?.Value;
                double cerebellumPerimeter = -1;
                Double.TryParse(cerebellumPerimeterString, out cerebellumPerimeter);

                string cerebelluAreaString = results.First(res => res.Key == "cerebellum area")?.Value;
                double cerebellumArea = -1;
                Double.TryParse(cerebelluAreaString, out cerebellumArea);

                AddLogMessage($"cerebellum perimeter is: {cerebellumPerimeter}", 2);
                AddLogMessage($"cerebellum area is: {cerebellumArea}", 2);

                double lobPeri_1 = -1;
                double lobPeri_2 = -1;
                double lobPeri_3 = -1;
                double lobPeri_4 = -1;
                double lobPeri_5 = -1;
                double lobPeri_6 = -1;
                double lobPeri_7 = -1;
                double lobPeri_8 = -1;
                double lobPeri_9 = -1;
                double lobPeri_10 = -1;

                double lobLen_1 = -1;
                double lobLen_2 = -1;
                double lobLen_3 = -1;
                double lobLen_4 = -1;
                double lobLen_5 = -1;
                double lobLen_6 = -1;
                double lobLen_7 = -1;
                double lobLen_8 = -1;
                double lobLen_9 = -1;
                double lobLen_10 = -1;

                double fisPeri_1 = -1;
                double fisPeri_2 = -1;
                double fisPeri_3 = -1;
                double fisPeri_4 = -1;
                double fisPeri_5 = -1;
                double fisPeri_6 = -1;
                double fisPeri_7 = -1;
                double fisPeri_8 = -1;
                double fisPeri_9 = -1;
                double fisPeri_10 = -1;

                double fisLen_1 = -1;
                double fisLen_2 = -1;
                double fisLen_3 = -1;
                double fisLen_4 = -1;
                double fisLen_5 = -1;
                double fisLen_6 = -1;
                double fisLen_7 = -1;
                double fisLen_8 = -1;
                double fisLen_9 = -1;
                double fisLen_10 = -1;

                string lobPeri_1_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "1" && res.Key == "perimeter")?.Value;
                if(string.IsNullOrEmpty(lobPeri_1_string) == false) Double.TryParse(lobPeri_1_string, out lobPeri_1);

                string lobPeri_2_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "2" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(lobPeri_2_string) == false) Double.TryParse(lobPeri_2_string, out lobPeri_2);

                string lobPeri_3_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "3" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(lobPeri_3_string) == false) Double.TryParse(lobPeri_3_string, out lobPeri_3);

                string lobPeri_4_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "4" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(lobPeri_4_string) == false) Double.TryParse(lobPeri_4_string, out lobPeri_4);

                string lobPeri_5_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "5" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(lobPeri_5_string) == false) Double.TryParse(lobPeri_5_string, out lobPeri_5);

                string lobPeri_6_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "6" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(lobPeri_6_string) == false) Double.TryParse(lobPeri_6_string, out lobPeri_6);

                string lobPeri_7_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "7" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(lobPeri_7_string) == false) Double.TryParse(lobPeri_7_string, out lobPeri_7);

                string lobPeri_8_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "8" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(lobPeri_8_string) == false) Double.TryParse(lobPeri_8_string, out lobPeri_8);

                string lobPeri_9_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "9" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(lobPeri_9_string) == false) Double.TryParse(lobPeri_9_string, out lobPeri_9);

                string lobPeri_10_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "10" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(lobPeri_10_string) == false) Double.TryParse(lobPeri_10_string, out lobPeri_10);



                string lobLen_1_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "1" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(lobLen_1_string) == false) Double.TryParse(lobLen_1_string, out lobLen_1);

                string lobLen_2_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "2" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(lobLen_2_string) == false) Double.TryParse(lobLen_2_string, out lobLen_2);

                string lobLen_3_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "3" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(lobLen_3_string) == false) Double.TryParse(lobLen_3_string, out lobLen_3);

                string lobLen_4_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "4" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(lobLen_4_string) == false) Double.TryParse(lobLen_4_string, out lobLen_4);

                string lobLen_5_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "5" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(lobLen_5_string) == false) Double.TryParse(lobLen_5_string, out lobLen_5);

                string lobLen_6_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "6" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(lobLen_6_string) == false) Double.TryParse(lobLen_6_string, out lobLen_6);

                string lobLen_7_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "7" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(lobLen_7_string) == false) Double.TryParse(lobLen_7_string, out lobLen_7);

                string lobLen_8_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "8" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(lobLen_8_string) == false) Double.TryParse(lobLen_8_string, out lobLen_8);

                string lobLen_9_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "9" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(lobLen_9_string) == false) Double.TryParse(lobLen_9_string, out lobLen_9);

                string lobLen_10_string = results.FirstOrDefault(res => res.ResultType == "lobule" && res.Identifier == "10" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(lobLen_10_string) == false) Double.TryParse(lobLen_10_string, out lobLen_10);



                string fisPeri_1_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "1" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(fisPeri_1_string) == false) Double.TryParse(fisPeri_1_string, out fisPeri_1);

                string fisPeri_2_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "2" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(fisPeri_2_string) == false) Double.TryParse(fisPeri_2_string, out fisPeri_2);

                string fisPeri_3_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "3" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(fisPeri_3_string) == false) Double.TryParse(fisPeri_3_string, out fisPeri_3);

                string fisPeri_4_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "4" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(fisPeri_4_string) == false) Double.TryParse(fisPeri_4_string, out fisPeri_4);

                string fisPeri_5_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "5" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(fisPeri_5_string) == false) Double.TryParse(fisPeri_5_string, out fisPeri_5);

                string fisPeri_6_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "6" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(fisPeri_6_string) == false) Double.TryParse(fisPeri_6_string, out fisPeri_6);

                string fisPeri_7_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "7" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(fisPeri_7_string) == false) Double.TryParse(fisPeri_7_string, out fisPeri_7);

                string fisPeri_8_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "8" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(fisPeri_8_string) == false) Double.TryParse(fisPeri_8_string, out fisPeri_8);

                string fisPeri_9_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "9" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(fisPeri_9_string) == false) Double.TryParse(fisPeri_9_string, out fisPeri_9);

                string fisPeri_10_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "10" && res.Key == "perimeter")?.Value;
                if (string.IsNullOrEmpty(fisPeri_10_string) == false) Double.TryParse(fisPeri_10_string, out fisPeri_10);


                string fisLen_1_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "1" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(fisLen_1_string) == false) Double.TryParse(fisLen_1_string, out fisLen_1);

                string fisLen_2_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "2" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(fisLen_2_string) == false) Double.TryParse(fisLen_2_string, out fisLen_2);

                string fisLen_3_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "3" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(fisLen_3_string) == false) Double.TryParse(fisLen_3_string, out fisLen_3);

                string fisLen_4_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "4" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(fisLen_4_string) == false) Double.TryParse(fisLen_4_string, out fisLen_4);

                string fisLen_5_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "5" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(fisLen_5_string) == false) Double.TryParse(fisLen_5_string, out fisLen_5);

                string fisLen_6_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "6" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(fisLen_6_string) == false) Double.TryParse(fisLen_6_string, out fisLen_6);

                string fisLen_7_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "7" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(fisLen_7_string) == false) Double.TryParse(fisLen_7_string, out fisLen_7);

                string fisLen_8_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "8" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(fisLen_8_string) == false) Double.TryParse(fisLen_8_string, out fisLen_8);

                string fisLen_9_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "9" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(fisLen_9_string) == false) Double.TryParse(fisLen_9_string, out fisLen_9);

                string fisLen_10_string = results.FirstOrDefault(res => res.ResultType == "fissure" && res.Identifier == "10" && res.Key == "length")?.Value;
                if (string.IsNullOrEmpty(fisLen_10_string) == false) Double.TryParse(fisLen_10_string, out fisLen_10);

                AddLogMessage($"lobus 1 perimeter is {lobPeri_1}", 3);
                AddLogMessage($"lobus 2 perimeter is {lobPeri_2}", 3);
                AddLogMessage($"lobus 3 perimeter is {lobPeri_3}", 3);
                AddLogMessage($"lobus 4 perimeter is {lobPeri_4}", 3);
                AddLogMessage($"lobus 5 perimeter is {lobPeri_5}", 3);
                AddLogMessage($"lobus 6 perimeter is {lobPeri_6}", 3);
                AddLogMessage($"lobus 7 perimeter is {lobPeri_7}", 3);
                AddLogMessage($"lobus 8 perimeter is {lobPeri_8}", 3);
                AddLogMessage($"lobus 9 perimeter is {lobPeri_9}", 3);
                AddLogMessage($"lobus 10 perimeter is {lobPeri_10}", 3);

                AddLogMessage($"lobus 1 length is {lobLen_1}", 3);
                AddLogMessage($"lobus 2 length is {lobLen_2}", 3);
                AddLogMessage($"lobus 3 length is {lobLen_3}", 3);
                AddLogMessage($"lobus 4 length is {lobLen_4}", 3);
                AddLogMessage($"lobus 5 length is {lobLen_5}", 3);
                AddLogMessage($"lobus 6 length is {lobLen_6}", 3);
                AddLogMessage($"lobus 7 length is {lobLen_7}", 3);
                AddLogMessage($"lobus 8 length is {lobLen_8}", 3);
                AddLogMessage($"lobus 9 length is {lobLen_9}", 3);
                AddLogMessage($"lobus 10 length is {lobLen_10}", 3);

                AddLogMessage($"fissure 1 perimeter is {fisPeri_1}", 3);
                AddLogMessage($"fissure 2 perimeter is {fisPeri_2}", 3);
                AddLogMessage($"fissure 3 perimeter is {fisPeri_3}", 3);
                AddLogMessage($"fissure 4 perimeter is {fisPeri_4}", 3);
                AddLogMessage($"fissure 5 perimeter is {fisPeri_5}", 3);
                AddLogMessage($"fissure 6 perimeter is {fisPeri_6}", 3);
                AddLogMessage($"fissure 7 perimeter is {fisPeri_7}", 3);
                AddLogMessage($"fissure 8 perimeter is {fisPeri_8}", 3);
                AddLogMessage($"fissure 9 perimeter is {fisPeri_9}", 3);
                AddLogMessage($"fissure 10 perimeter is {fisPeri_10}", 3);

                AddLogMessage($"fissure 1 length is {fisLen_1}", 3);
                AddLogMessage($"fissure 2 length is {fisLen_2}", 3);
                AddLogMessage($"fissure 3 length is {fisLen_3}", 3);
                AddLogMessage($"fissure 4 length is {fisLen_4}", 3);
                AddLogMessage($"fissure 5 length is {fisLen_5}", 3);
                AddLogMessage($"fissure 6 length is {fisLen_6}", 3);
                AddLogMessage($"fissure 7 length is {fisLen_7}", 3);
                AddLogMessage($"fissure 8 length is {fisLen_8}", 3);
                AddLogMessage($"fissure 9 length is {fisLen_9}", 3);
                AddLogMessage($"fissure 10 length is {fisLen_10}", 3);

                



















































                double lobPeri_1_ratio_in_cerebellum = -1;
                double lobPeri_2_ratio_in_cerebellum = -1;
                double lobPeri_3_ratio_in_cerebellum = -1;
                double lobPeri_4_ratio_in_cerebellum = -1;
                double lobPeri_5_ratio_in_cerebellum = -1;
                double lobPeri_6_ratio_in_cerebellum = -1;
                double lobPeri_7_ratio_in_cerebellum = -1;
                double lobPeri_8_ratio_in_cerebellum = -1;
                double lobPeri_9_ratio_in_cerebellum = -1;
                double lobPeri_10_ratio_in_cerebellum = -1;

                lobPeri_1_ratio_in_cerebellum = (lobPeri_1 / cerebellumPerimeter) * 100;
                lobPeri_2_ratio_in_cerebellum = (lobPeri_2 / cerebellumPerimeter) * 100;
                lobPeri_3_ratio_in_cerebellum = (lobPeri_3 / cerebellumPerimeter) * 100;
                lobPeri_4_ratio_in_cerebellum = (lobPeri_4 / cerebellumPerimeter) * 100;
                lobPeri_5_ratio_in_cerebellum = (lobPeri_5 / cerebellumPerimeter) * 100;
                lobPeri_6_ratio_in_cerebellum = (lobPeri_6 / cerebellumPerimeter) * 100;
                lobPeri_7_ratio_in_cerebellum = (lobPeri_7 / cerebellumPerimeter) * 100;
                lobPeri_8_ratio_in_cerebellum = (lobPeri_8 / cerebellumPerimeter) * 100;
                lobPeri_9_ratio_in_cerebellum = (lobPeri_9 / cerebellumPerimeter) * 100;
                lobPeri_10_ratio_in_cerebellum = (lobPeri_10 / cerebellumPerimeter) * 100;

                AddLogMessage($"lobus 1 percentage perimeter of cerebellum perimeter is {lobPeri_1_ratio_in_cerebellum}", 3);
                AddLogMessage($"lobus 2 percentage perimeter of cerebellum perimeter is {lobPeri_2_ratio_in_cerebellum}", 3);
                AddLogMessage($"lobus 3 percentage perimeter of cerebellum perimeter is {lobPeri_3_ratio_in_cerebellum}", 3);
                AddLogMessage($"lobus 4 percentage perimeter of cerebellum perimeter is {lobPeri_4_ratio_in_cerebellum}", 3);
                AddLogMessage($"lobus 5 percentage perimeter of cerebellum perimeter is {lobPeri_5_ratio_in_cerebellum}", 3);
                AddLogMessage($"lobus 6 percentage perimeter of cerebellum perimeter is {lobPeri_6_ratio_in_cerebellum}", 3);
                AddLogMessage($"lobus 7 percentage perimeter of cerebellum perimeter is {lobPeri_7_ratio_in_cerebellum}", 3);
                AddLogMessage($"lobus 8 percentage perimeter of cerebellum perimeter is {lobPeri_8_ratio_in_cerebellum}", 3);
                AddLogMessage($"lobus 9 percentage perimeter of cerebellum perimeter is {lobPeri_9_ratio_in_cerebellum}", 3);
                AddLogMessage($"lobus 10 percentage perimeter of cerebellum perimeter is {lobPeri_10_ratio_in_cerebellum}", 3);



                double fisPeri_1_ratio_in_cerebellum = -1;
                double fisPeri_2_ratio_in_cerebellum = -1;
                double fisPeri_3_ratio_in_cerebellum = -1;
                double fisPeri_4_ratio_in_cerebellum = -1;
                double fisPeri_5_ratio_in_cerebellum = -1;
                double fisPeri_6_ratio_in_cerebellum = -1;
                double fisPeri_7_ratio_in_cerebellum = -1;
                double fisPeri_8_ratio_in_cerebellum = -1;
                double fisPeri_9_ratio_in_cerebellum = -1;
                double fisPeri_10_ratio_in_cerebellum = -1;

                fisPeri_1_ratio_in_cerebellum = (fisPeri_1 / cerebellumPerimeter) * 100;
                fisPeri_2_ratio_in_cerebellum = (fisPeri_2 / cerebellumPerimeter) * 100;
                fisPeri_3_ratio_in_cerebellum = (fisPeri_3 / cerebellumPerimeter) * 100;
                fisPeri_4_ratio_in_cerebellum = (fisPeri_4 / cerebellumPerimeter) * 100;
                fisPeri_5_ratio_in_cerebellum = (fisPeri_5 / cerebellumPerimeter) * 100;
                fisPeri_6_ratio_in_cerebellum = (fisPeri_6 / cerebellumPerimeter) * 100;
                fisPeri_7_ratio_in_cerebellum = (fisPeri_7 / cerebellumPerimeter) * 100;
                fisPeri_8_ratio_in_cerebellum = (fisPeri_8 / cerebellumPerimeter) * 100;
                fisPeri_9_ratio_in_cerebellum = (fisPeri_9 / cerebellumPerimeter) * 100;
                fisPeri_10_ratio_in_cerebellum = (fisPeri_10 / cerebellumPerimeter) * 100;

                AddLogMessage($"fissure 1 percentage perimeter of cerebellum perimeter is {fisPeri_1_ratio_in_cerebellum}", 3);
                AddLogMessage($"fissure 2 percentage perimeter of cerebellum perimeter is {fisPeri_2_ratio_in_cerebellum}", 3);
                AddLogMessage($"fissure 3 percentage perimeter of cerebellum perimeter is {fisPeri_3_ratio_in_cerebellum}", 3);
                AddLogMessage($"fissure 4 percentage perimeter of cerebellum perimeter is {fisPeri_4_ratio_in_cerebellum}", 3);
                AddLogMessage($"fissure 5 percentage perimeter of cerebellum perimeter is {fisPeri_5_ratio_in_cerebellum}", 3);
                AddLogMessage($"fissure 6 percentage perimeter of cerebellum perimeter is {fisPeri_6_ratio_in_cerebellum}", 3);
                AddLogMessage($"fissure 7 percentage perimeter of cerebellum perimeter is {fisPeri_7_ratio_in_cerebellum}", 3);
                AddLogMessage($"fissure 8 percentage perimeter of cerebellum perimeter is {fisPeri_8_ratio_in_cerebellum}", 3);
                AddLogMessage($"fissure 9 percentage perimeter of cerebellum perimeter is {fisPeri_9_ratio_in_cerebellum}", 3);
                AddLogMessage($"fissure 10 percentage perimeter of cerebellum perimeter is {fisPeri_10_ratio_in_cerebellum}", 3);




                double sumLobLen = lobLen_1 + lobLen_2 + lobLen_3 + lobLen_4 + lobLen_5 + lobLen_6 + lobLen_7 +
                                   lobLen_8 + lobLen_9 + lobLen_10;

                double lobLen_1_ratio_sumLobLen = -1;
                double lobLen_2_ratio_sumLobLen = -1;
                double lobLen_3_ratio_sumLobLen = -1;
                double lobLen_4_ratio_sumLobLen = -1;
                double lobLen_5_ratio_sumLobLen = -1;
                double lobLen_6_ratio_sumLobLen = -1;
                double lobLen_7_ratio_sumLobLen = -1;
                double lobLen_8_ratio_sumLobLen = -1;
                double lobLen_9_ratio_sumLobLen = -1;
                double lobLen_10_ratio_sumLobLen = -1;

                lobLen_1_ratio_sumLobLen = (lobLen_1 / sumLobLen) * 100;
                lobLen_2_ratio_sumLobLen = (lobLen_2 / sumLobLen) * 100;
                lobLen_3_ratio_sumLobLen = (lobLen_3 / sumLobLen) * 100;
                lobLen_4_ratio_sumLobLen = (lobLen_4 / sumLobLen) * 100;
                lobLen_5_ratio_sumLobLen = (lobLen_5 / sumLobLen) * 100;
                lobLen_6_ratio_sumLobLen = (lobLen_6 / sumLobLen) * 100;
                lobLen_7_ratio_sumLobLen = (lobLen_7 / sumLobLen) * 100;
                lobLen_8_ratio_sumLobLen = (lobLen_8 / sumLobLen) * 100;
                lobLen_9_ratio_sumLobLen = (lobLen_9 / sumLobLen) * 100;
                lobLen_10_ratio_sumLobLen = (lobLen_10 / sumLobLen) * 100;

                AddLogMessage($"lobus 1 percentage length of all lobuli length is {lobLen_1_ratio_sumLobLen}", 3);
                AddLogMessage($"lobus 2 percentage length of all lobuli length is {lobLen_2_ratio_sumLobLen}", 3);
                AddLogMessage($"lobus 3 percentage length of all lobuli length is {lobLen_3_ratio_sumLobLen}", 3);
                AddLogMessage($"lobus 4 percentage length of all lobuli length is {lobLen_4_ratio_sumLobLen}", 3);
                AddLogMessage($"lobus 5 percentage length of all lobuli length is {lobLen_5_ratio_sumLobLen}", 3);
                AddLogMessage($"lobus 6 percentage length of all lobuli length is {lobLen_6_ratio_sumLobLen}", 3);
                AddLogMessage($"lobus 7 percentage length of all lobuli length is {lobLen_7_ratio_sumLobLen}", 3);
                AddLogMessage($"lobus 8 percentage length of all lobuli length is {lobLen_8_ratio_sumLobLen}", 3);
                AddLogMessage($"lobus 9 percentage length of all lobuli length is {lobLen_9_ratio_sumLobLen}", 3);
                AddLogMessage($"lobus 10 percentage length of all lobuli length is {lobLen_10_ratio_sumLobLen}", 3);


                double sumfisLen = fisLen_1 + fisLen_2 + fisLen_3 + fisLen_4 + fisLen_5 + fisLen_6 + fisLen_7 +
                                   fisLen_8 + fisLen_9 + fisLen_10;

                double fisLen_1_ratio_sumfisLen = -1;
                double fisLen_2_ratio_sumfisLen = -1;
                double fisLen_3_ratio_sumfisLen = -1;
                double fisLen_4_ratio_sumfisLen = -1;
                double fisLen_5_ratio_sumfisLen = -1;
                double fisLen_6_ratio_sumfisLen = -1;
                double fisLen_7_ratio_sumfisLen = -1;
                double fisLen_8_ratio_sumfisLen = -1;
                double fisLen_9_ratio_sumfisLen = -1;
                double fisLen_10_ratio_sumfisLen = -1;

                fisLen_1_ratio_sumfisLen = (fisLen_1 / sumfisLen) * 100;
                fisLen_2_ratio_sumfisLen = (fisLen_2 / sumfisLen) * 100;
                fisLen_3_ratio_sumfisLen = (fisLen_3 / sumfisLen) * 100;
                fisLen_4_ratio_sumfisLen = (fisLen_4 / sumfisLen) * 100;
                fisLen_5_ratio_sumfisLen = (fisLen_5 / sumfisLen) * 100;
                fisLen_6_ratio_sumfisLen = (fisLen_6 / sumfisLen) * 100;
                fisLen_7_ratio_sumfisLen = (fisLen_7 / sumfisLen) * 100;
                fisLen_8_ratio_sumfisLen = (fisLen_8 / sumfisLen) * 100;
                fisLen_9_ratio_sumfisLen = (fisLen_9 / sumfisLen) * 100;
                fisLen_10_ratio_sumfisLen = (fisLen_10 / sumfisLen) * 100;

                AddLogMessage($"fissure 1 percentage length of all fissures length is {fisLen_1_ratio_sumfisLen}", 3);
                AddLogMessage($"fissure 2 percentage length of all fissures length is {fisLen_2_ratio_sumfisLen}", 3);
                AddLogMessage($"fissure 3 percentage length of all fissures length is {fisLen_3_ratio_sumfisLen}", 3);
                AddLogMessage($"fissure 4 percentage length of all fissures length is {fisLen_4_ratio_sumfisLen}", 3);
                AddLogMessage($"fissure 5 percentage length of all fissures length is {fisLen_5_ratio_sumfisLen}", 3);
                AddLogMessage($"fissure 6 percentage length of all fissures length is {fisLen_6_ratio_sumfisLen}", 3);
                AddLogMessage($"fissure 7 percentage length of all fissures length is {fisLen_7_ratio_sumfisLen}", 3);
                AddLogMessage($"fissure 8 percentage length of all fissures length is {fisLen_8_ratio_sumfisLen}", 3);
                AddLogMessage($"fissure 9 percentage length of all fissures length is {fisLen_9_ratio_sumfisLen}", 3);
                AddLogMessage($"fissure 10 percentage length of all fissures length is {fisLen_10_ratio_sumfisLen}", 3);




                double fisPeri_1_ratio_lobPeri_Pre = -1;
                double fisPeri_1_ratio_lobPeri_Post = -1;
                double fisPeri_1_ratio_sum_lobPeri_PrePost = -1;

                if (fisPeri_1 > 0)
                {
                    if (lobPeri_1 > 0) fisPeri_1_ratio_lobPeri_Pre = (fisPeri_1 / lobPeri_1) * 100;
                    if (lobPeri_2 > 0) fisPeri_1_ratio_lobPeri_Post = (fisPeri_1 / lobPeri_2) * 100;
                    if (lobPeri_1 > 0 && lobPeri_2 > 0) fisPeri_1_ratio_sum_lobPeri_PrePost = (fisPeri_1 / (lobPeri_1 + lobPeri_2)) * 100;
                }

                double fisPeri_2_ratio_lobPeri_Pre = -1;
                double fisPeri_2_ratio_lobPeri_Post = -1;
                double fisPeri_2_ratio_sum_lobPeri_PrePost = -1;

                if (fisPeri_2 > 0)
                {
                    if (lobPeri_2 > 0) fisPeri_2_ratio_lobPeri_Pre = (fisPeri_2 / lobPeri_2) * 100;
                    if (lobPeri_3 > 0) fisPeri_2_ratio_lobPeri_Post = (fisPeri_2 / lobPeri_3) * 100;
                    if (lobPeri_2 > 0 && lobPeri_3 > 0) fisPeri_2_ratio_sum_lobPeri_PrePost = (fisPeri_2 / (lobPeri_2 + lobPeri_3)) * 100;
                }

                double fisPeri_3_ratio_lobPeri_Pre = -1;
                double fisPeri_3_ratio_lobPeri_Post = -1;
                double fisPeri_3_ratio_sum_lobPeri_PrePost = -1;

                if (fisPeri_3 > 0)
                {
                    if (lobPeri_3 > 0) fisPeri_3_ratio_lobPeri_Pre = (fisPeri_3 / lobPeri_3) * 100;
                    if (lobPeri_4 > 0) fisPeri_3_ratio_lobPeri_Post = (fisPeri_3 / lobPeri_4) * 100;
                    if (lobPeri_3 > 0 && lobPeri_4 > 0) fisPeri_3_ratio_sum_lobPeri_PrePost = (fisPeri_3 / (lobPeri_3 + lobPeri_4)) * 100;
                }

                double fisPeri_4_ratio_lobPeri_Pre = -1;
                double fisPeri_4_ratio_lobPeri_Post = -1;
                double fisPeri_4_ratio_sum_lobPeri_PrePost = -1;

                if (fisPeri_4 > 0)
                {
                    if (lobPeri_4 > 0) fisPeri_4_ratio_lobPeri_Pre = (fisPeri_4 / lobPeri_4) * 100;
                    if (lobPeri_5 > 0) fisPeri_4_ratio_lobPeri_Post = (fisPeri_4 / lobPeri_5) * 100;
                    if (lobPeri_4 > 0 && lobPeri_5 > 0) fisPeri_4_ratio_sum_lobPeri_PrePost = (fisPeri_4 / (lobPeri_4 + lobPeri_5)) * 100;
                }

                double fisPeri_5_ratio_lobPeri_Pre = -1;
                double fisPeri_5_ratio_lobPeri_Post = -1;
                double fisPeri_5_ratio_sum_lobPeri_PrePost = -1;

                if (fisPeri_5 > 0)
                {
                    if (lobPeri_5 > 0) fisPeri_5_ratio_lobPeri_Pre = (fisPeri_5 / lobPeri_5) * 100;
                    if (lobPeri_6 > 0) fisPeri_5_ratio_lobPeri_Post = (fisPeri_5 / lobPeri_6) * 100;
                    if (lobPeri_5 > 0 && lobPeri_6 > 0) fisPeri_5_ratio_sum_lobPeri_PrePost = (fisPeri_5 / (lobPeri_5 + lobPeri_6)) * 100;
                }

                double fisPeri_6_ratio_lobPeri_Pre = -1;
                double fisPeri_6_ratio_lobPeri_Post = -1;
                double fisPeri_6_ratio_sum_lobPeri_PrePost = -1;

                if (fisPeri_6 > 0)
                {
                    if (lobPeri_6 > 0) fisPeri_6_ratio_lobPeri_Pre = (fisPeri_6 / lobPeri_6) * 100;
                    if (lobPeri_7 > 0) fisPeri_6_ratio_lobPeri_Post = (fisPeri_6 / lobPeri_7) * 100;
                    if (lobPeri_6 > 0 && lobPeri_7 > 0) fisPeri_6_ratio_sum_lobPeri_PrePost = (fisPeri_6 / (lobPeri_6 + lobPeri_7)) * 100;
                }

                double fisPeri_7_ratio_lobPeri_Pre = -1;
                double fisPeri_7_ratio_lobPeri_Post = -1;
                double fisPeri_7_ratio_sum_lobPeri_PrePost = -1;

                if (fisPeri_7 > 0)
                {
                    if (lobPeri_7 > 0) fisPeri_7_ratio_lobPeri_Pre = (fisPeri_7 / lobPeri_7) * 100;
                    if (lobPeri_8 > 0) fisPeri_7_ratio_lobPeri_Post = (fisPeri_7 / lobPeri_8) * 100;
                    if (lobPeri_7 > 0 && lobPeri_8 > 0) fisPeri_7_ratio_sum_lobPeri_PrePost = (fisPeri_7 / (lobPeri_7 + lobPeri_8)) * 100;
                }

                double fisPeri_8_ratio_lobPeri_Pre = -1;
                double fisPeri_8_ratio_lobPeri_Post = -1;
                double fisPeri_8_ratio_sum_lobPeri_PrePost = -1;

                if (fisPeri_8 > 0)
                {
                    if (lobPeri_8 > 0) fisPeri_8_ratio_lobPeri_Pre = (fisPeri_8 / lobPeri_8) * 100;
                    if (lobPeri_9 > 0) fisPeri_8_ratio_lobPeri_Post = (fisPeri_8 / lobPeri_9) * 100;
                    if (lobPeri_8 > 0 && lobPeri_9 > 0) fisPeri_8_ratio_sum_lobPeri_PrePost = (fisPeri_8 / (lobPeri_8 + lobPeri_9)) * 100;
                }

                double fisPeri_9_ratio_lobPeri_Pre = -1;
                double fisPeri_9_ratio_lobPeri_Post = -1;
                double fisPeri_9_ratio_sum_lobPeri_PrePost = -1;

                if (fisPeri_9> 0)
                {
                    if (lobPeri_9 > 0) fisPeri_9_ratio_lobPeri_Pre = (fisPeri_9 / lobPeri_9) * 100;
                    if (lobPeri_10 > 0) fisPeri_9_ratio_lobPeri_Post = (fisPeri_9 / lobPeri_10) * 100;
                    if (lobPeri_9 > 0 && lobPeri_10 > 0) fisPeri_9_ratio_sum_lobPeri_PrePost = (fisPeri_9 / (lobPeri_9 + lobPeri_10)) * 100;
                }

                double fisPeri_10_ratio_lobPeri_Pre = -1;
                double fisPeri_10_ratio_lobPeri_Post = -1;
                double fisPeri_10_ratio_sum_lobPeri_PrePost = -1;

                if (fisPeri_10 > 0)
                {
                    if (lobPeri_10 > 0) fisPeri_10_ratio_lobPeri_Pre = (fisPeri_10 / lobPeri_10) * 100;
                    //if (lobPeri_11 > 0) fisPeri_10_ratio_lobPeri_Post = (fisPeri_10 / lobPeri_11) * 100;
                    //if (lobPeri_10 > 0 && lobPeri_11 > 0) fisPeri_10_ratio_sum_lobPeri_PrePost = (fisPeri_10 / (lobPeri_10 + lobPeri_11)) * 100;
                }


                AddLogMessage($"fissure 1 percentage perimeter of lobule pre perimeter is {fisPeri_1_ratio_lobPeri_Pre}", 3);
                AddLogMessage($"fissure 1 percentage perimeter of lobule post perimeter is {fisPeri_1_ratio_lobPeri_Post}", 3);
                AddLogMessage($"fissure 1 percentage perimeter of sum lobule pre & post perimeter is {fisPeri_1_ratio_sum_lobPeri_PrePost}", 3);

                AddLogMessage($"fissure 2 percentage perimeter of lobule pre perimeter is {fisPeri_2_ratio_lobPeri_Pre}", 3);
                AddLogMessage($"fissure 2 percentage perimeter of lobule post perimeter is {fisPeri_2_ratio_lobPeri_Post}", 3);
                AddLogMessage($"fissure 2 percentage perimeter of sum lobule pre & post perimeter is {fisPeri_2_ratio_sum_lobPeri_PrePost}", 3);

                AddLogMessage($"fissure 3 percentage perimeter of lobule pre perimeter is {fisPeri_3_ratio_lobPeri_Pre}", 3);
                AddLogMessage($"fissure 3 percentage perimeter of lobule post perimeter is {fisPeri_3_ratio_lobPeri_Post}", 3);
                AddLogMessage($"fissure 3 percentage perimeter of sum lobule pre & post perimeter is {fisPeri_3_ratio_sum_lobPeri_PrePost}", 3);

                AddLogMessage($"fissure 4 percentage perimeter of lobule pre perimeter is {fisPeri_4_ratio_lobPeri_Pre}", 3);
                AddLogMessage($"fissure 4 percentage perimeter of lobule post perimeter is {fisPeri_4_ratio_lobPeri_Post}", 3);
                AddLogMessage($"fissure 4 percentage perimeter of sum lobule pre & post perimeter is {fisPeri_4_ratio_sum_lobPeri_PrePost}", 3);

                AddLogMessage($"fissure 5 percentage perimeter of lobule pre perimeter is {fisPeri_5_ratio_lobPeri_Pre}", 3);
                AddLogMessage($"fissure 5 percentage perimeter of lobule post perimeter is {fisPeri_5_ratio_lobPeri_Post}", 3);
                AddLogMessage($"fissure 5 percentage perimeter of sum lobule pre & post perimeter is {fisPeri_5_ratio_sum_lobPeri_PrePost}", 3);

                AddLogMessage($"fissure 6 percentage perimeter of lobule pre perimeter is {fisPeri_6_ratio_lobPeri_Pre}", 3);
                AddLogMessage($"fissure 6 percentage perimeter of lobule post perimeter is {fisPeri_6_ratio_lobPeri_Post}", 3);
                AddLogMessage($"fissure 6 percentage perimeter of sum lobule pre & post perimeter is {fisPeri_6_ratio_sum_lobPeri_PrePost}", 3);

                AddLogMessage($"fissure 7 percentage perimeter of lobule pre perimeter is {fisPeri_7_ratio_lobPeri_Pre}", 3);
                AddLogMessage($"fissure 7 percentage perimeter of lobule post perimeter is {fisPeri_7_ratio_lobPeri_Post}", 3);
                AddLogMessage($"fissure 7 percentage perimeter of sum lobule pre & post perimeter is {fisPeri_7_ratio_sum_lobPeri_PrePost}", 3);

                AddLogMessage($"fissure 8 percentage perimeter of lobule pre perimeter is {fisPeri_8_ratio_lobPeri_Pre}", 3);
                AddLogMessage($"fissure 8 percentage perimeter of lobule post perimeter is {fisPeri_8_ratio_lobPeri_Post}", 3);
                AddLogMessage($"fissure 8 percentage perimeter of sum lobule pre & post perimeter is {fisPeri_8_ratio_sum_lobPeri_PrePost}", 3);

                AddLogMessage($"fissure 9 percentage perimeter of lobule pre perimeter is {fisPeri_9_ratio_lobPeri_Pre}", 3);
                AddLogMessage($"fissure 9 percentage perimeter of lobule post perimeter is {fisPeri_9_ratio_lobPeri_Post}", 3);
                AddLogMessage($"fissure 9 percentage perimeter of sum lobule pre & post perimeter is {fisPeri_9_ratio_sum_lobPeri_PrePost}", 3);

                AddLogMessage($"fissure 10 percentage perimeter of lobule pre perimeter is {fisPeri_10_ratio_lobPeri_Pre}", 3);
                AddLogMessage($"fissure 10 percentage perimeter of lobule post perimeter is {fisPeri_10_ratio_lobPeri_Post}", 3);
                AddLogMessage($"fissure 10 percentage perimeter of sum lobule pre & post perimeter is {fisPeri_10_ratio_sum_lobPeri_PrePost}", 3);






























































                Result result_lobPeri_1_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "1",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = lobPeri_1_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobPeri_1_ratio_in_cerebellum);

                Result result_lobPeri_2_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "2",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = lobPeri_2_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobPeri_2_ratio_in_cerebellum);

                Result result_lobPeri_3_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "3",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = lobPeri_3_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobPeri_3_ratio_in_cerebellum);

                Result result_lobPeri_4_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "4",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = lobPeri_4_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobPeri_4_ratio_in_cerebellum);

                Result result_lobPeri_5_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "5",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = lobPeri_5_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobPeri_5_ratio_in_cerebellum);

                Result result_lobPeri_6_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "6",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = lobPeri_6_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobPeri_6_ratio_in_cerebellum);

                Result result_lobPeri_7_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "7",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = lobPeri_7_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobPeri_7_ratio_in_cerebellum);

                Result result_lobPeri_8_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "8",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = lobPeri_8_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobPeri_8_ratio_in_cerebellum);

                Result result_lobPeri_9_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "9",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = lobPeri_9_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobPeri_9_ratio_in_cerebellum);

                Result result_lobPeri_10_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "10",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = lobPeri_10_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobPeri_10_ratio_in_cerebellum);













                Result result_fisPeri_1_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "1",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = fisPeri_1_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_1_ratio_in_cerebellum);

                Result result_fisPeri_2_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "2",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = fisPeri_2_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_2_ratio_in_cerebellum);

                Result result_fisPeri_3_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "3",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = fisPeri_3_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_3_ratio_in_cerebellum);

                Result result_fisPeri_4_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "4",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = fisPeri_4_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_4_ratio_in_cerebellum);

                Result result_fisPeri_5_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "5",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = fisPeri_5_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_5_ratio_in_cerebellum);

                Result result_fisPeri_6_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "6",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = fisPeri_6_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_6_ratio_in_cerebellum);

                Result result_fisPeri_7_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "7",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = fisPeri_7_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_7_ratio_in_cerebellum);

                Result result_fisPeri_8_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "8",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = fisPeri_8_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_8_ratio_in_cerebellum);

                Result result_fisPeri_9_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "9",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = fisPeri_9_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_9_ratio_in_cerebellum);

                Result result_fisPeri_10_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "10",
                    Key = "percentage perimeter of cerebellum perimeter",
                    Unit = "%",
                    Value = fisPeri_10_ratio_in_cerebellum.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_10_ratio_in_cerebellum);















                
                Result result_lobLen_1_ratio_sumLobLen = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "1",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = lobLen_1_ratio_sumLobLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobLen_1_ratio_sumLobLen);

                Result result_lobLen_2_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "2",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = lobLen_2_ratio_sumLobLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobLen_2_ratio_in_cerebellum);

                Result result_lobLen_3_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "3",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = lobLen_3_ratio_sumLobLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobLen_3_ratio_in_cerebellum);

                Result result_lobLen_4_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "4",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = lobLen_4_ratio_sumLobLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobLen_4_ratio_in_cerebellum);

                Result result_lobLen_5_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "5",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = lobLen_5_ratio_sumLobLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobLen_5_ratio_in_cerebellum);

                Result result_lobLen_6_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "6",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = lobLen_6_ratio_sumLobLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobLen_6_ratio_in_cerebellum);

                Result result_lobLen_7_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "7",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = lobLen_7_ratio_sumLobLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobLen_7_ratio_in_cerebellum);

                Result result_lobLen_8_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "8",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = lobLen_8_ratio_sumLobLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobLen_8_ratio_in_cerebellum);

                Result result_lobLen_9_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "9",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = lobLen_9_ratio_sumLobLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobLen_9_ratio_in_cerebellum);

                Result result_lobLen_10_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = "10",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = lobLen_10_ratio_sumLobLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_lobLen_10_ratio_in_cerebellum);













                Result result_fisLen_1_ratio_sumfisLen = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "1",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = fisLen_1_ratio_sumfisLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisLen_1_ratio_sumfisLen);

                Result result_fisLen_2_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "2",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = fisLen_2_ratio_sumfisLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisLen_2_ratio_in_cerebellum);

                Result result_fisLen_3_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "3",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = fisLen_3_ratio_sumfisLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisLen_3_ratio_in_cerebellum);

                Result result_fisLen_4_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "4",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = fisLen_4_ratio_sumfisLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisLen_4_ratio_in_cerebellum);

                Result result_fisLen_5_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "5",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = fisLen_5_ratio_sumfisLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisLen_5_ratio_in_cerebellum);

                Result result_fisLen_6_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "6",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = fisLen_6_ratio_sumfisLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisLen_6_ratio_in_cerebellum);

                Result result_fisLen_7_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "7",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = fisLen_7_ratio_sumfisLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisLen_7_ratio_in_cerebellum);

                Result result_fisLen_8_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "8",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = fisLen_8_ratio_sumfisLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisLen_8_ratio_in_cerebellum);

                Result result_fisLen_9_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "9",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = fisLen_9_ratio_sumfisLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisLen_9_ratio_in_cerebellum);

                Result result_fisLen_10_ratio_in_cerebellum = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "10",
                    Key = "percentage length of sum length in cerebellum",
                    Unit = "%",
                    Value = fisLen_10_ratio_sumfisLen.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisLen_10_ratio_in_cerebellum);











                Result result_fisPeri_1_ratio_lobPeri_Pre = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "1",
                    Key = "percentage perimeter of pre lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_1_ratio_lobPeri_Pre.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_1_ratio_lobPeri_Pre);

                Result result_fisPeri_1_ratio_lobPeri_Post = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "1",
                    Key = "percentage perimeter of post lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_1_ratio_lobPeri_Post.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_1_ratio_lobPeri_Post);

                Result result_fisPeri_1_ratio_sum_lobPeri_PrePost = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "1",
                    Key = "percentage perimeter of sum pre and post lobules perimeter",
                    Unit = "%",
                    Value = fisPeri_1_ratio_sum_lobPeri_PrePost.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_1_ratio_sum_lobPeri_PrePost);



                Result result_fisPeri_2_ratio_lobPeri_Pre = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "2",
                    Key = "percentage perimeter of pre lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_2_ratio_lobPeri_Pre.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_2_ratio_lobPeri_Pre);

                Result result_fisPeri_2_ratio_lobPeri_Post = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "2",
                    Key = "percentage perimeter of post lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_2_ratio_lobPeri_Post.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_2_ratio_lobPeri_Post);

                Result result_fisPeri_2_ratio_sum_lobPeri_PrePost = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "2",
                    Key = "percentage perimeter of sum pre and post lobules perimeter",
                    Unit = "%",
                    Value = fisPeri_2_ratio_sum_lobPeri_PrePost.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_2_ratio_sum_lobPeri_PrePost);



                Result result_fisPeri_3_ratio_lobPeri_Pre = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "3",
                    Key = "percentage perimeter of pre lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_3_ratio_lobPeri_Pre.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_3_ratio_lobPeri_Pre);

                Result result_fisPeri_3_ratio_lobPeri_Post = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "3",
                    Key = "percentage perimeter of post lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_3_ratio_lobPeri_Post.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_3_ratio_lobPeri_Post);

                Result result_fisPeri_3_ratio_sum_lobPeri_PrePost = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "3",
                    Key = "percentage perimeter of sum pre and post lobules perimeter",
                    Unit = "%",
                    Value = fisPeri_3_ratio_sum_lobPeri_PrePost.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_3_ratio_sum_lobPeri_PrePost);



                Result result_fisPeri_4_ratio_lobPeri_Pre = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "4",
                    Key = "percentage perimeter of pre lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_4_ratio_lobPeri_Pre.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_4_ratio_lobPeri_Pre);

                Result result_fisPeri_4_ratio_lobPeri_Post = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "4",
                    Key = "percentage perimeter of post lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_4_ratio_lobPeri_Post.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_4_ratio_lobPeri_Post);

                Result result_fisPeri_4_ratio_sum_lobPeri_PrePost = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "4",
                    Key = "percentage perimeter of sum pre and post lobules perimeter",
                    Unit = "%",
                    Value = fisPeri_4_ratio_sum_lobPeri_PrePost.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_4_ratio_sum_lobPeri_PrePost);



                Result result_fisPeri_5_ratio_lobPeri_Pre = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "5",
                    Key = "percentage perimeter of pre lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_5_ratio_lobPeri_Pre.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_5_ratio_lobPeri_Pre);

                Result result_fisPeri_5_ratio_lobPeri_Post = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "5",
                    Key = "percentage perimeter of post lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_5_ratio_lobPeri_Post.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_5_ratio_lobPeri_Post);

                Result result_fisPeri_5_ratio_sum_lobPeri_PrePost = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "5",
                    Key = "percentage perimeter of sum pre and post lobules perimeter",
                    Unit = "%",
                    Value = fisPeri_5_ratio_sum_lobPeri_PrePost.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_5_ratio_sum_lobPeri_PrePost);



                Result result_fisPeri_6_ratio_lobPeri_Pre = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "6",
                    Key = "percentage perimeter of pre lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_6_ratio_lobPeri_Pre.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_6_ratio_lobPeri_Pre);

                Result result_fisPeri_6_ratio_lobPeri_Post = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "6",
                    Key = "percentage perimeter of post lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_6_ratio_lobPeri_Post.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_6_ratio_lobPeri_Post);

                Result result_fisPeri_6_ratio_sum_lobPeri_PrePost = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "6",
                    Key = "percentage perimeter of sum pre and post lobules perimeter",
                    Unit = "%",
                    Value = fisPeri_6_ratio_sum_lobPeri_PrePost.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_6_ratio_sum_lobPeri_PrePost);



                Result result_fisPeri_7_ratio_lobPeri_Pre = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "7",
                    Key = "percentage perimeter of pre lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_7_ratio_lobPeri_Pre.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_7_ratio_lobPeri_Pre);

                Result result_fisPeri_7_ratio_lobPeri_Post = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "7",
                    Key = "percentage perimeter of post lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_7_ratio_lobPeri_Post.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_7_ratio_lobPeri_Post);

                Result result_fisPeri_7_ratio_sum_lobPeri_PrePost = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "7",
                    Key = "percentage perimeter of sum pre and post lobules perimeter",
                    Unit = "%",
                    Value = fisPeri_7_ratio_sum_lobPeri_PrePost.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_7_ratio_sum_lobPeri_PrePost);



                Result result_fisPeri_8_ratio_lobPeri_Pre = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "8",
                    Key = "percentage perimeter of pre lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_8_ratio_lobPeri_Pre.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_8_ratio_lobPeri_Pre);

                Result result_fisPeri_8_ratio_lobPeri_Post = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "8",
                    Key = "percentage perimeter of post lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_8_ratio_lobPeri_Post.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_8_ratio_lobPeri_Post);

                Result result_fisPeri_8_ratio_sum_lobPeri_PrePost = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "8",
                    Key = "percentage perimeter of sum pre and post lobules perimeter",
                    Unit = "%",
                    Value = fisPeri_8_ratio_sum_lobPeri_PrePost.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_8_ratio_sum_lobPeri_PrePost);



                Result result_fisPeri_9_ratio_lobPeri_Pre = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "9",
                    Key = "percentage perimeter of pre lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_9_ratio_lobPeri_Pre.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_9_ratio_lobPeri_Pre);

                Result result_fisPeri_9_ratio_lobPeri_Post = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "9",
                    Key = "percentage perimeter of post lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_9_ratio_lobPeri_Post.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_9_ratio_lobPeri_Post);

                Result result_fisPeri_9_ratio_sum_lobPeri_PrePost = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "9",
                    Key = "percentage perimeter of sum pre and post lobules perimeter",
                    Unit = "%",
                    Value = fisPeri_9_ratio_sum_lobPeri_PrePost.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_9_ratio_sum_lobPeri_PrePost);



                Result result_fisPeri_10_ratio_lobPeri_Pre = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "10",
                    Key = "percentage perimeter of pre lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_10_ratio_lobPeri_Pre.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_10_ratio_lobPeri_Pre);

                Result result_fisPeri_10_ratio_lobPeri_Post = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "10",
                    Key = "percentage perimeter of post lobule perimeter",
                    Unit = "%",
                    Value = fisPeri_10_ratio_lobPeri_Post.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_10_ratio_lobPeri_Post);

                Result result_fisPeri_10_ratio_sum_lobPeri_PrePost = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = "10",
                    Key = "percentage perimeter of sum pre and post lobules perimeter",
                    Unit = "%",
                    Value = fisPeri_10_ratio_sum_lobPeri_PrePost.ToString(FormWorker.FLOAT_ACCURACY)
                };
                mysql.CreateResultOnlyValid(result_fisPeri_10_ratio_sum_lobPeri_PrePost);






                progressBar1.PerformStep();
                AddLogMessage($"END OF CUT {cut.Id}", 1);
            }
        }

        private void AddLogMessage(string message, int tabAmout = 0)
        {
            for (int i = 0; i < tabAmout; i++)
            {
                message = "    " + message;
            }

            richTextBox1.AppendText(message + Environment.NewLine);
            richTextBox1.ScrollToCaret();
        }
    }
}
