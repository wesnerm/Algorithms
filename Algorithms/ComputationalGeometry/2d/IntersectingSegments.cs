using static System.Math;

namespace Algorithms.ComputationalGeometry._2d;

public class IntersectingSegments
{
    const double EPS = 1E-9;

    public struct Segment : IComparable<Segment>
    {
        public Point2D p, q;
        public int id;

        public double GetY(double x) => Abs(p.X - q.X) < EPS ? p.Y : p.Y + (q.Y - p.Y) * (x - p.X) / (q.X - p.X);

        public int CompareTo(Segment b)
        {
            double x = Max(Min(p.X, q.X), Min(b.p.X, b.q.X));
            return GetY(x).CompareTo(b.GetY(x));
        }

        static bool Intersect1d(double l1, double r1, double l2, double r2)
        {
            if (l1 > r1) Swap(ref l1, ref r1);
            if (l2 > r2) Swap(ref l2, ref r2);
            return Max(l1, l2) <= Min(r1, r2) + EPS;
        }

        static double Cross(Point2D a, Point2D b, Point2D c)
        {
            double s = a.Cross(b, c);
            return Abs(s) < EPS ? 0 : s;
        }

        public bool Intersect(Segment b) =>
            Intersect1d(p.X, q.X, b.p.X, b.q.X) &&
            Intersect1d(p.Y, q.Y, b.p.Y, b.q.Y) &&
            Cross(p, q, b.p) * Cross(p, q, b.q) <= 0 &&
            Cross(b.p, b.q, p) * Cross(b.p, b.q, q) <= 0;
    }

    struct Event : IComparable<Event>
    {
        public readonly double X;
        public readonly int Tp;
        public int Id;

        public Event(double x, int tp, int id)
        {
            X = x;
            Tp = tp;
            Id = id;
        }

        public int CompareTo(Event e) => Abs(X - e.X) > EPS ? X.CompareTo(e.X) : -Tp.CompareTo(e.Tp);
    }

#if false
    public Tuple<int, int> Solve(List<Segment> a)
    {
        int n = a.Count;
        var e = new List<Event>();
        for (int i = 0; i < n; ++i)
        {
            e.Add(new Event(Min(a[i].p.X, a[i].q.X), +1, i));
            e.Add(new Event(Max(a[i].p.X, a[i].q.X), -1, i));
        }
        e.Sort();

        var s = new SortedSet<Segment>();
        var where = new List<iterator>(a.Count);
        for (int i = 0; i < e.Count; ++i)
        {
            int id = e[i].Id;
            if (e[i].Tp == +1)
            {
                var nxt = s.lower_bound(a[id]);
                var prv = prev(nxt);
                if (nxt != null && Intersect(nxt, a[id]))
                    return Tuple.Create(nxt.id, id);
                if (prv != null && Intersect(prv, a[id]))
                    return Tuple.Create(prv.id, id);
                where[id] = s.insert(nxt, a[id]);
            }
            else
            {
                var nxt = next(where[id]);
                var prv = prev(where[id]);
                if (nxt != null && prv != null && Intersect(nxt, prv))
                    return Tuple.Create(prv.id, nxt.id);
                s.erase(where[id]);
            }
        }

        return null;
    }
#endif
}