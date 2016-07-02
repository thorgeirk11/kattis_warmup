using System;

public class KdTree
{
	public enum Axis
	{
		Horizontal,
		Vertical
	}
	
	static public Axis not(this Axis a)
	{
		return a == Axis.Vertical ? Axis.Horizontal : Axis.Vertical;
	}
	private static class Node
	{
		private Node(Point2D p, Node left, Node right, RectHV rect)
		{
			this.p = p;
			this.left = left;
			this.right = right;
			this.rect = rect;
		}

		private Point2D p;      // the point
		private RectHV rect;    // the axis-aligned rectangle corresponding to this node
		private Node left;      // the left/bottom subtree
		private Node right;     // the right/top subtree
	}

	Node head;
	int size;

	public KdTree()                                 // construct an empty set of points
	{
		head = null;
		size = 0;
	}

	public boolean isEmpty()                        // is the set empty?
	{
		return head == null;
	}

	public int size()                               // number of points in the set
	{
		return size;
	}

	private int compare(Point2D p1, Point2D p2, Axis type)
	{
		if (type == Axis.Horizontal) return p1.compareTo(p2);
		if (p1.x() < p2.x()) return -1;
		if (p1.x() > p2.x()) return +1;
		if (p1.y() < p2.y()) return -1;
		if (p1.y() > p2.y()) return +1;
		return 0;
	}

	public void insert(Point2D p)                   // add the point p to the set (if it is not already in the set)
	{
		head = insert(head, p, Axis.Vertical, new RectHV(0, 0, 1, 1));
	}

	private Node insert(Node n, Point2D p, Axis axis, RectHV rect)
	{
		if (n == null)
		{
			size++;
			return new Node(p, null, null, rect);
		}
		int cmp = compare(n.p, p, axis);
		if (cmp != 0)
		{
			if (cmp < 0) n.left = insert(n.left, p, Axis.not(axis), adjustRect(n.rect, n.p, axis, true));
			else n.right = insert(n.right, p, Axis.not(axis), adjustRect(n.rect, n.p, axis, false));
		}
		return n;
	}

	private RectHV adjustRect(RectHV rect, Point2D p, Axis axis, boolean IsLeftNode)
	{
		//This stuff is working:
		if (IsLeftNode)
		{
			if (axis == Axis.Vertical)
				return new RectHV(p.x(), rect.ymin(), rect.xmax(), rect.ymax());
			else
				return new RectHV(rect.xmin(), p.y(), rect.xmax(), rect.ymax());
		}
		else
		{
			if (axis == Axis.Vertical)
				return new RectHV(rect.xmin(), rect.ymin(), p.x(), rect.ymax());
			else
				return new RectHV(rect.xmin(), rect.ymin(), rect.xmax(), p.y());
		}
	}

	public boolean contains(Point2D p)              // does the set contain the point p?
	{
		return contains(head, p, Axis.Vertical);
	}

	private boolean contains(Node n, Point2D p, Axis axis)
	{
		if (n == null) return false;
		int cmp = compare(n.p, p, axis);
		if (cmp != 0)
		{
			if (cmp < 0) return contains(n.left, p, Axis.not(axis));
			else return contains(n.right, p, Axis.not(axis));
		}
		return true;
	}

	public void draw()                              // draw all of the points to standard draw
	{
		nodeDraw(head, Axis.Vertical);
	}

	private void nodeDraw(Node n, Axis axis)
	{
		if (n != null)
		{
			StdDraw.setPenRadius(0.001);
			if (axis == Axis.Horizontal)
			{
				StdDraw.setPenColor(Color.BLUE);
				StdDraw.line(n.rect.xmin(), n.p.y(), n.rect.xmax(), n.p.y());
			}
			else
			{
				StdDraw.setPenColor(Color.RED);
				StdDraw.line(n.p.x(), n.rect.ymin(), n.p.x(), n.rect.ymax());
			}
			StdDraw.setPenRadius(0.01);
			StdDraw.setPenColor(Color.black);
			n.p.draw();
			nodeDraw(n.left, Axis.not(axis));
			nodeDraw(n.right, Axis.not(axis));
		}
	}

	public Iterable<Point2D> range(RectHV rect)     // all points in the set that are inside the rectangle
	{
		List<Point2D> ls = new LinkedList<Point2D>();
		if (head != null)
			range(head, rect, ls);
		return ls;
	}

	private void range(Node n, RectHV rect, List<Point2D> ls)
	{
		if (n.left != null && rect.intersects(n.left.rect)) range(n.left, rect, ls);
		if (n.right != null && rect.intersects(n.right.rect)) range(n.right, rect, ls);
		if (rect.contains(n.p))
			ls.add(n.p);
	}

	public Point2D nearest(Point2D p)               // a nearest neighbor in the set to p; null if set is empty
	{
		if (head == null)
			return null;
		return nearest(head, p, head.p, true);
	}

	private Point2D nearest(Node n, Point2D p, Point2D cBest, boolean useX)
	{
		if (n == null)
			return cBest;
		int cmp = compare(n.p, p, useX ? Axis.Vertical : Axis.Horizontal);
		if (cmp == 0)
			return n.p;
		double bDis = cBest.distanceSquaredTo(p);
		if (n.p.distanceSquaredTo(p) < bDis)
			cBest = n.p;

		Node first, second; // first choice is the node that is more likely to contain the nearest point.
		if (cmp < 0)
		{
			first = n.left;
			second = n.right;
		}
		else
		{
			first = n.right;
			second = n.left;
		}

		if (first != null && first.rect.distanceSquaredTo(p) < bDis)
			cBest = nearest(first, p, cBest, !useX);
		if (second != null && second.rect.distanceSquaredTo(p) < cBest.distanceSquaredTo(p))
			cBest = nearest(second, p, cBest, !useX);
		return cBest;
	}

	public static void main(String[] args)
	{
		String filename = args[0];
		In in = new In(filename);
		KdTree kdtree = new KdTree();
		while (!in.isEmpty()) {
			double x = in.readDouble();
			double y = in.readDouble();
			Point2D p = new Point2D(x, y);
			kdtree.insert(p);
		}
		Random rand = new Random();
		long count = 0;
		long before = System.currentTimeMillis();
		while (System.currentTimeMillis() - before < 10000)
		{
			kdtree.nearest(new Point2D(rand.nextFloat(), rand.nextFloat()));
			count++;
		}
		StdOut.println(count / 10);
	}
	/**
     * ****************************************************************************
     * Test client
     * ****************************************************************************
     public static void main(String[] args) {
     int nrOfRecangles = StdIn.readInt();
     int nrOfPointsCont = StdIn.readInt();
     int nrOfPointsNear = StdIn.readInt();
     RectHV[] rectangles = new RectHV[nrOfRecangles];
     Point2D[] pointsCont = new Point2D[nrOfPointsCont];
     Point2D[] pointsNear = new Point2D[nrOfPointsNear];
     for (int i = 0; i < nrOfRecangles; i++) {
     rectangles[i] = new RectHV(StdIn.readDouble(), StdIn.readDouble(), StdIn.readDouble(), StdIn.readDouble());
     }
     for (int i = 0; i < nrOfPointsCont; i++) {
     pointsCont[i] = new Point2D(StdIn.readDouble(), StdIn.readDouble());
     }
     for (int i = 0; i < nrOfPointsNear; i++) {
     pointsNear[i] = new Point2D(StdIn.readDouble(), StdIn.readDouble());
     }
     KdTree set = new KdTree();
     for (int i = 0; !StdIn.isEmpty(); i++) {
     double x = StdIn.readDouble(), y = StdIn.readDouble();
     set.insert(new Point2D(x, y));
     }
     for (int i = 0; i < nrOfRecangles; i++) {
     // Query on rectangle i, sort the result, and print
     Iterable<Point2D> ptset = set.range(rectangles[i]);
     int ptcount = 0;
     for (Point2D p : ptset)
     ptcount++;
     Point2D[] ptarr = new Point2D[ptcount];
     int j = 0;
     for (Point2D p : ptset) {
     ptarr[j] = p;
     j++;
     }
     Arrays.sort(ptarr);
     StdOut.println("Inside rectangle " + (i + 1) + ":");
     for (j = 0; j < ptcount; j++)
     StdOut.println(ptarr[j]);
     }
     StdOut.println("Contain test:");
     for (int i = 0; i < nrOfPointsCont; i++) {
     StdOut.println((i + 1) + ": " + set.contains(pointsCont[i]));
     }

     StdOut.println("Nearest test:");
     for (int i = 0; i < nrOfPointsNear; i++) {
     StdOut.println((i + 1) + ": " + set.nearest(pointsNear[i]));
     }

     StdOut.println();
     }*/
}
