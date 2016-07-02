//using System;
//using System.Collections.Generic;

//class Program
//{
//	static void Main()
//	{
//		// 3 2
//		// 2 3 1
//		var line = Console.ReadLine().Split(' ');
//		var input = Console.ReadLine().Split(' ');
//		var N = int.Parse(line[0]);
//		var K = int.Parse(line[1]);

//		var map = new int[N];
//		for (int i = 0; i < N; i++)
//		{
//			map[i] = int.Parse(input[i]) - 1;
//		}
//		var result = Dance(N, K, map);
//		for (int i = 0; i < N; i++)
//		{
//			Console.Write((result[i] + 1) + " ");
//		}
//		Console.ReadLine();
//	}

//	static public int[] Dance(int N, int K, int[] map)
//	{
//		var union = new WeightedQuickUnionUF(N);
//		var cur = new int[N];
//		var old = new int[N];
//		for (int i = 0; i < N; i++)
//		{
//			cur[i] = old[i] = i;
//			union.Union(i, map[i]);
//		}

//		var sets = GetCiclingSets(N, union);
//		foreach (var set in sets)
//		{
//			var itterations = K % set.Count;
//			for (int j = 0; j < itterations; j++)
//			{
//				foreach (var index in set)
//				{
//					old[index] = cur[map[index]];
//				}

//				var temp = old;
//				old = cur;
//				cur = temp;
//			}
//			foreach (var index in set)
//			{
//				old[index] = cur[index];
//			}
//		}
//		return cur;
//	}

//	private static IEnumerable<List<int>> GetCiclingSets(int N, WeightedQuickUnionUF union)
//	{
//		var sets = new Dictionary<int, List<int>>(union.NumberOfCompnents);
//		for (int i = 0; i < N; i++)
//		{
//			var root = union.Find(i);
//			if (!sets.ContainsKey(root))
//			{
//				sets[root] = new List<int>(union.size[root]) { i };
//			}
//			else
//				sets[root].Add(i);
//		}
//		return sets.Values;
//	}
//}

//public class WeightedQuickUnionUF
//{
//	public int[] parent;    // parent[i] = parent of i
//	public int[] size;     // size[i] = number of sites in subtree rooted at i
//	public int NumberOfCompnents { get; private set; }      // number of components
//	public WeightedQuickUnionUF(int N)
//	{
//		NumberOfCompnents = N;
//		parent = new int[N];
//		size = new int[N];
//		for (int i = 0; i < N; i++)
//		{
//			parent[i] = i;
//			size[i] = 1;
//		}
//	}

//	public int Find(int p)
//	{
//		while (p != parent[p]) p = parent[p];
//		return p;
//	}

//	public void Union(int p, int q)
//	{
//		int rootP = Find(p);
//		int rootQ = Find(q);
//		if (rootP == rootQ) return;

//		if (size[rootP] < size[rootQ])
//		{
//			parent[rootP] = rootQ;
//			size[rootQ] += size[rootP];
//		}
//		else
//		{
//			parent[rootQ] = rootP;
//			size[rootP] += size[rootQ];
//		}
//		NumberOfCompnents--;
//	}
//}
