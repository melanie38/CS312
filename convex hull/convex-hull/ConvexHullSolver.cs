using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;

namespace _2_convex_hull
{
    class ConvexHullSolver
    {
        System.Drawing.Graphics g;
        System.Windows.Forms.PictureBox pictureBoxView;

        public ConvexHullSolver(System.Drawing.Graphics g, System.Windows.Forms.PictureBox pictureBoxView)
        {
            this.g = g;
            this.pictureBoxView = pictureBoxView;
        }

        public void Refresh()
        {
            // Use this especially for debugging and whenever you want to see what you have drawn so far
            pictureBoxView.Refresh();
        }

        public void Pause(int milliseconds)
        {
            // Use this especially for debugging and to animate your algorithm slowly
            pictureBoxView.Refresh();
            System.Threading.Thread.Sleep(milliseconds);
        }

        public void Solve(List<System.Drawing.PointF> pointList)
        {
            // TODO: Insert your code here
            //throw new NotImplementedException();
           
            //sort the list
            pointList.Sort((a,b) => (a.X.CompareTo(b.X)));
          
            // debugging purpose
            foreach (var point in pointList)
            {
                Debug.WriteLine("{0}, {1}", point.X, point.Y);
            }

            
        }

        private List<System.Drawing.PointF> DrawPolygon(List<System.Drawing.PointF> pointList)
        {
            if (pointList.Count > 2)
            {
                List<System.Drawing.PointF> leftPoints = new List<PointF>();
                List<System.Drawing.PointF> rightPoints = new List<PointF>();

                for (int i = 0; i < pointList.Count; i++)
                {
                    if (i <= pointList.Count / 2)
                    {
                        leftPoints.Add(pointList[i]);
                    }
                    else
                    {
                        rightPoints.Add(pointList[i]);
                    }
                }

                return DrawPolygon(leftPoints);
                //return DrawPolygon(rightPoints);
            }
        }
    }
}
