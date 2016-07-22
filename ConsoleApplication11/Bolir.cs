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

		//var max = ReadShirtRequestRand(shirtsRequests);
		//ReadShirtsRand(N, shirts, max);

		ReadShirtRequest(shirtsRequests);
		ReadShirts(N, shirts);

		var done = new HashSet<int>();
		if (!Preprocess(N, shirtsRequests, shirts, done))
		{
			Console.WriteLine("Neibb");
			return;
		}

		//var groups = new List<int[]>();

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
				
				shirts[size] -= 1;
				if (shirts[size] == 0) shirts.Remove(size);

				if (DFS(N, shirtsReqs, shirts, done)) return true;

				if (shirts.ContainsKey(size)) shirts[size] += 1;
				else shirts[size] = 1;
				
			}
			done.Remove(i);
			return false;
		}
		return false;
	}

	private static bool Preprocess(int N, ShirtRequest[] shirtsRequests, Dictionary<int, int> shirts, HashSet<int> done)
	{
		bool needToCheckAgain;
		do
		{
			needToCheckAgain = false;
			for (int i = 0; i < N; i++)
			{
				if (done.Contains(i)) continue;

				var req = shirtsRequests[i];
				if (req.Min == req.Max)
				{
					if (shirts.ContainsKey(req.Min))
					{
						done.Add(i);
						shirts[req.Min] -= 1;
						if (shirts[req.Min] == 0)
						{
							needToCheckAgain = true;
							shirts.Remove(req.Min);
						}
					}
					else
					{
						return false;
					}
				}
				else
				{
					var s = shirts.Keys.Where(shirt => shirt >= req.Min && shirt <= req.Max);
					var count = s.Count();
					if (count == 0)
					{
						return false;
					}
					if (count == 1)
					{
						needToCheckAgain = true;
						done.Add(i);
						var size = s.First();
						shirts[size] -= 1;
						if (shirts[size] == 0) shirts.Remove(size);
					}
				}
			}

			if (!needToCheckAgain)
			{
				var modifications = new Dictionary<int, int>();
				foreach (var s in shirts)
				{
					var reqIndex = -1;
					var found = true;
					for (int i = 0; i < N; i++)
					{
						if (done.Contains(i)) continue;

						var sReq = shirtsRequests[i];
						if (sReq.Min <= s.Key && sReq.Max >= s.Key)
						{
							if (reqIndex > -1)
							{
								found = false;
								break;
							}
							reqIndex = i;
						}
					}
					if (found)
					{
						done.Add(reqIndex);
						if (modifications.ContainsKey(s.Key))
							modifications[s.Key] += 1;
						else
							modifications[s.Key] = 1;
						needToCheckAgain = true;
					}
				}
				foreach (var mod in modifications)
				{
					shirts[mod.Key] -= mod.Value;
					if (shirts[mod.Key] <= 0) shirts.Remove(mod.Key);
				}
			}
		} while (needToCheckAgain);
		return true;
	}

	private static void ReadShirts(int N, Dictionary<int, int> shirts)
	{
		var input = Console.ReadLine().Split(' ');
		for (int i = 0; i < N; i++)
		{
			var key = int.Parse(input[i]);
			if (shirts.ContainsKey(key))
				shirts[key] += 1;
			else
				shirts[key] = 1;
		}
	}

	private static void ReadShirtRequest(ShirtRequest[] shirts)
	{
		for (int i = 0; i < shirts.Length; i++)
		{
			var input = Console.ReadLine().Split(' ');
			shirts[i] = new ShirtRequest
			{
				Min = int.Parse(input[0]),
				Max = int.Parse(input[1])
			};
		}
	}


	private static int ReadShirtRequestRand(ShirtRequest[] shirts)
	{
		var rand = new Random();
		var totalMax = 0;
		for (int i = 0; i < shirts.Length; i++)
		{
			var min = rand.Next(1, 101);
			var max = rand.Next(min, 101);
			if (totalMax < max) totalMax = max;
			shirts[i] = new ShirtRequest
			{
				Min = min,
				Max = max
			};
		}
		return totalMax;
	}
	private static void ReadShirtsRand(int N, Dictionary<int, int> shirts, int max)
	{
		var rand = new Random();
		for (int i = 0; i < N; i++)
		{
			var key = rand.Next(1, max + 1);
			if (shirts.ContainsKey(key))
				shirts[key] += 1;
			else
				shirts[key] = 1;
		}
	}
}