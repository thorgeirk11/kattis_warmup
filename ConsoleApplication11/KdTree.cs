//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;

//public class KdTree : ICollection<Point2D>
//{
//	public enum Axis
//	{
//		Horizontal,
//		Vertical
//	}

//	static public Axis not(Axis a)
//	{
//		return a == Axis.Vertical ? Axis.Horizontal : Axis.Vertical;
//	}
//	private class Node
//	{
//		public Node(Point2D p, Node left, Node right, RectHV rect)
//		{
//			this.p = p;
//			this.left = left;
//			this.right = right;
//			this.rect = rect;
//		}

//		public Point2D p { get; set; }
//		public RectHV rect { get; set; }
//		public Node left { get; set; }
//		public Node right { get; set; }
//	}

//	private Node _head;
//	public int Count { get; private set; }

//	public bool IsReadOnly
//	{
//		get { return false; }
//	}

//	public KdTree()
//	{
//		_head = null;
//		Count = 0;
//	}

//	public bool isEmpty()
//	{
//		return _head == null;
//	}

//	private static int Compare(Point2D p1, Point2D p2, Axis type)
//	{
//		if (type == Axis.Horizontal)
//		{
//			if (p1.X < p2.X) return +1;
//			if (p1.X > p2.X) return -1;
//			if (p1.Y < p2.Y) return +1;
//			if (p1.Y > p2.Y) return -1;
//		}
//		else
//		{
//			if (p1.X < p2.X) return -1;
//			if (p1.X > p2.X) return +1;
//			if (p1.Y < p2.Y) return -1;
//			if (p1.Y > p2.Y) return +1;
//		}
//		return 0;
//	}

//	public void Add(Point2D p)
//	{
//		_head = Add(_head, p, Axis.Vertical, RectHV.Everthing);
//	}

//	private Node Add(Node n, Point2D p, Axis axis, RectHV rect)
//	{
//		if (n == null)
//		{
//			Count++;
//			return new Node(p, null, null, rect);
//		}
//		var cmp = Compare(n.p, p, axis);
//		if (cmp != 0)
//		{
//			if (cmp < 0) n.left = Add(n.left, p, not(axis), AdjustRect(n.rect, n.p, axis, true));
//			else n.right = Add(n.right, p, not(axis), AdjustRect(n.rect, n.p, axis, false));
//		}
//		return n;
//	}

//	private static RectHV AdjustRect(RectHV rect, Point2D p, Axis axis, bool IsLeftNode)
//	{
//		//This stuff is working:
//		if (IsLeftNode)
//		{
//			if (axis == Axis.Vertical)
//				return new RectHV(p.X, rect.ymin, rect.xmax, rect.ymax);
//			else
//				return new RectHV(rect.xmin, p.Y, rect.xmax, rect.ymax);
//		}
//		else
//		{
//			if (axis == Axis.Vertical)
//				return new RectHV(rect.xmin, rect.ymin, p.X, rect.ymax);
//			else
//				return new RectHV(rect.xmin, rect.ymin, rect.xmax, p.Y);
//		}
//	}

//	public bool Contains(Point2D p)
//	{
//		return Contains(_head, p, Axis.Vertical);
//	}

//	private bool Contains(Node n, Point2D p, Axis axis)
//	{
//		if (n == null) return false;
//		var cmp = Compare(n.p, p, axis);
//		if (cmp != 0)
//		{
//			if (cmp < 0) return Contains(n.left, p, not(axis));
//			else return Contains(n.right, p, not(axis));
//		}
//		return true;
//	}

//	/// <summary>
//	/// All points in the set that are inside the rectangle
//	/// </summary>
//	/// <param name="rect"></param>
//	/// <returns></returns>
//	public IEnumerable<Point2D> Range(RectHV rect)
//	{
//		var ls = new List<Point2D>();
//		if (_head != null)
//			Range(_head, rect, ls);
//		return ls;
//	}

//	private void Range(Node n, RectHV rect, List<Point2D> ls)
//	{
//		if (n.left != null && rect.Intersects(n.left.rect)) Range(n.left, rect, ls);
//		if (n.right != null && rect.Intersects(n.right.rect)) Range(n.right, rect, ls);
//		if (rect.Contains(n.p))
//			ls.Add(n.p);
//	}

//	/// <summary>
//	/// Nearest neighbor in the set to p; (0.0) if set is empty
//	/// </summary>
//	/// <param name="p"></param>
//	/// <param name="ignore">todo: describe ignore parameter on Nearest</param>
//	/// <returns></returns>
//	public Point2D Nearest(Point2D p, HashSet<Point2D> ignore)
//	{
//		if (_head == null) throw new InvalidOperationException("2dTree is empty");
//		return Nearest(_head, p, _head, true, ignore).p;
//	}
//	private Node Nearest(Node n, Point2D p, Node closest, bool useX, HashSet<Point2D> ignore)
//	{
//		if (n == null)
//			return closest;
//		var cmp = Compare(n.p, p, useX ? Axis.Vertical : Axis.Horizontal);
//		double bDis;
//		if (!ignore.Contains(n.p))
//		{
//			if (ignore.Contains(closest.p))
//				closest = n;

//			if (cmp == 0)
//				return n;

//			bDis = closest.p.DistanceSquaredTo(p);
//			if (n.p.DistanceSquaredTo(p) < bDis)
//				closest = n;
//		}
//		else
//		{
//			bDis = double.MaxValue;
//		}

//		Node first, second; // first choice is the node that is more likely to contain the nearest point.
//		if (cmp < 0)
//		{
//			first = n.left;
//			second = n.right;
//		}
//		else
//		{
//			first = n.right;
//			second = n.left;
//		}

//		if (first != null && first.rect.DistanceSquaredTo(p) < bDis)
//			closest = Nearest(first, p, closest, !useX, ignore);
//		if (second != null && second.rect.DistanceSquaredTo(p) < bDis)
//			closest = Nearest(second, p, closest, !useX, ignore);
//		return closest;
//	}


//	public void Clear()
//	{
//		_head = null;
//		Count = 0;
//	}

//	public void CopyTo(Point2D[] array, int arrayIndex)
//	{
//		var points = GetPoints(_head).ToArray();
//		points.CopyTo(array, arrayIndex);
//	}


//	public IEnumerator<Point2D> GetEnumerator()
//	{
//		return GetPoints(_head).GetEnumerator();
//	}

//	IEnumerable<Point2D> GetPoints(Node n)
//	{
//		if (n == null) yield break;
//		yield return n.p;
//		foreach (var p in GetPoints(n.left))
//			yield return p;
//		foreach (var p in GetPoints(n.right))
//			yield return p;
//	}

//	IEnumerator IEnumerable.GetEnumerator()
//	{
//		return GetEnumerator();
//	}

//	public bool Remove(Point2D item)
//	{
//		throw new NotImplementedException();
//	}
//}
