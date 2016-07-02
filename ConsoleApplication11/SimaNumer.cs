//using System;
//using System.Collections.Generic;

//class Program
//{
//	class Node
//	{
//		public Node[] Connected { get; private set; }
//		public Node() { Connected = new Node[10]; }
//	}

//	static Node PhoneNumbers = new Node();
//	static Dictionary<Node, int> ConnectionCount = new Dictionary<Node, int>();

//	static void Main()
//	{
//		var N = int.Parse(Console.ReadLine());
//		for (int i = 0; i < N; i++)
//		{
//			ParseNumber(Console.ReadLine());
//		}

//		var Q = int.Parse(Console.ReadLine());
//		for (int i = 0; i < Q; i++)
//		{
//			Console.WriteLine(MatchNumber(Console.ReadLine()));
//		}
//	}

//	static int SumConnections(Node n, int index)
//	{
//		if (n == null) return 0;
//		if (index == 7) return 1;
//		var sum = 0;
//		if (ConnectionCount.TryGetValue(n, out sum)) return sum;
//		for (int i = 0; i < 10; i++)
//		{
//			sum += SumConnections(n.Connected[i], index + 1);
//		}
//		return ConnectionCount[n] = sum;
//	}

//	public static int MatchNumber(string number)
//	{
//		var curNode = PhoneNumbers;
//		for (int i = 0; i < number.Length; i++)
//		{
//			curNode = curNode.Connected[number[i] - '0'];
//			if (curNode == null) return 0;
//		}
//		return SumConnections(curNode, number.Length);
//	}

//	public static void ParseNumber(string input)
//	{
//		var curNode = PhoneNumbers;
//		for (int i = 0; i < input.Length; i++)
//		{
//			var c = input[i] - '0';
//			curNode = curNode.Connected[c] = curNode.Connected[c] ?? new Node();
//		}
//	}
//}