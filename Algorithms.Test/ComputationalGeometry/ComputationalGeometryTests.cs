using static System.Math;
using static Algorithms.ComputationalGeometry.Geometry;

//namespace Algorithms.Stanford
namespace Algorithms.ComputationalGeometry;

[TestFixture]
public class ComputationalGeometryTests
{
    [Test]
    public void DelaunayTriangulationTest()
    {
        var x = new List<double> { 0, 0, 1, 0.9 };
        var y = new List<double> { 0, 1, 0, 0.9 };

        List<Tuple<int, int, int>> tri = DelaunayTriangulation(x, y).ToList();

        //expected: 0 1 3
        //          0 3 2

        AreEqual(0, tri[0].Item1);
        AreEqual(1, tri[0].Item2);
        AreEqual(3, tri[0].Item3);

        AreEqual(0, tri[1].Item1);
        AreEqual(3, tri[1].Item2);
        AreEqual(2, tri[1].Item3);
    }

    [Test]
    public void RotateCcw90Test()
    {
        Point2D pt = RotateCcw90(new Point2D(2, 5));
        AreEqual(-5, pt.X);
        AreEqual(2, pt.Y);
    }

    [Test]
    public void RotateCw90Test()
    {
        Point2D pt = RotateCw90(new Point2D(2, 5));
        AreEqual(5, pt.X);
        AreEqual(-2, pt.Y);
    }

    [Test]
    public void RotateCcwPiOver2Test()
    {
        Point2D pt = RotateCcw(new Point2D(2, 5), PI / 2);
        AreEqual(-5.0, pt.X, 1e-5);
        AreEqual(2.0, pt.Y, 1e-5);
    }

    [Test]
    public void ProjectPointLineTest()
    {
        Point2D pt = ProjectPointLine(new Point2D(-5, -2), new Point2D(10, 4), new Point2D(3, 7));
        AreEqual(5, pt.X);
        AreEqual(2, pt.Y);
    }

    [Test]
    public void ProjectPointSegmentTest()
    {
        Point2D pt;
        pt = ProjectPointSegment(new Point2D(-5, -2), new Point2D(10, 4), new Point2D(3, 7));
        AreEqual(new Point2D(5, 2), pt);

        pt = ProjectPointSegment(new Point2D(7.5, 3), new Point2D(10, 4), new Point2D(3, 7));
        AreEqual(new Point2D(7.5, 3), pt);

        pt = ProjectPointSegment(new Point2D(-5, -2), new Point2D(2.5, 1), new Point2D(3, 7));
        AreEqual(new Point2D(2.5, 1), pt);
    }

    [Test]
    public void DistancePointPlaneTest()
    {
        double result = DistancePointPlane(4, -4, 3, 2, -2, 5, -8);
        AreEqual(6.78902858, result, 1e-6);
    }

    [Test]
    public void LinesParallelTest()
    {
        bool ans = LinesParallel(new Point2D(1, 1), new Point2D(3, 5), new Point2D(2, 1), new Point2D(4, 5));
        IsTrue(ans);

        ans = LinesParallel(new Point2D(1, 1), new Point2D(3, 5), new Point2D(2, 0), new Point2D(4, 5));
        IsFalse(ans);

        ans = LinesParallel(new Point2D(1, 1), new Point2D(3, 5), new Point2D(5, 9), new Point2D(7, 13));
        IsTrue(ans);
    }

    [Test]
    public void LinesCollinearTest()
    {
        // expected: 0 0 1
        bool ans = LinesCollinear(new Point2D(1, 1), new Point2D(3, 5), new Point2D(2, 1), new Point2D(4, 5));
        IsFalse(ans);
        ans = LinesCollinear(new Point2D(1, 1), new Point2D(3, 5), new Point2D(2, 0), new Point2D(4, 5));
        IsFalse(ans);
        ans = LinesCollinear(new Point2D(1, 1), new Point2D(3, 5), new Point2D(5, 9), new Point2D(7, 13));
        IsTrue(ans);
    }

    [Test]
    public void SegmentsIntersectTest()
    {
        bool ans = SegmentsIntersect(new Point2D(0, 0), new Point2D(2, 4), new Point2D(3, 1), new Point2D(-1, 3));
        IsTrue(ans);
        ans = SegmentsIntersect(new Point2D(0, 0), new Point2D(2, 4), new Point2D(4, 3), new Point2D(0, 5));
        IsTrue(ans);
        ans = SegmentsIntersect(new Point2D(0, 0), new Point2D(2, 4), new Point2D(2, -1), new Point2D(-2, 1));
        IsTrue(ans);
        ans = SegmentsIntersect(new Point2D(0, 0), new Point2D(2, 4), new Point2D(5, 5), new Point2D(1, 7));
        IsFalse(ans);
    }

    [Test]
    public void ComputeLineIntersectionTest()
    {
        Point2D pt =
            ComputeLineIntersection(new Point2D(0, 0), new Point2D(2, 4), new Point2D(3, 1), new Point2D(-1, 3));
        AreEqual(new Point2D(1, 2), pt);
    }

    [Test]
    public void ComputeCircleCenterTest()
    {
        Point2D pt = ComputeCircleCenter(new Point2D(-3, 4), new Point2D(6, 1), new Point2D(4, 5));
        AreEqual(new Point2D(1, 1), pt);
    }

    [Test]
    public void InPolygonTest()
    {
        var v = new[]
        {
            new Point2D(0, 0),
            new Point2D(5, 0),
            new Point2D(5, 5),
            new Point2D(0, 5),
        };

        AreEqual(true, new Point2D(2, 2).InPolygon(v, false));
        AreEqual(null, new Point2D(2, 0).InPolygon(v, false));
        AreEqual(null, new Point2D(0, 2).InPolygon(v, false));
        AreEqual(null, new Point2D(2, 5).InPolygon(v));
        AreEqual(null, new Point2D(0, 0).InPolygon(v));
        AreEqual(false, new Point2D(0, 6).InPolygon(v));
        ;
        AreEqual(false, new Point2D(6, 6).InPolygon(v));
        ;
    }

    [Test]
    public void InPolygonWindingTest()
    {
        var v = new[]
        {
            new Point2D(0, 0),
            new Point2D(5, 0),
            new Point2D(5, 5),
            new Point2D(0, 5),
        };

        AreEqual(true, new Point2D(2, 2).InPolygon(v));
        AreEqual(null, new Point2D(2, 0).InPolygon(v));
        AreEqual(null, new Point2D(0, 2).InPolygon(v));
        AreEqual(null, new Point2D(5, 2).InPolygon(v));
        AreEqual(null, new Point2D(2, 5).InPolygon(v));
        AreEqual(null, new Point2D(0, 0).InPolygon(v));
        AreEqual(false, new Point2D(0, 6).InPolygon(v));
        ;
        AreEqual(false, new Point2D(6, 6).InPolygon(v));
        ;
    }

    [Test]
    public void CircleLineIntersectionTest()
    {
        List<Point2D> u = CircleLineIntersection(new Point2D(0, 6), new Point2D(2, 6), new Point2D(1, 1), 5);
        u = u.ConvertAll(pt => new Point2D(Round(pt.X, 4), Round(pt.Y, 4)));
        AreEqual(1, u.Count);
        AreEqual(new Point2D(1, 6), u[0]);

        u = CircleLineIntersection(new Point2D(0, 9), new Point2D(9, 0), new Point2D(1, 1), 5);
        u = u.ConvertAll(pt => new Point2D(Round(pt.X, 4), Round(pt.Y, 4)));
        AreEqual(2, u.Count);
        Contains(new Point2D(5, 4), u);
        Contains(new Point2D(4, 5), u);

        u = CircleCircleIntersection(new Point2D(1, 1), new Point2D(10, 10), 5, 5);
        u = u.ConvertAll(pt => new Point2D(Round(pt.X, 4), Round(pt.Y, 4)));
        AreEqual(0, u.Count);

        u = CircleCircleIntersection(new Point2D(1, 1), new Point2D(8, 8), 5, 5);
        u = u.ConvertAll(pt => new Point2D(Round(pt.X, 4), Round(pt.Y, 4)));
        AreEqual(2, u.Count);
        Contains(new Point2D(4, 5), u);
        Contains(new Point2D(5, 4), u);

        u = CircleCircleIntersection(new Point2D(1, 1), new Point2D(4.5, 4.5), 10, Sqrt(2.0) / 2.0);
        u = u.ConvertAll(pt => new Point2D(Round(pt.X, 4), Round(pt.Y, 4)));
        AreEqual(0, u.Count);

        u = CircleCircleIntersection(new Point2D(1, 1), new Point2D(4.5, 4.5), 5, Sqrt(2.0) / 2.0);
        u = u.ConvertAll(pt => new Point2D(Round(pt.X, 4), Round(pt.Y, 4)));
        AreEqual(2, u.Count);
        Contains(new Point2D(4, 5), u);
        Contains(new Point2D(5, 4), u);
    }

    [Test]
    public void CentroidTest()
    {
        var pa = new[] { new Point2D(0, 0), new Point2D(5, 0), new Point2D(1, 1), new Point2D(0, 5) };
        List<Point2D> p = pa.ToList();
        Point2D c = ComputeCentroid(p);

        AreEqual(5.0, SignedArea(p));
        AreEqual(1.16666666, c.X, 1e-6);
        AreEqual(1.16666666, c.Y, 1e-6);
    }

    [Test]
    public void LatLongTest()
    {
        double x = -1.0;
        double y = 2.0;
        double z = -3.0;

        Spherical(x, y, z, out double r, out double lat, out double lon);
        Console.WriteLine(r + " " + lat + " " + lon);

        Rectangular(r, lat, lon, out x, out y, out z);
        Console.WriteLine(x + " " + y + " " + z);
    }
}