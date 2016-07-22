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
		var shirtsRequests = new Dictionary<ShirtRequest, int>();
		var shirts = new Dictionary<int, int>();

		//ReadShirtsRand(N, shirts, ReadShirtRequestRand(N, shirtsRequests));

		ReadShirtRequest(shirtsRequests, N);
		ReadShirts(N, shirts);

		if (!Preprocess(shirtsRequests, shirts))
		{
			Console.WriteLine("Neibb");
			return;
		}


		if (shirtsRequests.Count == 1)
		{
			var req = shirtsRequests.First().Key;
			var numberOfShirts = shirts.Where(s => s.Key >= req.Min && s.Key <= req.Max).Sum(i => i.Value);
			if (shirtsRequests[req] == numberOfShirts)
			{
				Console.WriteLine("Jebb");
			}
			else
			{
				Console.WriteLine("Neibb");
			}
			return;
		}


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

		foreach (var group in groups)
		{
			if (!DFS(shirtsRequests, shirts, group))
			{
				Console.WriteLine("Neibb");
				return;
			}
		}
		Console.WriteLine("Jebb");
	}

	private static bool DFS(Dictionary<ShirtRequest, int> shirtsReqs, Dictionary<int, int> shirts, ShirtRequest group)
	{
		if (shirtsReqs.Count == 0) return true;
		var requests = shirtsReqs.Keys.Where(i => i.Min >= group.Min && i.Max <= group.Max).ToArray();
		foreach (var req in requests)
		{
			shirtsReqs[req] -= 1;
			if (shirtsReqs[req] == 0)
				shirtsReqs.Remove(req);

			for (int size = req.Min; size <= req.Max; size++)
			{
				size = shirts.Keys.FirstOrDefault(s => s >= size && s <= req.Max);
				if (size == 0) break;

				shirts[size] -= 1;
				if (shirts[size] == 0) shirts.Remove(size);

				if (DFS(shirtsReqs, shirts, group)) return true;

				if (shirts.ContainsKey(size)) shirts[size] += 1;
				else shirts[size] = 1;
			}

			if (shirtsReqs.ContainsKey(req))
				shirtsReqs[req] += 1;
			else
				shirtsReqs[req] = 1;
			return false;
		}
		return false;
	}

	private static bool Preprocess(Dictionary<ShirtRequest, int> shirtsRequests, Dictionary<int, int> shirts)
	{
		bool needToCheckAgain;
		do
		{
			needToCheckAgain = false;
			foreach (var req in shirtsRequests.Keys.ToArray())
			{
				if (req.Min == req.Max)
				{
					if (shirts.ContainsKey(req.Min) &&
						shirts[req.Min] >= shirtsRequests[req])
					{
						shirts[req.Min] -= shirtsRequests[req];
						shirtsRequests.Remove(req);
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
				//else
				//{
				//	var s = shirts.Keys.Where(shirt => shirt >= req.Min && shirt <= req.Max);
				//	var count = s.Count();
				//	if (count == 0)
				//	{
				//		return false;
				//	}
				//	if (count == 1)
				//	{
				//		needToCheckAgain = true;


				//		shirtsRequests.Remove(req);
				//		var size = s.First();
				//		shirts[size] -= shirtsRequests[req];
				//		if (shirts[size] == 0) shirts.Remove(size);
				//	}
				//}
			}

			//if (!needToCheckAgain)
			//{
			//	var modifications = new Dictionary<int, int>();
			//	foreach (var s in shirts)
			//	{
			//		var reqIndex = -1;
			//		var found = true;
			//		for (int i = 0; i < N; i++)
			//		{
			//			var sReq = shirtsRequests[i];
			//			if (sReq.Min <= s.Key && sReq.Max >= s.Key)
			//			{
			//				if (reqIndex > -1)
			//				{
			//					found = false;
			//					break;
			//				}
			//				reqIndex = i;
			//			}
			//		}
			//		if (found)
			//		{
			//			done.Add(reqIndex);
			//			if (modifications.ContainsKey(s.Key))
			//				modifications[s.Key] += 1;
			//			else
			//				modifications[s.Key] = 1;
			//			needToCheckAgain = true;
			//		}
			//	}
			//	foreach (var mod in modifications)
			//	{
			//		shirts[mod.Key] -= mod.Value;
			//		if (shirts[mod.Key] <= 0) shirts.Remove(mod.Key);
			//	}
			//}
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
			var min = rand.Next(1, 101);
			var max = rand.Next(min, 101);
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