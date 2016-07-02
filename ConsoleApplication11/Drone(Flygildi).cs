using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public struct Point2D
{
	public int X; public int Y;
	public override string ToString() { return X + " " + Y; }
	internal long DistanceSquaredTo(Point2D p)
	{
		return (X - p.X) * (X - p.X) + (Y - p.Y) * (Y - p.Y);
	}
	internal long DistanceSquaredToOrigin()
	{
		return X * X + Y * Y;
	}
}

public class Node { public Point2D Point; public long Cost; public Node prev; }

class Program
{
	static Point2D[] Points;
	static int N;
	static void Main()
	{
		N = int.Parse(Console.ReadLine());
		Points = GetPoints(N);
		var sw = Stopwatch.StartNew();
		//for (int i = 0; i < 1000; i++)
		//{
			var done = new HashSet<Point2D>();
			var alwaysClosest = AlwaysClosest(done);
			done.Clear();
			var sum = DFS(new Node { Point = new Point2D(), Cost = 0 }, new Node { Cost = long.MaxValue }, done);
			Console.WriteLine(sum.Cost);
		//}
		Console.WriteLine(sw.ElapsedMilliseconds);
	}

	private static double AlwaysClosest(HashSet<Point2D> done)
	{
		var cur = new Point2D();
		double alwaysClosest = 0;
		for (int i = 0; i < N; i++)
		{
			var next = Closest(cur, done);
			done.Add(next);
			var dist = cur.DistanceSquaredTo(next);
			alwaysClosest += dist;
			cur = next;
		}
		return alwaysClosest + cur.X * cur.X + cur.Y * cur.Y; ;
	}

	static Node DFS(Node cur, Node best, HashSet<Point2D> done)
	{
		if (cur.Cost > best.Cost) return best;
		if (done.Count == Points.Length)
		{
			var newDist = cur.Cost + cur.Point.DistanceSquaredToOrigin();
			return new Node { Point = new Point2D(), prev = cur, Cost = newDist };
		}
		for (int i = 0; i < N; i++)
		{
			var p = Points[i];
			if (done.Contains(p)) continue;

			var newDist = cur.Cost + cur.Point.DistanceSquaredTo(p);
			if (newDist + p.DistanceSquaredToOrigin() > best.Cost)
				return best;

			done.Add(p);
			best = DFS(new Node { Point = p, prev = cur, Cost = newDist }, best, done);
			done.Remove(p);
		}
		return best;
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
		var rand = new Random();
		var points = new Point2D[N];
		for (int i = 0; i < N; i++)
		{
			//var data = Console.ReadLine().Split(' ');
			//points[i] = new Point2D
			//{
			//	X = int.Parse(data[0]),
			//	Y = int.Parse(data[1])
			//};

			points[i] = new Point2D
			{
				X = rand.Next(-10000, 10000),
				Y = rand.Next(-10000, 10000)
			};
		}
		return points;
	}
}