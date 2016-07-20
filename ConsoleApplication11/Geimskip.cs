//using System;
//using System.Collections.Generic;

//public struct Sphere
//{
//	public int X; public int Y; public int Z; public int R;
//	public override string ToString() { return string.Format("({0},{1},{2}) R {3}", X, Y, Z, R); }
//	public bool Collides(Sphere other)
//	{
//		var MinRange = R + other.R;
//		var Distance = Math.Sqrt(
//			Math.Pow(X - other.X, 2) +
//			Math.Pow(Y - other.Y, 2) +
//			Math.Pow(Z - other.Z, 2));
//		return MinRange >= Distance;
//	}
//}
//class Program
//{
//	static Sphere[] Ships;
//	static Sphere[] Bombs;
//	static void Main()
//	{
//		var N = int.Parse(Console.ReadLine());
//		Ships = GetSpheres(N);
//		var M = int.Parse(Console.ReadLine());
//		Bombs = GetSpheres(M);

//		var destroyed = new HashSet<int>();
//		var newBombs = new Queue<Sphere>();
//		for (int i = 0; i < N; i++)
//		{
//			var ship = Ships[i];
//			for (int j = 0; j < M; j++)
//			{
//				if (ship.Collides(Bombs[j]))
//				{
//					destroyed.Add(i);
//					ship.R *= 2;
//					newBombs.Enqueue(ship);
//					break;
//				}
//			}
//		}
//		while (newBombs.Count > 0)
//		{
//			var bomb = newBombs.Dequeue();
//			for (int i = 0; i < N; i++)
//			{
//				if (destroyed.Contains(i)) continue;
//				var ship = Ships[i];
//				if (bomb.Collides(ship))
//				{
//					destroyed.Add(i);
//					ship.R *= 2;
//					newBombs.Enqueue(ship);
//				}
//			}
//		}

//		Console.WriteLine(N - destroyed.Count);
//	}

//	private static Sphere[] GetSpheres(int N)
//	{
//		//var rand = new Random();
//		var points = new Sphere[N];
//		for (int i = 0; i < N; i++)
//		{
//			var data = Console.ReadLine().Split(' ');
//			points[i] = new Sphere
//			{
//				X = /*rand.Next(-10000, 10001),*/ int.Parse(data[0]),
//				Y = /*rand.Next(-10000, 10001),*/ int.Parse(data[1]),
//				Z = /*rand.Next(-10000, 10001),*/ int.Parse(data[2]),
//				R = /*rand.Next(1, 1000),*/ int.Parse(data[3])
//			};
//		}
//		return points;
//	}
//}