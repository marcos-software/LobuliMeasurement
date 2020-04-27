using System;
using System.Collections.Generic;

using System.Drawing;

namespace LobuliMeasurement
{
    static class Geometry
    {
        // For debugging.
        public static Point[] g_MinMaxCorners;
        public static Rectangle g_MinMaxBox;
        public static Point[] g_NonCulledPoints;

        // Find the points nearest the upper left, upper right,
        // lower left, and lower right corners.
        private static void GetMinMaxCorners(List<Point> points, ref Point ul, ref Point ur, ref Point ll, ref Point lr)
        {
            // Start with the first point as the solution.
            ul = points[0];
            ur = ul;
            ll = ul;
            lr = ul;

            // Search the other points.
            foreach (Point pt in points)
            {
                if (-pt.X - pt.Y > -ul.X - ul.Y) ul = pt;
                if (pt.X - pt.Y > ur.X - ur.Y) ur = pt;
                if (-pt.X + pt.Y > -ll.X + ll.Y) ll = pt;
                if (pt.X + pt.Y > lr.X + lr.Y) lr = pt;
            }

            g_MinMaxCorners = new Point[] { ul, ur, lr, ll }; // For debugging.
        }

        // Find a box that fits inside the MinMax quadrilateral.
        private static Rectangle GetMinMaxBox(List<Point> points)
        {
            // Find the MinMax quadrilateral.
            Point ul = new Point(0, 0), ur = ul, ll = ul, lr = ul;
            GetMinMaxCorners(points, ref ul, ref ur, ref ll, ref lr);

            // Get the coordinates of a box that lies inside this quadrilateral.
            int xmin, xmax, ymin, ymax;
            xmin = ul.X;
            ymin = ul.Y;

            xmax = ur.X;
            if (ymin < ur.Y) ymin = ur.Y;

            if (xmax > lr.X) xmax = lr.X;
            ymax = lr.Y;

            if (xmin < ll.X) xmin = ll.X;
            if (ymax > ll.Y) ymax = ll.Y;

            Rectangle result = new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
            g_MinMaxBox = result;    // For debugging.
            return result;
        }

        // Cull points out of the convex hull that lie inside the
        // trapezoid defined by the vertices with smallest and
        // largest X and Y coordinates.
        // Return the points that are not culled.
        private static List<Point> HullCull(List<Point> points)
        {
            // Find a culling box.
            Rectangle culling_box = GetMinMaxBox(points);

            // Cull the points.
            List<Point> results = new List<Point>();
            foreach (Point pt in points)
            {
                // See if (this point lies outside of the culling box.
                if (pt.X <= culling_box.Left ||
                    pt.X >= culling_box.Right ||
                    pt.Y <= culling_box.Top ||
                    pt.Y >= culling_box.Bottom)
                {
                    // This point cannot be culled.
                    // Add it to the results.
                    results.Add(pt);
                }
            }

            g_NonCulledPoints = new Point[results.Count];   // For debugging.
            results.CopyTo(g_NonCulledPoints);              // For debugging.
            return results;
        }

        // Return the points that make up a polygon's convex hull.
        // This method leaves the points list unchanged.
        public static List<Point> MakeConvexHull(List<Point> points)
        {
            // Cull.
            points = HullCull(points);

            // Find the remaining point with the smallest Y value.
            // if (there's a tie, take the one with the smaller X value.
            Point best_pt = points[0];
            foreach (Point pt in points)
            {
                if ((pt.Y < best_pt.Y) ||
                   ((pt.Y == best_pt.Y) && (pt.X < best_pt.X)))
                {
                    best_pt = pt;
                }
            }

            // Move this point to the convex hull.
            List<Point> hull = new List<Point>();
            hull.Add(best_pt);
            points.Remove(best_pt);

            // Start wrapping up the other points.
            float sweep_angle = 0;
            for (; ; )
            {
                // Find the point with smallest AngleValue
                // from the last point.
                int X = hull[hull.Count - 1].X;
                int Y = hull[hull.Count - 1].Y;
                best_pt = points[0];
                float best_angle = 3600;

                // Search the rest of the points.
                foreach (Point pt in points)
                {
                    float test_angle = AngleValue(X, Y, pt.X, pt.Y);
                    if ((test_angle >= sweep_angle) &&
                        (best_angle > test_angle))
                    {
                        best_angle = test_angle;
                        best_pt = pt;
                    }
                }

                // See if the first point is better.
                // If so, we are done.
                float first_angle = AngleValue(X, Y, hull[0].X, hull[0].Y);
                if ((first_angle >= sweep_angle) &&
                    (best_angle >= first_angle))
                {
                    // The first point is better. We're done.
                    break;
                }

                // Add the best point to the convex hull.
                hull.Add(best_pt);
                points.Remove(best_pt);

                sweep_angle = best_angle;

                // If all of the points are on the hull, we're done.
                if (points.Count == 0) break;
            }

            return hull;
        }

        public static Point FindCentroid(List<Point> _points)
        {
            // Add the first point at the end of the array.
            int num_points = _points.Count;
            Point[] pts = new Point[num_points + 1];
            _points.CopyTo(pts, 0);
            pts[num_points] = _points[0];

            // Find the centroid.
            float X = 0;
            float Y = 0;
            float second_factor;
            for (int i = 0; i < num_points; i++)
            {
                second_factor =
                    pts[i].X * pts[i + 1].Y -
                    pts[i + 1].X * pts[i].Y;
                X += (pts[i].X + pts[i + 1].X) * second_factor;
                Y += (pts[i].Y + pts[i + 1].Y) * second_factor;
            }

            // Divide by 6 times the polygon's area.
            float polygon_area = PolygonArea(_points);
            X /= (6 * polygon_area);
            Y /= (6 * polygon_area);

            // If the values are negative, the polygon is
            // oriented counterclockwise so reverse the signs.
            if (X < 0)
            {
                X = -X;
                Y = -Y;
            }

            return new Point((int)X, (int)Y);
        }

        // Return the polygon's area in "square units."
        // The value will be negative if the polygon is
        // oriented clockwise.
        private static float SignedPolygonArea(List<Point> points)
        {
            // Add the first point to the end.
            int num_points = points.Count;
            Point[] pts = new Point[num_points + 1];
            points.CopyTo(pts, 0);
            pts[num_points] = points[0];

            // Get the areas.
            float area = 0;
            for (int i = 0; i < num_points; i++)
            {
                area +=
                    (pts[i + 1].X - pts[i].X) *
                    (pts[i + 1].Y + pts[i].Y) / 2;
            }

            // Return the result.
            return area;
        }

        public static float PolygonArea(List<Point> points)
        {
            // Return the absolute value of the signed area.
            // The signed area is negative if the polyogn is
            // oriented clockwise.
            return Math.Abs(SignedPolygonArea(points));
        }


        // Return a number that gives the ordering of angles
        // WRST horizontal from the point (x1, y1) to (x2, y2).
        // In other words, AngleValue(x1, y1, x2, y2) is not
        // the angle, but if:
        //   Angle(x1, y1, x2, y2) > Angle(x1, y1, x2, y2)
        // then
        //   AngleValue(x1, y1, x2, y2) > AngleValue(x1, y1, x2, y2)
        // this angle is greater than the angle for another set
        // of points,) this number for
        //
        // This function is dy / (dy + dx).
        private static float AngleValue(int x1, int y1, int x2, int y2)
        {
            float dx, dy, ax, ay, t;

            dx = x2 - x1;
            ax = Math.Abs(dx);
            dy = y2 - y1;
            ay = Math.Abs(dy);
            if (ax + ay == 0)
            {
                // if (the two points are the same, return 360.
                t = 360f / 9f;
            }
            else
            {
                t = dy / (ax + ay);
            }
            if (dx < 0)
            {
                t = 2 - t;
            }
            else if (dy < 0)
            {
                t = 4 + t;
            }
            return t * 90;
        }

        public static Circle MakeCircle(IList<Point> points)
        {
            // Clone list to preserve the caller's data, do Durstenfeld shuffle
            List<Point> shuffled = new List<Point>(points);
            Random rand = new Random();
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                Point temp = shuffled[i];
                shuffled[i] = shuffled[j];
                shuffled[j] = temp;
            }

            // Progressively add points to circle or recompute circle
            Circle c = Circle.INVALID;
            for (int i = 0; i < shuffled.Count; i++)
            {
                CustomPoint p = new CustomPoint(shuffled[i].X, shuffled[i].Y);
                if (c.r < 0 || !c.Contains(p))
                {
                    List<CustomPoint> shuffeledRange = new List<CustomPoint>();
                    foreach (var point in shuffled.GetRange(0, i + 1))
                    {
                        shuffeledRange.Add(new CustomPoint(point.X, point.Y));
                    }
                    c = MakeCircleOnePoint(shuffeledRange, p);
                }
            }
            return c;
        }

        private static Circle MakeCircleOnePoint(List<CustomPoint> points, CustomPoint p)
        {
            Circle c = new Circle(p, 0);
            for (int i = 0; i < points.Count; i++)
            {
                CustomPoint q = points[i];
                if (!c.Contains(q))
                {
                    if (c.r == 0)
                        c = MakeDiameter(p, q);
                    else
                        c = MakeCircleTwoPoints(points.GetRange(0, i + 1), p, q);
                }
            }
            return c;
        }

        // Two boundary points known
        private static Circle MakeCircleTwoPoints(List<CustomPoint> points, CustomPoint p, CustomPoint q)
        {
            Circle circ = MakeDiameter(p, q);
            Circle left = Circle.INVALID;
            Circle right = Circle.INVALID;

            // For each point not in the two-point circle
            CustomPoint pq = q.Subtract(p);
            foreach (CustomPoint r in points)
            {
                if (circ.Contains(r))
                    continue;

                // Form a circumcircle and classify it on left or right side
                double cross = pq.Cross(r.Subtract(p));
                Circle c = MakeCircumcircle(p, q, r);
                if (c.r < 0)
                    continue;
                else if (cross > 0 && (left.r < 0 || pq.Cross(c.c.Subtract(p)) > pq.Cross(left.c.Subtract(p))))
                    left = c;
                else if (cross < 0 && (right.r < 0 || pq.Cross(c.c.Subtract(p)) < pq.Cross(right.c.Subtract(p))))
                    right = c;
            }

            // Select which circle to return
            if (left.r < 0 && right.r < 0)
                return circ;
            else if (left.r < 0)
                return right;
            else if (right.r < 0)
                return left;
            else
                return left.r <= right.r ? left : right;
        }


        public static Circle MakeDiameter(CustomPoint a, CustomPoint b)
        {
            CustomPoint c = new CustomPoint((a.x + b.x) / 2, (a.y + b.y) / 2);
            return new Circle(c, Math.Max(c.Distance(a), c.Distance(b)));
        }


        public static Circle MakeCircumcircle(CustomPoint a, CustomPoint b, CustomPoint c)
        {
            // Mathematical algorithm from Wikipedia: Circumscribed circle
            double ox = (Math.Min(Math.Min(a.x, b.x), c.x) + Math.Max(Math.Min(a.x, b.x), c.x)) / 2;
            double oy = (Math.Min(Math.Min(a.y, b.y), c.y) + Math.Max(Math.Min(a.y, b.y), c.y)) / 2;
            double ax = a.x - ox, ay = a.y - oy;
            double bx = b.x - ox, by = b.y - oy;
            double cx = c.x - ox, cy = c.y - oy;
            double d = (ax * (by - cy) + bx * (cy - ay) + cx * (ay - by)) * 2;
            if (d == 0)
                return Circle.INVALID;
            double x = ((ax * ax + ay * ay) * (by - cy) + (bx * bx + by * by) * (cy - ay) + (cx * cx + cy * cy) * (ay - by)) / d;
            double y = ((ax * ax + ay * ay) * (cx - bx) + (bx * bx + by * by) * (ax - cx) + (cx * cx + cy * cy) * (bx - ax)) / d;
            CustomPoint p = new CustomPoint(ox + x, oy + y);
            double r = Math.Max(Math.Max(p.Distance(a), p.Distance(b)), p.Distance(c));
            return new Circle(p, r);
        }
    }

    public struct Circle
    {

        public static readonly Circle INVALID = new Circle(new CustomPoint(0, 0), -1);

        private const double MULTIPLICATIVE_EPSILON = 1 + 1e-14;


        public CustomPoint c;   // Center
        public double r;  // Radius


        public Circle(CustomPoint c, double r)
        {
            this.c = c;
            this.r = r;
        }


        public bool Contains(CustomPoint p)
        {
            return c.Distance(p) <= r * MULTIPLICATIVE_EPSILON;
        }


        public bool Contains(ICollection<CustomPoint> ps)
        {
            foreach (CustomPoint p in ps)
            {
                if (!Contains(p))
                    return false;
            }
            return true;
        }

    }

    public struct CustomPoint
    {

        public double x;
        public double y;


        public CustomPoint(double x, double y)
        {
            this.x = x;
            this.y = y;
        }


        public CustomPoint Subtract(CustomPoint p)
        {
            return new CustomPoint(x - p.x, y - p.y);
        }


        public double Distance(CustomPoint p)
        {
            double dx = x - p.x;
            double dy = y - p.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }


        // Signed area / determinant thing
        public double Cross(CustomPoint p)
        {
            return x * p.y - y * p.x;
        }
    }
}
