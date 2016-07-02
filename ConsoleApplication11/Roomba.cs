//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text.RegularExpressions;

//class Program
//{
//	class Node : IComparable<Node>
//	{
//		public Node(int x, int y, Node prev)
//		{
//			Point = new Point { X = x, Y = y };
//			Covered = GetPath.Select(i => i.Point).Distinct().Count();
//			PrevNode = prev;
//			Depth = prev == null ? 0 : prev.Depth + 1;
//		}
//		public Point Point { get; private set; }
//		public Node PrevNode { get; private set; }
//		public int Depth { get; private set; }
//		public int Covered { get; private set; }
//		public IEnumerable<Node> GetPath
//		{
//			get
//			{
//				if (PrevNode != null)
//					return new[] { this }.Concat(PrevNode.GetPath);
//				else
//					return Enumerable.Empty<Node>();
//			}
//		}
//		public int DistanceToOrigin()
//		{
//			return Point.X + Point.Y;
//		}

//		public int CompareTo(Node other)
//		{
//			return Covered;
//		}
//	}
//	struct Point
//	{
//		public int X { get; set; }
//		public int Y { get; set; }
//	}
//	static readonly Point Origin = new Point();

//	static void Main()
//	{
//		var line = Console.ReadLine().Split(' ');
//		var N = int.Parse(line[0]);
//		var M = int.Parse(line[1]);

//		var cur = new Node(0, 0, null);
//		var queue = new SortedSet<Node> { cur };
//		var set = new HashSet<Point>();
//		while (cur.Covered == N * M &&
//			   cur.Point.Equals(Origin))
//		{
//			cur = queue.First();
//			queue.Remove(cur);

//			foreach (var item in GetPossibleDirections(N, M, cur))
//			{
//				if (!set.Contains(item.Point))
//				{
//					queue.Add(item);
//				}
//			}
//		}

//		foreach (var item in cur.GetPath)
//		{
//			Console.WriteLine(item.Point);
//		}
//		Console.ReadLine();
//	}

//	static IEnumerable<Node> GetPossibleDirections(int N, int M, Node cur)
//	{
//		if (cur.Point.X + 1 < M)
//		{
//			yield return new Node(cur.Point.X + 1, cur.Point.Y, cur);
//		}
//		if (cur.Point.X - 1 >= 0)
//		{
//			yield return new Node(cur.Point.X - 1, cur.Point.Y, cur);
//		}
//		if (cur.Point.Y + 1 < N)
//		{
//			yield return new Node(cur.Point.X, cur.Point.Y + 1, cur);
//		}
//		if (cur.Point.Y - 1 >= 0)
//		{
//			yield return new Node(cur.Point.X, cur.Point.Y - 1, cur);
//		}
//	}

//}