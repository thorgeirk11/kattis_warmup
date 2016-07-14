using System;
using System.Collections.Generic;
using System.Linq;

public struct Point2D
{
	public int X; public int Y;
	public override string ToString() { return X + " " + Y; }
	internal double DistanceSquaredTo(Point2D p)
	{
		return Math.Pow(X - p.X, 2) + Math.Pow(Y - p.Y, 2);
	}
}

class Program
{
	static Point2D[] Points;
	static int N;
	static void Main()
	{
		N = int.Parse(Console.ReadLine());
		Points = GetPoints(N);
		var cur = new Point2D();
		var done = new HashSet<Point2D>();
		double alwaysClosest = 0;

		if (Points.All(i => i.X == 0))
		{
			var max = Points.Max(i => i.Y);
			var min = Points.Min(i => i.Y);
			double dist = 0;
			if (max >= 0 && min <= 0)
				dist = Math.Sqrt(max * max) * 2 + Math.Sqrt(min * min) * 2;
			else if (max > 0 && min > 0)
				dist = Math.Sqrt(max * max) * 2;
			else if (max < 0 && min < 0)
				dist = Math.Sqrt(min * min) * 2;

			Console.WriteLine(dist);
			return;
		}
		for (int i = 0; i < N; i++)
		{
			var next = Closest(cur, done);
			done.Add(next);
			alwaysClosest += Math.Sqrt(cur.DistanceSquaredTo(next));
			cur = next;
		}
		alwaysClosest += Math.Sqrt(cur.X * cur.X + cur.Y * cur.Y);



		var sum = DFS(new Point2D(), 0, alwaysClosest, new HashSet<Point2D>());
		Console.WriteLine(sum);
	}

	static double DFS(Point2D cur, double curDist, double bestDist, HashSet<Point2D> done)

	{
		if (curDist > bestDist) return bestDist;
		if (done.Count == Points.Length)
		{
			curDist += Math.Sqrt(cur.X * cur.X + cur.Y * cur.Y);
			if (curDist > bestDist) return bestDist;
			else return curDist;
		}
		for (int i = 0; i < N; i++)
		{
			var p = Points[i];
			if (done.Contains(p)) continue;
			var newDist = curDist + Math.Sqrt(cur.DistanceSquaredTo(p));
			done.Add(p);
			bestDist = DFS(p, newDist, bestDist, done);
			done.Remove(p);
		}
		return bestDist;
	}

	static Point2D Closest(Point2D p, HashSet<Point2D> done)
	{
		return (from point in Points
				where !done.Contains(point)
				orderby Math.Pow(p.X - point.X, 2) + Math.Pow(p.Y - point.Y, 2)
				select point).First();
	}

	private static Point2D[] GetPoints(int N)
	{
		var points = new Point2D[N];
		for (int i = 0; i < N; i++)
		{
			var data = Console.ReadLine().Split(' ');
			points[i] = new Point2D
			{
				X = int.Parse(data[0]),
				Y = int.Parse(data[1])
			};
		}
		return points;
	}
}