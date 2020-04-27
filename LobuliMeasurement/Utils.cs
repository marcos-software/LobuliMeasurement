using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;

namespace LobuliMeasurement
{
    static class Utils
    {

        public static bool EndsWith(this string value, IEnumerable<string> values)
        {
            return values.Any(value.EndsWith);
        }
        
        public static Bitmap RotateImage(Bitmap bitmap, PointF offset, float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(bitmap.Width, bitmap.Height);
            rotatedBmp.SetResolution(bitmap.HorizontalResolution, bitmap.VerticalResolution);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(offset.X, offset.Y);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-offset.X, -offset.Y);

            //draw passed in image onto graphics object
            g.DrawImage(bitmap, new PointF(0, 0));

            return rotatedBmp;
        }

        public static Point RotatePoint(Point pointToRotate, Point centerPoint, double angleInDegrees)
        {
            double angleInRadians = angleInDegrees * (Math.PI / 180);
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Point
            {
                X =
                    (int)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                     sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (int)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                     cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }

        public static void GrandFullAccessToEveryone(string path)
        {
            try
            {
                DirectorySecurity sec = Directory.GetAccessControl(path);
                SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                sec.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.Modify | FileSystemRights.Synchronize, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                Directory.SetAccessControl(path, sec);
            }
            catch { }
        }

        public static bool CreateIfNotExists(string path, bool grandFullAccess = true)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                    try
                    {
                        if (grandFullAccess) GrandFullAccessToEveryone(path);
                    }
                    catch { }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }
    }

    public class Line
    {
        public Point[] GetPoints()
        {
            int quantity = Convert.ToInt32(Length);

            var points = new Point[quantity + 1];
            points[0] = PointStart;

            int ydiff = PointEnd.Y - PointStart.Y, xdiff = PointEnd.X - PointStart.X;
            double slope = (double)(PointEnd.Y - PointStart.Y) / (PointEnd.X - PointStart.X);
            double x, y;

            --quantity;

            for (double i = 0; i < quantity; i++)
            {
                y = slope == 0 ? 0 : ydiff * (i / quantity);
                x = slope == 0 ? xdiff * (i / quantity) : y / slope;
                points[(int)i] = new Point((int)Math.Round(x) + PointStart.X, (int)Math.Round(y) + PointStart.Y);
            }

            points[quantity + 1] = PointEnd;
            return points;
        }

        public readonly Point PointStart;
        public readonly Point PointEnd;
        public readonly Line OuterLine;
        public Point RelatedPoint;

        const double Rad2Deg = 180.0 / Math.PI;

        public double Angle => Math.Atan2(PointStart.Y - PointEnd.Y, PointEnd.X - PointStart.X) * Rad2Deg;

        private double length = -1;

        public double Length
        {
            get
            {
                if (length <= 0)
                {
                    length = GetDistanceBetweenTwoPoints(PointStart, PointEnd);
                }
                return length;
            }
        }

        public Line Reverse()
        {
            return new Line(PointEnd, PointStart);
        }

        public Line(Point _pointStart, Point _pointEnd, Line lastLine = null, int? offsetPixelForOuterLine = null)
        {
            if (_pointStart == null) throw new ArgumentNullException(nameof(_pointStart));
            if (_pointEnd == null) throw new ArgumentNullException(nameof(_pointEnd));

            if (_pointStart.Equals(_pointEnd)) throw new NotSupportedException($"{nameof(_pointStart)} and {nameof(_pointEnd)} must differ!");

            PointStart = _pointStart;
            PointEnd = _pointEnd;

            if (offsetPixelForOuterLine != null && offsetPixelForOuterLine > 0)
            {
                //generate points of offseted line
                var x1p = _pointStart.X + offsetPixelForOuterLine * (_pointEnd.Y - _pointStart.Y) / Length;
                var x2p = _pointEnd.X + offsetPixelForOuterLine * (_pointEnd.Y - _pointStart.Y) / Length;
                var y1p = _pointStart.Y + offsetPixelForOuterLine * (_pointStart.X - _pointEnd.X) / Length;
                var y2p = _pointEnd.Y + offsetPixelForOuterLine * (_pointStart.X - _pointEnd.X) / Length;

                Point outerLineStart = new Point((int)(x1p ?? 0), (int)(y1p ?? 0));
                Point outerLineEnd = new Point((int)(x2p ?? 0), (int)(y2p ?? 0));

                if (lastLine != null && lastLine.OuterLine != null)
                {
                    outerLineStart = lastLine.OuterLine.PointEnd;
                }

                if (outerLineStart != outerLineEnd)
                {
                    OuterLine = new Line(outerLineStart, outerLineEnd);
                    OuterLine.RelatedPoint = _pointStart;
                }
            }
        }

        public static Point Fraction(Point _pointStart, Point _pointEnd, float _frac)
        {
            return new Point((int)(_pointStart.X + _frac * (_pointEnd.X - _pointStart.X)),
                (int)(_pointStart.Y + _frac * (_pointEnd.Y - _pointStart.Y)));
        }

        public static double GetDistanceBetweenTwoPoints(Point _pointStart, Point _pointEnd)
        {
            if (_pointStart == null) throw new ArgumentNullException(nameof(_pointStart));
            if (_pointEnd == null) throw new ArgumentNullException(nameof(_pointEnd));

            return Math.Sqrt((_pointEnd.X - _pointStart.X) * (_pointEnd.X - _pointStart.X) +
                             (_pointEnd.Y - _pointStart.Y) * (_pointEnd.Y - _pointStart.Y));
        }

        public static double GetDistanceBetweenTwoPoints(PointF _pointStart, PointF _pointEnd)
        {
            if (_pointStart == null) throw new ArgumentNullException(nameof(_pointStart));
            if (_pointEnd == null) throw new ArgumentNullException(nameof(_pointEnd));

            return Math.Sqrt((_pointEnd.X - _pointStart.X) * (_pointEnd.X - _pointStart.X) +
                             (_pointEnd.Y - _pointStart.Y) * (_pointEnd.Y - _pointStart.Y));
        }

        public PointF GetLenghtenedPoint2(float distanceFromPointStart)
        {
            PointF p1 = new PointF(this.PointStart.X, this.PointStart.Y);
            PointF p2 = new PointF(this.PointEnd.X, this.PointEnd.Y);

            PointF v = new PointF()
            {
                X = p2.X - p1.X,
                Y = p2.Y - p1.Y
            };
            return TranslatePoint(p2, distanceFromPointStart, v);
        }

        static PointF TranslatePoint(PointF point, float offset, PointF vector)
        {
            float magnitude = (float)Math.Sqrt((vector.X * vector.X) + (vector.Y * vector.Y)); // = length
            vector.X /= magnitude;
            vector.Y /= magnitude;
            PointF translation = new PointF()
            {
                X = offset * vector.X,
                Y = offset * vector.Y
            };
            using (Matrix m = new Matrix())
            {
                m.Translate(translation.X, translation.Y);
                PointF[] pts = new PointF[] { point };
                m.TransformPoints(pts);
                return pts[0];
            }
        }

        public Point GetLenghtenedPoint(Point source, double distanceFromPointStart, bool sameDirection)
        {
            return GetPointOnLineFromDistance2(source, this, sameDirection, distanceFromPointStart);
        }

        public static Point GetPointOnLineFromDistance2(Point source, Line _line, bool sameDirection, double _distance)
        {
            double x = source.X, y = source.Y;
            var dist = _distance;
            //var angle = angel;

            double oldAngel = GetAngel(source, _line.PointStart, _line.PointEnd);

            var angle = sameDirection ? oldAngel : (-1 * oldAngel);

            x = x + dist * Math.Cos(AngleToRadians(angle));
            y = y + dist * Math.Sin(AngleToRadians(angle));

            return new Point(Convert.ToInt32(x), Convert.ToInt32(y));
        }

        private static double AngleToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        private static double GetAngel(Point _point1, Point _point2, Point _point3)
        {
            return Math.Atan2(_point2.Y - _point1.Y, _point2.X - _point1.X) - Math.Atan2(_point3.Y - _point2.Y, _point3.X - _point2.X);
        }
    }

    public class Fisur
    {
        public int Id = -1;
        public bool NotExistent;
        public Point Start;
        public Point End;
        public Point HighestPoint;
        public Point DeepestPoint;
        public Point IntersectionPoint;
        public double Length;
        public double Perimeter { get; set; }
        public double Area;
        public double Straigtness;
        public double PercentageOfCerebellum;
        public int dgvRowIndex = -1;
        public List<Point> PointsOnLine;
        public double AngleToX = 0;
        public double AngleToNextLorF = 0;
        public double DistanceToNextLorF = 0;
        public double Circularity = 0;
        public double Convexity = 0;
        public double Solidity = 0;
        public double MinCircleRadius = 0;
        public double MinCircleArea = 0;
        public double MinCirclePerimet = 0;
        public List<Point> ConexHullPoints;
        public double ConvexHullArea = 0;
        public double ConvexHullPerimet = 0;
        public double DistanceToCerebellumCenter = 0;

        public Fisur()
        {
            PointsOnLine = new List<Point>();
        }
    }
}
