//using System;

//class Program
//{
//	static void Main()
//	{
//		for (int i = 1; i <= 100; i++)
//		{
//			for (var j = Math.Pow(2, i); j > 0; j--)
//			{
//				// 35184372088832
//				// 35184366348153
//				if (IsSymmetry(j))
//				{
//					Console.WriteLine(i + " " + j);
//					break;
//				}
//			}
//		}
//	}

//	private static bool IsSymmetry(double symmetry)
//	{
//		var str = symmetry.ToString();
//		var strLen = str.Length;
//		for (int i = 0; i < strLen / 2; i++)
//		{
//			if (str[i] != str[strLen - i - 1])
//			{
//				return false;
//			}
//		}
//		return true;
//	}
//}