using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using OfficeOpenXml;
using Point = System.Drawing.Point;

namespace LobuliMeasurement
{
    public partial class FormWorker : Form
    {
        private readonly WorkerMethod _workerMethod;
        public string _filePath;
        private List<string> _filePathList = new List<string>();
        private int _fileCounter = 0;
        private Bitmap _bitmap;
        public List<Point> _coordinates = new List<Point>();
        private Point _centroid;
        private List<Fisur> _fisurList = new List<Fisur>();
        private List<Fisur> _lobuliList = new List<Fisur>();
        public string _note;
        public string _layer;

        public int _waitTime = 5;

        private bool _collectDatapoints;

        private double _cerebellumArea;
        private double _cerebellumPerimet;
        private double _minCircleArea;
        private double _minCirclePerimet;
        private double _minCircleRadius;
        private double _convexHullArea;
        private double _convexHullPerimet;
        private double _solidity;
        private double _circularity;
        private double _convexity;
        private double _deltaSM;
        private double _markArea;
        private double _markPerimeter;
        private double _markMinCircleRadius;
        private double _markMinCircleArea;
        private double _markMinCirclePerimet;
        private double _markConvexHullArea;
        private double _markConvexHullPerimet;
        private double _markConvexity;
        private double _markSolidity;
        private double _markCircularity;
        private double _markDistanceToCerebellumCenter;

        private List<Point> _markConexHullPoints = new List<Point>();

        private double _percentMarkAreaOfCerebellumgArea;
        private Point _center;

        private int _countFisur;
        private int _countExistentFisur;
        private double _sumFisurLength;
        private double _sumFisurPerimeter;
        private double _sumFisurArea;
        private double _sumFisurStraigtness;
        private double _averageFisurLength;
        private double _averageFisurPerimeter;
        private double _averageFisurArea;
        private double _averageFisurStraigtness;

        private int _countLobulus;
        private int _countExistentLobulus;
        private double _sumLobulusLength;
        private double _sumLobulusPerimeter;
        private double _sumLobulusArea;
        private double _sumLobulusStraigtness;
        private double _sumLobulusPercentageOfCerebellumArea;
        private double _averageLobulusLength;
        private double _averageLobulusPerimeter;
        private double _averageLobulusArea;
        private double _averageLobulusStraigtness;
        private double _averageLobulusPercentageOfCerebellumArea;

        private string _age;
        private string _animalNumber;
        private string _genotype;
        private string _method;
        private string _zoomFactor;
        private string _cutIdentifier;
        private DateTime _dateStaining;

        private Bitmap _originalPicture;
        private Bitmap _markedPicture;
        public Bitmap _originalPictureOVERWRITE;
        public Bitmap _markedPictureOVERWRITE;
        private Bitmap _analyzedPicture;

        private static readonly List<string> IMAGE_EXTENSIONS = new List<string>
        {
            ".png",
            ".jpg",
            ".jpeg",
            ".tif",
            ".tiff",
            ".bmp"
        };

        public const int fallbackOffset = 20; //use 28 only for P3, all others have to use 20!
        public const int fallbackBlurredFactor = 5;
        public const int fallbackIntersectionPointRestrictedRadius = 50;

        public int DEFAULT_OFFSET = fallbackOffset;
        public int DEFAULT_BLURRED_FACTOR = fallbackBlurredFactor;
        public int DEFAULT_INTERSECTION_POINT_RESTRICTED_RADIUS = fallbackIntersectionPointRestrictedRadius;

        private const bool DEFAULT_ALIGN = true;
        private const string UNIT = "µm";
        private static string UNIT_AREA = $"{UNIT}²";
        private const string CSV_DELIMITER = "\t";
        public const string FLOAT_ACCURACY = "F2";

        public int _offsetOuterLine = fallbackOffset;
        public int _blurredFactor = fallbackBlurredFactor;
        public int _intersectionPointRestrictedRadius = fallbackIntersectionPointRestrictedRadius;
        private bool _align = DEFAULT_ALIGN;
        private double _conversionFactor = ((double)100 / 76);
        private double _imageResizeFactor = 1;

        private int cropYStart = 0;
        private int cropYEnd = 758;
        private int cropXStart = 0;
        private int cropXEnd = 1024;

        private double _completeFactor => _conversionFactor * _imageResizeFactor;

        private Circle _circle;
        private Form _parentForm;

        public FormWorker(WorkerMethod workerMethod, Form parentForm)
        {
            _workerMethod = workerMethod;
            _parentForm = parentForm;

            InitializeComponent();
            Text = $"Lobuli Measurement - Method: {_workerMethod}";
            lblConversion100.Text = $"100 {UNIT} =";

            FillOptionsWithDefaultValues();
        }

        public void FillOptionsWithDefaultValues()
        {
            cheAutoAlign.CheckedChanged -= cheAutoAlign_CheckedChanged;
            nudOffset.ValueChanged -= nud_ValueChanged;
            nudBlurredFactor.ValueChanged -= nud_ValueChanged;
            nudIntersectionResArea.ValueChanged -= nud_ValueChanged;


            nudOffset.Value = DEFAULT_OFFSET;
            nudBlurredFactor.Value = DEFAULT_BLURRED_FACTOR;
            nudIntersectionResArea.Value = DEFAULT_INTERSECTION_POINT_RESTRICTED_RADIUS;
            
            cheAutoAlign.Checked = DEFAULT_ALIGN;


            cheAutoAlign.CheckedChanged += cheAutoAlign_CheckedChanged;
            nudOffset.ValueChanged += nud_ValueChanged;
            nudBlurredFactor.ValueChanged += nud_ValueChanged;
            nudIntersectionResArea.ValueChanged += nud_ValueChanged;
        }

        #region Get File or Directory

        private void GetFilePath()
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = $"Image Files|{IMAGE_EXTENSIONS.Aggregate((i, j) => $"*{i};*{j}")}";
                dialog.Title = "Please select an image file";
                dialog.CheckFileExists = true;
                dialog.Multiselect = false;

                if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    if (File.Exists(dialog.FileName))
                    {
                        _filePath = dialog.FileName;
                        Text += $" ({Path.GetFileNameWithoutExtension(_filePath)})";
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(_filePath))
            {

                DialogResult result = MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Yes", "No",
                    "No file selected, do you want to continue?");
                    //MessageBox.Show(
                    //"No file selected, do you want to continue?",
                    //"",
                    //MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.OK)
                {
                    GetFilePath();
                }
                else
                {
                    Close();
                }
            }
            else
            {
                InitializeFormDetails();
            }
        }

        private void GetDirectoryPath()
        {
            string directoryPath = null;
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.ShowNewFolderButton = false;
                dialog.Description = "Plese select the directory with image files";

                if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    if (Directory.Exists(dialog.SelectedPath))
                    {
                        directoryPath = dialog.SelectedPath;
                    }
                }
            }

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                DialogResult result = MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Yes", "No", "No directory selected, do you want to continue?");
                    //MessageBox.Show(
                    //"No directory selected, do you want to continue?",
                    //"",
                    //MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.OK)
                {
                    GetDirectoryPath();
                }
                else
                {
                    Close();
                }
            }
            else
            {
                IEnumerable<string> files = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(file => file.EndsWith(IMAGE_EXTENSIONS));

                if (!files.Any())
                {
                    DialogResult result = MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Yes", "No",
                        "No image files found in selected directory, do you want to choose another directory?"); 
                        //MessageBox.Show(
                        //"No image files found in selected directory, do you want to choose another directory?",
                        //"",
                        //MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.OK)
                    {
                        GetDirectoryPath();
                    }
                    else
                    {
                        Close();
                    }
                }
                else
                {
                    _filePathList = files.ToList();
                    InitializeFormDetails();
                }
            }
        }

        #endregion Get File or Directory

        private void InitializeFormDetails()
        {
            if (_workerMethod != WorkerMethod.MultiDatapointsAndMeasurement)
            {
                LoadImage(_filePath);
                //ProcessFile(_filePath);
            }
            else //WorkerMethod.MultiDatapointsAndMeasurement
            {
                NextFileFromDirectory();
            }
        }

        private void NextFileFromDirectory()
        {
            int newIndex = _fileCounter++;
            if (_filePathList.Count > newIndex)
            {
                string filePath = _filePathList[newIndex];
                if (!string.IsNullOrEmpty(filePath))
                {
                    LoadImage(filePath);
                    //ProcessFile(filePath);
                }
                else //empty filePath should not be possible!
                {
                    NextFileFromDirectory();
                }
            }
            else
            {
                DialogResult result = MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Close", "Cancel",
                    "All files processed, closing now!");
                if (result == DialogResult.OK)
                {
                    Close();
                }
                //MessageBox.Show("All files processed, closing now!");
                //Close();
            }
        }

        public void LoadImage(string filePath, bool collectInfo = true)
        {
            AddLogMessage($"Load image from {filePath}");
            Image image = Image.FromFile(filePath);
            
            if (image.Height > image.Width)
            {
                AddLogMessage($"Rotate image 90°");
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }

            _imageResizeFactor = (double)image.Height / pictureBox.Height;
            AddLogMessage($"Image Resize Factor is {_imageResizeFactor}");

            _bitmap = new Bitmap(image, pictureBox.Width, pictureBox.Height);
            

            pictureBox.Image = _bitmap;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Refresh();

            flipped = false;
            
            _originalPicture = new Bitmap(image);

            if (collectInfo)
            {
                FormCollectInfo info = new FormCollectInfo(filePath, _waitTime, _layer);

                info.ShowDialog();

                _age = info.Age;
                _genotype = info.Genotype;
                _animalNumber = info.AnimalNumber;
                _method = info.Method;
                _zoomFactor = info.ZoomFactor;
                _cutIdentifier = info.CutIdentifier;
                _layer = info.Layer;
                _dateStaining = info.DateStaining;

                lblAge.Text = _age;
                lblGenotype.Text = _genotype;
                lblAnimalNumber.Text = _animalNumber;
                lblMethod.Text = _method;
                lblZoomFactor.Text = _zoomFactor;
                lblCutIdentifier.Text = _cutIdentifier;
                lblLayer.Text = _layer;
            }

            //manche fotos sind falsch benannt, daher müsste (um die falsche Benennung auszugleichen) _zoomFactor=2 auch und nudConversionPixel.Value = 49; ergeben.
            //Damit aber nicht alle Ergebnisse nun anders sind beim erneuten Durchlauf, korrigiere ich das hier noch nicht.
            //Müsste man aber für ein neues Projekt machen (oder die Fotos direkt richtig benennen).

            //daher und nur für das projekt hier, umgehen ich die Analyse des ZoomFactors an der Stelle
            //das muss für andere projekte wiede rückgängig gemacht werden!!

            return;

            if (_zoomFactor == "2,5" || _zoomFactor == "2.5" || _zoomFactor == "2,5x" || _zoomFactor == "2.5x")
            {
                nudConversionPixel.Value = 49;
            }

            if (_zoomFactor == "5,0" || _zoomFactor == "5.0" || _zoomFactor == "5,0x" || _zoomFactor == "5.0x" || _zoomFactor == "5" || _zoomFactor == "5x")
            {
                nudConversionPixel.Value = 92;
            }

            if (_zoomFactor == "10,0" || _zoomFactor == "10.0" || _zoomFactor == "10,0x" || _zoomFactor == "10.0x" || _zoomFactor == "10" || _zoomFactor == "10x")
            {
                nudConversionPixel.Value = 184;
            }

            if (_zoomFactor == "20,0" || _zoomFactor == "20.0" || _zoomFactor == "20,0x" || _zoomFactor == "20.0x" || _zoomFactor == "20" || _zoomFactor == "20x")
            {
                nudConversionPixel.Value = 369;
            }

            if (_zoomFactor == "40,0" || _zoomFactor == "40.0" || _zoomFactor == "40,0x" || _zoomFactor == "40.0x" || _zoomFactor == "40" || _zoomFactor == "40x")
            {
                nudConversionPixel.Value = 745;
            }

            if (_zoomFactor == "63,0" || _zoomFactor == "63.0" || _zoomFactor == "63,0x" || _zoomFactor == "63.0x" || _zoomFactor == "63" || _zoomFactor == "63x")
            {
                nudConversionPixel.Value = 1170;
            }

            if (_zoomFactor == "100,0" || _zoomFactor == "100.0" || _zoomFactor == "100,0x" || _zoomFactor == "100.0x" || _zoomFactor == "100" || _zoomFactor == "100x")
            {
                nudConversionPixel.Value = 1900;
            }

            if (_coordinates != null && _coordinates.Count > 0)
            {

            }
        }
        
        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            Draw(e.X, e.Y);
        }

        private void CutAndSpin(ref List<Point> coordinates, ref Bitmap bitmap, ref PictureBox pictureBoxToUse)
        {
            Cut(ref coordinates, ref bitmap, ref pictureBoxToUse);
            Spin(ref coordinates, ref bitmap, ref pictureBoxToUse);
        }
        
        private List<PointF> GetEnlargedPolygon(
            List<PointF> old_points, float offset)
        {
            List<PointF> enlarged_points = new List<PointF>();
            int num_points = old_points.Count;
            for (int j = 0; j < num_points; j++)
            {
                // Find the new location for point j.
                // Find the points before and after j.
                int i = (j - 1);
                if (i < 0) i += num_points;
                int k = (j + 1) % num_points;

                // Move the points by the offset.
                Vector v1 = new Vector(
                    old_points[j].X - old_points[i].X,
                    old_points[j].Y - old_points[i].Y);
                v1.Normalize();
                v1 *= offset;
                Vector n1 = new Vector(-v1.Y, v1.X);

                PointF pij1 = new PointF(
                    (float) (old_points[i].X + n1.X),
                    (float) (old_points[i].Y + n1.Y));
                PointF pij2 = new PointF(
                    (float) (old_points[j].X + n1.X),
                    (float) (old_points[j].Y + n1.Y));

                Vector v2 = new Vector(
                    old_points[k].X - old_points[j].X,
                    old_points[k].Y - old_points[j].Y);
                v2.Normalize();
                v2 *= offset;
                Vector n2 = new Vector(-v2.Y, v2.X);

                PointF pjk1 = new PointF(
                    (float) (old_points[j].X + n2.X),
                    (float) (old_points[j].Y + n2.Y));
                PointF pjk2 = new PointF(
                    (float) (old_points[k].X + n2.X),
                    (float) (old_points[k].Y + n2.Y));

                // See where the shifted lines ij and jk intersect.
                bool lines_intersect, segments_intersect;
                PointF poi, close1, close2;
                FindIntersection(pij1, pij2, pjk1, pjk2,
                    out lines_intersect, out segments_intersect,
                    out poi, out close1, out close2);

                if (!float.IsNaN(poi.X) && !float.IsNaN(poi.Y))
                {
                    if (PointInPolygon(old_points, poi.X, poi.Y) == false)
                    {
                        enlarged_points.Add(poi);
                    }
                }
            }

            return enlarged_points;
        }

        private static void FindIntersection(
            PointF p1, PointF p2, PointF p3, PointF p4,
            out bool lines_intersect, out bool segments_intersect,
            out PointF intersection,
            out PointF close_p1, out PointF close_p2)
        {
            // Get the segments' parameters.
            float dx12 = p2.X - p1.X;
            float dy12 = p2.Y - p1.Y;
            float dx34 = p4.X - p3.X;
            float dy34 = p4.Y - p3.Y;

            // Solve for t1 and t2
            float denominator = (dy12 * dx34 - dx12 * dy34);

            float t1 =
                ((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
                / denominator;
            if (float.IsInfinity(t1))
            {
                // The lines are parallel (or close enough to it).
                lines_intersect = false;
                segments_intersect = false;
                intersection = new PointF(float.NaN, float.NaN);
                close_p1 = new PointF(float.NaN, float.NaN);
                close_p2 = new PointF(float.NaN, float.NaN);
                return;
            }

            lines_intersect = true;

            float t2 =
                ((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
                / -denominator;

            // Find the point of intersection.
            intersection = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);

            // The segments intersect if t1 and t2 are between 0 and 1.
            segments_intersect =
                ((t1 >= 0) && (t1 <= 1) &&
                 (t2 >= 0) && (t2 <= 1));

            // Find the closest points on the segments.
            if (t1 < 0)
            {
                t1 = 0;
            }
            else if (t1 > 1)
            {
                t1 = 1;
            }

            if (t2 < 0)
            {
                t2 = 0;
            }
            else if (t2 > 1)
            {
                t2 = 1;
            }

            close_p1 = new PointF(p1.X + dx12 * t1, p1.Y + dy12 * t1);
            close_p2 = new PointF(p3.X + dx34 * t2, p3.Y + dy34 * t2);
        }

        public bool PointInPolygon(List<PointF> points, float X, float Y)
        {
            // Get the angle between the point and the
            // first and last vertices.
            int max_point = points.Count - 1;
            float total_angle = GetAngle(
                points[max_point].X, points[max_point].Y,
                X, Y,
                points[0].X, points[0].Y);

            // Add the angles from the point
            // to each other pair of vertices.
            for (int i = 0; i < max_point; i++)
            {
                total_angle += GetAngle(
                    points[i].X, points[i].Y,
                    X, Y,
                    points[i + 1].X, points[i + 1].Y);
            }

            // The total angle should be 2 * PI or -2 * PI if
            // the point is in the polygon and close to zero
            // if the point is outside the polygon.
            return (Math.Abs(total_angle) > 0.000001);
        }

        public static float GetAngle(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the dot product.
            float dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

            // Get the cross product.
            float cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

            // Calculate the angle.
            return (float) Math.Atan2(cross_product, dot_product);
        }

        private static float DotProduct(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the dot product.
            return (BAx * BCx + BAy * BCy);
        }

        public static float CrossProductLength(float Ax, float Ay,
            float Bx, float By, float Cx, float Cy)
        {
            // Get the vectors' coordinates.
            float BAx = Ax - Bx;
            float BAy = Ay - By;
            float BCx = Cx - Bx;
            float BCy = Cy - By;

            // Calculate the Z coordinate of the cross product.
            return (BAx * BCy - BAy * BCx);
        }

        private void Cut(ref List<Point> coordinates, ref Bitmap bitmap, ref PictureBox pictureBoxToUse)
        {
            AddLogMessage($"Generate enlarged polygon");
            var enlargedPolygon = GetEnlargedPolygon(coordinates.Select(p => new PointF(p.X, p.Y)).ToList(), -10);

            if (enlargedPolygon.Count < 3)
            {
                coordinates.Reverse();
                enlargedPolygon = GetEnlargedPolygon(coordinates.Select(p => new PointF(p.X, p.Y)).ToList(), -10);
            }

            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(enlargedPolygon.ToArray());

            AddLogMessage($"Cut out enlarged polygon");
            Bitmap bmp = new Bitmap(bitmap.Width, bitmap.Height);

            using (Graphics G = Graphics.FromImage(bmp))
            {
                G.Clip = new Region(gp);
                G.DrawImage(bitmap, 0, 0);
                pictureBoxToUse.Image = bmp;
                pictureBoxToUse.Refresh();
            }

            bitmap = new Bitmap(pictureBoxToUse.Image);
        }

        private void Spin(ref List<Point> coordinates, ref Bitmap bitmap, ref PictureBox pictureBoxToUse)
        {
            if (_align == false) return;
            Point startPoint = coordinates.FirstOrDefault();
            Point stopPoint = coordinates.LastOrDefault();

            if (startPoint != null && stopPoint != null)
            {
                float xDiff = stopPoint.X - startPoint.X;
                float yDiff = stopPoint.Y - startPoint.Y;
                double angel = (Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI) * -1;

                AddLogMessage($"Spin by {angel}°");

                Point pivot = new Point(bitmap.Width / 2, bitmap.Height / 2);

                bitmap = Utils.RotateImage(bitmap, pivot, (float) angel);

                pictureBoxToUse.Image = bitmap;
                pictureBoxToUse.Refresh();

                List<Point> newCoordinateList = new List<Point>();

                foreach (Point coordinate in coordinates)
                {
                    Point newPoint = Utils.RotatePoint(coordinate, pivot, (float) angel);
                    newCoordinateList.Add(newPoint);
                }

                coordinates = newCoordinateList;
            }
        }


        private void MeasureStart(List<Line> outerLines, List<Line> lines)
        {
            AddLogMessage("Start measuring");
            _bitmap = new Bitmap(pictureBox.Image.Width, pictureBox.Image.Height);
            pictureBox.Image = _bitmap;
            pictureBox.Refresh();

            DoMinimumCircle();
            DoConvexHull();

            _cerebellumArea = Geometry.PolygonArea(_coordinates) * _completeFactor;
            AddLogMessage($"cerebellum area: {_cerebellumArea.ToString(FLOAT_ACCURACY)} {UNIT}²");
            lblCerArea.Text = $"cerebellum area: {_cerebellumArea.ToString(FLOAT_ACCURACY)} {UNIT}²";

            _deltaSM = Line.GetDistanceBetweenTwoPoints(_centroid, new Point((int)_circle.c.x, (int)_circle.c.y)) *
                       _completeFactor;
            AddLogMessage($"ΔSM: {_deltaSM.ToString(FLOAT_ACCURACY)} {UNIT}");
            lblDeltaSM.Text = $"ΔSM: {_deltaSM.ToString(FLOAT_ACCURACY)} {UNIT}";

            AddLogMessage("Draw cerebellum area");
            using (Graphics g = Graphics.FromImage(_bitmap))
            {
                if (_coordinates.Count >= 3)
                {
                    g.FillPolygon(new SolidBrush(Color.LightBlue), _coordinates.ToArray());
                }

                if (cheDeltaSM.Checked)
                {
                    if (_centroid.X >= 0 && _centroid.Y >= 0 && _circle.c.x >= 0 && _circle.c.y >= 0)
                    {
                        g.DrawLine(new Pen(Color.Black, 2), _centroid.X, _centroid.Y, (int) _circle.c.x,
                            (int) _circle.c.y);
                    }

                    Point fractionPoint =
                        Line.Fraction(_centroid, new Point((int)_circle.c.x, (int)_circle.c.y), 0.5f);

                    if (fractionPoint.X >= 0 && fractionPoint.Y >= 0)
                    {
                        RectangleF rectf3 = new RectangleF(fractionPoint.X, fractionPoint.Y, 30, 30);
                        g.DrawString($"Δ", new Font("Microsoft Sans Serif", 16), Brushes.Black, rectf3);
                    }

                    SolidBrush myBrush = new SolidBrush(Color.DeepSkyBlue);

                    if (_centroid.X >= 3 && _centroid.Y >= 3)
                    {
                        g.FillEllipse(myBrush, new Rectangle(_centroid.X - 3, _centroid.Y - 3, 7, 7));
                    }

                    if (_centroid.X >= 0 && _centroid.Y >= 0)
                    {
                        RectangleF rectf = new RectangleF(_centroid.X, _centroid.Y, 30, 30);
                        g.DrawString($"S", new Font("Microsoft Sans Serif", 16), Brushes.DeepSkyBlue, rectf);
                    }

                    SolidBrush myBrush2 = new SolidBrush(Color.Magenta);

                    if (_circle.c.x >= 3 && _circle.c.y >= 3)
                    {
                        g.FillEllipse(myBrush2, new Rectangle((int) _circle.c.x - 3, (int) _circle.c.y - 3, 7, 7));
                    }

                    if (_circle.c.x >= 0 && _circle.c.y >= 0)
                    {
                        RectangleF rectf2 = new RectangleF((int) _circle.c.x, (int) _circle.c.y, 30, 30);
                        g.DrawString($"M", new Font("Microsoft Sans Serif", 16), Brushes.Magenta, rectf2);
                    }
                }

                g.DrawImage(_bitmap, 0, 0);
            }

            pictureBox.Image = _bitmap;
            pictureBox.Refresh();

            

            //make multithread
            AddLogMessage("Draw offset");
            for (int i = 1; i < _coordinates.Count; i++)
            {
                Point start = _coordinates[i - 1];
                Point end = _coordinates[i];

                if (start != end)
                {
                    Line lastLine = lines.LastOrDefault();
                    Line line = new Line(start, end, lastLine, _offsetOuterLine);

                    _cerebellumPerimet += line.Length;
                    lines.Add(line);

                    if (line.OuterLine != null)
                    {
                        outerLines.Add(line.OuterLine);

                        using (Graphics g = Graphics.FromImage(_bitmap))
                        {
                            if (line.OuterLine.PointStart != null && line.OuterLine.PointStart.X >= 0 &&
                                line.OuterLine.PointStart.Y >= 0 &&
                                line.OuterLine.PointEnd != null && line.OuterLine.PointEnd.X >= 0 &&
                                line.OuterLine.PointEnd.Y >= 0)
                            {
                                g.DrawLine(new Pen(Color.Turquoise, 2), line.OuterLine.PointStart,
                                    line.OuterLine.PointEnd);
                            }

                            g.DrawImage(_bitmap, new Point(0, 0));
                        }

                        pictureBox.Image = _bitmap;
                        pictureBox.Refresh();
                    }
                }
            }

            AddLogMessage("Draw cerebellum area again");
            using (Graphics g = Graphics.FromImage(_bitmap))
            {
                if (_coordinates.Count >= 3)
                {
                    g.FillPolygon(new SolidBrush(Color.LightBlue), _coordinates.ToArray());
                }

                if (cheDeltaSM.Checked)
                {
                    if (_centroid.X >= 0 && _centroid.Y >= 0 && _circle.c.x >= 0 && _circle.c.y >= 0)
                    {
                        g.DrawLine(new Pen(Color.Black, 2), _centroid.X, _centroid.Y, (int) _circle.c.x,
                            (int) _circle.c.y);
                    }
                    
                    Point fractionPoint =
                        Line.Fraction(_centroid, new Point((int) _circle.c.x, (int) _circle.c.y), 0.5f);

                    if (fractionPoint.X >= 0 && fractionPoint.Y >= 0)
                    {
                        RectangleF rectf3 = new RectangleF(fractionPoint.X, fractionPoint.Y, 30, 30);
                        g.DrawString($"Δ", new Font("Microsoft Sans Serif", 16), Brushes.Black, rectf3);
                    }

                    SolidBrush myBrush = new SolidBrush(Color.DeepSkyBlue);

                    if (_centroid.X >= 3 && _centroid.Y >= 3)
                    {
                        g.FillEllipse(myBrush, new Rectangle(_centroid.X - 3, _centroid.Y - 3, 7, 7));
                    }

                    if (_centroid.X >= 0 && _centroid.Y >= 0)
                    {
                        RectangleF rectf = new RectangleF(_centroid.X, _centroid.Y, 30, 30);
                        g.DrawString($"S", new Font("Microsoft Sans Serif", 16), Brushes.DeepSkyBlue, rectf);
                    }

                    SolidBrush myBrush2 = new SolidBrush(Color.Magenta);

                    if (_circle.c.x >= 3 && _circle.c.y >= 3)
                    {
                        g.FillEllipse(myBrush2, new Rectangle((int) _circle.c.x - 3, (int) _circle.c.y - 3, 7, 7));
                    }

                    if (_circle.c.x >= 0 && _circle.c.y >= 0)
                    {
                        RectangleF rectf2 = new RectangleF((int) _circle.c.x, (int) _circle.c.y, 30, 30);
                        g.DrawString($"M", new Font("Microsoft Sans Serif", 16), Brushes.Magenta, rectf2);
                    }
                }

                g.DrawImage(_bitmap, 0, 0);
            }

            pictureBox.Image = _bitmap;
            pictureBox.Refresh();

            _cerebellumPerimet = _cerebellumPerimet * _completeFactor;
            AddLogMessage($"cerebellum perimet: {_cerebellumPerimet.ToString(FLOAT_ACCURACY)} {UNIT}");
            lblCerPeri.Text = $"cerebellum perimet: {_cerebellumPerimet.ToString(FLOAT_ACCURACY)} {UNIT}";

            _solidity = _cerebellumArea / _convexHullArea;
            _circularity = _minCirclePerimet / _cerebellumPerimet;
            _convexity = _convexHullPerimet / _cerebellumPerimet;

            AddLogMessage($"solidity: {_solidity.ToString(FLOAT_ACCURACY)}");
            AddLogMessage($"circularity: {_circularity.ToString(FLOAT_ACCURACY)}");
            AddLogMessage($"convexity: {_convexity.ToString(FLOAT_ACCURACY)}");

            lblSolarity.Text = $"solidity: {_solidity.ToString(FLOAT_ACCURACY)}";
            lblCircularity.Text = $"circularity: {_circularity.ToString(FLOAT_ACCURACY)}";
            lblConvexity.Text = $"convexity: {_convexity.ToString(FLOAT_ACCURACY)}";


            AddLogMessage("Draw cerebellum perimet");
            for (int l = 1; l < _coordinates.Count; l++)
            {
                Point newPoint = _coordinates[l];
                Point oldPoint = _coordinates[l - 1];

                using (Graphics g = Graphics.FromImage(_bitmap))
                {
                    if (oldPoint.X >= 0 && oldPoint.Y >= 0 && newPoint.X >= 0 && newPoint.Y >= 0)
                    {
                        Pen mypen = new Pen(Color.Red, 2);
                        g.DrawLine(mypen, oldPoint.X, oldPoint.Y, newPoint.X, newPoint.Y);
                    }

                    pictureBox.Invalidate();
                }
            }
        }
        private void Measure(bool againWithoutAutomaticFindFissuresAndLobules = false)
        {
            // _coordinates.Reverse();

            List<Line> lines = new List<Line>();
            List<Line> outerLines = new List<Line>();

            MeasureStart(outerLines, lines);

            bool manual = false;

            if (againWithoutAutomaticFindFissuresAndLobules == false)
            {
                FindFissures(outerLines, lines);

                DrawFissures(lines);

                FindLobules();

                DrawLobules(lines);

                DrawMarkArea();

                

                DialogResult resRegoCorrect = MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Yes", "No",
                    "Were all lobules recognized correctly ?");
                //MessageBox.Show("Were all lobules recognized correctly ?", "Check",
                //MessageBoxButtons.YesNo);

                if (resRegoCorrect == DialogResult.Cancel)
                {
                    manual = true;

                    DialogResult resFlip = MessageBoxAutocloseWithButtons.Show(_waitTime, true, "No", "Yes, flip",
                        "Need to flip image ?");
                    //MessageBox.Show("Need to flip image ?", "Check",
                    //MessageBoxButtons.YesNo);

                    bool neetToFlip = resFlip == DialogResult.Cancel;

                    if (neetToFlip)
                    {
                        Flip();
                        Analyze();
                        return;
                    }
                    // manuell bestimmen

                    lines.Clear();
                    outerLines.Clear();

                    _fisurList.Clear();
                    _lobuliList.Clear();

                    _cerebellumArea = 0;
                    _cerebellumPerimet = 0;
                    _minCircleArea = 0;
                    _minCirclePerimet = 0;
                    _minCircleRadius = 0;
                    _convexHullArea = 0;
                    _convexHullPerimet = 0;
                    _solidity = 0;
                    _circularity = 0;
                    _convexity = 0;
                    _deltaSM = 0;
                    _markArea = 0;
                    _markPerimeter = 0;
                    _markMinCircleRadius = 0;
                    _markMinCircleArea = 0;
                    _markMinCirclePerimet = 0;
                    _markConvexHullArea = 0;
                    _markConvexHullPerimet = 0;
                    _markConvexity = 0;
                    _markSolidity = 0;
                    _markCircularity = 0;
                    _markDistanceToCerebellumCenter = 0;

                    _markConexHullPoints = new List<Point>();

                    _percentMarkAreaOfCerebellumgArea = 0;
                    _center = new Point();

                    _countFisur = 0;
                    _sumFisurLength = 0;
                    _sumFisurPerimeter = 0;
                    _sumFisurArea = 0;
                    _sumFisurStraigtness = 0;
                    _averageFisurLength = 0;
                    _averageFisurPerimeter = 0;
                    _averageFisurArea = 0;
                    _averageFisurStraigtness = 0;

                    _countLobulus = 0;
                    _sumLobulusLength = 0;
                    _sumLobulusPerimeter = 0;
                    _sumLobulusArea = 0;
                    _sumLobulusStraigtness = 0;
                    _sumLobulusPercentageOfCerebellumArea = 0;
                    _averageLobulusLength = 0;
                    _averageLobulusPerimeter = 0;
                    _averageLobulusArea = 0;
                    _averageLobulusStraigtness = 0;
                    _averageLobulusPercentageOfCerebellumArea = 0;


                    MeasureStart(outerLines, lines);

                    for (int i = 1; i <= 9; i++)
                    {
                        
                        DialogResult resLobExist = MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Yes", "No",
                            $"Does lobule #{i} exist ?");
                        //MessageBox.Show($"Does lobule #{i} exist ?", "Check",
                        //MessageBoxButtons.YesNo);
                        bool resLob = resLobExist == DialogResult.OK;

                        if (resLob == false)
                        {
                            Fisur lobuliNotExist = new Fisur
                            {
                                NotExistent = true
                            };

                            _lobuliList.Add(lobuliNotExist);
                        }
                        else
                        {
                            Point lastLobuleEnd = new Point(_coordinates[0].X, _coordinates[0].Y);
                         

                            if (_lobuliList.Count > 0)
                            {
                                for (int j = _lobuliList.Count - 1; j >= 0; j--)
                                {
                                    Fisur lastLobule = _lobuliList[j];

                                    if (lastLobule.NotExistent == false)
                                    {
                                        lastLobuleEnd = new Point(lastLobule.End.X, lastLobule.End.Y);
                                        break;
                                    }
                                }
                            }

                            FormChooseLobule fcl = new FormChooseLobule(i, lastLobuleEnd, pictureBox.Image, lines, _waitTime);
                            fcl.ShowDialog();

                            if (fcl.lobuleNotExist == true)
                            {
                                Fisur lobuliNotExist = new Fisur
                                {
                                    NotExistent = true,
                                    Id = i
                                };

                                _lobuliList.Add(lobuliNotExist);
                            }
                            else
                            {
                                Point lobuliStart = new Point(fcl.startPoint.X, fcl.startPoint.Y);
                                Point lobuliEnd = new Point(fcl.endPoint.X, fcl.endPoint.Y);
                                Point lobuliDeepestPoint = Line.Fraction(lobuliStart, lobuliEnd, 0.5f);

                                Fisur lobule = new Fisur
                                {
                                    Start = lobuliStart,
                                    End = lobuliEnd,
                                    DeepestPoint = lobuliDeepestPoint,
                                    Id = i
                                };

                                DrawLobule(i, lines, lobuliStart, lobuliEnd, lobule);

                                _lobuliList.Add(lobule);
                            }
                        }
                    }

                    DrawMarkArea();

                    for (int lobuleIdentifier = 1; lobuleIdentifier < _lobuliList.Count - 1; lobuleIdentifier++)
                    {
                        Fisur thisLobule = _lobuliList[lobuleIdentifier - 1];
                        Fisur nextLobule = _lobuliList[lobuleIdentifier];
                        
                        if (thisLobule.NotExistent || nextLobule.NotExistent)
                        {
                            Fisur fisurNotExist = new Fisur
                            {
                                NotExistent = true,
                                Id = thisLobule.Id
                            };

                            _fisurList.Add(fisurNotExist);
                        }
                        else
                        {
                            if(FindFissures(outerLines, lines, false, thisLobule, nextLobule) == false)
                            {
                                FindFissures(outerLines, lines, true, thisLobule, nextLobule);
                            }
                        }

                        DrawFissures(lines);

                    }

                    //MeasureStart(outerLines, lines);

                    //for (int i = 1; i < _lobuliList.Count; i++)
                    //{
                    //    Fisur lobuli = _lobuliList[i - 1];
                    //    DrawLobule(i, lines, lobuli.Start, lobuli.End, lobuli);
                    //}

                    //DrawFissures(lines);

                }
            }
            else
            {
                DrawFissures(lines);
            }

            foreach (var fisur in _fisurList)
            {
                if (fisur.Perimeter < 1 || fisur.Length < 1 || fisur.Area < 1)
                {
                    fisur.Perimeter = 0;
                    fisur.Length = 0;
                    fisur.Perimeter = 0;
                }
            }

            foreach (var lobule in _lobuliList)
            {
                if (lobule.Perimeter < 1 || lobule.Length < 1 || lobule.Area < 1)
                {
                    lobule.Perimeter = 0;
                    lobule.Length = 0;
                    lobule.Perimeter = 0;
                    lobule.Straigtness = 0;
                    lobule.PercentageOfCerebellum = 0;
                }
            }

            AddFissureToDgv(manual);

            AddLobulesToDgv();

            _markArea = _cerebellumArea - _sumLobulusArea;
            AddLogMessage($"White matter area: {_markArea}");
            lblMarkArea.Text = $"white matter area: {_markArea.ToString(FLOAT_ACCURACY)} {UNIT}²";

            _percentMarkAreaOfCerebellumgArea = (_markArea / _cerebellumArea) * 100;
            AddLogMessage($"Percentage of white matter area in cerebellum area: {_percentMarkAreaOfCerebellumgArea}");
            lblMarkCerebellum.Text = $"white matter in cerebellum: {_percentMarkAreaOfCerebellumgArea.ToString(FLOAT_ACCURACY)} %";

            CalculateAngleAndDistances();

            CalculateCircularityConvexitySolidityWhiteMatter(lines);

            int xVerschiebung = 0;
            int yVerschiebung = 0;

            DrawScale(ref _bitmap, ref pictureBox, ref _circle, ref xVerschiebung, ref yVerschiebung);
            
            if(MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Yes", "No",
                "Would you like to upload the analyzed data & results now?") == DialogResult.OK)
            //if (MessageBox.Show("Would you like to upload the analyzed data & results now?", "Upload", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                UploadResults();
            }
        }

        private void CalculateAngleAndDistances()
        {
            int _lobuliListCount = _lobuliList.Count;
            for (int i = 0; i < _lobuliListCount; i++)
            {
                Fisur thisLobule = _lobuliList.ElementAtOrDefault(i);
                Fisur nextLobule = _lobuliList.ElementAtOrDefault(i + 1);

                if (thisLobule == null || thisLobule.NotExistent) continue;

                Circle circle;

                thisLobule.AngleToX = (new Line(thisLobule.DeepestPoint, thisLobule.HighestPoint).Angle - 180) * -1;
                Point startPoint = Line.Fraction(thisLobule.Start, thisLobule.End, 0.5f);

                if (nextLobule != null && nextLobule.NotExistent == false)
                {
                    thisLobule.AngleToNextLorF = ((new Line(nextLobule.DeepestPoint, nextLobule.HighestPoint).Angle - 180) * -1) - (thisLobule.AngleToX);
                    
                    Point endPoint = Line.Fraction(nextLobule.Start, nextLobule.End, 0.5f);

                    thisLobule.DistanceToNextLorF = Line.GetDistanceBetweenTwoPoints(startPoint, endPoint) * _completeFactor;
                }

                circle = Geometry.MakeCircle(thisLobule.PointsOnLine);
                PointF center = new PointF((float)circle.c.x, (float)circle.c.y);
                
                //_bitmap = new Bitmap(pictureBox.Image);
                pictureBox.Image = _bitmap;
                pictureBox.Refresh();

                //needet for drawing

                float radiusTemp = (float)circle.r;
                if (radiusTemp > 0)
                {
                    thisLobule.MinCircleRadius = radiusTemp * _completeFactor;
                    thisLobule.MinCircleArea = Math.PI * (thisLobule.MinCircleRadius * thisLobule.MinCircleRadius);
                    thisLobule.MinCirclePerimet = (2 * Math.PI * radiusTemp) * _completeFactor;
                }

                //using (Graphics g = Graphics.FromImage(_bitmap))
                //{
                //    Pen pen = new Pen(Color.Magenta, 2);
                //    g.DrawEllipse(pen, center.X - radiusTemp, center.Y - radiusTemp, 2 * radiusTemp, 2 * radiusTemp);
                //    g.DrawImage(_bitmap, 0, 0);
                //}

                //pictureBox.Image = _bitmap;
                //pictureBox.Refresh();

                if (thisLobule.PointsOnLine.Count > 6)
                {
                    thisLobule.ConexHullPoints = Geometry.MakeConvexHull(thisLobule.PointsOnLine);

                    thisLobule.ConvexHullArea = Geometry.PolygonArea(thisLobule.ConexHullPoints) * _completeFactor;

                    double distance = 0;
                    for (int j = 1; j < thisLobule.ConexHullPoints.Count; j++)
                    {
                        Point thisPoint = thisLobule.ConexHullPoints[j];
                        Point lastPoint = thisLobule.ConexHullPoints[j - 1];

                        distance += Line.GetDistanceBetweenTwoPoints(thisPoint, lastPoint);
                    }

                    thisLobule.ConvexHullPerimet = distance * _completeFactor;
                    thisLobule.Convexity = thisLobule.ConvexHullPerimet / (thisLobule.Perimeter * _completeFactor);
                }

                thisLobule.Solidity = thisLobule.Area / thisLobule.ConvexHullArea;
                thisLobule.Circularity = (thisLobule.MinCirclePerimet / _completeFactor) / (thisLobule.Perimeter * _completeFactor);
                thisLobule.DistanceToCerebellumCenter = Line.GetDistanceBetweenTwoPoints(startPoint, _center) * _completeFactor;
                
                int abstandAusserhalbKreis = 50;

                int newWidthHeigt = (int)((radiusTemp + (abstandAusserhalbKreis * 2)) * 2);

                Bitmap newBitmap = new Bitmap(newWidthHeigt, newWidthHeigt);

                int xNeu = abstandAusserhalbKreis + (int)radiusTemp;
                int xAlt = (int)circle.c.x;

                int xVerschiebung = xAlt - xNeu;

                int yNeu = abstandAusserhalbKreis + (int)radiusTemp;
                int yAlt = (int)circle.c.y;

                int yVerschiebung = yAlt - yNeu;

                List<Point> verschobenePointsOnLine = new List<Point>();

                foreach (Point pointOnLine in thisLobule.PointsOnLine)
                {
                    int newX = pointOnLine.X - xVerschiebung;
                    int newY = pointOnLine.Y - yVerschiebung;

                    if (newX >= 0 && newY >= 0)
                    {
                        verschobenePointsOnLine.Add(new Point(newX, newY));
                    }
                }

                List<Point> verschobeneConvexHullPoints = new List<Point>();

                foreach (Point convexHullPoint in thisLobule.ConexHullPoints ?? new List<Point>())
                {
                    int newX = convexHullPoint.X - xVerschiebung;
                    int newY = convexHullPoint.Y - yVerschiebung;

                    if (newX >= 0 && newY >= 0)
                    {
                        verschobeneConvexHullPoints.Add(new Point(newX, newY));
                    }
                }

                Point deepestPoint = Line.Fraction(thisLobule.Start, thisLobule.End, 0.5f);

                if (thisLobule.DeepestPoint.X > 0 && thisLobule.DeepestPoint.Y > 0)
                {
                    deepestPoint = new Point(thisLobule.DeepestPoint.X - xVerschiebung, thisLobule.DeepestPoint.Y - yVerschiebung);
                }

                using (Graphics g = Graphics.FromImage(newBitmap))
                {
                    if (verschobeneConvexHullPoints.Count >= 3)
                    {
                        g.FillPolygon(new SolidBrush(Color.Khaki), verschobeneConvexHullPoints.ToArray());
                    }

                    if (verschobenePointsOnLine.Count >= 3)
                    {
                        g.FillPolygon(new SolidBrush(Color.LightBlue), verschobenePointsOnLine.ToArray());
                    }

                    Pen pen = new Pen(Color.Magenta, 2);

                    float xEli = center.X - radiusTemp - xVerschiebung;
                    float yEli = center.Y - radiusTemp - yVerschiebung;

                    if (xEli >= 0 && yEli >= 0)
                    {
                        g.DrawEllipse(pen, xEli, yEli, 2 * radiusTemp, 2 * radiusTemp);
                    }

                    Point pt1 = new Point(thisLobule.Start.X - xVerschiebung, thisLobule.Start.Y - yVerschiebung);
                    Point pt2 = new Point(thisLobule.End.X - xVerschiebung, thisLobule.End.Y - yVerschiebung);

                    if (pt1.X >= 0 && pt1.Y >= 0 && pt2.X >= 0 && pt2.Y >= 0)
                    {
                        g.DrawLine(new Pen(Color.IndianRed, 1), pt1, pt2);
                    }

                    SolidBrush myBrush0 = new SolidBrush(Color.Green);

                    float xEli2 = thisLobule.HighestPoint.X - xVerschiebung - 3;
                    float yEli2 = thisLobule.HighestPoint.Y - yVerschiebung - 3;

                    if (xEli2 >= 0 && yEli2 >= 0)
                    {
                        g.FillEllipse(myBrush0, new Rectangle((int) xEli2, (int) yEli2, 7, 7));
                    }

                    Point pt12 = new Point(thisLobule.HighestPoint.X - xVerschiebung,
                        thisLobule.HighestPoint.Y - yVerschiebung);

                    if (pt12.X >= 0 && pt12.Y >= 0 && deepestPoint.X >= 0 && deepestPoint.Y >= 0)
                    {
                        g.DrawLine(new Pen(Color.Green, 1) {DashStyle = DashStyle.Dot}, pt12, deepestPoint);
                    }

                    SolidBrush myBrush = new SolidBrush(Color.BlueViolet);

                    if (deepestPoint.X >= 3 && deepestPoint.Y >= 3)
                    {
                        g.FillEllipse(myBrush, new Rectangle(deepestPoint.X - 3, deepestPoint.Y - 3, 7, 7));
                    }

                    g.DrawImage(newBitmap, 0, 0);
                }

                int idToCheck = thisLobule.Id;

                if (idToCheck < 0)
                {
                    idToCheck = i + 1;
                }

                int xStart = (int)(center.X - radiusTemp - xVerschiebung);
                int yStart = (int)(center.Y - radiusTemp - yVerschiebung - 20);

                switch (idToCheck)
                {
                    case 1:
                        pictureBox1.Image = newBitmap;
                        pictureBox1.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox1, ref circle, ref xStart, ref yStart);
                        break;
                    case 2:
                        pictureBox2.Image = newBitmap;
                        pictureBox2.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox2, ref circle, ref xStart, ref yStart);
                        break;
                    case 3:
                        pictureBox3.Image = newBitmap;
                        pictureBox3.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox3, ref circle, ref xStart, ref yStart);
                        break;
                    case 4:
                        pictureBox4.Image = newBitmap;
                        pictureBox4.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox4, ref circle, ref xStart, ref yStart);
                        break;
                    case 5:
                        pictureBox5.Image = newBitmap;
                        pictureBox5.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox5, ref circle, ref xStart, ref yStart);
                        break;
                    case 6:
                        pictureBox6.Image = newBitmap;
                        pictureBox6.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox6, ref circle, ref xStart, ref yStart);
                        break;
                    case 7:
                        pictureBox7.Image = newBitmap;
                        pictureBox7.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox7, ref circle, ref xStart, ref yStart);
                        break;
                    case 8:
                        pictureBox8.Image = newBitmap;
                        pictureBox8.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox8, ref circle, ref xStart, ref yStart);
                        break;
                    case 9:
                        pictureBox9.Image = newBitmap;
                        pictureBox9.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox9, ref circle, ref xStart, ref yStart);
                        break;
                }
            }

            int _fisurListCont = _fisurList.Count;
            for (int i = 0; i < _fisurListCont; i++)
            {
                Fisur thisFisur = _fisurList.ElementAtOrDefault(i);
                Fisur nextFisur = _fisurList.ElementAtOrDefault(i + 1);

                if (thisFisur == null || thisFisur.NotExistent) continue;

                Circle circle;

                thisFisur.AngleToX = (new Line(thisFisur.DeepestPoint, thisFisur.HighestPoint).Angle - 180 ) * -1;
                Point startPoint = Line.Fraction(thisFisur.Start, thisFisur.End, 0.5f);

                if (nextFisur != null && nextFisur.NotExistent == false)
                {
                    thisFisur.AngleToNextLorF = ((new Line(nextFisur.DeepestPoint, nextFisur.HighestPoint).Angle - 180) * -1) - (thisFisur.AngleToX);
                    
                    Point endPoint = Line.Fraction(nextFisur.Start, nextFisur.End, 0.5f);

                    thisFisur.DistanceToNextLorF = Line.GetDistanceBetweenTwoPoints(startPoint, endPoint) * _completeFactor;
                }

                circle = Geometry.MakeCircle(thisFisur.PointsOnLine);
                PointF center = new PointF((float)circle.c.x, (float)circle.c.y);

                //needet for drawing

                float radiusTemp = (float)circle.r;
                if (radiusTemp > 0)
                {
                    thisFisur.MinCircleRadius = radiusTemp * _completeFactor;
                    thisFisur.MinCircleArea = Math.PI * (thisFisur.MinCircleRadius * thisFisur.MinCircleRadius);
                    thisFisur.MinCirclePerimet = (2 * Math.PI * radiusTemp) * _completeFactor;
                }

                if (thisFisur.PointsOnLine.Count > 6)
                {
                    thisFisur.ConexHullPoints = Geometry.MakeConvexHull(thisFisur.PointsOnLine);

                    thisFisur.ConvexHullArea = Geometry.PolygonArea(thisFisur.ConexHullPoints) / _completeFactor;

                    double distance = 0;
                    for (int j = 1; j < thisFisur.ConexHullPoints.Count; j++)
                    {
                        Point thisPoint = thisFisur.ConexHullPoints[j];
                        Point lastPoint = thisFisur.ConexHullPoints[j - 1];

                        distance += Line.GetDistanceBetweenTwoPoints(thisPoint, lastPoint);
                    }

                    thisFisur.ConvexHullPerimet = distance / _completeFactor;
                    thisFisur.Convexity = thisFisur.ConvexHullPerimet / (thisFisur.Perimeter * _completeFactor);
                }

                thisFisur.Solidity = thisFisur.Area / thisFisur.ConvexHullArea;
                thisFisur.Circularity = (thisFisur.MinCirclePerimet / _completeFactor) / (thisFisur.Perimeter * _completeFactor);
                
                thisFisur.DistanceToCerebellumCenter = Line.GetDistanceBetweenTwoPoints(startPoint, _center) * _completeFactor;

                int abstandAusserhalbKreis = 50;

                int newWidthHeigt = (int)((radiusTemp + (abstandAusserhalbKreis * 2)) * 2);

                Bitmap newBitmap = new Bitmap(newWidthHeigt, newWidthHeigt);

                int xNeu = abstandAusserhalbKreis + (int)radiusTemp;
                int xAlt = (int)circle.c.x;

                int xVerschiebung = xAlt - xNeu;

                int yNeu = abstandAusserhalbKreis + (int)radiusTemp;
                int yAlt = (int)circle.c.y;

                int yVerschiebung = yAlt - yNeu;

                List<Point> verschobenePointsOnLine = new List<Point>();

                foreach (Point pointOnLine in thisFisur.PointsOnLine)
                {
                    int newX = pointOnLine.X - xVerschiebung;
                    int newY = pointOnLine.Y - yVerschiebung;

                    if (newX >= 0 && newY >= 0)
                    {
                        verschobenePointsOnLine.Add(new Point(newX, newY));
                    }
                }

                List<Point> verschobeneConvexHullPoints = new List<Point>();

                foreach (Point convexHullPoint in thisFisur.ConexHullPoints ?? new List<Point>())
                {
                    int newX = convexHullPoint.X - xVerschiebung;
                    int newY = convexHullPoint.Y - yVerschiebung;

                    if (newX >= 0 && newY >= 0)
                    {
                        verschobeneConvexHullPoints.Add(new Point(newX, newY));
                    }
                }

                using (Graphics g = Graphics.FromImage(newBitmap))
                {
                    if (verschobeneConvexHullPoints.Count >= 3)
                    {
                        g.FillPolygon(new SolidBrush(Color.Khaki), verschobeneConvexHullPoints.ToArray());
                    }

                    g.DrawImage(newBitmap, 0, 0);

                    if (verschobenePointsOnLine.Count >= 3)
                    {
                        g.FillPolygon(new SolidBrush(Color.Blue), verschobenePointsOnLine.ToArray());
                    }

                    Pen pen = new Pen(Color.Magenta, 2);
                    float xEli = center.X - radiusTemp - xVerschiebung;
                    float yEli = center.Y - radiusTemp - yVerschiebung;

                    if (xEli >= 0 && yEli >= 0)
                    {
                        g.DrawEllipse(pen, xEli, yEli, 2 * radiusTemp, 2 * radiusTemp);
                    }

                    g.DrawImage(newBitmap, 0, 0);
                }

                int idToCheck = thisFisur.Id;

                if (idToCheck < 0)
                {
                    idToCheck = i + 1;
                }

                int xStart = (int)(center.X - radiusTemp - xVerschiebung);
                int yStart = (int)(center.Y - radiusTemp - yVerschiebung - 20);

                switch (idToCheck)
                {
                    case 1:
                        pictureBox18.Image = newBitmap;
                        pictureBox18.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox18, ref circle, ref xStart, ref yStart);
                        break;
                    case 2:
                        pictureBox17.Image = newBitmap;
                        pictureBox17.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox17, ref circle, ref xStart, ref yStart);
                        break;
                    case 3:
                        pictureBox16.Image = newBitmap;
                        pictureBox16.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox16, ref circle, ref xStart, ref yStart);
                        break;
                    case 4:
                        pictureBox15.Image = newBitmap;
                        pictureBox15.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox15, ref circle, ref xStart, ref yStart);
                        break;
                    case 5:
                        pictureBox14.Image = newBitmap;
                        pictureBox14.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox14, ref circle, ref xStart, ref yStart);
                        break;
                    case 6:
                        pictureBox13.Image = newBitmap;
                        pictureBox13.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox13, ref circle, ref xStart, ref yStart);
                        break;
                    case 7:
                        pictureBox12.Image = newBitmap;
                        pictureBox12.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox12, ref circle, ref xStart, ref yStart);
                        break;
                    case 8:
                        pictureBox11.Image = newBitmap;
                        pictureBox11.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox11, ref circle, ref xStart, ref yStart);
                        break;
                    case 9:
                        pictureBox10.Image = newBitmap;
                        pictureBox10.Refresh();
                        DrawScale(ref newBitmap, ref pictureBox10, ref circle, ref xStart, ref yStart);
                        break;
                }
            }
        }

        private void CalculateCircularityConvexitySolidityWhiteMatter(List<Line> lines)
        {
            List<Point> markPoints = new List<Point>();

            int _lobuliListCount = _lobuliList.Count;
            for (int i = 0; i < _lobuliListCount; i++)
            {
                Fisur thisLobule = _lobuliList.ElementAtOrDefault(i);
                Fisur nextLobule = _lobuliList.ElementAtOrDefault(i + 1);
                

                if (thisLobule == null || thisLobule.NotExistent) continue;

                markPoints.Add(thisLobule.Start);
                markPoints.Add(thisLobule.End);
                _markPerimeter += (Line.GetDistanceBetweenTwoPoints(thisLobule.Start, thisLobule.End)) * _completeFactor;

                if (nextLobule == null) continue;

                if (nextLobule.NotExistent)
                {
                    Fisur next2Lobule = null;
                    for (int k = i + 2; k < _lobuliListCount; k++)
                    {
                        next2Lobule = _lobuliList.ElementAtOrDefault(k);
                        if (next2Lobule != null && next2Lobule.NotExistent == false)
                            break;
                    }

                    if(next2Lobule == null || next2Lobule.NotExistent) continue;
                    
                    bool foundStart = false;

                    foreach (Line line in lines)
                    {
                        if (line.OuterLine == null)
                        {
                            continue;
                        }

                        if (foundStart == false)
                        {
                            if (line.PointStart == thisLobule.End)
                            {
                                //start found
                                foundStart = true;

                                markPoints.Add(line.PointStart);
                                _markPerimeter += line.Length * _completeFactor;
                            }
                        }
                        else
                        {
                            if (line.PointStart != next2Lobule.Start && line.PointEnd != next2Lobule.Start)
                            {
                                //in white mark
                                markPoints.Add(line.PointStart);
                                _markPerimeter += line.Length * _completeFactor;

                            }
                            else
                            {
                                //end found
                                foundStart = false;

                                markPoints.Add(next2Lobule.Start);
                                _markPerimeter += line.Length * _completeFactor;
                            }
                        }
                    }
                }
            }


            Circle circle = Geometry.MakeCircle(markPoints);
            PointF center = new PointF((float)circle.c.x, (float)circle.c.y);

            //needet for drawing

            float radiusTemp = (float)circle.r;
            if (radiusTemp > 0)
            {
                _markMinCircleRadius = radiusTemp * _completeFactor;
                _markMinCircleArea = Math.PI * (_markMinCircleRadius * _markMinCircleRadius);
                _markMinCirclePerimet = (2 * Math.PI * radiusTemp) * _completeFactor;
            }

            if (markPoints.Count > 6)
            {
                _markConexHullPoints = Geometry.MakeConvexHull(markPoints);

                _markConvexHullArea = Geometry.PolygonArea(_markConexHullPoints) * _completeFactor;

                double distance = 0;
                for (int j = 1; j < _markConexHullPoints.Count; j++)
                {
                    Point thisPoint = _markConexHullPoints[j];
                    Point lastPoint = _markConexHullPoints[j - 1];

                    distance += Line.GetDistanceBetweenTwoPoints(thisPoint, lastPoint);
                }

                _markConvexHullPerimet = distance * _completeFactor;
                _markConvexity = _markConvexHullPerimet / (_markPerimeter * _completeFactor);
            }

            _markSolidity = _markArea / _markConvexHullArea;
            _markCircularity = (_markMinCirclePerimet) / (_markPerimeter * _completeFactor);
            _markDistanceToCerebellumCenter = Line.GetDistanceBetweenTwoPoints(center, _center) * _completeFactor;

            int abstandAusserhalbKreis = 50;

            int newWidthHeigt = (int)((radiusTemp + (abstandAusserhalbKreis * 2)) * 2);

            Bitmap whiteMatterAreBitmap = new Bitmap(newWidthHeigt, newWidthHeigt);

            int xNeu = abstandAusserhalbKreis + (int) radiusTemp;
            int xAlt = (int)circle.c.x;

            int xVerschiebung = xAlt - xNeu;

            int yNeu = abstandAusserhalbKreis + (int)radiusTemp;
            int yAlt = (int)circle.c.y;

            int yVerschiebung = yAlt - yNeu;

            List<Point> verschobeneMarkPoints = new List<Point>();

            foreach (Point markPoint in markPoints)
            {
                int newX = markPoint.X - xVerschiebung;
                int newY = markPoint.Y - yVerschiebung;

                if (newX >= 0 && newY >= 0)
                {
                    verschobeneMarkPoints.Add(new Point(newX, newY));
                }
            }

            List<Point> verschobeneConvexHullPoints = new List<Point>();

            foreach (Point convexHullPoint in _markConexHullPoints)
            {
                int newX = convexHullPoint.X - xVerschiebung;
                int newY = convexHullPoint.Y - yVerschiebung;

                if (newX >= 0 && newY >= 0)
                {
                    verschobeneConvexHullPoints.Add(new Point(newX, newY));
                }
            }

            using (Graphics g = Graphics.FromImage(whiteMatterAreBitmap))
            {
                if (verschobeneConvexHullPoints != null && verschobeneConvexHullPoints.Count >= 3)
                {
                    g.FillPolygon(new SolidBrush(Color.Khaki), verschobeneConvexHullPoints.ToArray());
                }

                if (verschobeneMarkPoints != null && verschobeneMarkPoints.Count >= 3)
                {
                    g.FillPolygon(new SolidBrush(Color.LightBlue), verschobeneMarkPoints.ToArray());
                }

                Pen pen = new Pen(Color.Magenta, 2);

                float xEl = center.X - radiusTemp - xVerschiebung;
                float yEl = center.Y - radiusTemp - yVerschiebung;

                if(xEl >= 0 && yEl >= 0)
                g.DrawEllipse(pen, xEl, yEl, 2 * radiusTemp, 2 * radiusTemp);

                g.DrawImage(whiteMatterAreBitmap, 0, 0);
            }

            pictureBox19.Image = whiteMatterAreBitmap;
            pictureBox19.Refresh();

            int xStart = (int)(center.X - radiusTemp - xVerschiebung);
            int yStart = (int)(center.Y - radiusTemp - yVerschiebung - 20);

            DrawScale(ref whiteMatterAreBitmap, ref pictureBox19, ref circle, ref xStart, ref yStart);

            pictureBox19.Image = whiteMatterAreBitmap;
            pictureBox19.Refresh();
        }

        private void DrawMarkArea()
        {
            //looks ugly
            //List<Point> markAreCoordinates = new List<Point>();

            //foreach (Fisur lobule in _lobuliList)
            //{
            //    markAreCoordinates.Add(lobule.Start);
            //    markAreCoordinates.Add(lobule.End);
            //}

            //using (Graphics g = Graphics.FromImage(_bitmap))
            //{
            //    HatchBrush hBrush = new HatchBrush(
            //        HatchStyle.Percent20,
            //        Color.Aquamarine,
            //        Color.Transparent);
            //    g.FillPolygon(hBrush, markAreCoordinates.ToArray());
            //}

            //pictureBox.Image = _bitmap;
            //pictureBox.Refresh();
        }

        private void DrawLobule(int _lobuleIndex, List<Line> lines, Point start, Point end, Fisur lobule)
        {
            bool foundStart = false;
            List<Point> lobulusCoordinates = new List<Point>();

            lobule.PointsOnLine.Clear();

            foreach (Line line in lines)
            {
                try
                {
                    if (line.OuterLine == null)
                    {
                        continue;
                    }

                    if (foundStart == false)
                    {
                        if (line.PointStart == start)
                        {
                            //start found
                            foundStart = true;
                            lobule.Perimeter += line.Length * _completeFactor;

                            if (line.PointStart.X > 0 && line.PointStart.Y > 0)
                            {
                                lobule.PointsOnLine.Add(line.PointStart);
                            }

                            Point[] points = line.GetPoints();
                            if (points != null && points.Length > 0)
                            {
                                foreach (var point in points.ToList())
                                {
                                    if (point.X > 0 && point.Y > 0)
                                    {
                                        lobule.PointsOnLine.Add(point);
                                    }
                                }
                            }

                            using (Graphics g = Graphics.FromImage(_bitmap))
                            {
                                
                                Pen mypen = new Pen(Color.Green, 2);
                                if (line.PointStart.X >= 0 && line.PointStart.Y >= 0 && line.PointEnd.X >= 0 && line.PointEnd.Y >= 0)
                                    g.DrawLine(mypen, line.PointStart.X, line.PointStart.Y, line.PointEnd.X,
                                    line.PointEnd.Y);
                            }

                            pictureBox.Image = _bitmap;
                            pictureBox.Refresh();

                            lobulusCoordinates.Add(line.PointStart);

                        }
                    }
                    else
                    {
                        if (line.PointStart != end && line.PointEnd != end)
                        {
                            //in fisure

                            Point[] points = line.GetPoints();
                            if (points != null && points.Length > 0)
                            {
                                foreach (var point in points.ToList())
                                {
                                    if (point.X > 0 && point.Y > 0)
                                    {
                                        lobule.PointsOnLine.Add(point);
                                    }
                                }
                            }


                            using (Graphics g = Graphics.FromImage(_bitmap))
                            {
                                Pen mypen = new Pen(Color.Green, 2);
                                if (line.PointStart.X >= 0 && line.PointStart.Y >= 0 && line.PointEnd.X >= 0 && line.PointEnd.Y >= 0)
                                    g.DrawLine(mypen, line.PointStart.X, line.PointStart.Y, line.PointEnd.X,
                                    line.PointEnd.Y);
                            }

                            pictureBox.Image = _bitmap;
                            pictureBox.Refresh();

                            lobule.Perimeter += line.Length * _completeFactor;

                            double currentDistance =
                                Line.GetDistanceBetweenTwoPoints(lobule.DeepestPoint, line.PointEnd) * _completeFactor;
                            if (currentDistance > lobule.Length)
                            {
                                lobule.Length = currentDistance;
                                lobule.HighestPoint = line.PointEnd;
                            }

                            lobulusCoordinates.Add(line.PointStart);

                        }
                        else
                        {
                            //end found
                            foundStart = false;


                            if (end.X > 0 && end.Y > 0)
                            {
                                lobule.PointsOnLine.Add(end);
                            }

                            lobule.Perimeter += line.Length * _completeFactor;
                            lobulusCoordinates.Add(end);
                            lobulusCoordinates.Add(lobule.DeepestPoint);
                            lobule.Area = Geometry.PolygonArea(lobulusCoordinates) * _completeFactor;
                            lobule.Straigtness = 1 - Math.Abs((lobule.Length / lobule.Perimeter) - 0.5);

                            Point lobulusCentroid = Geometry.FindCentroid(lobulusCoordinates);
                            Point deepestPoint = Line.Fraction(lobule.Start, lobule.End, 0.5f);

                            if (lobule.DeepestPoint.X > 0 && lobule.DeepestPoint.Y > 0)
                            {
                                deepestPoint = lobule.DeepestPoint;
                            }

                            try
                            {
                                using (Graphics g = Graphics.FromImage(_bitmap))
                                {
                                    if(lobule.Start.X >= 0 && lobule.Start.Y >= 0 && lobule.End.X >= 0 && lobule.End.Y >= 0)
                                    g.DrawLine(new Pen(Color.IndianRed, 1), lobule.Start, lobule.End);

                                    SolidBrush myBrush0 = new SolidBrush(Color.Green);
                                    if(lobule.HighestPoint.X >= 3 && lobule.HighestPoint.Y >= 3)
                                    g.FillEllipse(myBrush0,
                                        new Rectangle((int) lobule.HighestPoint.X - 3, (int) lobule.HighestPoint.Y - 3,
                                            7,
                                            7));

                                    if(lobule.HighestPoint.X >= 0 && lobule.HighestPoint.Y >= 0 && deepestPoint.X >= 0 && deepestPoint.Y >= 0)
                                    g.DrawLine(new Pen(Color.Green, 1) {DashStyle = DashStyle.Dot}, lobule.HighestPoint,
                                        deepestPoint);

                                    SolidBrush myBrush = new SolidBrush(Color.BlueViolet);
                                    if(deepestPoint.X >= 3 && deepestPoint.Y >= 3)
                                    g.FillEllipse(myBrush, new Rectangle(deepestPoint.X - 3, deepestPoint.Y - 3, 7, 7));


                                    RectangleF rectf = new RectangleF(lobulusCentroid.X, lobulusCentroid.Y, 50, 30);
                                    if(lobulusCentroid.X >= 0 && lobulusCentroid.Y >= 0)
                                    g.DrawString($"L{_lobuleIndex}", new Font("Microsoft Sans Serif", 16),
                                        Brushes.Green,
                                        rectf);
                                }
                            }catch(Exception ex)
                            { }

                            pictureBox.Image = _bitmap;
                            pictureBox.Refresh();
                        }
                    }

                }catch(Exception ex)
                { }
            }
        }

        private bool FindFissures(List<Line> outerLines, List<Line> lines, bool againWithMilderedSettings = false, Fisur thisLobule = null, Fisur nextLobule = null)
        {
            //make multithread
            AddLogMessage("Find fissures");
            int outerLinesCount = outerLines.Count;

            List<PointF> intersecPoints = new List<PointF>();

            Fisur foundFisure = null;

            if (thisLobule == null && nextLobule == null)
            {
                _fisurList.Clear();
            }

            float distanceToEnlengthLine = 2;

            for (int i = 0; i < outerLinesCount; i++)
            {
                bool startPointIsOnThisLobule = false;

                if (foundFisure != null) break;

                Line line1 = outerLines[i];
                Point line1Start = line1.PointStart;
                Point line1End = line1.PointEnd;

                if (line1Start == line1End) continue;
                
                Line line1Reverse = line1.Reverse();

                PointF line1StartOuter = line1.GetLenghtenedPoint2(distanceToEnlengthLine);
                PointF line1EndOuter = line1Reverse.GetLenghtenedPoint2(distanceToEnlengthLine);

                if (line1StartOuter == line1EndOuter) continue;

                Line line1Check = new Line(new Point((int)line1StartOuter.X, (int)line1StartOuter.Y), new Point((int)line1EndOuter.X, (int)line1EndOuter.Y));

                if (line1Check.Length < line1.Length)
                {
                    line1StartOuter = line1Start;
                    line1EndOuter = line1End;
                }

                if (thisLobule != null)
                {
                    //check if startpoint is on thisLobule

                    foreach (var point in thisLobule.PointsOnLine)
                    {
                        var distancePoints = Line.GetDistanceBetweenTwoPoints(point, line1.RelatedPoint);
                        if (distancePoints < 5)
                        {
                            //just for debug
                            //using (Graphics g = Graphics.FromImage(_bitmap))
                            //{
                            //    SolidBrush myBrush = new SolidBrush(Color.BlueViolet);
                            //    g.FillEllipse(myBrush,
                            //        new Rectangle(line1.RelatedPoint.X - 3, line1.RelatedPoint.Y - 3, 7, 7));
                            //}

                            //pictureBox.Image = _bitmap;
                            //pictureBox.Refresh();

                            //AddLogMessage($"Found start point on line1.RelatedPoint: X: {line1.RelatedPoint.X} Y: {line1.RelatedPoint.Y}");

                            startPointIsOnThisLobule = true;
                            break;
                        }
                    }
                }

                if (thisLobule != null && startPointIsOnThisLobule == false) continue;

                int maxItoCheck = i + 2;

                if (againWithMilderedSettings == true && thisLobule != null)
                {
                    maxItoCheck = Math.Max(i - 20, 0);
                }

                for (int j = outerLinesCount - 1; j >= maxItoCheck; j--)
                {
                    bool endPointIsOnNextLobule = false;
                    Line line2 = outerLines[j];
                    Point line2Start = line2.PointStart;
                    Point line2End = line2.PointEnd;

                    if (line2Start == line2End) continue;

                    Line line2Reverse = line2.Reverse();

                    PointF line2StartOuter = line2.GetLenghtenedPoint2(distanceToEnlengthLine);
                    PointF line2EndOuter = line2Reverse.GetLenghtenedPoint2(distanceToEnlengthLine);

                    if (line2StartOuter == line2EndOuter) continue;

                    Line line2Check = new Line(new Point((int)line2StartOuter.X, (int)line2StartOuter.Y), new Point((int)line2EndOuter.X, (int)line2EndOuter.Y));


                    if (line2Check.Length < line2.Length)
                    {
                        line2StartOuter = line2Start;
                        line2EndOuter = line2End;
                    }

                    bool linesIntersect = false;
                    bool segmentsIntersec = false;
                    PointF intersectionPoint;
                    FindIntersection(line1StartOuter, line1EndOuter, line2StartOuter, line2EndOuter, out linesIntersect,
                        out segmentsIntersec, out intersectionPoint, out PointF x, out PointF y);

                    if (segmentsIntersec)
                    {

                        bool inOtherIntersectionArea = false;
                        foreach (PointF intersecPoint in intersecPoints)
                        {
                            if (Line.GetDistanceBetweenTwoPoints(intersecPoint, intersectionPoint) <
                                _intersectionPointRestrictedRadius)
                            {
                                inOtherIntersectionArea = true;
                                break;
                            }
                        }

                        if (inOtherIntersectionArea)
                        {
                            continue;
                        }

                        if (againWithMilderedSettings == false || nextLobule == null)
                        {
                            bool tooNearToStartPoint = false;
                            var distancePoints = Line.GetDistanceBetweenTwoPoints(new PointF(line1End.X, line1End.Y),
                                new PointF(line2Start.X, line2Start.Y));
                            if (distancePoints < 1)
                            {
                                tooNearToStartPoint = true;
                            }

                            if (tooNearToStartPoint)
                            {
                                continue;
                            }
                        }

                        //check line length, should be at least a minimum of _intersectionPointRestrictedRadius
                        if (againWithMilderedSettings == false || nextLobule == null)
                        {
                            double lineDistance = 0;
                            for (int n = i; n <= j; n++)
                            {
                                Line line3 = outerLines[n];
                                lineDistance += (line3.Length);
                            }

                            var distToBeSmaller = _intersectionPointRestrictedRadius;
                            if (thisLobule != null && nextLobule != null)
                            {
                                distToBeSmaller = distToBeSmaller / 15;
                            }

                            if (lineDistance < distToBeSmaller)
                            {
                                continue;
                            }
                        }

                        if (nextLobule != null)
                        {
                            //check if endpoint is on nextLobule

                            foreach (var point in nextLobule.PointsOnLine)
                            {
                                var distancePoints2 = Line.GetDistanceBetweenTwoPoints(point, line2.RelatedPoint);
                                if (distancePoints2 < 15)
                                {
                                    //should be circa offset ( + 133%)
                                    var distanceToLine1RelatedPoint = Line.GetDistanceBetweenTwoPoints(point, line1.RelatedPoint);
                                    if (distanceToLine1RelatedPoint <= (_offsetOuterLine * 2.33333) )
                                    {
                                        //just for debug
                                        //using (Graphics g = Graphics.FromImage(_bitmap))
                                        //{
                                        //    Pen myPen = new Pen(Color.Black);
                                        //    g.DrawRectangle(myPen, new Rectangle((int)line1.RelatedPoint.X - 3, (int)line1.RelatedPoint.Y - 3, 7, 7));
                                        //    g.DrawImage(_bitmap, new Point(0, 0));

                                        //    RectangleF rectf = new RectangleF(line1.RelatedPoint.X + 10, line1.RelatedPoint.Y, 100, 30);
                                        //    g.DrawString($"start point", new Font("Microsoft Sans Serif", 10), Brushes.Black, rectf);
                                        //}

                                        //pictureBox.Image = _bitmap;
                                        //pictureBox.Refresh();


                                        //using (Graphics g = Graphics.FromImage(_bitmap))
                                        //{
                                        //    Pen myPen = new Pen(Color.Black);
                                        //    g.DrawRectangle(myPen,
                                        //        new Rectangle((int) line2.RelatedPoint.X - 3,
                                        //            (int) line2.RelatedPoint.Y - 3, 7, 7));
                                        //    g.DrawImage(_bitmap, new Point(0, 0));

                                        //    RectangleF rectf = new RectangleF(line2.RelatedPoint.X + 10,
                                        //        line2.RelatedPoint.Y, 100, 30);
                                        //    g.DrawString($"end point", new Font("Microsoft Sans Serif", 10),
                                        //        Brushes.Black, rectf);
                                        //}

                                        //pictureBox.Image = _bitmap;
                                        //pictureBox.Refresh();

                                        //AddLogMessage($"Found end point on line2.RelatedPoint: X: {line2.RelatedPoint.X} Y: {line2.RelatedPoint.Y}");

                                        endPointIsOnNextLobule = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (thisLobule != null && nextLobule != null)
                        {
                            if (endPointIsOnNextLobule == false)
                            {
                                continue;
                            }
                        }


                        using (Graphics g = Graphics.FromImage(_bitmap))
                        {
                            SolidBrush myBrush = new SolidBrush(Color.Green);

                            if(intersectionPoint.X >= 3 && intersectionPoint.Y >= 3)
                            g.FillEllipse(myBrush,
                                new Rectangle((int)intersectionPoint.X - 3, (int)intersectionPoint.Y - 3, 7, 7));
                            g.DrawImage(_bitmap, new Point(0, 0));
                        }

                        pictureBox.Image = _bitmap;
                        pictureBox.Refresh();

                        AddLogMessage(
                            $"Intersection #{intersecPoints.Count + 1} point found on X: {intersectionPoint.X * _conversionFactor} Y: {intersectionPoint.Y * _conversionFactor}");
                        intersecPoints.Add(intersectionPoint);

                        Fisur fisur = new Fisur()
                        {
                            Start = line1.RelatedPoint,
                            End = line2.RelatedPoint,
                            IntersectionPoint = new Point((int)intersectionPoint.X, (int)intersectionPoint.Y),
                            HighestPoint = Line.Fraction(line1.RelatedPoint, line2.RelatedPoint, 0.5f),
                            Perimeter = 0,
                            Length = 0
                        };

                        if (thisLobule != null)
                        {
                            fisur.Id = thisLobule.Id;
                        }

                        AddLogMessage(
                            $"Fissure F{_fisurList.Count + 1} starts on X: {fisur.Start.X * _conversionFactor} Y: {fisur.Start.Y * _conversionFactor}, ends on X: {fisur.End.X * _conversionFactor} Y: {fisur.End.Y * _conversionFactor}");
                        
                        if (thisLobule != null && nextLobule != null)
                        {
                            foundFisure = fisur;
                        }
                        else
                        {
                            _fisurList.Add(fisur);
                        }

                        if (foundFisure != null) break;

                        //now jump to endPoint and go on from the endPoint
                        int newIndex = outerLines.IndexOf(line2);

                        if (newIndex > i)
                        {
                            i = newIndex;
                        }

                        break;
                    }
                }
            }

            if (thisLobule != null && foundFisure == null) return false;
            
            //kick out fissures with to small areas
            List<Fisur> validFisureList = new List<Fisur>();

            foreach (Fisur fisur in foundFisure == null ? _fisurList : new List<Fisur>{foundFisure})
            {
                List<Point> fisurCoordinates = new List<Point>();

                bool foundStart = false;
                foreach (Line line in lines)
                {
                    if (line.OuterLine == null)
                    {
                        continue;
                    }

                    if (foundStart == false)
                    {
                        if (Line.GetDistanceBetweenTwoPoints(new PointF(line.PointStart.X, line.PointStart.Y),
                            new PointF(fisur.Start.X, fisur.Start.Y)) < 5)
                        {
                            //start found
                            foundStart = true;
                            fisurCoordinates.Add(line.PointStart);
                        }
                    }
                    else
                    {
                        if (fisurCoordinates.Count > 2 &&
                            ((Line.GetDistanceBetweenTwoPoints(new PointF(line.PointStart.X, line.PointStart.Y),
                                 new PointF(fisur.End.X, fisur.End.Y)) < 5)
                            ||
                            (Line.GetDistanceBetweenTwoPoints(new PointF(line.PointEnd.X, line.PointEnd.Y),
                                 new PointF(fisur.End.X, fisur.End.Y)) < 5)))
                        {
                            //end found
                            foundStart = false;
                            fisurCoordinates.Add(fisur.HighestPoint);
                            fisurCoordinates.Add(fisur.End);
                            fisur.Area = Geometry.PolygonArea(fisurCoordinates) * _completeFactor;

                        }
                        else
                        {
                            //in fisure
                            fisurCoordinates.Add(line.PointStart);
                        }
                    }
                }

                if (fisur.Area > 10 || foundFisure != null)
                {
                    validFisureList.Add(fisur);
                }
            }

            if (foundFisure == null)
            {
                _fisurList = validFisureList;
            }
            else
            {
                _fisurList.AddRange(validFisureList);
            }

            return validFisureList.Count > 0;
        }

        private void DrawFissures(List<Line> lines)
        {
            int fisurIndex = 0;
            foreach (Fisur fisur in _fisurList)
            {
                try
                {
                    bool allowedToAddPerimeter = false;
                    bool allowedToAddPointsOnLine = false;

                    if (fisur.Perimeter <= 0) allowedToAddPerimeter = true;
                    if (fisur.PointsOnLine == null || fisur.PointsOnLine.Count == 0) allowedToAddPointsOnLine = true;

                    ++fisurIndex;
                    int fissurId = fisur.Id >= 0 ? fisur.Id : fisurIndex;

                    AddLogMessage($"Draw fissure F{fissurId}");
                    List<Point> fisurCoordinates = new List<Point>();

                    bool foundStart = false;
                    foreach (Line line in lines)
                    {
                        if (line.OuterLine == null)
                        {
                            continue;
                        }

                        if (foundStart == false)
                        {
                            if (line.PointStart == fisur.Start)
                            {
                                //start found
                                foundStart = true;

                                if(allowedToAddPerimeter) fisur.Perimeter += line.Length * _completeFactor;


                                if (line.PointStart.X > 0 && line.PointStart.Y > 0)
                                {
                                    if (allowedToAddPointsOnLine) fisur.PointsOnLine.Add(line.PointStart);
                                }

                                Point[] points = line.GetPoints();
                                if (points != null && points.Length > 0)
                                {
                                    foreach (var point in points.ToList())
                                    {
                                        if (point.X > 0 && point.Y > 0)
                                        {
                                            if (allowedToAddPointsOnLine) fisur.PointsOnLine.Add(point);
                                        }
                                    }
                                }

                                using (Graphics g = Graphics.FromImage(_bitmap))
                                {
                                    Pen mypen = new Pen(Color.Blue, 2);
                                    if(line.PointStart.X >= 0 && line.PointStart.Y >= 0 && line.PointEnd.X >= 0 && line.PointEnd.Y >= 0)
                                    g.DrawLine(mypen, line.PointStart.X, line.PointStart.Y, line.PointEnd.X,
                                        line.PointEnd.Y);
                                }

                                pictureBox.Image = _bitmap;
                                pictureBox.Refresh();

                                fisurCoordinates.Add(line.PointStart);
                            }
                        }
                        else
                        {
                            if (line.PointStart != fisur.End && line.PointEnd != fisur.End)
                            {
                                //in fisure

                                Point[] points = line.GetPoints();
                                if (points != null && points.Length > 0)
                                {
                                    foreach (var point in points.ToList())
                                    {
                                        if (point.X > 0 && point.Y > 0)
                                        {
                                            if (allowedToAddPointsOnLine) fisur.PointsOnLine.Add(point);
                                        }
                                    }
                                }

                                if (allowedToAddPerimeter) fisur.Perimeter += line.Length * _completeFactor;

                                double currentDistance =
                                    Line.GetDistanceBetweenTwoPoints(fisur.HighestPoint, line.PointEnd) *
                                    _completeFactor;
                                if (currentDistance > fisur.Length)
                                {
                                    fisur.Length = currentDistance;
                                    fisur.DeepestPoint = line.PointEnd;
                                }

                                using (Graphics g = Graphics.FromImage(_bitmap))
                                {
                                    Pen mypen = new Pen(Color.Blue, 2);
                                    if (line.PointStart.X >= 0 && line.PointStart.Y >= 0 && line.PointEnd.X >= 0 && line.PointEnd.Y >= 0)
                                        g.DrawLine(mypen, line.PointStart.X, line.PointStart.Y, line.PointEnd.X,
                                        line.PointEnd.Y);
                                }

                                pictureBox.Image = _bitmap;
                                pictureBox.Refresh();

                                fisurCoordinates.Add(line.PointStart);
                            }
                            else
                            {
                                //end found
                                foundStart = false;

                                if (fisur.End.X > 0 && fisur.End.Y > 0)
                                {
                                    if (allowedToAddPointsOnLine) fisur.PointsOnLine.Add(fisur.End);
                                }

                                if (allowedToAddPerimeter) fisur.Perimeter += line.Length * _completeFactor;
                                fisurCoordinates.Add(fisur.HighestPoint);
                                fisurCoordinates.Add(fisur.End);

                                if (allowedToAddPointsOnLine) fisur.PointsOnLine = fisurCoordinates;



                                fisur.Area = Geometry.PolygonArea(fisurCoordinates) * _completeFactor;
                                fisur.Straigtness = 1 - Math.Abs((fisur.Length / fisur.Perimeter) - 0.5);

                                //fisurCoordinates.Add(fisur.IntersectionPoint);

                                Point fisurCentroid = new Point();

                                if (fisurCoordinates.Count > 6)
                                {
                                    fisurCentroid = Geometry.FindCentroid(fisurCoordinates);
                                }

                                if (fisur.DeepestPoint.X > 0 && fisur.DeepestPoint.Y > 0)
                                {
                                    fisurCentroid = fisur.DeepestPoint;
                                }

                                if (fisurCentroid.X <= 0 && fisurCentroid.Y <= 0)
                                {
                                    fisurCentroid = fisur.Start;
                                }

                                if (fisurCentroid.X <= 0 && fisurCentroid.Y <= 0)
                                {
                                    fisurCentroid = fisur.End;
                                }

                                try
                                {
                                    using (Graphics g = Graphics.FromImage(_bitmap))
                                    {
                                        HatchBrush hBrush = new HatchBrush(
                                            HatchStyle.Percent30,
                                            Color.Blue,
                                            Color.Transparent);
                                        if (fisurCoordinates.Count >= 3)
                                        {
                                            g.FillPolygon(hBrush, fisurCoordinates.ToArray());
                                        }


                                        SolidBrush myBrush = new SolidBrush(Color.BlueViolet);

                                        if(fisurCentroid.X >= 3 && fisurCentroid.Y >= 3)
                                        g.FillEllipse(myBrush,
                                            new Rectangle(fisurCentroid.X - 3, fisurCentroid.Y - 3, 7, 7));

                                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                                        if(fisurCentroid.X >= -20 && fisurCentroid.Y >= 0)
                                        g.DrawLine(new Pen(Color.Blue, 2), fisurCentroid,
                                            new Point(fisurCentroid.X + 20, fisurCentroid.Y));
                                        RectangleF rectf = new RectangleF(fisurCentroid.X + 20, fisurCentroid.Y - 10,
                                            50,
                                            30);

                                        if (fisurCentroid.X >= -20 && fisurCentroid.Y >= 10)
                                            g.DrawString($"F{fissurId}", new Font("Microsoft Sans Serif", 16), Brushes.Blue,
                                            rectf);
                                    }
                                }catch(Exception ex)
                                { }

                                pictureBox.Image = _bitmap;
                                pictureBox.Refresh();

                                AddLogMessage($"Fissure F{fissurId} perimeter: {fisur.Perimeter}");
                                AddLogMessage($"Fissure F{fissurId} length: {fisur.Length}");
                                AddLogMessage($"Fissure F{fissurId} area: {fisur.Area}");
                                AddLogMessage($"Fissure F{fissurId} straigtness: {fisur.Straigtness}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    
                }
            }
        }

        private void AddFissureToDgv(bool manual)
        {
            _countFisur = _fisurList.Count;
            _countExistentFisur = 0;
            int k = 1;
            dgvResults.Rows.Clear();
            foreach (Fisur fisur in _fisurList)
            {
                if (fisur.NotExistent == false) _countExistentFisur++;

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvResults);
                row.Cells[0].Value = $"F{k++}";

                if (fisur.NotExistent)
                {
                    row.Cells[1].Value = "0";
                    row.Cells[2].Value = "0";
                    row.Cells[3].Value = "0";
                    row.Cells[4].Value = "0";
                    
                    _sumFisurLength += 0;
                    _sumFisurPerimeter += 0;
                    _sumFisurArea += 0;
                    _sumFisurStraigtness += 0;
                }
                else
                {
                    row.Cells[1].Value = fisur.Perimeter.ToString("F0");
                    row.Cells[2].Value = fisur.Length.ToString("F0");
                    row.Cells[3].Value = fisur.Area.ToString("F0");
                    row.Cells[4].Value = fisur.Straigtness.ToString("F3");

                    _sumFisurLength += fisur.Length;
                    _sumFisurPerimeter += fisur.Perimeter;
                    _sumFisurArea += fisur.Area;
                    _sumFisurStraigtness += fisur.Straigtness;
                }

                dgvResults.Rows.Add(row);
            }

            _averageFisurLength = _sumFisurLength / _countFisur;
            _averageFisurPerimeter = _sumFisurPerimeter / _countFisur;
            _averageFisurArea = _sumFisurArea / _countFisur;
            _averageFisurStraigtness = _sumFisurStraigtness / _countFisur;

            DataGridViewRow rowSum = new DataGridViewRow();
            rowSum.CreateCells(dgvResults);
            rowSum.Cells[0].Value = "F Σ";
            rowSum.Cells[1].Value = _sumFisurPerimeter.ToString("F0");
            rowSum.Cells[2].Value = _sumFisurLength.ToString("F0");
            rowSum.Cells[3].Value = _sumFisurArea.ToString("F0");
            rowSum.Cells[4].Value = _sumFisurStraigtness.ToString("F3");
            dgvResults.Rows.Add(rowSum);

            DataGridViewRow rowAverage = new DataGridViewRow();
            rowAverage.CreateCells(dgvResults);
            rowAverage.Cells[0].Value = "F Ø";
            rowAverage.Cells[1].Value = _averageFisurPerimeter.ToString("F0");
            rowAverage.Cells[2].Value = _averageFisurLength.ToString("F0");
            rowAverage.Cells[3].Value = _averageFisurArea.ToString("F0");
            rowAverage.Cells[4].Value = _averageFisurStraigtness.ToString("F3");
            dgvResults.Rows.Add(rowAverage);

            AddLogMessage($"Number of Fissures: {_countFisur}");

            AddLogMessage($"Fissure Σ perimeter: {_sumFisurPerimeter}");
            AddLogMessage($"Fissure Σ length: {_sumFisurLength}");
            AddLogMessage($"Fissure Σ area: {_sumFisurArea}");
            AddLogMessage($"Fissure Σ straigtness: {_sumFisurStraigtness}");

            AddLogMessage($"Fissure Ø perimeter: {_averageFisurPerimeter}");
            AddLogMessage($"Fissure Ø length: {_averageFisurLength}");
            AddLogMessage($"Fissure Ø area: {_averageFisurArea}");
            AddLogMessage($"Fissure Ø straigtness: {_averageFisurStraigtness}");
        }

        private void FindLobules()
        {
            AddLogMessage("Find Lobules");

            _lobuliList.Clear();

            if (_fisurList.Count > 0 && _coordinates.Count > 0)
            {
                Fisur firstLobulus = new Fisur
                {
                    Start = _coordinates[0],
                    End = _fisurList[0].DeepestPoint,
                    DeepestPoint = Line.Fraction(_coordinates[0], _fisurList[0].DeepestPoint, 0.5f)
                };

                _lobuliList.Add(firstLobulus);
            }

            for (int i = 0; i < _fisurList.Count - 1; i++)
            {
                Point lobuliStart = _fisurList[i].DeepestPoint;
                Point lobuliEnd = _fisurList[i + 1].DeepestPoint;
                Point lobuliDeepestPoint = Line.Fraction(lobuliStart, lobuliEnd, 0.5f);

                Fisur lobuli = new Fisur
                {
                    Start = lobuliStart,
                    End = lobuliEnd,
                    DeepestPoint = lobuliDeepestPoint
                };

                _lobuliList.Add(lobuli);
            }

            if (_fisurList.Count > 0 && _coordinates.Count > 0)
            {

                Fisur lastLobulus = new Fisur
                {
                    Start = _fisurList[_fisurList.Count - 1].DeepestPoint,
                    End = _coordinates[_coordinates.Count - 1],
                    DeepestPoint = Line.Fraction(_fisurList[_fisurList.Count - 1].DeepestPoint, _coordinates[_coordinates.Count - 1], 0.5f)
                };

                _lobuliList.Add(lastLobulus);
            }
        }

        private void DrawLobules(List<Line> lines)
        {
            int lobulusIndex = 0;
            foreach (Fisur lobulus in _lobuliList)
            {
                try
                {
                    ++lobulusIndex;
                    int lobulusId = lobulus.Id >= 0 ? lobulus.Id : lobulusIndex;
                    AddLogMessage($"Draw Lobule L{lobulusId}");
                    List<Point> lobulusCoordinates = new List<Point>();

                    bool foundStart = false;
                    foreach (Line line in lines)
                    {
                        if (foundStart == false)
                        {
                            if (line.PointStart == lobulus.Start)
                            {
                                //start found
                                foundStart = true;
                                lobulus.Perimeter += line.Length * _completeFactor;
                                lobulusCoordinates.Add(line.PointStart);

                                if (line.PointStart.X > 0 && line.PointStart.Y > 0)
                                {
                                    lobulus.PointsOnLine.Add(line.PointStart);
                                }

                                Point[] points = line.GetPoints();
                                if (points != null && points.Length > 0)
                                {
                                    foreach (var point in points.ToList())
                                    {
                                        if (point.X > 0 && point.Y > 0)
                                        {
                                            lobulus.PointsOnLine.Add(point);
                                        }
                                    }
                                }

                            }
                        }
                        else
                        {
                            if (line.PointStart != lobulus.End && line.PointEnd != lobulus.End)
                            {
                                //in lobulus

                                Point[] points = line.GetPoints();
                                if (points != null && points.Length > 0)
                                {
                                    foreach (var point in points.ToList())
                                    {
                                        if (point.X > 0 && point.Y > 0)
                                        {
                                            lobulus.PointsOnLine.Add(point);
                                        }
                                    }
                                }

                                lobulus.Perimeter += line.Length * _completeFactor;

                                double currentDistance =
                                    Line.GetDistanceBetweenTwoPoints(lobulus.DeepestPoint, line.PointEnd) *
                                    _completeFactor;
                                if (currentDistance > lobulus.Length)
                                {
                                    lobulus.Length = currentDistance;
                                    lobulus.HighestPoint = line.PointEnd;
                                }

                                lobulusCoordinates.Add(line.PointStart);
                            }
                            else
                            {
                                //end found

                                foundStart = false;


                                if (lobulus.End.X > 0 && lobulus.End.Y > 0)
                                {
                                    lobulus.PointsOnLine.Add(lobulus.End);
                                }

                                lobulus.Perimeter += line.Length * _completeFactor;
                                lobulusCoordinates.Add(lobulus.End);
                                lobulusCoordinates.Add(lobulus.DeepestPoint);
                                lobulus.Area = Geometry.PolygonArea(lobulusCoordinates) * _completeFactor;
                                lobulus.Straigtness = 1 - Math.Abs((lobulus.Length / lobulus.Perimeter) - 0.5);

                                Point lobulusCentroid = Geometry.FindCentroid(lobulusCoordinates);
                                Point deepestPoint = Line.Fraction(lobulus.Start, lobulus.End, 0.5f);

                                if (lobulus.DeepestPoint.X > 0 && lobulus.DeepestPoint.Y > 0)
                                {
                                    deepestPoint = lobulus.DeepestPoint;
                                }


                                try
                                {
                                    using (Graphics g = Graphics.FromImage(_bitmap))
                                    {
                                        if(lobulus.Start.X >= 0 && lobulus.Start.Y >= 0 && lobulus.End.X >= 0 && lobulus.End.Y >= 0)
                                        g.DrawLine(new Pen(Color.IndianRed, 1), lobulus.Start, lobulus.End);

                                        SolidBrush myBrush0 = new SolidBrush(Color.Green);

                                        if(lobulus.HighestPoint.X >= 3 && lobulus.HighestPoint.Y >= 3)
                                        g.FillEllipse(myBrush0,
                                            new Rectangle((int) lobulus.HighestPoint.X - 3,
                                                (int) lobulus.HighestPoint.Y - 3, 7, 7));

                                        if (lobulus.HighestPoint.X >= 0 && lobulus.HighestPoint.Y >= 0 && deepestPoint.X >= 0 && deepestPoint.Y >= 0)
                                            g.DrawLine(new Pen(Color.Green, 1) {DashStyle = DashStyle.Dot},
                                            lobulus.HighestPoint, deepestPoint);

                                        SolidBrush myBrush = new SolidBrush(Color.BlueViolet);

                                        if (deepestPoint.X >= 3 && deepestPoint.Y >= 3)
                                            g.FillEllipse(myBrush,
                                            new Rectangle(deepestPoint.X - 3, deepestPoint.Y - 3, 7, 7));

                                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                                        RectangleF rectf = new RectangleF(lobulusCentroid.X, lobulusCentroid.Y, 50, 30);
                                        if(lobulusCentroid.X >= 0 && lobulusCentroid.Y >= 0)
                                        g.DrawString($"L{lobulusId}", new Font("Microsoft Sans Serif", 16), Brushes.Red,
                                            rectf);
                                    }
                                }catch(Exception ex)
                                { }

                                pictureBox.Image = _bitmap;
                                pictureBox.Refresh();

                                AddLogMessage($"Lobule L{lobulusId} perimeter: {lobulus.Perimeter}");
                                AddLogMessage($"Lobule L{lobulusId} length: {lobulus.Length}");
                                AddLogMessage($"Lobule L{lobulusId} area: {lobulus.Area}");
                                AddLogMessage($"Lobule L{lobulusId} straigtness: {lobulus.Straigtness}");
                            }
                        }
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }

        private void AddLobulesToDgv()
        {
            //add empty row
            DataGridViewRow rowEmpty = new DataGridViewRow();
            rowEmpty.CreateCells(dgvResults);
            rowEmpty.Cells[0].Value = " ";
            rowEmpty.Cells[1].Value = " ";
            rowEmpty.Cells[2].Value = " ";
            rowEmpty.Cells[3].Value = " ";
            rowEmpty.Cells[4].Value = " ";
            dgvResults.Rows.Add(rowEmpty);

            _countLobulus = _lobuliList.Count;
            _countExistentLobulus = 0;
            int m = 1;
            foreach (Fisur lobulus in _lobuliList)
            {
                if (lobulus.NotExistent == false) _countExistentLobulus++;
                
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvResults);
                row.Cells[0].Value = $"L{m++}";

                if (lobulus.NotExistent)
                {
                    row.Cells[1].Value = "0";
                    row.Cells[2].Value = "0";
                    row.Cells[3].Value = "0";
                    row.Cells[4].Value = "0";

                    _sumLobulusLength += 0;
                    _sumLobulusPerimeter += 0;
                    _sumLobulusArea += 0;
                    _sumLobulusStraigtness += 0;
                }
                else
                {
                    row.Cells[1].Value = lobulus.Perimeter.ToString("F0");
                    row.Cells[2].Value = lobulus.Length.ToString("F0");
                    row.Cells[3].Value = lobulus.Area.ToString("F0");
                    row.Cells[4].Value = lobulus.Straigtness.ToString("F3");

                    _sumLobulusLength += lobulus.Length;
                    _sumLobulusPerimeter += lobulus.Perimeter;
                    _sumLobulusArea += lobulus.Area;
                    _sumLobulusStraigtness += lobulus.Straigtness;
                }
                
                dgvResults.Rows.Add(row);

                lobulus.dgvRowIndex = dgvResults.RowCount - 1;
            }

            _averageLobulusLength = _sumLobulusLength / _countLobulus;
            _averageLobulusPerimeter = _sumLobulusPerimeter / _countLobulus;
            _averageLobulusArea = _sumLobulusArea / _countLobulus;
            _averageLobulusStraigtness = _sumLobulusStraigtness / _countLobulus;

            DataGridViewRow rowSum2 = new DataGridViewRow();
            rowSum2.CreateCells(dgvResults);
            rowSum2.Cells[0].Value = "L Σ";
            rowSum2.Cells[1].Value = _sumLobulusPerimeter.ToString("F0");
            rowSum2.Cells[2].Value = _sumLobulusLength.ToString("F0");
            rowSum2.Cells[3].Value = _sumLobulusArea.ToString("F0");
            rowSum2.Cells[4].Value = _sumLobulusStraigtness.ToString("F3");
            dgvResults.Rows.Add(rowSum2);

            DataGridViewRow rowAverage2 = new DataGridViewRow();
            rowAverage2.CreateCells(dgvResults);
            rowAverage2.Cells[0].Value = "L Ø";
            rowAverage2.Cells[1].Value = _averageLobulusPerimeter.ToString("F0");
            rowAverage2.Cells[2].Value = _averageLobulusLength.ToString("F0");
            rowAverage2.Cells[3].Value = _averageLobulusArea.ToString("F0");
            rowAverage2.Cells[4].Value = _averageLobulusStraigtness.ToString("F3");
            dgvResults.Rows.Add(rowAverage2);

            foreach (Fisur lobulus in _lobuliList)
            {
                lobulus.PercentageOfCerebellum = (lobulus.Area / _cerebellumArea) * 100;
                dgvResults.Rows[lobulus.dgvRowIndex].Cells[5].Value = lobulus.PercentageOfCerebellum.ToString(FLOAT_ACCURACY);
                _sumLobulusPercentageOfCerebellumArea += lobulus.PercentageOfCerebellum;
            }

            _averageLobulusPercentageOfCerebellumArea = _sumLobulusPercentageOfCerebellumArea / _countLobulus;

            dgvResults.Rows[dgvResults.RowCount - 2].Cells[5].Value = _sumLobulusPercentageOfCerebellumArea.ToString(FLOAT_ACCURACY);
            dgvResults.Rows[dgvResults.RowCount - 1].Cells[5].Value = _averageLobulusPercentageOfCerebellumArea.ToString(FLOAT_ACCURACY);

            AddLogMessage($"Number of Lobules: {_countLobulus}");

            AddLogMessage($"Lobules Σ perimeter: {_sumLobulusPerimeter}");
            AddLogMessage($"Lobules Σ length: {_sumLobulusLength}");
            AddLogMessage($"Lobules Σ area: {_sumLobulusArea}");
            AddLogMessage($"Lobules Σ straigtness: {_sumLobulusStraigtness}");
            AddLogMessage($"Lobules Σ Percentage Area of Cerebellum Area: {_sumLobulusPercentageOfCerebellumArea}");

            AddLogMessage($"Lobules Ø perimeter: {_averageLobulusPerimeter}");
            AddLogMessage($"Lobules Ø length: {_averageLobulusLength}");
            AddLogMessage($"Lobules Ø area: {_averageLobulusArea}");
            AddLogMessage($"Lobules Ø straigtness: {_averageLobulusStraigtness}");
            AddLogMessage($"Lobules Ø Percentage Area of Cerebellum Area: {_averageLobulusPercentageOfCerebellumArea}");
        }

        private void DrawScale(ref Bitmap bitmapToUse, ref PictureBox pictureBoxToUse, ref Circle circleToUse, ref int xStart, ref int yStart)
        {
            // big picturebox
            // x 1024
            // y 768

            int oldY50Replacement = (int) (pictureBoxToUse.Image.Height * 0.065);
            int oldY10Replacement = (int)(oldY50Replacement / 5);
            int oldY5Replacement = (int)(oldY10Replacement / 2);
            int oldY65Replacement = oldY50Replacement + oldY10Replacement + oldY5Replacement;
            int oldY30Replacement = (int)(oldY50Replacement  * 3);
            int oldY15Replacement = (int)(oldY5Replacement * 3);

            int oldX10Replacement = (int)(pictureBoxToUse.Image.Width * 0.009765);
            int oldX3Replacement = (int)(oldX10Replacement * 3);

            AddLogMessage("Draw scale");
            try
            {
                using (Graphics g = Graphics.FromImage(bitmapToUse))
                {
                    int yToStart = (int) ((float) circleToUse.c.y + (float) circleToUse.r);

                    if ((yToStart + oldY50Replacement) > pictureBoxToUse.Image.Height)
                    {
                        yToStart = (int) ((float) circleToUse.c.y - (float) circleToUse.r) - oldY65Replacement;
                        if ((yToStart + oldY50Replacement) < oldY5Replacement)
                        {
                            yToStart = oldY10Replacement;
                        }
                    }

                    if (yToStart < circleToUse.c.y - circleToUse.r)
                    {
                        //scale is above circle
                        cropYStart = yToStart - oldY30Replacement;
                        cropYEnd = (int) circleToUse.c.y + (int) circleToUse.r + oldY30Replacement;
                    }
                    else
                    {
                        //scale is below circle
                        cropYStart = (int) circleToUse.c.y - (int) circleToUse.r - oldY30Replacement;
                        cropYEnd = yToStart + oldY50Replacement;
                    }

                    if (cropYStart < 0) cropYStart = 0;
                    if (cropYEnd > pictureBoxToUse.Image.Height) cropYEnd = pictureBoxToUse.Image.Height;


                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                    Pen pen = new Pen(Color.Black);
                    int yOfLine = (int) (yToStart + oldY15Replacement);

                    int xMinimumOfLine = (int) (circleToUse.c.x - circleToUse.r);

                    if (xStart > 0)
                    {
                        xMinimumOfLine = xStart;
                    }

                    if (yStart > 0)
                    {
                        yOfLine = 10; //bitmapToUse.Height - yStart;
                    }

                    cropXStart = xMinimumOfLine - oldX3Replacement;
                    if (cropXStart < 0) cropXStart = 0;


                    //if (xVerschiebung > 0)
                    //{
                    //    xMinimumOfLine = Math.Abs(xMinimumOfLine - xVerschiebung);
                    //}

                    int step = 100;

                    int diameterInMicrometer = (int) (2 * circleToUse.r * _completeFactor);
                    if (diameterInMicrometer <= 50)
                    {
                        step = 10;
                    }
                    else if (diameterInMicrometer <= 100)
                    {
                        step = 20;
                    }
                    else if (diameterInMicrometer <= 500)
                    {
                        step = 100;
                    }
                    else if (diameterInMicrometer <= 1000)
                    {
                        step = 200;
                    }
                    else if (diameterInMicrometer <= 2500)
                    {
                        step = 500;
                    }
                    else
                    {
                        step = 1000;
                    }

                    AddLogMessage($"Draw step every {step} µm");

                    int x = xMinimumOfLine;
                    for (int i = 0; i <= (int) Math.Ceiling(((double) diameterInMicrometer) / step); i++)
                    {
                       
                        x = xMinimumOfLine + (int) (step * i / _completeFactor);
                        if (x < 0 || yOfLine < 0) continue;
                        g.DrawLine(pen, x, yOfLine, x, yOfLine + 15);

                        RectangleF rectf2 = new RectangleF(((float) x + 2), ((float) yOfLine + 17), 80, 30);
                        g.DrawString((step * i).ToString("D") + $" {UNIT}", new Font("Microsoft Sans Serif", 10),
                            Brushes.Black, rectf2);

                    }

                    if (xMinimumOfLine >= 0 && yOfLine >= 0)
                    {
                        g.DrawLine(pen, (float) (xMinimumOfLine), yOfLine, (float) (x), yOfLine);
                    }

                    cropXEnd = x + 60;
                    if (cropXEnd > pictureBoxToUse.Image.Width) cropXEnd = pictureBoxToUse.Image.Width;
                }
            }catch(Exception ex)
            { }

            //give space for text
            //draw scale

            pictureBoxToUse.Image = bitmapToUse;
            pictureBoxToUse.Refresh();
        }

        private void RepositionImageAndDatapoints()
        {
            int shouldX = pictureBox.Image.Width / 2;
            int shouldY = pictureBox.Image.Height / 2;
            int isX = (int)_circle.c.x;
            int isY = (int)_circle.c.y;

            int deltaX = shouldX - isX;
            int deltaY = shouldY - isY;

            if (deltaX != 0 || deltaY != 0)
            {
                if (_circle.c.x + deltaX > 10 && _circle.c.x + deltaX < pictureBox.Image.Width - 10 &&
                    _circle.c.y + deltaY > 10 && _circle.c.y + deltaY < pictureBox.Image.Height - 10)
                {

                    AddLogMessage($"Center image by moving x: {deltaX} y: {deltaY}");
                    List<Point> newCoordinates = new List<Point>();

                    foreach (Point coordinate in _coordinates)
                    {
                        int newX = coordinate.X + deltaX;
                        int newY = coordinate.Y + deltaY;

                        newCoordinates.Add(new Point(newX, newY));
                    }
                    
                    _coordinates.Clear();
                    _coordinates = newCoordinates;

                    _circle.c.x += deltaX;
                    _circle.c.y += deltaY;

                    using (Graphics g = Graphics.FromImage(_bitmap))
                    {
                        g.DrawImage(_bitmap, deltaX, deltaY);
                    }

                    pictureBox.Image = _bitmap;
                    pictureBox.Refresh();
                }
            }
        }

        private void DoMinimumCircle()
        {
            AddLogMessage("Generate minimum circle");
            _circle = Geometry.MakeCircle(_coordinates);
            RepositionImageAndDatapoints();

            AddLogMessage($"minimum circle center is on X: {_circle.c.x * _conversionFactor} Y: {_circle.c.y * _conversionFactor}");

            PointF center = new PointF((float) _circle.c.x, (float) _circle.c.y);
            _center = new Point((int)center.X, (int)center.Y);

            float radius = (float) _circle.r;
            if (radius > 0)
            {
                _minCircleRadius = radius * _completeFactor;
                AddLogMessage($"min circle radius: {_minCircleRadius.ToString(FLOAT_ACCURACY)} {UNIT}");
                lblMinRadius.Text = $"min circle radius: {_minCircleRadius.ToString(FLOAT_ACCURACY)} {UNIT}";

                _minCircleArea = Math.PI * (_minCircleRadius * _minCircleRadius);
                AddLogMessage($"min circle area: {_minCircleArea.ToString(FLOAT_ACCURACY)} {UNIT}²");
                lblMinArea.Text = $"min circle area: {_minCircleArea.ToString(FLOAT_ACCURACY)} {UNIT}²";

                _minCirclePerimet = 2 * Math.PI * radius * _completeFactor;
                AddLogMessage($"min circle perimeter: {_minCirclePerimet.ToString(FLOAT_ACCURACY)} {UNIT}");
                lblMinPeri.Text = $"min circle perimeter: {_minCirclePerimet.ToString(FLOAT_ACCURACY)} {UNIT}";

                AddLogMessage("Draw minimum circle");
                using (Graphics g = Graphics.FromImage(_bitmap))
                {
                    Pen pen = new Pen(Color.Magenta, 2);
                    float x1 = center.X - radius;
                    float y1 = center.Y - radius;

                    if (x1 >= 0 && y1 >= 0)
                    {
                        g.DrawEllipse(pen, x1, y1, 2 * radius, 2 * radius);
                    }

                    g.DrawImage(_bitmap, 0, 0);
                }

                //_bitmap = new Bitmap(pictureBox.Image);
                pictureBox.Image = _bitmap;
                pictureBox.Refresh();
            }
        }

        private void DoConvexHull()
        {
            AddLogMessage("Generate convex hull polygon");
            List<Point> conexHullPoints = Geometry.MakeConvexHull(_coordinates);

            _convexHullArea = Geometry.PolygonArea(conexHullPoints) * _completeFactor;
            AddLogMessage($"convex hull area: {_convexHullArea.ToString(FLOAT_ACCURACY)} {UNIT}²");
            lblHullArea.Text = $"convex hull area: {_convexHullArea.ToString(FLOAT_ACCURACY)} {UNIT}²";

            double distance = 0;
            for (int i = 1; i < conexHullPoints.Count; i++)
            {
                Point thisPoint = conexHullPoints[i];
                Point lastPoint = conexHullPoints[i - 1];

                distance += Line.GetDistanceBetweenTwoPoints(thisPoint, lastPoint);
            }

            _convexHullPerimet = distance * _completeFactor;
            AddLogMessage($"convex hull perimeter: {_convexHullPerimet.ToString(FLOAT_ACCURACY)} {UNIT}");
            lblHullPeri.Text = $"convex hull perimeter: {_convexHullPerimet.ToString(FLOAT_ACCURACY)} {UNIT}";

            AddLogMessage("Draw convex hull");

            using (Graphics g = Graphics.FromImage(_bitmap))
            {
                if (conexHullPoints.Count >= 3)
                {
                    g.FillPolygon(new SolidBrush(Color.Khaki), conexHullPoints.ToArray());
                }

                g.DrawImage(_bitmap, 0, 0);
            }

            //_bitmap = new Bitmap(pictureBox.Image);
            pictureBox.Image = _bitmap;
            pictureBox.Refresh();
        }

        private void Flip()
        {
            AddLogMessage("Flip");
            if (_bitmap != null)
            {
                pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipY);
                pictureBox.Image = _bitmap;
                pictureBox.Refresh();
            }

            flipped = !flipped;
            FlipCoordinates();
        }

        private void FlipCoordinates()
        {
            int maxX = pictureBox.Size.Width;

            List<Point> flippedCoordinates = new List<Point>();
            foreach (Point point in _coordinates)
            {
                Point flippedPoint = new Point((maxX - point.X), point.Y);

                flippedCoordinates.Add(flippedPoint);
            }

            _coordinates = flippedCoordinates;

            DrawCoordinates();
        }

        private void DrawCoordinates()
        {
            if (_coordinates != null)
            {
                Point? lastPoint = null;
                foreach (Point coordinate in _coordinates)
                {
                    try
                    {
                        if (lastPoint.HasValue)
                        {
                            using (Graphics g = Graphics.FromImage(pictureBox.Image))
                            {
                                Pen mypen = new Pen(Color.Red, 5);
                                if(lastPoint.Value.X >= 0 && lastPoint.Value.Y >= 0 && coordinate.X >= 0 && coordinate.Y >= 0)
                                g.DrawLine(mypen, lastPoint.Value.X, lastPoint.Value.Y, coordinate.X, coordinate.Y);
                                pictureBox.Invalidate();
                            }

                        }

                        lastPoint = coordinate;
                    }catch(Exception ex)
                    { }
                }
            }
        }
        

        private void nud_ValueChanged(object sender, EventArgs e)
        {
            _offsetOuterLine = (int) nudOffset.Value;
            _blurredFactor = (int) nudBlurredFactor.Value;
            _intersectionPointRestrictedRadius = (int) nudIntersectionResArea.Value;
            Reset();
        }

        private bool draw = false;

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            draw = false;
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            draw = true;
            Draw(e.X, e.Y);
        }

        private void Draw(int x, int y)
        {
            if (draw)
            {
                Point newPoint = new Point(x, y);

                int indexOfLastPoint = _coordinates.Count;
                if (indexOfLastPoint == 0)
                {
                    _coordinates.Add(newPoint);
                }
                else
                {
                    Point oldPoint = _coordinates[indexOfLastPoint - 1];

                    if (Line.GetDistanceBetweenTwoPoints(newPoint, oldPoint) > _blurredFactor)
                    {
                        _coordinates.Add(newPoint);

                        try
                        {
                            using (Graphics g = Graphics.FromImage(_bitmap))
                            {
                                Pen mypen = new Pen(Color.Red, 5);
                                if(oldPoint.X >= 0 && oldPoint.Y >= 0 && newPoint.X >= 0 && newPoint.Y >= 0)
                                g.DrawLine(mypen, oldPoint.X, oldPoint.Y, newPoint.X, newPoint.Y);
                                pictureBox.Invalidate();
                            }
                        }catch(Exception ex)
                        { }

                        AddLogMessage($"Addet datapoint on X:{newPoint.X * _conversionFactor} Y:{newPoint.Y * _conversionFactor}");
                    }
                }
            }
        }

        private bool analyzed = false;

        private void Analyze(bool justMeasure = false)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_filePath);
            string resultPath = Path.Combine(Path.GetDirectoryName(_filePath), fileNameWithoutExtension);

            //create result dir
            AddLogMessage($"Create result directory: {resultPath}");
            Utils.CreateIfNotExists(resultPath);

            string fileName = Path.GetFileNameWithoutExtension(_filePath);
            DoAScreenshot(Path.Combine(resultPath, $"{fileName} - Markings.png"));

            _markedPicture = new Bitmap(pictureBox.Image);

            AddLogMessage($"Start analyzing");
            _cerebellumArea = 0;
            _cerebellumPerimet = 0;
            _minCircleArea = 0;
            _minCirclePerimet = 0;
            _minCircleRadius = 0;
            _convexHullArea = 0;
            _convexHullPerimet = 0;
            _solidity = 0;
            _circularity = 0;
            _convexity = 0;

            if (_coordinates.Count > 7)
            {
                CheckIfCoordinatesNeedReverse();

                if (!justMeasure)
                {
                    CutAndSpin(ref _coordinates, ref _bitmap, ref pictureBox);
                }

                Measure();

                analyzed = true;
            }
            else
            {
                //MessageBox.Show($"not enough datapoints found (current: {_coordinates.Count})");
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    $"not enough datapoints found (current: {_coordinates.Count})");

                if (_waitTime < 5)
                {
                    Close();
                }
            }
        }

        private void CheckIfCoordinatesNeedReverse()
        {
            var enlargedPolygon = GetEnlargedPolygon(_coordinates.Select(p => new PointF(p.X, p.Y)).ToList(), -10);

            if (enlargedPolygon.Count < 3)
            {
                _coordinates.Reverse();
                return;
            }

            List<Point> conexHullPointsCoordinates = Geometry.MakeConvexHull(_coordinates);
            List<Point> enlargedPointList = enlargedPolygon.Select(p => new Point((int)p.X, (int)p.Y)).ToList();
            List<Point> conexHullPointsEnlarged = Geometry.MakeConvexHull(enlargedPointList);

            var convexHullAreaCoordinates = Geometry.PolygonArea(conexHullPointsCoordinates);
            var convexHullAreaEnlarged = Geometry.PolygonArea(conexHullPointsEnlarged);

            if(convexHullAreaEnlarged < convexHullAreaCoordinates)
            {
                _coordinates.Reverse();
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            Analyze();
        }

        private bool flipped = false;

        private void btnFlip_Click(object sender, EventArgs e)
        {
            if (analyzed == false)
            {
                if (flipped)
                {
                    Reset(true);
                    FlipCoordinates();
                }
                else
                {
                    Reset(true);
                    Flip();
                }
            }
            else
            {
                pictureBox.Image = new Bitmap(_bitmap.Width, _bitmap.Height);
                pictureBox.Refresh();

                FlipCoordinates();
                Analyze();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset(bool withoutClearingCoordinates = false)
        {
            AddLogMessage("Reset");
            if(withoutClearingCoordinates == false)_coordinates = new List<Point>();
            LoadImage(_filePath, false);
        }

        /// <summary>
        /// Rotates the specified point around another center.
        /// </summary>
        /// <param name="center">Center point to rotate around.</param>
        /// <param name="pt">Point to rotate.</param>
        /// <param name="degree">Rotation degree. A value between 1 to 360.</param>
        private static Point RotatePoint(Point center, Point pt, float degree)
        {
            double x1, x2, y1, y2;
            x1 = center.X;
            y1 = center.Y;
            x2 = pt.X;
            y2 = pt.Y;
            double distance = Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
            degree *= (float)(Math.PI / 180);
            double x3, y3;
            x3 = distance * Math.Cos(degree) + x1;
            y3 = distance * Math.Sin(degree) + y1;
            return new Point((int)x3, (int)y3);
        }

        private void btnResetOptions_Click(object sender, EventArgs e)
        {
            AddLogMessage("Reset options");
            _offsetOuterLine = DEFAULT_OFFSET;
            _blurredFactor = DEFAULT_BLURRED_FACTOR;
            _intersectionPointRestrictedRadius = DEFAULT_INTERSECTION_POINT_RESTRICTED_RADIUS;
            _align = DEFAULT_ALIGN;

            nudOffset.Value = DEFAULT_OFFSET;
            nudBlurredFactor.Value = DEFAULT_BLURRED_FACTOR;
            nudIntersectionResArea.Value = DEFAULT_INTERSECTION_POINT_RESTRICTED_RADIUS;
            cheAutoAlign.Checked = DEFAULT_ALIGN;

            Reset();
        }

        private void cheAutoAlign_CheckedChanged(object sender, EventArgs e)
        {
            cheAutoAlign.CheckedChanged -= cheAutoAlign_CheckedChanged;
            _align = !_align;
            cheAutoAlign.Checked = _align;
            Reset();
            cheAutoAlign.CheckedChanged += cheAutoAlign_CheckedChanged;
        }

        private void nudConversionPixel_ValueChanged(object sender, EventArgs e)
        {
            _conversionFactor = ((double)100 / (double)nudConversionPixel.Value);
        }

        private void btnSafe_Click(object sender, EventArgs e)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_filePath);
            string resultPath = Path.Combine(Path.GetDirectoryName(_filePath), fileNameWithoutExtension);

            //create result dir
            AddLogMessage($"Create result directory: {resultPath}");
            Utils.CreateIfNotExists(resultPath);

            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = $"PNG|*.png";
                dialog.Title = "Safe a screenshot";
                string fileName = Path.GetFileNameWithoutExtension(_filePath);
                dialog.FileName = $"{fileName} - Screenshot.png";
                dialog.InitialDirectory = resultPath;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    DoAScreenshot(dialog.FileName);
                }
            }
        }

        private void DoAScreenshot(string filePath)
        {
            if (filePath.ToLower().EndsWith(".png") == false)
            {
                filePath += ".png";
            }

            Bitmap bmp = new Bitmap(Width, Height);
            DrawToBitmap(bmp, new Rectangle(0, 0, Width, Height));
            bmp.Save(filePath, ImageFormat.Png);

            AddLogMessage($"Successfully safed screenshot to {filePath}");
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            //SafeResultsLocal();
            UploadResults();
        }

        private void UploadResults()
        {
            using (var b = new Bitmap(pictureBox.Image.Width, pictureBox.Image.Height))
            {
                b.SetResolution(pictureBox.Image.HorizontalResolution, pictureBox.Image.VerticalResolution);

                using (var g = Graphics.FromImage(b))
                {
                    g.Clear(Color.White);
                    g.DrawImageUnscaled(pictureBox.Image, 0, 0);
                }

                _analyzedPicture = new Bitmap(b);
            }

            string note = string.Empty;
            if (string.IsNullOrEmpty(_note))
            {
                DialogResult dialogResultAnotation = MessageBoxAutocloseWithButtons.Show(_waitTime, true, "No", "Yes",
                    "Would you like to add an annotation?");
                //MessageBox.Show("Should this measurement be included in the analysis?", "Include in analysis", MessageBoxButtons.YesNo);
                if (dialogResultAnotation == DialogResult.Cancel)
                {
                    using (FormNote formNote = new FormNote())
                    {

                        if (formNote.ShowDialog(this) == DialogResult.OK)
                        {
                            note = formNote.annotation;
                        }
                        formNote.Dispose();
                    }
                }
            }
            else
            {
                note = _note;
            }

            FormPleaseWait pleaseWait = new FormPleaseWait("uploading data");
            pleaseWait.Owner = this;
            pleaseWait.StartPosition = FormStartPosition.CenterParent;
            pleaseWait.Show(this);
            int x = this.DesktopBounds.Left + (this.Width - pleaseWait.Width) / 2;
            int y = this.DesktopBounds.Top + (this.Height - pleaseWait.Height) / 2;
            pleaseWait.SetDesktopLocation(x, y);

            Cut cut = new Cut
            {
                Age = Convert.ToInt16((string.IsNullOrEmpty(_age) ? "0": _age)),
                Genotype = (string.IsNullOrEmpty(_genotype) ? "unknown" : _genotype),
                Animal = (string.IsNullOrEmpty(_animalNumber) ? "unknown" : _animalNumber),
                CutIdentifier = (string.IsNullOrEmpty(_cutIdentifier) ? "unknown" : _cutIdentifier),
                Method = (string.IsNullOrEmpty(_method) ? "unknown" : _method),
                DateMeasurement = DateTime.Now,
                DateStaining = _dateStaining,
                ZoomFactor = (float) Convert.ToDouble((string.IsNullOrEmpty(_zoomFactor) ? "0,0" : _zoomFactor)),
                Layer = (string.IsNullOrEmpty(_layer)? "unknown" : _layer),
                Note = string.Empty //note - dont send note here, use procedure for better logging!                
            };

            MySQL mysql = new MySQL();
            mysql._waitTime = _waitTime;

            mysql.CreateCut(cut);

            if (cut.Id <= 0)
            {
                MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                    $"Something went wrong. Please try it again.");
                //MessageBox.Show("Something went wrong. Please try it again.");
                if (_waitTime < 5)
                {
                    Close();
                }
                else
                {
                    return;
                }
            }

            if (string.IsNullOrEmpty(note) == false)
            {
                mysql.CreateNote(cut.Id, Environment.MachineName, note);
            }


            DialogResult dialogResult = MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Yes", "No",
                $"Should this measurement be included in the analysis?");
            //MessageBox.Show("Should this measurement be included in the analysis?", "Include in analysis", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Cancel)
            {
                mysql.ExcludeFromAnalytics(cut.Id);
            }

            mysql.CreateOption(new Option() { CutId = cut.Id, Key = "original file name", Unit = "", Value = Path.GetFileName(_filePath) });
            mysql.CreateOption(new Option() { CutId = cut.Id, Key = "software version", Unit = "", Value = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion });
            mysql.CreateOption(new Option() { CutId = cut.Id, Key = "minimal distance between 2 points", Unit = "pixel", Value = nudBlurredFactor.Value.ToString("F0") });
            mysql.CreateOption(new Option() { CutId = cut.Id, Key = "offset", Unit = "pixel", Value = nudOffset.Value.ToString("F0") });
            mysql.CreateOption(new Option() { CutId = cut.Id, Key = "intersection point restriction area", Unit = "pixel", Value = nudIntersectionResArea.Value.ToString("F0") });
            mysql.CreateOption(new Option() { CutId = cut.Id, Key = $"conversion factor (100 {UNIT} = x pixel)", Unit = "", Value = nudConversionPixel.Value.ToString("F0") });
            mysql.CreateOption(new Option() { CutId = cut.Id, Key = $"PC Name", Unit = "", Value = Environment.MachineName });
            mysql.CreateOption(new Option() { CutId = cut.Id, Key = $"PC Resolution", Unit = "pixel", Value = $"{Screen.FromControl(this).Bounds.Width} x {Screen.FromControl(this).Bounds.Height}"});

            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "cerebellum area", Identifier = null, ResultType = "global", Unit = UNIT_AREA, Value = _cerebellumArea.ToString(FLOAT_ACCURACY)});
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "cerebellum perimeter", Identifier = null, ResultType = "global", Unit = UNIT , Value = _cerebellumPerimet.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "minimal circle area", Identifier = null, ResultType = "global", Unit = UNIT_AREA, Value = _minCircleArea.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "minimal circle perimeter", Identifier = null, ResultType = "global", Unit = UNIT, Value = _minCirclePerimet.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "minimal circle radius", Identifier = null, ResultType = "global", Unit = UNIT, Value = _minCircleRadius.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "convex hull area", Identifier = null, ResultType = "global", Unit = UNIT_AREA, Value = _convexHullArea.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "convex hull perimeter", Identifier = null, ResultType = "global", Unit = UNIT, Value = _convexHullPerimet.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "solidity", Identifier = null, ResultType = "global", Unit = "", Value = _solidity.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "circularity", Identifier = null, ResultType = "global", Unit = "", Value = _circularity.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "convexity", Identifier = null, ResultType = "global", Unit = "", Value = _convexity.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "ΔSM", Identifier = null, ResultType = "global", Unit = UNIT, Value = _deltaSM.ToString(FLOAT_ACCURACY) });


            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter area", Identifier = null, ResultType = "white matter", Unit = UNIT_AREA, Value = _markArea.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter percentage", Identifier = null, ResultType = "white matter", Unit = "%", Value = _percentMarkAreaOfCerebellumgArea.ToString(FLOAT_ACCURACY) });

            //new
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter perimeter", Identifier = null, ResultType = "white matter", Unit = UNIT, Value = _markPerimeter.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter minimal circle radius", Identifier = null, ResultType = "white matter", Unit = UNIT, Value = _markMinCircleRadius.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter minimal circle area", Identifier = null, ResultType = "white matter", Unit = UNIT_AREA, Value = _markMinCircleArea.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter minimal circle perimeter", Identifier = null, ResultType = "white matter", Unit = UNIT, Value = _markMinCirclePerimet.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter convex hull area", Identifier = null, ResultType = "white matter", Unit = UNIT_AREA, Value = _markConvexHullArea.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter convex hull perimeter", Identifier = null, ResultType = "white matter", Unit = UNIT, Value = _markConvexHullPerimet.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter convexity", Identifier = null, ResultType = "white matter", Unit = "", Value = _markConvexity.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter solidity", Identifier = null, ResultType = "white matter", Unit = "", Value = _markSolidity.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter circularity", Identifier = null, ResultType = "white matter", Unit = "", Value = _markCircularity.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "white matter center distance to cerebellum center", Identifier = null, ResultType = "white matter", Unit = UNIT, Value = _markDistanceToCerebellumCenter.ToString(FLOAT_ACCURACY) });



            short f = 0;
            foreach (Fisur fisur in _fisurList)
            {
                f++;

                Result resultPerimeter = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "perimeter",
                    Unit = UNIT,
                    Value = fisur.Perimeter.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultPerimeter);

                Result resultLength = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "length",
                    Unit = UNIT,
                    Value = fisur.Length.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultLength);

                Result resultArea = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "area",
                    Unit = UNIT_AREA,
                    Value = fisur.Area.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultArea);

                Result resultStraigtness = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "straigtness",
                    Unit = "",
                    Value = fisur.Straigtness.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultStraigtness);

                //new

                Result resultAngleToX = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "angle to x-axis",
                    Unit = "°",
                    Value = fisur.AngleToX.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultAngleToX);

                Result resultAngleToNextF = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "angle to next fissure",
                    Unit = "°",
                    Value = fisur.AngleToNextLorF.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultAngleToNextF);

                Result resultDistanceToNextF = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "distance to next fissure",
                    Unit = UNIT,
                    Value = fisur.DistanceToNextLorF.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultDistanceToNextF);

                Result resultCircularity = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "circularity",
                    Unit = "",
                    Value = fisur.Circularity.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultCircularity);

                Result resultConvexity = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "convexity",
                    Unit = "",
                    Value = fisur.Convexity.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultConvexity);

                Result resultSolidity = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "solidity",
                    Unit = "",
                    Value = fisur.Solidity.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultSolidity);

                Result resultMinCircleRadius = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "min circle radius",
                    Unit = UNIT,
                    Value = fisur.MinCircleRadius.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultMinCircleRadius);

                Result resultMinCircleArea = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "min circle area",
                    Unit = UNIT_AREA,
                    Value = fisur.MinCircleArea.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultMinCircleArea);

                Result resultMinCirclePerimet = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "min circle perimeter",
                    Unit = UNIT,
                    Value = fisur.MinCirclePerimet.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultMinCirclePerimet);

                Result resultConvexHullArea = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "convex hull area",
                    Unit = UNIT_AREA,
                    Value = fisur.ConvexHullArea.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultConvexHullArea);

                Result resultConvexHullPerimet = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "convex hull perimeter",
                    Unit = UNIT,
                    Value = fisur.ConvexHullPerimet.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultConvexHullPerimet);

                Result resultDistanceToCerebellumCenter = new Result
                {
                    CutId = cut.Id,
                    ResultType = "fissure",
                    Identifier = f.ToString("F0"),
                    Key = "distance to cerebellum center",
                    Unit = UNIT,
                    Value = fisur.DistanceToCerebellumCenter.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultDistanceToCerebellumCenter);
            }

            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "number of fissures", Identifier = null, ResultType = "fissure", Unit = "", Value = _countExistentFisur.ToString("F0") });

            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "perimeter", Identifier = "Σ", ResultType = "fissure", Unit = UNIT, Value = _sumFisurPerimeter.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "length", Identifier = "Σ", ResultType = "fissure", Unit = UNIT, Value = _sumFisurLength.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "area", Identifier = "Σ", ResultType = "fissure", Unit = UNIT_AREA, Value = _sumFisurArea.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "straigtness", Identifier = "Σ", ResultType = "fissure", Unit = "", Value = _sumFisurStraigtness.ToString(FLOAT_ACCURACY) });

            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "perimeter", Identifier = "Ø", ResultType = "fissure", Unit = UNIT, Value = _averageFisurPerimeter.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "length", Identifier = "Ø", ResultType = "fissure", Unit = UNIT, Value = _averageFisurLength.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "area", Identifier = "Ø", ResultType = "fissure", Unit = UNIT_AREA, Value = _averageFisurArea.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "straigtness", Identifier = "Ø", ResultType = "fissure", Unit = "", Value = _averageFisurStraigtness.ToString(FLOAT_ACCURACY) });

            short l = 0;
            foreach (Fisur lobulus in _lobuliList)
            {
                l++;

                Result resultPerimeter = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "perimeter",
                    Unit = UNIT,
                    Value = lobulus.Perimeter.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultPerimeter);

                Result resultLength = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "length",
                    Unit = UNIT,
                    Value = lobulus.Length.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultLength);

                Result resultArea = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "area",
                    Unit = UNIT_AREA,
                    Value = lobulus.Area.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultArea);

                Result resultStraigtness = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "straigtness",
                    Unit = "",
                    Value = lobulus.Straigtness.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultStraigtness);

                Result resultInCerebellumg = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "percentage area in cerebellum area",
                    Unit = "%",
                    Value = lobulus.PercentageOfCerebellum.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultInCerebellumg);

                //new

                Result resultAngleToX = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "angle to x-axis",
                    Unit = "°",
                    Value = lobulus.AngleToX.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultAngleToX);

                Result resultAngleToNextF = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "angle to next lobule",
                    Unit = "°",
                    Value = lobulus.AngleToNextLorF.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultAngleToNextF);

                Result resultDistanceToNextF = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "distance to next lobule",
                    Unit = UNIT,
                    Value = lobulus.DistanceToNextLorF.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultDistanceToNextF);

                Result resultCircularity = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "circularity",
                    Unit = "",
                    Value = lobulus.Circularity.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultCircularity);

                Result resultConvexity = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "convexity",
                    Unit = "",
                    Value = lobulus.Convexity.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultConvexity);

                Result resultSolidity = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "solidity",
                    Unit = "",
                    Value = lobulus.Solidity.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultSolidity);

                Result resultMinCircleRadius = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "min circle radius",
                    Unit = UNIT,
                    Value = lobulus.MinCircleRadius.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultMinCircleRadius);

                Result resultMinCircleArea = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "min circle area",
                    Unit = UNIT_AREA,
                    Value = lobulus.MinCircleArea.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultMinCircleArea);

                Result resultMinCirclePerimet = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "min circle perimeter",
                    Unit = UNIT,
                    Value = lobulus.MinCirclePerimet.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultMinCirclePerimet);

                Result resultConvexHullArea = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "convex hull area",
                    Unit = UNIT_AREA,
                    Value = lobulus.ConvexHullArea.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultConvexHullArea);

                Result resultConvexHullPerimet = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "convex hull perimeter",
                    Unit = UNIT,
                    Value = lobulus.ConvexHullPerimet.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultConvexHullPerimet);

                Result resultDistanceToCerebellumCenter = new Result
                {
                    CutId = cut.Id,
                    ResultType = "lobule",
                    Identifier = l.ToString("F0"),
                    Key = "distance to cerebellum center",
                    Unit = UNIT,
                    Value = lobulus.DistanceToCerebellumCenter.ToString(FLOAT_ACCURACY)
                };
                mysql.CreateResult(resultDistanceToCerebellumCenter);
            }

            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "number of lobules", Identifier = null, ResultType = "lobule", Unit = "", Value = _countExistentLobulus.ToString("F0") });

            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "perimeter", Identifier = "Σ", ResultType = "lobule", Unit = UNIT, Value = _sumLobulusPerimeter.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "length", Identifier = "Σ", ResultType = "lobule", Unit = UNIT, Value = _sumLobulusLength.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "area", Identifier = "Σ", ResultType = "lobule", Unit = UNIT_AREA, Value = _sumLobulusArea.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "straigtness", Identifier = "Σ", ResultType = "lobule", Unit = "", Value = _sumLobulusStraigtness.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "percentage area in cerebellum area", Identifier = "Σ", ResultType = "lobule", Unit = "%", Value = _sumLobulusPercentageOfCerebellumArea.ToString(FLOAT_ACCURACY) });

            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "perimeter", Identifier = "Ø", ResultType = "lobule", Unit = UNIT, Value = _averageLobulusPerimeter.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "length", Identifier = "Ø", ResultType = "lobule", Unit = UNIT, Value = _averageLobulusLength.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "area", Identifier = "Ø", ResultType = "lobule", Unit = UNIT_AREA, Value = _averageLobulusArea.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "straigtness", Identifier = "Ø", ResultType = "lobule", Unit = "", Value = _averageLobulusStraigtness.ToString(FLOAT_ACCURACY) });
            mysql.CreateResult(new Result() { CutId = cut.Id, Key = "percentage area in cerebellum area", Identifier = "Ø", ResultType = "lobule", Unit = "%", Value = _averageLobulusPercentageOfCerebellumArea.ToString(FLOAT_ACCURACY) });


            mysql.CreateCoordinate(cut.Id, _coordinates);

            if (_originalPictureOVERWRITE != null || _originalPicture != null)
            {
                using (var stream = new MemoryStream())
                {
                    if (_originalPictureOVERWRITE != null)
                    {
                        _originalPictureOVERWRITE.Save(stream, ImageFormat.Jpeg);
                    }
                    else
                    {
                        _originalPicture.Save(stream, ImageFormat.Jpeg);
                    }

                    mysql.CreateImage(cut.Id, "original image", stream.ToArray());
                }
            }

            if (_markedPictureOVERWRITE != null || _markedPicture != null)
            {
                using (var stream = new MemoryStream())
                {
                    if (_markedPictureOVERWRITE != null)
                    {
                        _markedPictureOVERWRITE.Save(stream, ImageFormat.Jpeg);
                    }
                    else
                    {
                        _markedPicture.Save(stream, ImageFormat.Jpeg);
                    }

                    mysql.CreateImage(cut.Id, "marked image", stream.ToArray());
                }
            }

            if ( _analyzedPicture != null)
            {
                using (var stream = new MemoryStream())
                {
                    _analyzedPicture.Save(stream, ImageFormat.Jpeg);
                    mysql.CreateImage(cut.Id, "analyzed image", stream.ToArray());
                }
            }

            //new

            if (pictureBox19.Image != null)
            {
                try
                {
                    Bitmap _whiteMatterImage;
                    using (var b = new Bitmap(pictureBox19.Image.Width, pictureBox19.Image.Height))
                    {
                        b.SetResolution(pictureBox19.Image.HorizontalResolution, pictureBox19.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox19.Image, 0, 0);
                        }

                        _whiteMatterImage = new Bitmap(b);
                    }

                    using (var stream = new MemoryStream())
                    {
                        _whiteMatterImage.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "white matter area image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox1.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox1.Image.Width, pictureBox1.Image.Height))
                    {
                        b.SetResolution(pictureBox1.Image.HorizontalResolution, pictureBox1.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox1.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "l1 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox2.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox2.Image.Width, pictureBox2.Image.Height))
                    {
                        b.SetResolution(pictureBox2.Image.HorizontalResolution, pictureBox2.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox2.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "l2 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox3.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox3.Image.Width, pictureBox3.Image.Height))
                    {
                        b.SetResolution(pictureBox3.Image.HorizontalResolution, pictureBox3.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox3.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "l3 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox4.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox4.Image.Width, pictureBox4.Image.Height))
                    {
                        b.SetResolution(pictureBox4.Image.HorizontalResolution, pictureBox4.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox4.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "l4 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox5.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox5.Image.Width, pictureBox5.Image.Height))
                    {
                        b.SetResolution(pictureBox5.Image.HorizontalResolution, pictureBox5.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox5.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "l5 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox6.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox6.Image.Width, pictureBox6.Image.Height))
                    {
                        b.SetResolution(pictureBox6.Image.HorizontalResolution, pictureBox6.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox6.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "l6 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox7.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox7.Image.Width, pictureBox7.Image.Height))
                    {
                        b.SetResolution(pictureBox7.Image.HorizontalResolution, pictureBox7.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox7.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "l7 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox8.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox8.Image.Width, pictureBox8.Image.Height))
                    {
                        b.SetResolution(pictureBox8.Image.HorizontalResolution, pictureBox8.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox8.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "l8 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox9.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox9.Image.Width, pictureBox9.Image.Height))
                    {
                        b.SetResolution(pictureBox9.Image.HorizontalResolution, pictureBox9.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox9.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "l9 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox18.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox18.Image.Width, pictureBox18.Image.Height))
                    {
                        b.SetResolution(pictureBox18.Image.HorizontalResolution, pictureBox18.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox18.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "f1 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox17.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox17.Image.Width, pictureBox17.Image.Height))
                    {
                        b.SetResolution(pictureBox17.Image.HorizontalResolution, pictureBox17.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox17.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "f2 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }


            if (pictureBox16.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox16.Image.Width, pictureBox16.Image.Height))
                    {
                        b.SetResolution(pictureBox16.Image.HorizontalResolution, pictureBox16.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox16.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "f3 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox15.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox15.Image.Width, pictureBox15.Image.Height))
                    {
                        b.SetResolution(pictureBox15.Image.HorizontalResolution, pictureBox15.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox15.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "f4 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox14.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox14.Image.Width, pictureBox14.Image.Height))
                    {
                        b.SetResolution(pictureBox14.Image.HorizontalResolution, pictureBox14.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox14.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "f5 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox13.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox13.Image.Width, pictureBox13.Image.Height))
                    {
                        b.SetResolution(pictureBox13.Image.HorizontalResolution, pictureBox13.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox13.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "f6 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox12.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox12.Image.Width, pictureBox12.Image.Height))
                    {
                        b.SetResolution(pictureBox12.Image.HorizontalResolution, pictureBox12.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox12.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "f7 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox11.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox11.Image.Width, pictureBox11.Image.Height))
                    {
                        b.SetResolution(pictureBox11.Image.HorizontalResolution, pictureBox11.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox11.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "f8 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            if (pictureBox10.Image != null)
            {
                try
                {
                    Bitmap _image;
                    using (var b = new Bitmap(pictureBox10.Image.Width, pictureBox10.Image.Height))
                    {
                        b.SetResolution(pictureBox10.Image.HorizontalResolution, pictureBox10.Image.VerticalResolution);

                        using (var g = Graphics.FromImage(b))
                        {
                            g.Clear(Color.White);
                            g.DrawImageUnscaled(pictureBox10.Image, 0, 0);
                        }

                        _image = new Bitmap(b);
                    }
                    using (var stream = new MemoryStream())
                    {
                        _image.Save(stream, ImageFormat.Jpeg);
                        mysql.CreateImage(cut.Id, "f9 image", stream.ToArray());
                    }
                }
                catch (Exception ex)
                {
                }
            }

            pleaseWait.Close();

            if(MessageBoxAutocloseWithButtons.Show(_waitTime, true, "No", "Yes",
                $"Show result online?") == DialogResult.Cancel)
            //if (MessageBox.Show("Show result online?", "Result", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Process.Start($"{Config.HomepageBaseUrl}/php/details.php?age={cut.Age}&genotype={cut.Genotype}&animal={cut.Animal}&cutidentifier={cut.CutIdentifier}");
            }

            if (MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Yes", "No",
                    $"Next image?") == DialogResult.OK)
            //if (MessageBox.Show("Next image?", "Next", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Close();
            }
        }

        private void SafeResultsLocal()
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_filePath);
            string resultPath = Path.Combine(Path.GetDirectoryName(_filePath), fileNameWithoutExtension);

            //create result dir
            AddLogMessage($"Create result directory: {resultPath}");
            Utils.CreateIfNotExists(resultPath);

            //copy original image
            AddLogMessage("Safe original image");
            string originalFullFileName = Path.Combine(resultPath, $"{Path.GetFileName(_filePath)} - Original.png");
            if (File.Exists(originalFullFileName))
            {
                try
                {
                    File.Delete(originalFullFileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (File.Exists(originalFullFileName) == false)
            {
                File.Copy(_filePath, originalFullFileName);
            }

            //safe result image
            AddLogMessage("Safe result image");

            //add white backgroud
            Bitmap bmpWhiteBackgroud = new Bitmap(_bitmap.Width, _bitmap.Height);
            Rectangle rect = new Rectangle(Point.Empty, _bitmap.Size);
            try
            {
                using (Graphics G = Graphics.FromImage(bmpWhiteBackgroud))
                {
                    G.Clear(Color.White);
                    G.DrawImageUnscaledAndClipped(_bitmap, rect);
                }
            }catch(Exception ex)
            { }

            //safe result image
            string resultFullFileName = Path.Combine(resultPath, $"{fileNameWithoutExtension} - Result.png");
            if (File.Exists(resultFullFileName))
            {
                try
                {
                    File.Delete(resultFullFileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (File.Exists(resultFullFileName) == false)
            {
                bmpWhiteBackgroud.Save(resultFullFileName, ImageFormat.Jpeg);
            }

            //safe datapoints to csv
            AddLogMessage($"Safe datapoints csv");
            StringBuilder csvDatapoints = new StringBuilder();

            csvDatapoints.AppendLine($"X{CSV_DELIMITER}Y");

            foreach (Point coordinate in _coordinates)
            {
                double x = coordinate.X * _imageResizeFactor;
                double y = coordinate.Y * _imageResizeFactor;
                csvDatapoints.AppendLine($"{x:F4}{CSV_DELIMITER}{y:F4}");
            }

            string datapointsFullFileName = Path.Combine(resultPath, $"{fileNameWithoutExtension} - Datapoints.txt");
            if (File.Exists(datapointsFullFileName))
            {
                try
                {
                    File.Delete(datapointsFullFileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (File.Exists(datapointsFullFileName) == false)
            {
                File.WriteAllText(datapointsFullFileName, csvDatapoints.ToString(), Encoding.UTF8);
            }

            //safe result to csv
            AddLogMessage($"Safe results csv");
            StringBuilder resultDatapoints = new StringBuilder();

            resultDatapoints.AppendLine($"Lobules measurement result");
            resultDatapoints.AppendLine($"created on{CSV_DELIMITER}{DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            resultDatapoints.AppendLine($"original file{CSV_DELIMITER}{originalFullFileName}");
            resultDatapoints.AppendLine($"software version{CSV_DELIMITER}{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion}");
            resultDatapoints.AppendLine($"minimal distance between 2 points{CSV_DELIMITER}{nudBlurredFactor.Value}");
            resultDatapoints.AppendLine($"offset{CSV_DELIMITER}{nudOffset.Value}");
            resultDatapoints.AppendLine($"intersection point restriction area{CSV_DELIMITER}{nudIntersectionResArea.Value}");
            resultDatapoints.AppendLine($"age (days, postnatal){CSV_DELIMITER}{_age}");
            resultDatapoints.AppendLine($"genotype{CSV_DELIMITER}{_genotype}");
            resultDatapoints.AppendLine($"animal number{CSV_DELIMITER}{_animalNumber}");
            resultDatapoints.AppendLine($"method{CSV_DELIMITER}{_method}");
            resultDatapoints.AppendLine($"zoom factor{CSV_DELIMITER}{_zoomFactor}");
            resultDatapoints.AppendLine($"cut identifier{CSV_DELIMITER}{_cutIdentifier}");
            resultDatapoints.AppendLine($"layer{CSV_DELIMITER}{_layer}");
            resultDatapoints.AppendLine($"cerebellum area{CSV_DELIMITER}{_cerebellumArea}");
            resultDatapoints.AppendLine($"cerebellum perimeter{CSV_DELIMITER}{_cerebellumPerimet}");
            resultDatapoints.AppendLine($"minimal circle area{CSV_DELIMITER}{_minCircleArea}");
            resultDatapoints.AppendLine($"minimal circle perimeter{CSV_DELIMITER}{_minCirclePerimet}");
            resultDatapoints.AppendLine($"minimal circle radius{CSV_DELIMITER}{_minCircleRadius}");
            resultDatapoints.AppendLine($"convex hull area{CSV_DELIMITER}{_convexHullArea}");
            resultDatapoints.AppendLine($"convex hull perimeter{CSV_DELIMITER}{_convexHullPerimet}");
            resultDatapoints.AppendLine($"solidity{CSV_DELIMITER}{_solidity}");
            resultDatapoints.AppendLine($"circularity{CSV_DELIMITER}{_circularity}");
            resultDatapoints.AppendLine($"convexity{CSV_DELIMITER}{_convexity}");

            int i = 1;
            foreach (Fisur fisur in _fisurList)
            {
                resultDatapoints.AppendLine($"fisur F{i++}{CSV_DELIMITER}{fisur.Length}");
            }

            string resultCsvFullFileName = Path.Combine(resultPath, $"{fileNameWithoutExtension} - Result.txt");
            if (File.Exists(resultCsvFullFileName))
            {
                try
                {
                    File.Delete(resultCsvFullFileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            if (File.Exists(resultCsvFullFileName) == false)
            {
                File.WriteAllText(resultCsvFullFileName, resultDatapoints.ToString(), Encoding.UTF8);
            }

            //safe result to excel
            AddLogMessage($"Safe results excel");
            string resultExcelFullFileName = Path.Combine(resultPath, $"{fileNameWithoutExtension} - Result.xlsx");
            if (File.Exists(resultExcelFullFileName))
            {
                try
                {
                    File.Delete(resultExcelFullFileName);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            using (ExcelPackage excel = new ExcelPackage(new FileInfo(resultExcelFullFileName)))
            {
                excel.Workbook.Worksheets.Add("Results");
                excel.Workbook.Worksheets.Add("Datapoints");

                ExcelWorksheet excelWorksheetDatapoints = excel.Workbook.Worksheets["Datapoints"];

                excelWorksheetDatapoints.Column(1).Width = 20;
                excelWorksheetDatapoints.Column(2).Width = 20;

                var headerRowDatapoints = new List<string[]>()
                {
                    new string[] { "X", "Y" }
                };

                string headerRangeDatapoints = "A1:" + Char.ConvertFromUtf32(headerRowDatapoints[0].Length + 64) + "1";
                excelWorksheetDatapoints.Cells[headerRangeDatapoints].LoadFromArrays(headerRowDatapoints);
                excelWorksheetDatapoints.Cells[headerRangeDatapoints].Style.Font.Size = 22;
                excelWorksheetDatapoints.Cells[headerRangeDatapoints].Style.Font.Name = "Calibri";

                var cellDataPoints = new List<object[]>();

                foreach (Point coordinate in _coordinates)
                {
                    double x = coordinate.X * _imageResizeFactor;
                    double y = coordinate.Y * _imageResizeFactor;
                    cellDataPoints.Add(new object[] { x, y });
                }

                excelWorksheetDatapoints.Cells[2, 1].LoadFromArrays(cellDataPoints);

                ExcelWorksheet excelWorksheetResults = excel.Workbook.Worksheets["Results"];

                excelWorksheetResults.Column(1).Width = 40;
                excelWorksheetResults.Column(2).Width = 40;

                var headerRowResults = new List<string[]>()
                {
                    new string[] { "Lobuli Measurement Result" }
                };

                string headerRangeResults = "A1:" + Char.ConvertFromUtf32(headerRowResults[0].Length + 64) + "1";
                excelWorksheetResults.Cells[headerRangeResults].LoadFromArrays(headerRowResults);
                excelWorksheetResults.Cells[headerRangeResults].Style.Font.Size = 22;
                excelWorksheetResults.Cells[headerRangeResults].Style.Font.Name = "Calibri";

                excelWorksheetResults.Cells[2, 1].Value = $"created on {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
                excelWorksheetResults.Cells[2, 1].Style.Font.Size = 9;
                excelWorksheetResults.Cells[2, 1].Style.Font.Name = "Calibri";

                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 4, "Original file:", originalFullFileName, 9);

                //excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 6, "Software version:", FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion, 9);
                //excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 7, "Minimal distance between 2 datapoints:", nudBlurredFactor.Value.ToString("####"), 9);
                //excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 8, "Offset:", nudOffset.Value.ToString("####"), 9);
                //excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 9, "intersection point restriction area:", nudIntersectionResArea.Value.ToString("####"), 9);

                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 6, "Age:", $"{_age} days (postnatal)");
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 7, "GenoTypeIsKO:", _genotype);
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 8, "Animal number:", _animalNumber);
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 9, "Method:", _method);
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 10, "Zoom factor:", $"{_zoomFactor} (100 µm = {nudConversionPixel.Value:####} Pixel)");
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 11, "Cut identifier:", _cutIdentifier);
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 12, "Layer:", _layer);

                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 14, "Cerebellum area [µm²]:", _cerebellumArea.ToString("N2", CultureInfo.CurrentCulture));
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 15, "Cerebellum perimeter [µm]:", _cerebellumPerimet.ToString("N2", CultureInfo.CurrentCulture));

                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 17, "Minimal circle are [µm²]:", _minCircleArea.ToString("N2", CultureInfo.CurrentCulture));
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 18, "Minimal circle perimeter [µm]:", _minCirclePerimet.ToString("N2", CultureInfo.CurrentCulture));
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 19, "Minimal circle radius [µm]:", _minCircleRadius.ToString("N2", CultureInfo.CurrentCulture));

                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 21, "Convex hull area [µm²]:", _convexHullArea.ToString("N2", CultureInfo.CurrentCulture));
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 22, "Convex hull perimeter [µm]:", _convexHullPerimet.ToString("N2", CultureInfo.CurrentCulture));

                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 23, "Solidity:", _solidity.ToString("N2", CultureInfo.CurrentCulture));
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 24, "Circularity:", _circularity.ToString("N2", CultureInfo.CurrentCulture));
                excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 25, "Convexity:", _convexity.ToString("N2", CultureInfo.CurrentCulture));

                int j = 0;
                foreach (Fisur fisur in _fisurList)
                {
                    j++;
                    excelWorksheetResults = WriteExcelRow(excelWorksheetResults, 26 + j, $"Fisur F{j}:", fisur.Length.ToString("N2", CultureInfo.CurrentCulture));
                }

                Bitmap croppedBitmap = new Bitmap(cropXEnd - cropXStart, cropYEnd - cropYStart);

                using (Graphics g = Graphics.FromImage(croppedBitmap))
                {
                    if (cropXStart >= 0 && cropYStart >= 0)
                    {
                        Rectangle section = new Rectangle(cropXStart, cropYStart, cropXEnd - cropXStart,
                            cropYEnd - cropYStart);
                        g.DrawImage(bmpWhiteBackgroud, 0, 0, section, GraphicsUnit.Pixel);
                    }
                }

                var picture = excelWorksheetResults.Drawings.AddPicture("Result", croppedBitmap);
                picture.SetPosition(5, 0, 2, 0);

                excel.Save();
            }

            AddLogMessage($"=== FINISHED ===");
        }

        private ExcelWorksheet WriteExcelRow(ExcelWorksheet excelWorksheet, int rowNumber, string titel, string value, int fontSize = 11)
        {
            excelWorksheet.Cells[rowNumber, 1].Value = titel;
            excelWorksheet.Cells[rowNumber, 2].Value = value;
            excelWorksheet.Cells[rowNumber, 1].Style.Font.Size = fontSize;
            excelWorksheet.Cells[rowNumber, 2].Style.Font.Size = fontSize;
            excelWorksheet.Cells[rowNumber, 1].Style.Font.Name = "Calibri";
            excelWorksheet.Cells[rowNumber, 2].Style.Font.Name = "Calibri";

            return excelWorksheet;
        }


        private void FormWorker_Shown(object sender, EventArgs e)
        {
            switch (_workerMethod)
            {
                case WorkerMethod.JustDatapoints:
                case WorkerMethod.JustMeasurement:
                case WorkerMethod.SingleDatapointsAndMeasurement:
                    GetFilePath();
                    break;
                case WorkerMethod.ReanalyzeSingleDatapointsAndMeasurement:
                    if (!string.IsNullOrEmpty(_filePath))
                    {
                        Text += $" ({Path.GetFileNameWithoutExtension(_filePath)})";
                        InitializeFormDetails();

                        DrawCoordinates();

                        Analyze(true);
                    }
                    else
                    {
                        GetFilePath();
                    }
                    break;
                case WorkerMethod.MultiDatapointsAndMeasurement:
                    GetDirectoryPath();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_workerMethod), _workerMethod, null);
            }
        }

        private void AddLogMessage(string message)
        {
            ConsoleTextEditor.AppendText(message + Environment.NewLine);
            ConsoleTextEditor.ScrollToCaret();
        }

        private void lblPerimeter_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("Microsoft Sans Serif", 8);
            Brush brush = new SolidBrush(Color.Black);
            e.Graphics.TranslateTransform(0, 70);
            e.Graphics.RotateTransform(-75);
            e.Graphics.DrawString("Perimeter [µm]", font, brush, 0f, 0f);
        }

        private void lblLength_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("Microsoft Sans Serif", 8);
            Brush brush = new SolidBrush(Color.Black);
            e.Graphics.TranslateTransform(0, 70);
            e.Graphics.RotateTransform(-75);
            e.Graphics.DrawString("Length [µm]", font, brush, 0f, 0f);
        }

        private void lblArea_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("Microsoft Sans Serif", 8);
            Brush brush = new SolidBrush(Color.Black);
            e.Graphics.TranslateTransform(0, 70);
            e.Graphics.RotateTransform(-75);
            e.Graphics.DrawString("Area [µm²]", font, brush, 0f, 0f);
        }

        private void lblStraigtness_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("Microsoft Sans Serif", 8);
            Brush brush = new SolidBrush(Color.Black);
            e.Graphics.TranslateTransform(0, 70);
            e.Graphics.RotateTransform(-75);
            e.Graphics.DrawString("Straigtness", font, brush, 0f, 0f);
        }

        private void lblPercentageInCerebellum_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("Microsoft Sans Serif", 8);
            Brush brush = new SolidBrush(Color.Black);
            e.Graphics.TranslateTransform(0, 70);
            e.Graphics.RotateTransform(-75);
            e.Graphics.DrawString("% in cerebellum", font, brush, 0f, 0f);
        }
    }
}
