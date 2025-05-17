using static System.Math;

namespace Algorithms.ComputationalGeometry;

public struct Box2D
{

    /// <summary>
    /// number type for coordinates, and its maximum value
    /// </summary>

    public double X;
    public double Y;
    public double Right;
    public double Bottom;

    public double Left => X;

    public double Top => Y;

    public bool IsEmpty => Right <= X || Bottom <= Y;

    public double Width
    {
        get { return Right - Left; }
        set { Right = Left + value; }
    }

    public double Height
    {
        get { return Bottom - Top; }
        set { Bottom = Top + value; }
    }

    public Point2D Location {
        get { return new Point2D(Left, Right); }
        set
        {
            X = value.X;
            Y = value.Y;
        }
    }

    public Point2D Size
    {
        get { return new Point2D(Width, Height); }
        set
        {
            Right = Left + value.X;
            Bottom = Top + value.Y;
        }
    }

    public static readonly Box2D Infinite = new Box2D { X = long.MinValue, Right = long.MaxValue, Y = long.MinValue, Bottom = long.MaxValue };
    public static readonly Box2D Empty = new Box2D {X = long.MaxValue, Right = long.MinValue, Y = long.MaxValue, Bottom = long.MinValue };

    public Box2D(double left, double top, double right, double bottom)
    {
        X = left;
        Y = top;
        Right = right;
        Bottom = bottom;
    }

    public Box2D( Point2D point, Point2D size )
    {
        X = point.X;
        Y = point.Y;
        Right = point.X + size.X;
        Bottom = point.Y + size.Y;
    }

    public static Box2D FromWidthHeight(double x0, double y0, double w, double h)
    {
        return new Box2D
        {
            X = x0,
            Y = y0,
            Right = x0 + w,
            Bottom = y0 + h,
        };
    }

    public Point2D[] GetPoints()
    {
        return new[]
        {
            new Point2D(Left, Top),
            new Point2D(Left, Bottom),
            new Point2D(Right, Bottom),
            new Point2D(Right, Top),
        };
    }

    // computes bounding box from a bunch of points
    public static Box2D From(IEnumerable<Point2D> points)
    {
        var b = new Box2D();

        foreach(var v in points)
        {
            b.X = Min(b.Left, v.X);
            b.Y = Min(b.Top, v.Y);
            b.Right = Max(b.Right, v.X);
            b.Bottom = Max(b.Bottom, v.Y);
        }

        return b;
    }

    // squared distance between a point and this bbox, 0 if inside
    public double DistanceSquared(Point2D p)
    {
        var x = (p.X < Left)
            ? Left : (p.X > Right)
            ? Right : p.X;

        var y = (p.Y < Top)
            ? Top : (p.Y > Bottom)
            ? Bottom : p.Y;

        var dx = x - p.X;
        var dy = y - p.Y;
        return dx * dx + dy * dy;
    }

    public Box2D Union(Box2D box)
    {
        return new Box2D(
            Min(Left, box.Left), Min(Top, box.Top),
            Max(Right, box.Right), Max(Bottom, box.Bottom));
    }

    public Box2D Intersection(Box2D box)
    {
        return new Box2D(
            Max(Left, box.Left), Max(Top, box.Top),
            Min(Right, box.Right), Min(Bottom, box.Bottom));
    }

    public bool Contains(Box2D box)
    {
        return box.X >= X && box.Y >= Y && box.Right <= Right && box.Bottom <= Bottom;
    }

    public bool Contains(Point2D point)
    {
        return point.X >= X && point.Y >= Y && point.X <= Right && point.Y <= Bottom;
    }

    public bool IntersectsWith(Box2D box)
    {
        return box.X < Right && box.Y < Bottom && box.Right > X && box.Bottom > Y;
    }

    public Box2D Floor()
    {
        return new Box2D(Math.Ceiling(Left), Math.Ceiling(Top),
            Math.Floor(Right), Math.Floor(Bottom));
    }

    public Box2D Ceiling()
    {
        return new Box2D(
            Math.Floor(Left),
            Math.Floor(Top),
            Math.Ceiling(Right),
            Math.Ceiling(Bottom));
    }

    public Box2D Invert()
    {
        return new Box2D(Top, Left, Bottom, Right);
    }

    public static Box2D operator - (Box2D a, Box2D b)
    {
        var xdiff1 = b.Left - a.Left;
        var xdiff2 = a.Right - b.Right;
        var ydiff1 = b.Top - a.Left;
        var ydiff2 = a.Bottom - b.Bottom;
        var xMax = Max(xdiff1, xdiff2);
        var yMax = Max(ydiff1, ydiff2);

        if (xMax >= a.Width || yMax >= a.Height)
            return a;

        return FromWidthHeight(
            xdiff1 > xdiff2 ? a.Left : b.Right,
            ydiff1 > ydiff2 ? a.Top : b.Bottom,
            Max(xMax, 0),
            Max(yMax, 0));
    }

    public Box2D Inflate(double x, double y)
    {
        return new Box2D(X - x, Y - y, Right + x, Bottom + y);
    }

    public Box2D Offset(double x, double y)
    {
        return new Box2D(X + x, Y + y, Right + x, Bottom + y);
    }

}