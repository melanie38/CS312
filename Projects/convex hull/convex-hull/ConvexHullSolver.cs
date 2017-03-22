using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

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

			// sort the list
			pointList.Sort((a, b) => (a.X.CompareTo(b.X)));

			pointList = DrawPolygon(pointList);

			// draw the polygon
			g.DrawPolygon(new Pen(Color.Red), pointList.ToArray());
		}

		private List<PointF> DrawPolygon(List<System.Drawing.PointF> pointList)
		{
			if (pointList.Count < 4)
			{
				return pointList;
			}
			else
			{
				List<PointF> leftPoly, rightPoly = new List<PointF>();

				leftPoly = DrawPolygon(leftHalf(pointList));
				rightPoly = DrawPolygon(rightHalf(pointList));

				return merge(leftPoly, rightPoly);
			}
		}

		private List<PointF> leftHalf(List<PointF> pointList)
		{
			List<System.Drawing.PointF> leftPoints = new List<PointF>();
			for (int i = 0; i < pointList.Count / 2; i++)
			{
				leftPoints.Add(pointList[i]);
			}

			return leftPoints;
		}

		private List<PointF> rightHalf(List<PointF> pointList)
		{
			List<System.Drawing.PointF> rightPoints = new List<PointF>();
			for (int i = pointList.Count / 2; i < pointList.Count; i++)
			{
				rightPoints.Add(pointList[i]);
			}

			return rightPoints;
		}

		private List<PointF> merge(List<PointF> leftPoly, List<PointF> rightPoly)
		{
			Tuple<PointF, PointF> upperCommonTangent = GetUpperCommonTangent(leftPoly, rightPoly);
			Tuple<PointF, PointF> lowerCommonTangent = GetLowerCommonTangent(leftPoly, rightPoly);
			int indexUpperRightPoly = rightPoly.FindIndex(a => a == upperCommonTangent.Item2);
			int indexLowerRightPoly = rightPoly.FindIndex(a => a == lowerCommonTangent.Item2);
			int indexLowerLeftPoly = leftPoly.FindIndex(a => a == lowerCommonTangent.Item1);
			int indexUpperLeftPoly = leftPoly.FindIndex(a => a == upperCommonTangent.Item1);

			// add points clockwise

			List<PointF> mergedPoly = new List<PointF>();
			mergedPoly.Add(upperCommonTangent.Item1);
			mergedPoly.Add(upperCommonTangent.Item2);
			// add points in between
			if (indexUpperRightPoly != indexLowerRightPoly)
			{
				for (int i = indexUpperRightPoly + 1; i < indexLowerRightPoly; i++)
				{
					mergedPoly.Add(rightPoly[i]);
				}
			}

			mergedPoly.Add(lowerCommonTangent.Item2);
			mergedPoly.Add(lowerCommonTangent.Item1);

			if (indexUpperLeftPoly != indexLowerLeftPoly)
			{
				for (int i = indexLowerLeftPoly + 1; i < indexUpperLeftPoly; i++)
				{
					mergedPoly.Add(rightPoly[i]);
				}
			}

			return mergedPoly;
		}

		private Tuple<PointF, PointF> GetUpperCommonTangent(List<PointF> leftPoly, List<PointF> rightPoly)
		{
			// make the rightmost point the first point in the list
			var rightMost = leftPoly[leftPoly.Count - 1];
			var leftMost = rightPoly[0];

			PointF upperCommonTangentLeft, upperCommonTangentRight, lowerCommonTangentRight, lowerCommonTangentLeft;
			bool moveLeft;
			bool moveRight;
			int index;
			double sign;

			do
			{
				moveLeft = false;
				moveRight = false;
				index = 0;
				sign = CalculateSign(leftPoly[leftPoly.Count - 1], rightPoly[index], rightPoly[index + 1]);

				while (sign > 0)
				{
					moveRight = true;
					index++;
					upperCommonTangentRight = rightPoly[index];
					sign = CalculateSign(leftPoly[leftPoly.Count - 1], rightPoly[index], rightPoly[index + 1]);
				}
			}
			while (moveLeft || moveRight);

			return new Tuple<PointF, PointF>(item1: new PointF(0,0), item2: new PointF(0,0));
		}

		private Tuple<PointF, PointF> GetLowerCommonTangent(List<PointF> leftPoly, List<PointF> rightPoly)
		{
			return new Tuple<PointF, PointF>(item1: new PointF(0,0), item2: new PointF(0,0));
		}

		private double CalculateSign(PointF pivot, PointF start, PointF end)
		{
			return (start.X - pivot.X) * (end.Y - pivot.Y) - (end.X - pivot.X) * (start.Y - pivot.Y);
		}
	}
}

/*
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Linq;
using PtF = System.Drawing.PointF;

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


		// My code begins here: This is the mother function that interfaces with the GUI
		public void Solve(List<System.Drawing.PointF> pointList)
		{
			// Sort points
			pointList = pointList.OrderBy(a => a.X).ToList();

			// Compute convex hull
			List<PtF> p = GetConvexHull(pointList);

			// Draw convex hull
			g.DrawPolygon(new Pen(Color.Red), p.ToArray());
		}

		// This function takes a list of points and returns its convex hull (list)
		// It will be called recursively
		public List<PtF> GetConvexHull(List<PtF> pointList)
		{
			// Recursive cases
			// Should never be zero, unless pointList is empty to begin with
			if (pointList.Count == 0)
			{
				throw new Exception("pointlist too small");
			}
			// Base case: return a polygon with no edges or 1 edge
			else if (pointList.Count == 1 || pointList.Count == 2)
			{
				return pointList;
			}
			// Recursive case: break the set in half and merge the result of each subhalf
			else
			{
				List<PtF> s1, s2;
				int midway = pointList.Count / 2;
				s1 = pointList.Take(midway).ToList();
				s2 = pointList.Skip(midway).ToList();
				return Merge(GetConvexHull(s1), GetConvexHull(s2));
			}
		}

		// Takes two clockwise ordered polygons and merges them into a single convex hull
		// Time complexity 
		public List<PtF> Merge(List<PtF> lp, List<PtF> rp)
		{
			// Get upper and lower common tangents
			Tuple<PtF, PtF> uct = UpperCommonTangent(lp, rp);
			Tuple<PtF, PtF> lct = LowerCommonTangent(lp, rp);

			// Add points clockwise beginning at uct
			// Add uct
			List<PtF> merged = new List<PtF>();
			merged.Add(uct.Item1);
			merged.Add(uct.Item2);

			// Index search is O(n) in size of lp/rp
			int u1_idx = lp.FindIndex(a => a == uct.Item1);
			int u2_idx = rp.FindIndex(a => a == uct.Item2);
			int l1_idx = lp.FindIndex(a => a == lct.Item1);
			int l2_idx = rp.FindIndex(a => a == lct.Item2);

			// Add pts between u2 and l2 on rp
			if (l2_idx != u2_idx)
			{
				if (l2_idx > u2_idx)
				{
					for (int i = u2_idx + 1; i < l2_idx; i++)
					{
						merged.Add(rp[i]);
					}
				}
				else
				{
					for (int i = u2_idx + 1; i < rp.Count; i++)
					{
						merged.Add(rp[i]);
					}
					for (int i = 0; i < l2_idx; i++)
					{
						merged.Add(rp[i]);
					}
				}
			}


			// Add lct
			if (u2_idx != l2_idx)
			{
				merged.Add(lct.Item2);
			}
			if (u1_idx != l1_idx)
			{
				merged.Add(lct.Item1);
			}

			// Add pts between lct.1 and uct.1 on lp
			if (l1_idx != u1_idx)
			{
				if (l1_idx < u1_idx)
				{
					for (int i = l1_idx + 1; i < u1_idx; i++)
					{
						merged.Add(lp[i]);
					}
				}
				else
				{
					for (int i = l1_idx + 1; i < lp.Count; i++)
					{
						merged.Add(lp[i]);
					}
					for (int i = 0; i < u1_idx; i++)
					{
						merged.Add(lp[i]);
					}
				}
			}

			return merged;
		}

		// Returns upper common tangent of two polygons
		// Time complexity
		public Tuple<PtF, PtF> UpperCommonTangent(List<PtF> lp, List<PtF> rp)
		{
			// O(n) in size of lp/rp
			PtF l = GetFurthest("right", lp);
			PtF r = GetFurthest("left", rp);
			Tuple<PtF, PtF> tangent = Tuple.Create(l, r);

			bool climb_right = true;
			bool climb_left = true;

			// This while loop iterates once for 1 left climb and 1 right climb
			// It terminates when we can no longer climb either left or right at all
			// O(TODO)
			while (climb_left || climb_right)
			{
				climb_right = false;
				climb_left = false;

				// This loop iterates once for a right climb step
				// It terminates after 1 right climb (or attempt)
				while (true)
				{
					PtF next = GetNext(rp, tangent.Item2);

					// Propose a climb step, end the loop if it is rejected
					double turn = Turn(tangent.Item1, tangent.Item2, next);
					if (turn > 0)
					{
						climb_right = true;
						tangent = Tuple.Create(tangent.Item1, next);
					}
					else
					{
						break;
					}
				}

				// This loop iterates once for a left climb step
				// It terminates after 1 left climb (or attempt)
				while (true)
				{
					PtF next = GetPrev(lp, tangent.Item1);

					// Propose a climb step, end the loop if it is rejected
					double turn = Turn(tangent.Item2, tangent.Item1, next);
					if (turn < 0)
					{
						climb_left = true;
						tangent = Tuple.Create(next, tangent.Item2);
					}
					else
					{
						break;
					}
				}
			}

			return tangent;
		}

		// Returns lower common tangent of two polygons
		// Algorithmically essentially the same as UpperCommonTangent()
		// Time complexity
		public Tuple<PtF, PtF> LowerCommonTangent(List<PtF> lp, List<PtF> rp)
		{
			// O(n) in size of lp/rp
			PtF l = GetFurthest("right", lp);
			PtF r = GetFurthest("left", rp);
			Tuple<PtF, PtF> tangent = Tuple.Create(l, r);

			bool climb_right = true;
			bool climb_left = true;

			// This while loop iterates once for 1 left climb and 1 right climb
			// It terminates when we can no longer climb either left or right at all
			// O(TODO)
			while (climb_left || climb_right)
			{
				climb_right = false;
				climb_left = false;

				// This loop iterates once for a right climb step
				// It terminates after 1 right climb (or attempt)
				while (true)
				{
					PtF next = GetPrev(rp, tangent.Item2);

					// Propose a climb step, end the loop if it is rejected
					double turn = Turn(tangent.Item1, tangent.Item2, next);
					if (turn < 0)
					{
						climb_right = true;
						tangent = Tuple.Create(tangent.Item1, next);
					}
					else
					{
						break;
					}
				}

				// This loop iterates once for a left climb step
				// It terminates after 1 left climb (or attempt)
				while (true)
				{
					PtF next = GetNext(lp, tangent.Item1);

					// Propose a climb step, end the loop if it is rejected
					double turn = Turn(tangent.Item2, tangent.Item1, next);
					if (turn > 0)
					{
						climb_left = true;
						tangent = Tuple.Create(next, tangent.Item2);
					}
					else
					{
						break;
					}
				}
			}

			return tangent;
		}

		// Get the furthest left or right point in a polygon
		// Time complexity O(n) in size of p
		public PtF GetFurthest(string direction, List<PtF> p)
		{
			if (direction != "left" && direction != "right")
			{
				throw new Exception("not a valid direction");
			}

			PtF furthest = p[0];
			foreach (PtF pt in p)
			{
				if (direction == "left" && pt.X < furthest.X)
				{
					furthest = pt;
				}
				else if (direction == "right" && pt.X > furthest.X)
				{
					furthest = pt;
				}
			}
			return furthest;
		}

		// Returns the pseudo-cross product between vectors ab and ac
		// 0 if no turn, >0 if left turn, <0 if right turn
		// Time complexity O(1)
		public double Turn(PtF a, PtF b, PtF c)
		{
			return (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
		}

		// Returns the next element of the clockwise-sorted list after curr
		// Time complexity O(n) in size of p
		public PtF GetNext(List<PtF> p, PtF curr)
		{
			for (int i = 0; i < p.Count; i++)
			{
				if (p[i] == curr)
				{
					if (i == p.Count - 1)
					{
						return p[0];
					}
					return p[i + 1];
				}
			}
			return curr;
		}

		// Returns the previous element of the clockwise-sorted list before curr
		// Time complexity O(n) in size of p
		public PtF GetPrev(List<PtF> p, PtF curr)
		{
			for (int i = 0; i < p.Count; i++)
			{
				if (p[i] == curr)
				{
					if (i == 0)
					{
						return p[p.Count - 1];
					}
					return p[i - 1];
				}
			}
			return curr;
		}
	}
}
*/
