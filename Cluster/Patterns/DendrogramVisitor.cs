using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Socona.Clustering.Utilities;

namespace Socona.Clustering.Patterns
{
    public class DendrogramVisitor:INodeVisitor
    {
        private int count;
        private double hjv;
        private int cutLevel;
        private int hlevel;
        private double leftMargin;
        private double bottomMargin;
        private double gap;
        private bool drawLabel;
        private double boxx;
        private double boxy;
        private double height;
        private double width;
        Dendrogram dg;
        SortedDictionary<int, KeyValuePair<int, int>> lines;
        SortedDictionary<int, KeyValuePair<double, double>> points;
        public DendrogramVisitor(double hjv, int llevel, int hlevel)
        {
            // TODO: Complete member initialization
            count = 0;
            leftMargin = 30;
            bottomMargin = 20;
            gap = 15;
            drawLabel = true;
            this.hjv = hjv;
            this.cutLevel = llevel;
            Debug.Assert(hlevel >= llevel, "hlevel Must Larger or Equal Than llevel");
            //double x1, y1, x2, y2;
            boxx = 100;
            boxy = 100;
            width = 390;
            height = 540;
            dg = new Dendrogram();
            lines = new SortedDictionary<int, KeyValuePair<int, int>>();
            points = new SortedDictionary<int, KeyValuePair<double, double>>();

            int numLeaves = hlevel - llevel + 1;

            if (numLeaves > 60)
            {
                drawLabel = false;
            }
            if (gap * numLeaves > height - bottomMargin)
            {
                gap = (height - bottomMargin) / numLeaves;
            }
            else
            {
                height = gap * numLeaves + bottomMargin;

            }
            dg.SetBox(boxx, boxy, boxx + width, boxy + height);
            
        }

        public void Visit(LeafNode node)
        {
            double x = boxx + leftMargin;
            double y = bottomMargin + boxy + gap * count;
            ++count;

            dg.DrawCircle(x, y, 1.5);

            if (drawLabel)
            {
                dg.DrawText(x, y, node.Id.ToString());
            }
            points.Add(node.Id, new KeyValuePair<double, double>(x, y));
            Debug.Assert(x != double.MaxValue, "x is invalid");
        }

        public void Visit(InternalNode node)
        {
            if (node.GetChildrenCount() != 2)
            {
                throw new Exception("DendrogramVisitor Can Only Handles Nodes With 2 Children");
            }
            double x, y;
            if (node.Level > cutLevel)
            {
                lines.Add(node.Id, new KeyValuePair<int, int>(node[0].Id, node[1].Id));
                x = node.JoinValue * (width - leftMargin) / hjv + leftMargin + boxx;
                points.Add(node.Id, new KeyValuePair<double, double>(x, double.MaxValue));
                Debug.Assert(x != double.MaxValue, "x is invalid");
                node[0].Accept(this);
                node[1].Accept(this);
            }
            else
            {
                x = boxx + leftMargin;
                y = bottomMargin + boxy + gap * count;
                ++count;
                dg.DrawDot(x, y);
                if (drawLabel)
                {
                    dg.DrawText(x, y, node.Id.ToString());
                }
                points.Add(node.Id, new KeyValuePair<double, double>(x, y));
                Debug.Assert(x != double.MaxValue, "x is invalid");
            }
        }
        
        public void Save(string fileName)
        {
            int topid = 0;
            foreach (var v in lines)
            {
                if (v.Key > topid)
                {
                    topid = v.Key;
                }
            }

            points[topid]=new KeyValuePair<double,double> (points[topid].Key, GetX(topid));
            foreach (var v in lines)
            {
                DrawLink(v.Value.Key, v.Key);
                DrawLink(v.Value.Value, v.Key);
            }
            dg.Save(fileName);
        }
        private double GetX(int id)
        {
            int id0 = lines[id].Key;
            int id1 = lines[id].Value;

            double x1, x2;
            if (points[id0].Value == double.MaxValue)
            {
                x1 = GetX(id0);
                points[id0] = new KeyValuePair<double, double>(points[id0].Key, x1);

                if (points[id1].Value == double.MaxValue)
                {
                    x2 = GetX(id1);
                    points[id1] = new KeyValuePair<double, double>(points[id1].Key, x2);
                }
                else
                {
                    x2 = points[id1].Value;
                }
            }else
            {
                x1 = points[id0].Value;
                if (points[id1].Value == double.MaxValue)
                {
                    x2 = GetX(id1);
                    points[id1] = new KeyValuePair<double, double>(points[id1].Key, x2);
                }
                else
                {
                    x2 = points[id1].Value;
                }
            }
            return 0.5 * (x1 + x2);
        }
        private void DrawLink(int id0,int id1)
        {
            double x1=points[id0].Key;
            double y1=points[id0].Value;

            double x2=points[id1].Key;
            double y2=points[id1].Value;
            Debug.Assert(x1 != double.MaxValue, "x1 is Invalid");
            Debug.Assert(x2 != double.MaxValue, "x2 is Invalid");
            Debug.Assert(y1 != double.MaxValue, "y1 is Invalid");
            Debug.Assert(y2 != double.MaxValue, "y2 is Invalid");
            if(x1==boxx+leftMargin)
            {
                x1+=1.5;
            }
            dg.DrawLine(x1,y1,x2,y1);
            dg.DrawLine(x2,y1,x2,y2);
        }
    }
}
