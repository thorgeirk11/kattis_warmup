//using System;
//using System.Collections.Generic;
//using System.Linq;

//public struct Point2d
//{
//	public int X; public int Y;
//	public override string ToString() { return X + " " + Y; }
//	internal double DistanceSquaredTo(Point2d p)
//	{
//		return Math.Pow(X - p.X, 2) + Math.Pow(Y - p.Y, 2);
//	}
//}

//class Program
//{
//	static Point2d[] Points;
//	static int N;
//	static void Main()
//	{
//		N = int.Parse(Console.ReadLine());
//		Points = GetPoints(N);
//		var cur = new Point2d();
//		var done = new HashSet<Point2d>();
//		double alwaysClosest = 0;

//		if (Points.All(i => i.X == 0))
//		{
//			var max = Points.Max(i => i.Y);
//			var min = Points.Min(i => i.Y);
//			double dist = 0;
//			if (max >= 0 && min <= 0)
//				dist = Math.Sqrt(max * max) * 2 + Math.Sqrt(min * min) * 2;
//			else if (max > 0 && min > 0)
//				dist = Math.Sqrt(max * max) * 2;
//			else if (max < 0 && min < 0)
//				dist = Math.Sqrt(min * min) * 2;

//			Console.WriteLine(dist);
//			return;
//		}
//		for (int i = 0; i < N; i++)
//		{
//			var next = Closest(cur, done);
//			done.Add(next);
//			alwaysClosest += Math.Sqrt(cur.DistanceSquaredTo(next));
//			cur = next;
//		}
//		alwaysClosest += Math.Sqrt(cur.X * cur.X + cur.Y * cur.Y);



//		var sum = DFS(new Point2d(), 0, alwaysClosest, new HashSet<Point2d>());
//		Console.WriteLine(sum);
//	}

//	static double DFS(Point2d cur, double curDist, double bestDist, HashSet<Point2d> done)

//	{
//		if (curDist > bestDist) return bestDist;
//		if (done.Count == Points.Length)
//		{
//			curDist += Math.Sqrt(cur.X * cur.X + cur.Y * cur.Y);
//			if (curDist > bestDist) return bestDist;
//			else return curDist;
//		}
//		for (int i = 0; i < N; i++)
//		{
//			var p = Points[i];
//			if (done.Contains(p)) continue;
//			var newDist = curDist + Math.Sqrt(cur.DistanceSquaredTo(p));
//			done.Add(p);
//			bestDist = DFS(p, newDist, bestDist, done);
//			done.Remove(p);
//		}
//		return bestDist;
//	}

//	static Point2d Closest(Point2d p, HashSet<Point2d> done)
//	{
//		return (from point in Points
//				where !done.Contains(point)
//				orderby Math.Pow(p.X - point.X, 2) + Math.Pow(p.Y - point.Y, 2)
//				select point).First();
//	}

//	private static Point2d[] GetPoints(int N)
//	{
//		var points = new Point2d[N];
//		for (int i = 0; i < N; i++)
//		{
//			var data = Console.ReadLine().Split(' ');
//			points[i] = new Point2d
//			{
//				X = int.Parse(data[0]),
//				Y = int.Parse(data[1])
//			};
//		}
//		return points;
//	}
//}