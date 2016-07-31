using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

class Program
{
	static List<int> SearchOrder = new List<int>();
	static HashSet<ShirtRequest> isSearchingGroup = new HashSet<ShirtRequest>();



	private const int RandSize = 50;
	struct ShirtRequest
	{
		public int Min; public int Max;
		public override string ToString()
		{
			return "(" + Min + ", " + Max + ")";
		}
		public bool InsideRange(int shirt)
		{
			return shirt >= Min && shirt <= Max;
		}
	}

	static void Main()
	{
		//TextReader In = File.OpenText("input.txt");
		TextReader In = Console.In;

		var N = int.Parse(In.ReadLine());
		var shirtReqs = new Dictionary<ShirtRequest, int>();
		var shirts = new Dictionary<int, int>();

		ReadShirtRequest(N, shirtReqs, In);
		ReadShirts(N, shirts, In);

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

		SearchOrder = shirts.Keys.OrderBy(i => i).ToList();
		if (DFS(shirtReqs, shirts))
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
	//			ReadShirtRequestRand(N, shirtReqs);
	//			ReadShirtsRand(N, shirts, shirtReqs);
	//			shirtsReqs2 = shirtReqs.ToDictionary(i => i.Key, i => i.Value);
	//			shirts2 = shirts.ToDictionary(i => i.Key, i => i.Value);

	//			if (!Preprocess(shirtReqs, shirts))
	//				continue;
	//			if (!RemoveEqualMinMax(shirtReqs, shirts))
	//				continue;
	//			if (shirtReqs.Count == 0)
	//				continue;
	//			break;
	//		} while (true);

	//		var newShirtReqs = shirtReqs.ToDictionary(i => i.Key, i => i.Value);
	//		var newShirts = shirts.ToDictionary(i => i.Key, i => i.Value);
	//		SearchOrder = shirts.Keys.OrderBy(i => i).ToList();
	//		Console.Write(itt++ + " Started: ");
	//		var sw = Stopwatch.StartNew();
	//		var ans1 = DFS(shirtReqs, shirts);
	//		var time1 = sw.ElapsedMilliseconds;
	//		Console.WriteLine(time1);
	//		if (time1 > 3000)
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
				if (!shirts.ContainsKey(req.Min)) return false;

				var left = Subtract(shirts, req.Min, shirtReqs[req]);
				if (left < 0) return false;
				done.Add(req);
			}
			foreach (var req in done) shirtReqs.Remove(req);
		} while (done.Count > 0);
		return true;
	}

	private static bool DFS(Dictionary<ShirtRequest, int> shirtReqs, Dictionary<int, int> shirts)
	{
		int prevCount;
		do
		{
			prevCount = shirtReqs.Count;
			if (!Preprocess(shirtReqs, shirts))
				return false;
			if (!RemoveEqualMinMax(shirtReqs, shirts))
				return false;
			if (!CheckGroupings(shirtReqs, shirts))
				return false;
			if (shirtReqs.Count == 0)
				return true;
		} while (prevCount > shirtReqs.Count);

		var shirt = SearchOrder.First(s => shirts.ContainsKey(s));
		Subtract(shirts, shirt);

		foreach (var req in shirtReqs.Keys)
		{
			if (req.Min > shirt || req.Max < shirt) continue;

			var newShirtReqs = shirtReqs.ToDictionary(i => i.Key, i => i.Value);
			var newShirts = shirts.ToDictionary(i => i.Key, i => i.Value);

			Subtract(newShirtReqs, req);

			if (DFS(newShirtReqs, newShirts))
				return true;
		}
		return false;
	}

	private static bool CheckGroupings(Dictionary<ShirtRequest, int> shirtReqs, Dictionary<int, int> shirts)
	{
		var done = new List<ShirtRequest>();
		var shirtsDone = new List<int>();
		do
		{
			done.Clear();
			shirtsDone.Clear();
			if (shirts.Count == 0) return true;
			var min = shirts.Keys.Min();
			var max = shirts.Keys.Max();
			//Check for each range that there are enough shirts to satisfy requests.
			foreach (var req in shirtReqs.OrderBy(i => i.Key.Max - i.Key.Min))
			{
				if (isSearchingGroup.Contains(req.Key) || (req.Key.Max == max && req.Key.Min == min))
					continue;
				var r = shirtReqs.Where(i => i.Key.Min >= req.Key.Min && i.Key.Max <= req.Key.Max);
				var s = shirts.Where(i => req.Key.InsideRange(i.Key));
				var reqCount = r.Sum(i => i.Value);
				var shirtCount = s.Sum(i => i.Value);
				
				if (shirtCount < reqCount)
				{
					return false;
				}

				var range = shirtReqs.Where(i => !(i.Key.Min > req.Key.Max || i.Key.Max < req.Key.Min));
				var rangeCount = range.Sum(i => i.Value);

				if (shirtCount == rangeCount)
				{
					isSearchingGroup.Add(req.Key);

					var newShirtReqs = range.ToDictionary(i => i.Key, i => i.Value);
					var newShirts = s.ToDictionary(i => i.Key, i => i.Value);
					if (DFS(newShirtReqs, newShirts))
					{
						done.AddRange(range.Select(i => i.Key));
						shirtsDone.AddRange(s.Select(i => i.Key));
						break;
					}
					else
					{
						return false;
					}
				}
			}
			foreach (var shirt in shirtsDone) shirts.Remove(shirt);
			foreach (var req in done) shirtReqs.Remove(req);
		} while (done.Count > 0);
		return true;
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
				foreach (var shirt in shirts.Keys.Where(shirt => req.InsideRange(shirt)))
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

			// Check if there is only one request that can apply to a shirt 
			var shirtsDone = new List<int>();
			foreach (var s in shirts)
			{
				var count = 0;
				var req = default(ShirtRequest);
				foreach (var request in shirtReqs.Where(r => r.Key.InsideRange(s.Key)))
				{
					count++;
					if (count > 1) break;
					req = request.Key;
				}
				if (count == 1)
				{
					shirtsDone.Add(s.Key);

					var left = shirtReqs[req] -= s.Value;
					if (left < 0) return false;
					if (left == 0) shirtReqs.Remove(req);
					needToCheckAgain = true;
				}
				if (count == 0)
					return false;
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
				Add(shirtReqs, group, shirtReqs[req]);
				shirtReqs.Remove(req);
			}
		}
	}

	private static void ReadShirts(int N, Dictionary<int, int> shirts, TextReader inputSrc)
	{
		var input = inputSrc.ReadLine().Split(' ');
		for (int i = 0; i < N; i++)
		{
			Add(shirts, int.Parse(input[i]));
		}
	}

	private static void ReadShirtRequest(int N, Dictionary<ShirtRequest, int> shirtReqs, TextReader inputSrc)
	{
		for (int i = 0; i < N; i++)
		{
			var input = inputSrc.ReadLine().Split(' ');
			Add(shirtReqs, new ShirtRequest
			{
				Min = int.Parse(input[0]),
				Max = int.Parse(input[1])
			});
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
			Add(shirtRequests, new ShirtRequest
			{
				Min = min,
				Max = max
			});
		}
		return totalMax;
	}
	private static void ReadShirtsRand(int N, Dictionary<int, int> shirts, Dictionary<ShirtRequest, int> reqs)
	{

		var rand = new Random();
		for (int i = 0; i < N; i++)
		{
			var r = 0;
			do { r = rand.Next(1, RandSize + 1); } while (!reqs.Any(req => req.Key.InsideRange(r)));
			Add(shirts, r);
		}
	}
	private static int Subtract<T>(Dictionary<T, int> dic, T item, int amount = 1)
	{
		var left = dic[item] -= amount;
		if (left == 0) dic.Remove(item);
		return left;
	}
	private static int Add<T>(Dictionary<T, int> dic, T item, int amount = 1)
	{
		if (dic.ContainsKey(item))
			return dic[item] += amount;
		else
			return dic[item] = amount;
	}
}