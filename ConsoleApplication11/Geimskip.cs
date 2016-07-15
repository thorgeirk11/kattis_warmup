using System;
using System.Collections.Generic;
using System.Linq;

public struct Sphere
{
	public int X; public int Y; public int Z; public int R;
	public override string ToString() { return string.Format("({0},{1},{2}) R {3}", X, Y, Z, R); }
	public bool Collides(Sphere other)
	{
		var MinRange = R + other.R;
		var Distance = Math.Sqrt(
			Math.Pow(X - other.X, 2) +
			Math.Pow(Y - other.Y, 2) +
			Math.Pow(Z - other.Z, 2));
		return MinRange >= Distance;
	}
}

class Program
{
	static Sphere[] Ships;
	static Sphere[] Bombs;
	static int N;
	static int M;
	static void Main()
	{
		N = int.Parse(Console.ReadLine());
		Ships = GetSpheres(N);
		M = int.Parse(Console.ReadLine());
		Bombs = GetSpheres(M);

		var destroyed = new HashSet<int>();
		var newBombs = new Queue<Sphere>();
		for (int i = 0; i < N; i++)
		{
			var ship = Ships[i];
			for (int j = 0; j < M; j++)
			{
				var bomb = Bombs[j];
				if (ship.Collides(bomb))
				{
					destroyed.Add(i);
					ship.R *= 2;
					newBombs.Enqueue(ship);
					break;
				}
			}
		}
		while (newBombs.Count > 0)
		{
			var bomb = newBombs.Dequeue();
			for (int i = 0; i < N; i++)
			{
				if (destroyed.Contains(i)) continue;
				var ship = Ships[i];
				if (ship.Collides(bomb))
				{
					destroyed.Add(i);
					ship.R *= 2;
					newBombs.Enqueue(ship);
					break;
				}
			}
		}

		Console.WriteLine(N - destroyed.Count);
	}

	private static Sphere[] GetSpheres(int N)
	{
		var spheres = new Sphere[N];
		for (int i = 0; i < N; i++)
		{
			var data = Console.ReadLine().Split(' ');
			spheres[i] = new Sphere
			{
				X = int.Parse(data[0]),
				Y = int.Parse(data[1]),
				Z = int.Parse(data[2]),
				R = int.Parse(data[3])
			};
		}
		return spheres;
	}
}