using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Socona.Expor.Results.TextIO;
using Socona.Expor.Results;


namespace Socona.Expor.Maths.Geometry
{

    public class XYCurve : IResult, ITextWriteable, IEnumerable<Point>
    {
        /**
         * Simplification threshold
         */
        protected static double THRESHOLD = 1E-13;

        /**
         * X and Y positions
         */
        protected List<double> data;

        /**
         * Label of X axis
         */
        protected String labelx;

        /**
         * Label of Y axis
         */
        protected String labely;

        /**
         * Minimum and maximum for X axis
         */
        protected double minx = Double.PositiveInfinity,
            maxx = Double.NegativeInfinity;

        /**
         * Minimum and maximum for Y axis
         */
        protected double miny = Double.PositiveInfinity,
            maxy = Double.NegativeInfinity;

        /**
         * Constructor with labels
         * 
         * @param labelx Label for X axis
         * @param labely Label for Y axis
         */
        public XYCurve(String labelx, String labely)
        {

            this.data = new List<double>();
            this.labelx = labelx;
            this.labely = labely;
        }

        /**
         * Constructor with size estimate and labels.
         * 
         * @param labelx Label for X axis
         * @param labely Label for Y axis
         * @param size Estimated size (initial allocation size)
         */
        public XYCurve(String labelx, String labely, int size)
        {

            this.data = new List<double>(size * 2);
            this.labelx = labelx;
            this.labely = labely;
        }

        /**
         * Constructor.
         */
        public XYCurve() :
            this("X", "Y")
        {
        }

        /**
         * Constructor with size estimate
         * 
         * @param size Estimated size (initial allocation size)
         */
        public XYCurve(int size) :
            this("X", "Y", size)
        {
        }

        /**
         * Constructor, cloning an existing curve.
         * 
         * @param curve Curve to clone.
         */
        public XYCurve(XYCurve curve) :
            base()
        {
            this.data = new List<double>(curve.data);
            this.labelx = curve.labelx;
            this.labely = curve.labely;
            this.minx = curve.minx;
            this.maxx = curve.maxx;
            this.miny = curve.miny;
            this.maxy = curve.maxy;
        }

        /**
         * Add a coordinate pair, but don't simplify
         * 
         * @param x X coordinate
         * @param y Y coordinate
         */
        public void Add(double x, double y)
        {
            data.Add(x);
            data.Add(y);
            minx = Math.Min(minx, x);
            maxx = Math.Max(maxx, x);
            miny = Math.Min(miny, y);
            maxy = Math.Max(maxy, y);
        }

        /**
         * Add a coordinate pair, performing curve simplification if possible.
         * 
         * @param x X coordinate
         * @param y Y coordinate
         */
        public void AddAndSimplify(double x, double y)
        {
            // simplify curve when possible:
            int len = data.Count;
            if (len >= 4)
            {
                // Look at the previous 2 points
                double l1x = data[(len - 4)];
                double l1y = data[(len - 3)];
                double l2x = data[(len - 2)];
                double l2y = data[(len - 1)];
                // Differences:
                double ldx = l2x - l1x;
                double ldy = l2y - l1y;
                double cdx = x - l2x;
                double cdy = y - l2y;
                // X simplification
                if ((ldx == 0) && (cdx == 0))
                {
                    data.RemoveRange(len - 2, 2);

                }
                // horizontal simplification
                else if ((ldy == 0) && (cdy == 0))
                {
                    data.RemoveRange(len - 2, 2);
                }
                // diagonal simplification
                else if (ldy > 0 && cdy > 0)
                {
                    if (Math.Abs((ldx / ldy) - (cdx / cdy)) < THRESHOLD)
                    {
                        data.RemoveRange(len - 2, 2);
                    }
                }
            }
            Add(x, y);
        }

        /**
         * Get label of x axis
         * 
         * @return label of x axis
         */
        public String GetLabelx()
        {
            return labelx;
        }

        /**
         * Get label of y axis
         * 
         * @return label of y axis
         */
        public String GetLabely()
        {
            return labely;
        }

        /**
         * Minimum on x axis.
         * 
         * @return Minimum on X
         */
        public double GetMinx()
        {
            return minx;
        }

        /**
         * Maximum on x axis.
         * 
         * @return Maximum on X
         */
        public double GetMaxx()
        {
            return maxx;
        }

        /**
         * Minimum on y axis.
         * 
         * @return Minimum on Y
         */
        public double GetMiny()
        {
            return miny;
        }

        /**
         * Maximum on y axis.
         * 
         * @return Maximum on Y
         */
        public double GetMaxy()
        {
            return maxy;
        }

        /**
         * Curve X value at given position
         * 
         * @param off Offset
         * @return X value
         */
        public double GetX(int off)
        {
            return data[(off << 1)];
        }

        /**
         * Curve Y value at given position
         * 
         * @param off Offset
         * @return Y value
         */
        public double GetY(int off)
        {
            return data[((off << 1) + 1)];
        }

        /**
         * Size of curve.
         * 
         * @return curve length
         */
        public int Count
        {
            get { return data.Count >> 1; }
        }

        /**
         * Get an iterator for the curve.
         * 
         * Note: this is <em>not</em> a Java style iterator, since the only way to Get
         * positions is using "next" in Java style. Here, we can have two Getters for
         * current values!
         * 
         * Instead, use this style of iteration: <blockquote>
         * 
         * <pre>
         * {@code 
         * for (XYCurve.Itr it = curve.iterator(); it.valid(); it.advance()) {
         *   doSomethingWith(it.GetX(), it.GetY());
         * }
         * }
         * </pre>
         * 
         * </blockquote>
         * 
         * @return Iterator
         */



        public virtual void WriteToText(TextWriterStream sout, String label)
        {
            sout.CommentPrint(labelx);
            sout.CommentPrint(" ");
            sout.CommentPrint(labely);
            sout.Flush();
            for (int pos = 0; pos < data.Count; pos += 2)
            {
                sout.InlinePrint(data[(pos)]);
                sout.InlinePrint(data[(pos + 1)]);
                sout.Flush();
            }
        }

        public override String ToString()
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("XYCurve[");
            buf.Append(labelx).Append(",").Append(labely).Append(":");
            for (int pos = 0; pos < data.Count; pos += 2)
            {
                buf.Append(" ").Append(data[pos]).Append(",").Append(data[pos + 1]);
            }
            buf.Append("]");
            return buf.ToString();
        }


        public virtual String LongName
        {
            get { return labelx + "-" + labely + "-Curve"; }
        }


        public virtual String ShortName
        {
            get { return (labelx + "-" + labely + "-curve").ToLower(); }
        }

        /**
         * Compute the area under curve for a curve
         * <em>monotonously increasing in X</em>. You might need to relate this to the
         * total area of the chart.
         * 
         * @param curve Curve
         * @return Area under curve.
         */
        public static double AreaUnderCurve(XYCurve curve)
        {
            List<double> data = curve.data;
            double prevx = data[0], prevy = data[1];
            if (prevx > curve.minx)
            {
                throw new InvalidOperationException("Curves must be monotone on X for areaUnderCurve to be valid.");
            }
            double area = 0.0;
            for (int pos = 2; pos < data.Count; pos += 2)
            {
                double curx = data[(pos)], cury = data[(pos + 1)];
                if (prevx > curx)
                {
                    throw new InvalidOperationException("Curves must be monotone on X for areaUnderCurve to be valid.");
                }
                area += (curx - prevx) * (prevy + cury) * .5; // .5 = mean Y
                prevx = curx;
                prevy = cury;
            }
            if (prevx < curve.maxx)
            {
                throw new InvalidOperationException("Curves must be monotone on X for areaUnderCurve to be valid.");
            }
            return area;
        }



        public IEnumerator<Point> GetEnumerator()
        {
            return new XYCurveItr(this.data);


        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new XYCurveItr(this.data);
        }
    } /**
   * Iterator for the curve. 2D, does not follow Java collections style. The
   * reason is that we want to have {@code #GetX()} and {@code #GetY()}
   * operations, which does not work consistently with Java's
   * <code>next()</code> style of iterations.
   * 
   * Instead, use this style of iteration: <blockquote>
   * 
   * <pre>
   * {@code 
   * for (XYCurve.Itr it = curve.iterator(); it.valid(); it.advance()) {
   *   doSomethingWith(it.GetX(), it.GetY());
   * }
   * }
   * </pre>
   * 
   * </blockquote>
   * 
   * @author Erich Schubert
   * 
   * @apiviz.exclude
   */
    public class XYCurveItr : IEnumerator<Point>
    {
        /**
         * Iterator position
         */
        protected int pos = 0;
        private IList<double> points;
        private Point curPoint;
        /**
         * Get x value of current element.
         * 
         * @return X value of current element
         */
        public XYCurveItr(IList<double> points)
        {
            this.points = points;
            pos = -1;
            curPoint = default(Point);
        }



        public Point Current
        {
            get { return curPoint; }
        }

        public void Dispose()
        {
        }

        object System.Collections.IEnumerator.Current
        {
            get { return curPoint; }
        }

        public bool MoveNext()
        {
            if (++pos >= points.Count)
            {
                return false;
            }
            else
            {
                curPoint = new Point { X = points[2 * pos], Y = points[2 * pos + 1] };
            }
            return true;
        }

        public void Reset()
        {
            pos = -1;
        }
    }
    public struct Point { public double X; public double Y;}
}
