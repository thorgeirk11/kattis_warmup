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
		var done = new HashSet<Point2D>();
		var sums = new double[N + 1];
		var alwaysClosest = AlwaysClosest(done, sums);
		done.Clear();
		var sums2 = new double[N + 1];
		var sum = DFS(new Point2D(), 0, double.MaxValue, sums2, done);
		Console.WriteLine(sum);
	}

	private static double AlwaysClosest(HashSet<Point2D> done, double[] sums)
	{
		var cur = new Point2D();
		double alwaysClosest = 0;
		for (int i = 0; i < N; i++)
		{
			var next = Closest(cur, done);
			done.Add(next);
			var dist = cur.DistanceSquaredTo(next);
			alwaysClosest += dist;
			sums[i] = dist;
			cur = next;
		}
		var d = cur.X * cur.X + cur.Y * cur.Y;
		alwaysClosest += d;
		sums[N] = d;
		return alwaysClosest;
	}

	static double DFS(Point2D cur, double curDist, double bestDist, double[] sums, HashSet<Point2D> done)
	{
		if (curDist > bestDist) return bestDist;
		if (done.Count == Points.Length)
		{
			curDist += cur.X * cur.X + cur.Y * cur.Y;
			sums[done.Count] = curDist;
			if (curDist > bestDist) return bestDist;
			else return curDist;
		}
		for (int i = 0; i < N; i++)
		{
			var p = Points[i];
			if (done.Contains(p)) continue;

			var newDist = curDist + cur.DistanceSquaredTo(p);
			if (newDist + cur.X * cur.X + cur.Y * cur.Y > bestDist) return bestDist;
			sums[done.Count] = newDist;
			done.Add(p);
			bestDist = DFS(p, newDist, bestDist, sums, done);
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