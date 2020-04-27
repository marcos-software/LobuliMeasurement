using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace LobuliMeasurement
{
    public partial class FormChooseLobule : Form
    {
        private int _lobuleIndex;
        private Point _lastLobuleEnd;
        private Image _OriginalImage;
        private Image _ThisImage;
        private Color _BackColor;
        private int _ZoomFactor = 6;

        private bool _pickStartClicked;
        private bool _pickEndClicked;

        private List<Line> _lines;

        private double _maximalDistanceBetweenPicketAndLine = 10;

        public bool lobuleNotExist = false;
        public Point startPoint;
        public Point endPoint;

        private int _waitTime = 0;

        public FormChooseLobule(int lobuleIndex, Point lastLobuleEnd, Image image, List<Line> lines, int waitTime)
        {
            _lobuleIndex = lobuleIndex;
            _lastLobuleEnd = lastLobuleEnd;
            _OriginalImage = image;
            _waitTime = waitTime;


            _lines = lines;

            InitializeComponent();

            Reset();
        }

        private void DrawStartPoint(Point startPoint)
        {
          
            using (Graphics g = Graphics.FromImage(_ThisImage))
            {

                Pen myPen = new Pen(Color.Black);
                g.DrawRectangle(myPen, new Rectangle((int)startPoint.X - 3, (int)startPoint.Y - 3, 7, 7));
                g.DrawImage(_ThisImage, new Point(0, 0));

                RectangleF rectf = new RectangleF(startPoint.X + 10, startPoint.Y, 100, 30);
                g.DrawString($"start point", new Font("Microsoft Sans Serif", 10), Brushes.Black, rectf);
            }

            picImage.Image = _ThisImage;
            picImage.Refresh();
        }

        private void DrawEndPoint(Point endPoint)
        {

            using (Graphics g = Graphics.FromImage(_ThisImage))
            {

                Pen myPen = new Pen(Color.Black);
                g.DrawRectangle(myPen, new Rectangle((int)endPoint.X - 3, (int)endPoint.Y - 3, 7, 7));
                g.DrawImage(_ThisImage, new Point(0, 0));

                RectangleF rectf = new RectangleF(endPoint.X + 10, endPoint.Y, 100, 30);
                g.DrawString($"end point", new Font("Microsoft Sans Serif", 10), Brushes.Black, rectf);
            }

            picImage.Image = _ThisImage;
            picImage.Refresh();
        }

        private void picImage_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateZoomedImage(e);
        }

        private void UpdateZoomedImage(MouseEventArgs e)
        {
            // Calculate the width and height of the portion of the image we want
            // to show in the picZoom picturebox. This value changes when the zoom
            // factor is changed.
            int zoomWidth = picZoom.Width / _ZoomFactor;
            int zoomHeight = picZoom.Height / _ZoomFactor;

            // Calculate the horizontal and vertical midpoints for the crosshair
            // cursor and correct centering of the new image
            int halfWidth = zoomWidth / 2;
            int halfHeight = zoomHeight / 2;

            // Create a new temporary bitmap to fit inside the picZoom picturebox
            Bitmap tempBitmap = new Bitmap(zoomWidth, zoomHeight, PixelFormat.Format24bppRgb);

            // Create a temporary Graphics object to work on the bitmap
            Graphics bmGraphics = Graphics.FromImage(tempBitmap);

            // Clear the bitmap with the selected backcolor
            bmGraphics.Clear(_BackColor);

            // Set the interpolation mode
            bmGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int x = e.X;
            int y = e.Y;


            // Draw the portion of the main image onto the bitmap
            // The target rectangle is already known now.
            // Here the mouse position of the cursor on the main image is used to
            // cut out a portion of the main image.
            bmGraphics.DrawImage(picImage.Image,
                                 new Rectangle(0, 0, zoomWidth, zoomHeight),
                                 new Rectangle(x - halfWidth, y - halfHeight, zoomWidth, zoomHeight),
                                 GraphicsUnit.Pixel);

            // Draw the bitmap on the picZoom picturebox
            picZoom.Image = tempBitmap;

            // Draw a crosshair on the bitmap to simulate the cursor position
            bmGraphics.DrawLine(Pens.Black, halfWidth + 1, halfHeight - 4, halfWidth + 1, halfHeight - 1);
            bmGraphics.DrawLine(Pens.Black, halfWidth + 1, halfHeight + 6, halfWidth + 1, halfHeight + 3);
            bmGraphics.DrawLine(Pens.Black, halfWidth - 4, halfHeight + 1, halfWidth - 1, halfHeight + 1);
            bmGraphics.DrawLine(Pens.Black, halfWidth + 6, halfHeight + 1, halfWidth + 3, halfHeight + 1);

            // Dispose of the Graphics object
            bmGraphics.Dispose();

            // Refresh the picZoom picturebox to reflect the changes
            picZoom.Refresh();

            if (_pickStartClicked)
            {
                txbStartX.Text = x.ToString();
                txbStartY.Text = y.ToString();
            }
            if (_pickEndClicked)
            {
                txbEndX.Text = x.ToString();
                txbEndY.Text = y.ToString();
            }
        }

        private void btnPickStart_Click(object sender, EventArgs e)
        {
            PickStart();
        }

        private void PickStart()
        {
            _pickEndClicked = false;
            picImage.Cursor = Cursors.Cross;
            picZoom.Visible = true;
            _pickStartClicked = true;
            lbladvice.Visible = true;
            lbladvice.Text = "Now pick START point";
        }

        private void btnPickEnd_Click(object sender, EventArgs e)
        {
            PickEnd();
        }

        private void PickEnd()
        {
            _pickStartClicked = false;
            picImage.Cursor = Cursors.Cross;
            picZoom.Visible = true;
            _pickEndClicked = true;
            lbladvice.Visible = true;
            lbladvice.Text = "Now pick END point";
        }

        private void picImage_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;

            if (_pickEndClicked)
            {
                if (FindNearestPointOnline(x, y, out Point endPoint) == false)
                {
                    MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                        $"No point where found, please pick end point ON line!");
                    //MessageBox.Show("No point where found, please pick end point ON line!");
                    return;
                }

                _pickEndClicked = false;
                lbladvice.Visible = false;
                picImage.Cursor = Cursors.Default;
                picZoom.Visible = false;
                txbEndX.Text = endPoint.X.ToString();
                txbEndY.Text = endPoint.Y.ToString();

                DrawEndPoint(endPoint);

                btnPickEnd.Enabled = false;
                txbEndX.Enabled = false;
                txbEndY.Enabled = false;
            }

            if (_pickStartClicked)
            {
                if (FindNearestPointOnline(x, y, out Point startPoint) == false)
                {
                    MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Ok", "Cancel",
                        $"No point where found, please pick start point ON line!");
                    //MessageBox.Show("No point where found, please pick start point ON line!");
                    return;
                }

                _pickStartClicked = false;
                lbladvice.Visible = false;
                picImage.Cursor = Cursors.Default;
                picZoom.Visible = false;
                txbStartX.Text = startPoint.X.ToString();
                txbStartY.Text = startPoint.Y.ToString();

                DrawStartPoint(startPoint);

                btnPickStart.Enabled = false;
                txbStartX.Enabled = false;
                txbStartY.Enabled = false;

                PickEnd();
            }

            if(string.IsNullOrEmpty(txbStartX.Text) == false &&
               string.IsNullOrEmpty(txbStartY.Text) == false && 
               string.IsNullOrEmpty(txbEndX.Text) == false &&
               string.IsNullOrEmpty(txbEndY.Text) == false)
            {
                int startX = Convert.ToInt32(txbStartX.Text);
                int startY = Convert.ToInt32(txbStartY.Text);
                int endX = Convert.ToInt32(txbEndX.Text);
                int endY = Convert.ToInt32(txbEndY.Text);

                DrawLobule(new Point(startX, startY), new Point(endX, endY));

                if(MessageBoxAutocloseWithButtons.Show(_waitTime, true, "Yes", "No",
                    $"Is this correct?") == DialogResult.OK)
                //if (MessageBox.Show("Is this correct?", "Check", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Ok();
                }
            }
        }

        private void DrawLobule(Point start, Point end)
        {
            bool foundStart = false;
            List<Point> lobulusCoordinates = new List<Point>();

            foreach (Line line in _lines)
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


                        using (Graphics g = Graphics.FromImage(_ThisImage))
                        {
                            Pen mypen = new Pen(Color.Green, 2);
                            g.DrawLine(mypen, line.PointStart.X, line.PointStart.Y, line.PointEnd.X,
                                line.PointEnd.Y);
                        }

                        picImage.Image = _ThisImage;
                        picImage.Refresh();

                        lobulusCoordinates.Add(line.PointStart);
                    }
                }
                else
                {
                    if (line.PointStart != end && line.PointEnd != end)
                    {
                        //in fisure
                        
                        using (Graphics g = Graphics.FromImage(_ThisImage))
                        {
                            Pen mypen = new Pen(Color.Green, 2);
                            g.DrawLine(mypen, line.PointStart.X, line.PointStart.Y, line.PointEnd.X,
                                line.PointEnd.Y);
                        }

                        picImage.Image = _ThisImage;
                        picImage.Refresh();

                        lobulusCoordinates.Add(line.PointStart);

                    }
                    else
                    {
                        //end found
                        foundStart = false;
                        lobulusCoordinates.Add(end);
                        Point lobulusCentroid = Geometry.FindCentroid(lobulusCoordinates);

                        using (Graphics g = Graphics.FromImage(_ThisImage))
                        {
                            try
                            {
                                RectangleF rectf = new RectangleF(lobulusCentroid.X, lobulusCentroid.Y, 50, 30);
                                g.DrawString($"L{_lobuleIndex}", new Font("Microsoft Sans Serif", 16), Brushes.Green,
                                    rectf);
                            }
                            catch (Exception ex)
                            {
                                //just for debug
                            }
                        }

                        picImage.Image = _ThisImage;
                        picImage.Refresh();
                    }
                }
            }
        }

        private bool FindNearestPointOnline(int x, int y, out Point point)
        {
            point = new Point();

            Point pickedPoint = new Point(x, y);

            double nearedDistance = 99999;
           
            foreach (Line line in _lines)
            {
                Point middlePoint = Line.Fraction(line.PointStart, line.PointEnd, 0.5f);

                double distance = Line.GetDistanceBetweenTwoPoints(pickedPoint, middlePoint);

                if (distance < nearedDistance)
                {
                    nearedDistance = distance;
                    point = line.PointStart;
                }
            }

            return nearedDistance < _maximalDistanceBetweenPicketAndLine;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void Reset()
        {
            _ThisImage = (Image)_OriginalImage.Clone();

            lbladvice.Visible = false;

            if(_lobuleIndex > 1 && _lastLobuleEnd.X > 0 && _lastLobuleEnd.Y > 0)
            {
                txbStartX.Text = _lastLobuleEnd.X.ToString();
                txbStartY.Text = _lastLobuleEnd.Y.ToString();
            }
            txbEndX.Text = "";
            txbEndY.Text = "";

            label2.Text = $"#{_lobuleIndex}";

            _BackColor = picImage.BackColor;

            picImage.SizeMode = PictureBoxSizeMode.CenterImage;
            picZoom.SizeMode = PictureBoxSizeMode.StretchImage;

            picImage.BackColor = _BackColor;
            picZoom.BackColor = _BackColor;

            picImage.Image = _ThisImage;

            picZoom.Visible = false;

            txbStartX.Enabled = true;
            txbStartY.Enabled = true;
            txbEndX.Enabled = true;
            txbEndY.Enabled = true;

            btnPickStart.Enabled = true;
            btnPickEnd.Enabled = true;

            if (string.IsNullOrEmpty(txbStartX.Text) == false && string.IsNullOrEmpty(txbStartY.Text) == false)
            {
                int startX = Convert.ToInt32(txbStartX.Text);
                int startY = Convert.ToInt32(txbStartY.Text);

                DrawStartPoint(new Point(startX, startY));
               
                PickEnd();
            }
            else
            {
                PickStart();
            }
        }

        private void btnNotExist_Click(object sender, EventArgs e)
        {
            lobuleNotExist = true;
            Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Ok();
        }

        private void Ok()
        {
            if (string.IsNullOrEmpty(txbStartX.Text) == false &&
                string.IsNullOrEmpty(txbStartY.Text) == false &&
                string.IsNullOrEmpty(txbEndX.Text) == false &&
                string.IsNullOrEmpty(txbEndY.Text) == false)
            {
                int startX = Convert.ToInt32(txbStartX.Text);
                int startY = Convert.ToInt32(txbStartY.Text);
                int endX = Convert.ToInt32(txbEndX.Text);
                int endY = Convert.ToInt32(txbEndY.Text);

                startPoint = new Point(startX, startY);
                endPoint = new Point(endX, endY);

                Close();
            }
        }

    }
}
