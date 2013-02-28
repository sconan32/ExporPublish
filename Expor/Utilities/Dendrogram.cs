using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Socona.Expor.Utilities
{
    public class Dendrogram
    {
        private StringBuilder sb;
        private double x1, y1;
        private double x2, y2;
        public Dendrogram()
        { 
            sb = new StringBuilder();
            x1 = 0;
            x2 = 100;
            y1 = 0;
            y2 = 100;
        }
        public void SetBox(double x1, double y1, double x2, double y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        public void DrawDot(double x, double y)
        {
            sb.Append("% Dot\n");
            sb.Append(" 3 slw ");
            sb.Append(" 1 slc ");
            sb.Append(" 0 slj ");
            sb.Append("n " + x.ToString("0.####") + " " + y.ToString("0.####") + " ");
            sb.Append("m " + x + " " + y + " ");
            sb.Append("1 0.0000 0.0000 0.0000 srgb stroke");
            sb.Append("\n");
        }
        public void DrawCircle(double x, double y, double r)
        {
            sb.Append("% Ellipse\n");
            sb.Append(" 0.3 slw ");
            sb.Append(" 1 slc ");
            sb.Append(" 0 slj \n");
            sb.Append("gs " + x.ToString("0.####") + " " + y.ToString("0.####") + " tr");
            sb.Append(" n " + r.ToString("0.####") + " 0 m 0 0 " + r.ToString("0.####") + " 0.0 360.0 arc ");
            sb.Append(" 0.0000 0.0000 0.0000 srgb stroke gr");
            sb.Append("\n");
        }
        public void DrawLine(double x1, double y1, double x2, double y2)
        {
            sb.Append("% Line\n");
            sb.Append(" 0.3 slw ");
            sb.Append(" 1 slc ");
            sb.Append(" 0 slj ");
            sb.Append("n " + x1.ToString("0.####") + " " + y1.ToString("0.####") + " ");
            sb.Append("m " + x2.ToString("0.####") + " " + y2.ToString("0.####") + " ");
            sb.Append("l 0.0000 0.0000 0.0000 srgb stroke");
            sb.Append("\n");
        }
        public void DrawText(double x, double y, string text)
        {
            sb.Append("% Text\n");
            sb.Append("gs /Times-Roman ff 8 scf sf ");
            sb.Append("n " + (x - 7 - text.Length * 3).ToString("0.####") + " " + (y - 3).ToString("0.####") + " ");
            sb.Append("m (" + text+ ")");
            sb.Append(" 0.0000 0.0000 0.0000 srgb sh gr");
            sb.Append("\n");
        }
        public void Save(string fileName)
        {

            FileStream fs;
            if (File.Exists(fileName))
                fs = File.Open(fileName, FileMode.Truncate, FileAccess.Write);
            else
                fs = File.OpenWrite(fileName);
            StreamWriter sr = new StreamWriter(fs);
            
            StringBuilder tsb = new StringBuilder();
            tsb.Append("%!PS-Adobe-2.0 EPSF-2.0\n");
            tsb.Append("%%Title: " + fileName + "\n");
            tsb.Append("%%Creator: Socona Clustering Lib \n");
            tsb.Append("%%CreationDate: " +
                DateTime.Now.ToShortDateString() + " " +
                DateTime.Now.ToShortTimeString() + "\n");
            tsb.Append("%%BoundingBox: " +
                string.Format("{0} {1} {2} {3}\n",
                x1.ToString("0.########"), y1.ToString("0.########"), 
                x2.ToString("0.########"), y2.ToString("0.########")));
            //tsb.Append("%Magnification: 1.0000\n");
            tsb.Append("%%EndComments\n\n");

            tsb.Append("/cp {closepath} bind def\n");
            tsb.Append("/ef {eofill} bind def\n");
            tsb.Append("/gr {grestore} bind def\n");
            tsb.Append("/gs {gsave} bind def\n");
            tsb.Append("/sa {save} bind def\n");
            tsb.Append("/rs {restore} bind def\n");
            tsb.Append("/l {lineto} bind def\n");
            tsb.Append("/m {moveto} bind def\n");
            tsb.Append("/rm {rmoveto} bind def\n");
            tsb.Append("/n {newpath} bind def\n");
            tsb.Append("/s {stoke} bind def\n");
            tsb.Append("/sh {show} bind def\n");
            tsb.Append("/slc {setlinecap} bind def\n");
            tsb.Append("/slj {setlinejoin} bind def\n");
            tsb.Append("/slw {setlinewidth} bind def\n");
            tsb.Append("/srgb {setrgbcolor} bind def\n");
            tsb.Append("/rot {rotate} bind def\n");
            tsb.Append("/sc {scale} bind def\n");
            tsb.Append("/sd {setdash} bind def\n");
            tsb.Append("/ff {findfont} bind def\n");
            tsb.Append("/sf {setfont} bind def\n");
            tsb.Append("/scf {scalefont} bind def\n");
            tsb.Append("/sw {stringwidth} bind def\n");
            tsb.Append("/sd {setdash} bind def\n");
            tsb.Append("/tr {translate} bind def\n");

            tsb.Append(" 0.5 setlinewidth\n");
            tsb.Append(sb.ToString() + "\n");
            tsb.Append("showpage\n");
            tsb.Append("%%Trailer\n");
            tsb.Append("%EOF\n");
            sr.Write(tsb.ToString());
            sr.Close();
            fs.Close();

        }
    }
}
