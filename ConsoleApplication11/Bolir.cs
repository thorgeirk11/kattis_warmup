using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

class Program
{
	private const int RandSize = 50;
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
		var shirtReqs = new Dictionary<ShirtRequest, int>();
		var shirts = new Dictionary<int, int>();

		ReadShirtRequest(N, shirtReqs);
		ReadShirts(N, shirts);
		
		int prevCount;
		do
		{
			prevCount = shirtReqs.Count;
			if (!Preprocess(shirtReqs, shirts))
			{
				Console.WriteLine("Neibb");
				return;
			}
			if (!RemoveEqualMinMax(shirtReqs, shirts))
			{
				Console.WriteLine("Neibb");
				return;
			}
		} while (prevCount > shirtReqs.Count);

		var newShirtReqs = shirtReqs.ToDictionary(i => i.Key, i => i.Value);
		var newShirts = shirts.ToDictionary(i => i.Key, i => i.Value);
		var simpleWorked = SimpleAssgiment(shirtReqs, shirts, newShirtReqs, newShirts);
		if (simpleWorked || Process(shirtReqs, shirts, true))
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
	//	var shirtReqs = new Dictionary<ShirtRequest, int>();
	//	var shirts = new Dictionary<int, int>();
	//	var shirtsReqs2 = new Dictionary<ShirtRequest, int>();
	//	var shirts2 = new Dictionary<int, int>();
	//	int itt = 0;
	//	do
	//	{
	//		do
	//		{
	//			shirts.Clear();
	//			shirtReqs.Clear();
	//			ReadShirtsRand(N, shirts, ReadShirtRequestRand(N, shirtReqs));
	//			shirtsReqs2 = shirtReqs.ToDictionary(i => i.Key, i => i.Value);
	//			shirts2 = shirts.ToDictionary(i => i.Key, i => i.Value);

	//			if (!Preprocess(shirtReqs, shirts)) continue;
	//			break;
	//		} while (true);

	//		var newShirtReqs = shirtReqs.ToDictionary(i => i.Key, i => i.Value);
	//		var newShirts = shirts.ToDictionary(i => i.Key, i => i.Value);
	//		var simpleWorked = SimpleAssgiment(shirtReqs, shirts, newShirtReqs, newShirts);
	//		if (simpleWorked) continue;

	//		var sw = Stopwatch.StartNew();
	//		var ans1 = Process(shirtReqs, shirts, true);
	//		var time1 = sw.ElapsedMilliseconds;
	//		Console.WriteLine(itt++ + " " + time1);
	//		if (time1 > 1000)
	//		{
	//			Print(shirtsReqs2, shirts2);
	//		}
	//	} while (true);
	//}

	private static bool SimpleAssgiment(Dictionary<ShirtRequest, int> shirtReqs, Dictionary<int, int> shirts, Dictionary<ShirtRequest, int> newShirtReqs, Dictionary<int, int> newShirts)
	{
		foreach (var req in from r in shirtReqs.Keys
							orderby r.Min, shirts.Count(i => i.Key >= r.Min && i.Key <= r.Max)
							select r)
		{
			var left = newShirtReqs[req] -= 1;
			if (left == 0) newShirtReqs.Remove(req);

			if (newShirts.Count == 0) return true;
			var shirt = newShirts.Keys.OrderBy(i => i).First();
			if (shirt >= req.Min && shirt <= req.Max)
			{
				left = newShirts[shirt] -= 1;
				if (left == 0) newShirts.Remove(shirt);
			}
			else return false;
		}
		return true;
	}


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

	private static bool RemoveEqualMinMax(Dictionary<ShirtRequest, int> shirtReqs, Dictionary<int, int> shirts)
	{
		var done = new List<ShirtRequest>();
		do
		{
			done.Clear();
			LimitRequestRanges(shirtReqs, shirts);
			foreach (var req in shirtReqs.Keys)
			{
				if (req.Min != req.Max) continue;
				if (shirts.ContainsKey(req.Min) &&
					shirts[req.Min] >= shirtReqs[req])
				{
					done.Add(req);
					var left = shirts[req.Min] -= shirtReqs[req];
					if (left == 0) shirts.Remove(req.Min);
				}
				else
					return false;
			}
			foreach (var req in done) shirtReqs.Remove(req);
		} while (done.Count > 0);
		return true;
	}

	private static bool Process(Dictionary<ShirtRequest, int> shirtsReqs, Dictionary<int, int> shirts, bool shouldPreprocess)
	{
		if (shouldPreprocess)
		{
			int prevCount;
			do
			{
				prevCount = shirtsReqs.Count;
				if (!Preprocess(shirtsReqs, shirts))
					return false;
				if (!RemoveEqualMinMax(shirtsReqs, shirts))
					return false;
			} while (prevCount > shirtsReqs.Count);
		}

		var groups = GetRangeGroups(shirtsReqs);
		foreach (var group in groups)
		{
			if (!DFS(shirtsReqs, shirts, group))
			{
				return false;
			}
		}
		return true;
	}

	private static List<ShirtRequest> GetRangeGroups(Dictionary<ShirtRequest, int> shirtReqs)
	{
		var groups = new List<ShirtRequest>();
		foreach (var req in shirtReqs.Keys.ToArray())
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

	private static bool DFS(Dictionary<ShirtRequest, int> shirtReqs, Dictionary<int, int> shirts, ShirtRequest requestGroup)
	{
		if (shirtReqs.Count == 0) return true;
		foreach (var size in shirts.Keys.OrderBy(i => i))
		{
			if (requestGroup.Min > size || requestGroup.Max < size) continue;
			foreach (var req in shirtReqs.Keys)
			{
				if (req.Min > size || req.Max < size) continue;

				var newShirtReqs = shirtReqs.Where(i => i.Key.Min >= requestGroup.Min && i.Key.Max <= requestGroup.Max).ToDictionary(i => i.Key, i => i.Value);
				var newShirts = shirts.Where(i => i.Key >= requestGroup.Min && i.Key <= requestGroup.Max).ToDictionary(i => i.Key, i => i.Value);

				var shouldPreprocess = false;
				newShirts[size] -= 1;
				if (newShirts[size] == 0)
				{
					newShirts.Remove(size);
					shouldPreprocess = true;
				}

				newShirtReqs[req] -= 1;
				if (newShirtReqs[req] == 0)
				{
					newShirtReqs.Remove(req);
					shouldPreprocess = true;
				}

				if (Process(newShirtReqs, newShirts, shouldPreprocess)) return true;
			}
			return false;
		}
		return false;
	}

	private static bool Preprocess(Dictionary<ShirtRequest, int> shirtReqs, Dictionary<int, int> shirts)
	{
		var done = new List<ShirtRequest>();
		bool needToCheckAgain;
		do
		{
			needToCheckAgain = false;
			done.Clear();
			// Check if there is only one shirt that can be apply to a request.
			foreach (var req in shirtReqs.Keys)
			{
				var count = 0;
				var size = 0;
				foreach (var shirt in shirts.Keys.Where(shirt => shirt >= req.Min && shirt <= req.Max))
				{
					count++;
					if (count > 1) break;
					size = shirt;
				}
				if (count == 0) return false;
				if (count == 1)
				{
					needToCheckAgain = true;
					var left = shirts[size] -= shirtReqs[req];
					if (left < 0) return false;
					if (left == 0) shirts.Remove(size);
					done.Add(req);
				}
			}
			foreach (var req in done) shirtReqs.Remove(req);

			if (!needToCheckAgain)
			{
				// Check if there is only one request that can apply to a shirt 
				var shirtsDone = new List<int>();
				foreach (var s in shirts)
				{
					var count = 0;
					var req = default(ShirtRequest);
					foreach (var request in shirtReqs.Where(i => i.Key.Min <= s.Key && i.Key.Max >= s.Key))
					{
						count++;
						if (count > 1) break;
						req = request.Key;
					}
					if (count == 1)
					{
						shirtsDone.Add(s.Key);

						var left = shirtReqs[req] -= s.Value;
						if (left == 0) shirtReqs.Remove(req);
						if (left < 0) return false;
						needToCheckAgain = true;
					}
					if (count == 0) return false;
				}
				foreach (var shirt in shirtsDone) shirts.Remove(shirt);

				if (shirts.Count == 0 || shirtReqs.Count == 0) return true;

				// Check if there are equal number of shirts and request for the edge shirts
				var minShirt = shirts.Min(i => i.Key);
				var numMinReq = shirtReqs.Where(i => i.Key.Min <= minShirt).Sum(i => i.Value);
				if (shirts[minShirt] == numMinReq)
				{
					shirts.Remove(minShirt);
					foreach (var req in shirtReqs.Keys.Where(i => i.Min <= minShirt).ToArray())
						shirtReqs.Remove(req);
					needToCheckAgain = true;
				}
				else if (shirts[minShirt] > numMinReq)
				{
					return false;
				}

				if (shirts.Count == 0 || shirtReqs.Count == 0) return true;

				var maxShirt = shirts.Max(i => i.Key);
				var numMaxReq = shirtReqs.Where(i => i.Key.Max >= maxShirt).Sum(i => i.Value);
				if (shirts[maxShirt] == numMaxReq)
				{
					shirts.Remove(maxShirt);
					foreach (var req in shirtReqs.Keys.Where(i => i.Max >= maxShirt).ToArray())
						shirtReqs.Remove(req);
					needToCheckAgain = true;
				}
				else if (shirts[maxShirt] > numMaxReq)
				{
					return false;
				}

				if (shirts.Count == 0 || shirtReqs.Count == 0) return true;

				//Check for each range that there are enough shirts to satisfy requests.
				foreach (var req in shirtReqs)
				{
					var r = shirtReqs.Where(i => i.Key.Min >= req.Key.Min && i.Key.Max <= req.Key.Max);
					var s = shirts.Where(i => i.Key >= req.Key.Min && i.Key <= req.Key.Max);
					var reqCount = r.Sum(i => i.Value);
					var shirtCount = s.Sum(i => i.Value);
					if (shirtCount < reqCount)
						return false;

					var range = shirtReqs.Where(i => !(i.Key.Min > req.Key.Max || i.Key.Max < req.Key.Min));
					var rangeCount = range.Sum(i => i.Value);
					if (shirtCount == rangeCount)
					{
						shirtsDone.AddRange(s.Select(i => i.Key));
						done.AddRange(range.Select(i => i.Key));
						needToCheckAgain = true;
					}
				}
				foreach (var shirt in shirtsDone) shirts.Remove(shirt);
				foreach (var req in done) shirtReqs.Remove(req);

			}
		} while (needToCheckAgain);

		return true;
	}

	
	private static void LimitRequestRanges(Dictionary<ShirtRequest, int> shirtReqs, Dictionary<int, int> shirts)
	{
		foreach (var req in shirtReqs.Keys.ToArray())
		{
			var group = new ShirtRequest
			{
				Min = shirts.Keys.Where(size => size >= req.Min).Min(),
				Max = shirts.Keys.Where(size => size <= req.Max).Max(),
			};
			if (!group.Equals(req))
			{
				if (shirtReqs.ContainsKey(group))
					shirtReqs[group] += shirtReqs[req];
				else
					shirtReqs[group] = shirtReqs[req];
				shirtReqs.Remove(req);
			}
		}
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

	private static void ReadShirtRequest(int N, Dictionary<ShirtRequest, int> shirts)
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