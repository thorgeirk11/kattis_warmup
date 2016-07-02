//using System;

//public sealed class RectHV
//{
//	public readonly double xmin, ymin;   // minimum x- and y-coordinates
//	public readonly double xmax, ymax;   // maximum x- and y-coordinates

//	public static readonly RectHV Everthing = new RectHV(double.MinValue, double.MinValue, double.MaxValue, double.MaxValue);

//	public RectHV(double xmin, double ymin, double xmax, double ymax)
//	{
//		if (double.IsNaN(xmin) || double.IsNaN(xmax))
//			throw new ArgumentException("x-coordinate cannot be NaN");
//		if (double.IsNaN(ymin) || double.IsNaN(ymax))
//			throw new ArgumentException("y-coordinates cannot be NaN");
//		if (xmax < xmin || ymax < ymin)
//		{
//			throw new ArgumentException("Invalid rectangle");
//		}
//		this.xmin = xmin;
//		this.ymin = ymin;
//		this.xmax = xmax;
//		this.ymax = ymax;
//	}

//	public bool Intersects(RectHV that)
//	{
//		return this.xmax >= that.xmin && this.ymax >= that.ymin
//			&& that.xmax >= this.xmin && that.ymax >= this.ymin;
//	}

//	public bool Contains(Point2D p)
//	{
//		return (p.X >= xmin) && (p.X <= xmax)
//			&& (p.Y >= ymin) && (p.Y <= ymax);
//	}

//	public double DistanceTo(Point2D p)
//	{
//		return Math.Sqrt(DistanceSquaredTo(p));
//	}

//	public double DistanceSquaredTo(Point2D p)
//	{
//		var dx = 0.0;
//		var dy = 0.0;
//		if (p.X < xmin) dx = p.X - xmin;
//		else if (p.X > xmax) dx = p.X - xmax;
//		if (p.Y < ymin) dy = p.Y - ymin;
//		else if (p.Y > ymax) dy = p.Y - ymax;
//		return dx * dx + dy * dy;
//	}

//	public override bool Equals(object other)
//	{
//		if (other == this) return true;
//		if (other == null) return false;
//		var that = (RectHV)other;
//		if (that == null) return false;
//		if (this.xmin != that.xmin) return false;
//		if (this.ymin != that.ymin) return false;
//		if (this.xmax != that.xmax) return false;
//		if (this.ymax != that.ymax) return false;
//		return true;
//	}

//	public override int GetHashCode()
//	{
//		var hash1 = xmin.GetHashCode();
//		var hash2 = ymin.GetHashCode();
//		var hash3 = xmax.GetHashCode();
//		var hash4 = ymax.GetHashCode();
//		return 31 * (31 * (31 * hash1 + hash2) + hash3) + hash4;
//	}

//	public override string ToString()
//	{
//		return "[" + xmin + ", " + xmax + "] x [" + ymin + ", " + ymax + "]";
//	}
//}
