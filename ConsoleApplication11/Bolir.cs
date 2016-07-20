using System;
using System.Linq;
using System.Collections.Generic;

class Program
{
	struct ShirtRequest
	{
		public int Min; public int Max;
		public override string ToString()
		{
			return "(" + Min + ", " + Max + ")";
		}
	}

	static void Main()
	{
		var N = int.Parse(Console.ReadLine());
		var shirtsRequests = new ShirtRequest[N];
		var shirts = new Dictionary<int, int>();

		var max = ReadShirtRequest(shirtsRequests);
		ReadShirts(N, shirts, max);

		var done = new HashSet<int>();
		if (!Preprocess(N, shirtsRequests, shirts, done))
		{
			Console.WriteLine("Neibb");
			return;
		}

		var answer = DFS(N, shirtsRequests, shirts, done);
		if (answer)
		{
			Console.WriteLine("Jebb");
		}
		else
		{
			Console.WriteLine("Neibb");
		}
	}

	private static bool DFS(int N, ShirtRequest[] shirtsReqs, Dictionary<int, int> shirts, HashSet<int> done)
	{
		if (done.Count == N) return true;

		for (int i = 0; i < N; i++)
		{
			if (done.Contains(i)) continue;

			var req = shirtsReqs[i];
			done.Add(i);

			for (int size = req.Min; size <= req.Max; size++)
			{
				size = shirts.Keys.FirstOrDefault(s => s >= size && s <= req.Max);
				if (size == 0) break;
				if (shirts[size] != 0)
				{
					shirts[size] -= 1;
					if (DFS(N, shirtsReqs, shirts, done)) return true;
					shirts[size] += 1;
				}
			}
			done.Remove(i);
		}
		return false;
	}

	private static bool Preprocess(int N, ShirtRequest[] shirtsRequests, Dictionary<int, int> shirts, HashSet<int> done)
	{
		for (int i = 0; i < N; i++)
		{
			var req = shirtsRequests[i];
			if (req.Min == req.Max)
			{
				if (shirts.ContainsKey(req.Min))
				{
					done.Add(i);
					shirts[req.Min] -= 1;
					if (shirts[req.Min] == 0) shirts.Remove(req.Min);
				}
				else
				{
					return false;
				}
			}
			else if (!shirts.Keys.Any(shirt => shirt >= req.Min && shirt <= req.Max))
			{
				return false;
			}
		}
		return true;
	}

	private static void ReadShirts(int N, Dictionary<int, int> shirts, int max)
	{
		var rand = new Random();
		//var input = Console.ReadLine().Split(' ');
		for (int i = 0; i < N; i++)
		{
			var key = rand.Next(1, max + 1); //int.Parse(input[i]);
			if (shirts.ContainsKey(key))
				shirts[key] += 1;
			else
				shirts[key] = 1;
		}
	}

	private static int ReadShirtRequest(ShirtRequest[] shirts)
	{
		var rand = new Random();
		var totalMax = 0;
		for (int i = 0; i < shirts.Length; i++)
		{
			//var input = Console.ReadLine().Split(' ');
			var min = rand.Next(1, 1001);
			var max = rand.Next(min, 1001);
			if (totalMax < max) totalMax = max;
			shirts[i] = new ShirtRequest
			{
				Min = min,
				Max = max
				//Min = int.Parse(input[0]),
				//Max = int.Parse(input[1])
			};
		}
		return totalMax;
	}
}