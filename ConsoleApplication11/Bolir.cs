using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
	private const int RandSize = 41;

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
		var shirtsReqs = new Dictionary<ShirtRequest, int>();
		var shirts = new Dictionary<int, int>();

		ReadShirtRequest(shirtsReqs, N);
		ReadShirts(N, shirts);

		if (RemoveEqualMinMax(shirtsReqs, shirts) &&
			Process(shirtsReqs, shirts))
		{
			Console.WriteLine("Jebb");
		}
		else
		{
			Console.WriteLine("Neibb");
		}
	}
	//static void Main()
	//{
	//	var N = int.Parse(Console.ReadLine());
	//	var shirtsReqs = new Dictionary<ShirtRequest, int>();
	//	var shirts = new Dictionary<int, int>();
	//	var shirtsReqs2 = new Dictionary<ShirtRequest, int>();
	//	var shirts2 = new Dictionary<int, int>();
	//	var itt = 0;
	//	do
	//	{
	//		do
	//		{
	//			shirts.Clear();
	//			shirtsReqs.Clear();
	//			ReadShirtsRand(N, shirts, ReadShirtRequestRand(N, shirtsReqs));
	//			shirtsReqs2 = shirtsReqs.ToDictionary(i => i.Key, i => i.Value);
	//			shirts2 = shirts.ToDictionary(i => i.Key, i => i.Value);
	//			//Console.WriteLine(itt++);
	//		} while (!Preprocess(shirtsReqs, shirts));

	//		var sw = Stopwatch.StartNew();
	//		if (RemoveEqualMinMax(shirtsReqs, shirts) && 
	//			Process(shirtsReqs, shirts))
	//		{
	//			Console.WriteLine("Jebb " + itt++ + " " + sw.ElapsedMilliseconds);
	//		}
	//		else
	//		{
	//			Console.WriteLine("Neibb " + itt++ + " " + sw.ElapsedMilliseconds);
	//		}
	//		if (sw.ElapsedMilliseconds > 2000)
	//		{
	//			Print(shirtsReqs2, shirts2);
	//		}
	//	} while (true);
	//}
	private static void Print(Dictionary<ShirtRequest, int> shirtsReqs, Dictionary<int, int> shirts)
	{
		foreach (var item in shirtsReqs.OrderBy(i => i.Key.Min))
		{
			for (int i = 0; i < item.Value; i++)
				Console.WriteLine("{0} {1}", item.Key.Min, item.Key.Max);
		}

		var s = shirts.Aggregate("", (i, j) => i + string.Join(" ", Enumerable.Repeat(j.Key, j.Value).ToArray()) + " ");
		Console.WriteLine(s);
	}

	private static bool RemoveEqualMinMax(Dictionary<ShirtRequest, int> shirtsReqs, Dictionary<int, int> shirts)
	{
		var done = new List<ShirtRequest>();
		foreach (var req in shirtsReqs.Keys)
		{
			if (req.Min != req.Max) continue;
			if (shirts.ContainsKey(req.Min) &&
				shirts[req.Min] >= shirtsReqs[req])
			{
				done.Add(req);
				var left = shirts[req.Min] -= shirtsReqs[req];
				if (left == 0) shirts.Remove(req.Min);
			}
			else
				return false;
		}
		foreach (var req in done) shirtsReqs.Remove(req);
		return true;
	}

	private static bool Process(Dictionary<ShirtRequest, int> shirtsReqs, Dictionary<int, int> shirts)
	{
		if (!Preprocess(shirtsReqs, shirts))
		{
			return false;
		}

		if (shirtsReqs.Count == 1)
		{
			var req = shirtsReqs.First().Key;
			var numberOfShirts = shirts.Where(s => s.Key >= req.Min && s.Key <= req.Max).Sum(i => i.Value);
			return shirtsReqs[req] == numberOfShirts;
		}
		var groups = GetRangeGroups(shirtsReqs);
		foreach (var group in groups)
		{
			var numShirts = shirts.Where(s => s.Key >= group.Min && s.Key <= group.Max).Sum(i => i.Value);
			var numReq = shirtsReqs.Where(s => s.Key.Min >= group.Min && s.Key.Max <= group.Max).Sum(i => i.Value);
			if (numReq != numShirts || !DFS(shirtsReqs, shirts, group))
			{
				return false;
			}
		}
		return true;
	}

	private static List<ShirtRequest> GetRangeGroups(Dictionary<ShirtRequest, int> shirtsRequests)
	{
		var groups = new List<ShirtRequest>();
		foreach (var req in shirtsRequests.Keys)
		{
			if (groups.Count == 0) groups.Add(req);
			var fittingGroup = groups.FirstOrDefault(g => g.Min <= req.Min && g.Max >= req.Max);
			if (!fittingGroup.Equals(default(ShirtRequest))) continue; //Group already Found

			var min = req.Min;
			if (groups.Any(g => req.Min >= g.Min && req.Min <= g.Max))
				min = groups.Where(g => req.Min >= g.Min && req.Min <= g.Max).Min(g => g.Min);

			var max = req.Max;
			if (groups.Any(g => req.Max >= g.Min && req.Max <= g.Max))
				max = groups.Where(g => req.Max >= g.Min && req.Max <= g.Max).Max(g => g.Max);

			var newGroup = new ShirtRequest { Min = min, Max = max };

			groups.RemoveAll(g => g.Max <= newGroup.Max && g.Min >= newGroup.Min);
			groups.Add(newGroup);
		}
		return groups;
	}

	private static bool DFS(Dictionary<ShirtRequest, int> shirtsReqs, Dictionary<int, int> shirts, ShirtRequest requestGroup)
	{
		if (shirtsReqs.Count == 0) return true;
		foreach (var req in shirtsReqs.Keys.OrderBy(i => shirts.Count(s => s.Key >= i.Min && s.Key <= i.Max)))
		{
			for (int size = req.Min; size <= req.Max; size++)
			{
				size = shirts.Keys.FirstOrDefault(s => s >= size && s <= req.Max);
				if (size == 0) break;

				shirts[size] -= 1;
				if (shirts[size] == 0) shirts.Remove(size);

				var newShirtReqs = shirtsReqs.Where(i => i.Key.Min >= requestGroup.Min && i.Key.Max <= requestGroup.Max).ToDictionary(i => i.Key, i => i.Value);
				var newShirts = shirts.Where(i => i.Key >= requestGroup.Min && i.Key <= requestGroup.Max).ToDictionary(i => i.Key, i => i.Value);

				newShirtReqs[req] -= 1;
				if (newShirtReqs[req] == 0)
					newShirtReqs.Remove(req);

				if (Process(newShirtReqs, newShirts)) return true;

				if (shirts.ContainsKey(size)) shirts[size] += 1;
				else shirts[size] = 1;
			}
			return false;
		}
		return false;
	}

	private static bool Preprocess(Dictionary<ShirtRequest, int> shirtsRequests, Dictionary<int, int> shirts)
	{
		var done = new List<ShirtRequest>();
		bool needToCheckAgain;
		do
		{
			needToCheckAgain = false;
			done.Clear();
			foreach (var req in shirtsRequests.Keys)
			{
				var count = 0;
				var size = 0;
				foreach (var shirt in shirts.Keys.Where(shirt => shirt >= req.Min && shirt <= req.Max))
				{
					count++;
					if (count > 1) break;
					size = shirt;
				}
				if (count == 0)
				{
					return false;
				}
				if (count == 1)
				{
					needToCheckAgain = true;
					var left = shirts[size] -= shirtsRequests[req];
					if (left < 0) return false;
					if (left == 0) shirts.Remove(size);
					done.Add(req);
				}
			}
			foreach (var req in done) shirtsRequests.Remove(req);

			if (!needToCheckAgain)
			{
				var shirtsDone = new List<int>();
				foreach (var s in shirts)
				{
					var count = 0;
					var req = default(KeyValuePair<ShirtRequest, int>);
					foreach (var shirt in shirtsRequests.Where(i => i.Key.Min <= s.Key && i.Key.Max >= s.Key))
					{
						count++;
						if (count > 1) break;
						req = shirt;
					}
					if (count == 1)
					{
						if (s.Value > req.Value) return false;
						shirtsDone.Add(s.Key);

						var left = shirtsRequests[req.Key] -= s.Value;
						if (left == 0) shirtsRequests.Remove(req.Key);
						needToCheckAgain = true;
					}
				}
				foreach (var shirt in shirtsDone) shirts.Remove(shirt);
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

	private static void ReadShirtRequest(Dictionary<ShirtRequest, int> shirts, int N)
	{
		for (int i = 0; i < N; i++)
		{
			var input = Console.ReadLine().Split(' ');
			var key = new ShirtRequest
			{
				Min = int.Parse(input[0]),
				Max = int.Parse(input[1])
			};
			if (shirts.ContainsKey(key))
				shirts[key] += 1;
			else
				shirts[key] = 1;
		}
	}

	private static int ReadShirtRequestRand(int N, Dictionary<ShirtRequest, int> shirtRequests)
	{
		var rand = new Random();
		var totalMax = 0;
		for (int i = 0; i < N; i++)
		{
			var min = rand.Next(1, RandSize);
			var max = rand.Next(min, RandSize);
			if (totalMax < max) totalMax = max;
			var key = new ShirtRequest
			{
				Min = min,
				Max = max
			};
			if (shirtRequests.ContainsKey(key))
				shirtRequests[key] += 1;
			else
				shirtRequests[key] = 1;
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