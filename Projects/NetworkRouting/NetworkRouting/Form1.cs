using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NetworkRouting
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void clearAll()
		{
			startNodeIndex = -1;
			stopNodeIndex = -1;
			sourceNodeBox.Clear();
			sourceNodeBox.Refresh();
			targetNodeBox.Clear();
			targetNodeBox.Refresh();
			arrayTimeBox.Clear();
			arrayTimeBox.Refresh();
			heapTimeBox.Clear();
			heapTimeBox.Refresh();
			differenceBox.Clear();
			differenceBox.Refresh();
			pathCostBox.Clear();
			pathCostBox.Refresh();
			arrayCheckBox.Checked = false;
			arrayCheckBox.Refresh();
			return;
		}

		private void clearSome()
		{
			arrayTimeBox.Clear();
			arrayTimeBox.Refresh();
			heapTimeBox.Clear();
			heapTimeBox.Refresh();
			differenceBox.Clear();
			differenceBox.Refresh();
			pathCostBox.Clear();
			pathCostBox.Refresh();
			return;
		}

		private void generateButton_Click(object sender, EventArgs e)
		{
			int randomSeed = int.Parse(randomSeedBox.Text);
			int size = int.Parse(sizeBox.Text);

			Random rand = new Random(randomSeed);
			seedUsedLabel.Text = "Random Seed Used: " + randomSeed.ToString();

			clearAll();
			this.adjacencyList = generateAdjacencyList(size, rand);
			List<PointF> points = generatePoints(size, rand);
			resetImageToPoints(points);
			this.points = points;
		}

		// Generates the distance matrix.  Values of -1 indicate a missing edge.  Loopbacks are at a cost of 0.
		private const int MIN_WEIGHT = 1;
		private const int MAX_WEIGHT = 100;
		private const double PROBABILITY_OF_DELETION = 0.35;

		private const int NUMBER_OF_ADJACENT_POINTS = 3;

		private List<HashSet<int>> generateAdjacencyList(int size, Random rand)
		{
			List<HashSet<int>> adjacencyList = new List<HashSet<int>>();

			for (int i = 0; i < size; i++)
			{
				HashSet<int> adjacentPoints = new HashSet<int>();
				while (adjacentPoints.Count < 3)
				{
					int point = rand.Next(size);
					if (point != i) adjacentPoints.Add(point);
				}
				adjacencyList.Add(adjacentPoints);
			}

			return adjacencyList;
		}

		private List<PointF> generatePoints(int size, Random rand)
		{
			List<PointF> points = new List<PointF>();
			for (int i = 0; i < size; i++)
			{
				points.Add(new PointF((float)(rand.NextDouble() * pictureBox.Width), (float)(rand.NextDouble() * pictureBox.Height)));
			}
			return points;
		}

		private void resetImageToPoints(List<PointF> points)
		{
			pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
			Graphics graphics = Graphics.FromImage(pictureBox.Image);
			Pen pen;

			if (points.Count < 100)
				pen = new Pen(Color.Blue);
			else
				pen = new Pen(Color.LightBlue);
			foreach (PointF point in points)
			{
				graphics.DrawEllipse(pen, point.X, point.Y, 2, 2);
			}

			this.graphics = graphics;
			pictureBox.Invalidate();
		}

		// These variables are instantiated after the "Generate" button is clicked
		private List<PointF> points = new List<PointF>();
		private Graphics graphics;
		private List<HashSet<int>> adjacencyList;

		// Use this to generate paths (from start) to every node; then, just return the path of interest from start node to end node
		private void solveButton_Click(object sender, EventArgs e)
		{
			// This was the old entry point, but now it is just some form interface handling
			bool ready = true;

			if (startNodeIndex == -1)
			{
				sourceNodeBox.Focus();
				sourceNodeBox.BackColor = Color.Red;
				ready = false;
			}
			if (stopNodeIndex == -1)
			{
				if (!sourceNodeBox.Focused)
					targetNodeBox.Focus();
				targetNodeBox.BackColor = Color.Red;
				ready = false;
			}
			if (points.Count > 0)
			{
				resetImageToPoints(points);
				paintStartStopPoints();
			}
			else
			{
				ready = false;
			}
			if (ready)
			{
				clearSome();
				solveButton_Clicked();  // Here is the new entry point
			}
		}

		private void solveButton_Clicked()
		{
			// *** Implement this method, use the variables "startNodeIndex" and "stopNodeIndex" as the indices for your start and stop points, respectively ***

			if (arrayCheckBox.Checked)
			{
				var watch = System.Diagnostics.Stopwatch.StartNew();

				djikstraArray();

				watch.Stop();
				var elapsedMs = watch.Elapsed.TotalSeconds.ToString();

				arrayTimeBox.Text = elapsedMs;
			}
			else
			{
				var watch = System.Diagnostics.Stopwatch.StartNew();

				djikstraHeap();

				watch.Stop();
				var elapsedMs = watch.Elapsed.TotalSeconds.ToString();

				heapTimeBox.Text = elapsedMs;
			}
		}

		private const int DIST = 0;
		private const int PREV = 1;
		private const int VISITED = 2;
		private const int INDEX = 3;

		private void djikstraArray()
		{
			List<int[]> ipath = new List<int[]>();

			initialization(ipath);

			for (int i = 0; i < ipath.Count; i++)
			{
				var id = findmindist(ipath);

				if (id == -1)
				{
					pathCostBox.Text = "unreachable";
					break;
				}

				var node = ipath[id];

				if (node[INDEX] == stopNodeIndex && node[DIST] != 0)
				{
					printpath(node, ipath);
					break;
				}

				var neighbor = adjacencyList[node[INDEX]];

				foreach (int vertex in neighbor)
				{
					var neigh = ipath[vertex];
					var index1 = node[INDEX];
					var index2 = vertex;

					var dist = node[DIST] + eucldist(points[index1], points[index2]);

					if (dist < neigh[DIST])
					{
						neigh[DIST] = dist;
						neigh[PREV] = node[INDEX];
					}

				}
				node[VISITED] = 1;
			}
		}

		private void djikstraHeap()
		{
			// where is array
			// heap
			// list of indexes
			// bubble up start node at insertio

			var whereis = new List<int>();
			var heap = new List<int>();
			var indexes = new List<int>();

			foreach (PointF vertex in points)
			{
				var nodeid = points.IndexOf(vertex);
				var info = new int[4];

				if (nodeid != startNodeIndex)
				{
					info[DIST] = Int32.MaxValue;
				}
				else
				{
					info[DIST] = 0;
					info[VISITED] = 0;
				}

				info[INDEX] = nodeid;

				// Priority list
				//ipath.Add(info);
			}
		}

		private void printpath(int[] node, List<int[]> ipath)
		{
			pathCostBox.Text = node[DIST].ToString();
			// draw line
			while (node[INDEX] != startNodeIndex)
			{
				PointF pt1 = points[node[INDEX]];
				PointF pt2 = points[node[PREV]];
				var dist = eucldist(pt1, pt2);

				printline(pt1, pt2);
				printdist(dist, pt1);

				node = ipath[node[PREV]];
			}
		}
		private void initialization(List<int[]> ipath)
		{
			foreach (PointF vertex in points)
			{
				var nodeid = points.IndexOf(vertex);
				var info = new int[4];

				if (nodeid != startNodeIndex)
				{
					info[DIST] = Int32.MaxValue;
				}
				else
				{
					info[DIST] = 0;
					info[VISITED] = 0;
				}

				info[INDEX] = nodeid;

				// Priority listt
				ipath.Add(info);
			}
		}

		private void insert(List<int> heap, int node)
		{
			bubbleup(heap, node, heap.Count + 1);
		}

		private void bubbleup(List<int> heap, int node, int i)
		{
			var p = (int) Math.Ceiling((double) i / 2);

			while (i != 1 && heap[p] > heap[node])
			{
				heap[i] = heap[p];
				i = p;
				p = (int) Math.Ceiling((double) i / 2);
			}

			heap[i] = node;
				
		}

		private void percolate()
		{
			
		}

		private void decreaseKey(List<int> heap, int node)
		{
			//bubbleup(heap, node, );
		}

		private void deletemin()
		{
		}

		private int minchild(List<int> heap, int i)
		{
			if (2 * i > heap.Count)
			{
				return 0;
			}
			else
			{
				return 0;
			}
		}

		private void printline(PointF pt1, PointF pt2)
		{
			Pen pen = new Pen(Color.Blue, 3);

			graphics.DrawLine(pen, pt1, pt2); ;
		}

		private void printdist(int dist, PointF pt1)
		{
			var drawFont = new Font("Arial", 16);
			var drawBrush = new SolidBrush(Color.Black);

			graphics.DrawString(dist.ToString(), drawFont, drawBrush, pt1);
		}

		private int findmindist(List<int[]> ipath)
		{
			var index = -1;
			var min = Int32.MaxValue;

			for (var i = 0; i < ipath.Count; i++)
			{
				var vertex = ipath[i];

				if (vertex[VISITED] == 0)
				{
					if (vertex[DIST] < min)
					{
						index = i;
						min = ipath[i][DIST];
					}
				}
			}

			return index;
		}

		/*
		 * Time complexity = O(1) since the computation is independant of the number of points
		 */
		public int eucldist(PointF v1, PointF v2)
		{
			float dx = v1.X - v2.X;
			float dy = v1.Y - v2.Y;

			return (int) Math.Sqrt(Math.Pow((double) dx, 2) + Math.Pow((double) dy, 2));
		}

        private Boolean startStopToggle = true;
        private int startNodeIndex = -1;
        private int stopNodeIndex = -1;
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (points.Count > 0)
            {
                Point mouseDownLocation = new Point(e.X, e.Y);
                int index = ClosestPoint(points, mouseDownLocation);
                if (startStopToggle)
                {
                    startNodeIndex = index;
                    sourceNodeBox.ResetBackColor();
                    sourceNodeBox.Text = "" + index;
                }
                else
                {
                    stopNodeIndex = index;
                    targetNodeBox.ResetBackColor();
                    targetNodeBox.Text = "" + index;
                }
                resetImageToPoints(points);
                paintStartStopPoints();
            }
        }

        private void sourceNodeBox_Changed(object sender, EventArgs e)
        {
            if (points.Count > 0)
            {
                try{ startNodeIndex = int.Parse(sourceNodeBox.Text); }
                catch { startNodeIndex = -1; }
                if (startNodeIndex < 0 | startNodeIndex > points.Count-1)
                    startNodeIndex = -1;
                if(startNodeIndex != -1)
                {
                    sourceNodeBox.ResetBackColor();
                    resetImageToPoints(points);
                    paintStartStopPoints();
                    startStopToggle = !startStopToggle;
                }
            }
        }

        private void targetNodeBox_Changed(object sender, EventArgs e)
        {
            if (points.Count > 0)
            {
                try { stopNodeIndex = int.Parse(targetNodeBox.Text); }
                catch { stopNodeIndex = -1; }
                if (stopNodeIndex < 0 | stopNodeIndex > points.Count-1)
                    stopNodeIndex = -1;
                if(stopNodeIndex != -1)
                {
                    targetNodeBox.ResetBackColor();
                    resetImageToPoints(points);
                    paintStartStopPoints();
                    startStopToggle = !startStopToggle;
                }
            }
        }
        
        private void paintStartStopPoints()
        {
            if (startNodeIndex > -1)
            {
                Graphics graphics = Graphics.FromImage(pictureBox.Image);
                graphics.DrawEllipse(new Pen(Color.Green, 6), points[startNodeIndex].X, points[startNodeIndex].Y, 1, 1);
                this.graphics = graphics;
                pictureBox.Invalidate();
            }

            if (stopNodeIndex > -1)
            {
                Graphics graphics = Graphics.FromImage(pictureBox.Image);
                graphics.DrawEllipse(new Pen(Color.Red, 2), points[stopNodeIndex].X - 3, points[stopNodeIndex].Y - 3, 8, 8);
                this.graphics = graphics;
                pictureBox.Invalidate();
            }
        }

        private int ClosestPoint(List<PointF> points, Point mouseDownLocation)
        {
            double minDist = double.MaxValue;
            int minIndex = 0;

            for (int i = 0; i < points.Count; i++)
            {
                double dist = Math.Sqrt(Math.Pow(points[i].X-mouseDownLocation.X,2) + Math.Pow(points[i].Y - mouseDownLocation.Y,2));
                if (dist < minDist)
                {
                    minIndex = i;
                    minDist = dist;
                }
            }

            return minIndex;
        }
    }
}
