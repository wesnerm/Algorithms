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

        var tri = DelaunayTriangulation(x, y).ToList();

        //expected: 0 1 3
        //          0 3 2

        Assert.AreEqual(0, tri[0].Item1);
        Assert.AreEqual(1, tri[0].Item2);
        Assert.AreEqual(3, tri[0].Item3);

        Assert.AreEqual(0, tri[1].Item1);
        Assert.AreEqual(3, tri[1].Item2);
        Assert.AreEqual(2, tri[1].Item3);
    }

    [Test]
    public void RotateCcw90Test()
    {
        var pt = RotateCcw90(new Point2D(2, 5));
        Assert.AreEqual(-5, pt.X);
        Assert.AreEqual(2, pt.Y);
    }

    [Test]
    public void RotateCw90Test()
    {
        var pt = RotateCw90(new Point2D(2, 5));
        Assert.AreEqual(5, pt.X);
        Assert.AreEqual(-2, pt.Y);
    }

    [Test]
    public void RotateCcwPiOver2Test()
    {
        var pt = RotateCcw(new Point2D(2, 5), PI / 2);
        Assert.AreEqual(-5.0, pt.X, 1e-5);
        Assert.AreEqual(2.0, pt.Y, 1e-5);
    }

    [Test]
    public void ProjectPointLineTest()
    {
        var pt = ProjectPointLine(new Point2D(-5, -2), new Point2D(10, 4), new Point2D(3, 7));
        Assert.AreEqual(5, pt.X);
        Assert.AreEqual(2, pt.Y);
    }

    [Test]
    public void ProjectPointSegmentTest()
    {
        Point2D pt;
        pt = ProjectPointSegment(new Point2D(-5, -2), new Point2D(10, 4), new Point2D(3, 7));
        Assert.AreEqual(new Point2D(5, 2), pt);

        pt = ProjectPointSegment(new Point2D(7.5, 3), new Point2D(10, 4), new Point2D(3, 7));
        Assert.AreEqual(new Point2D(7.5, 3), pt);

        pt = ProjectPointSegment(new Point2D(-5, -2), new Point2D(2.5, 1), new Point2D(3, 7));
        Assert.AreEqual(new Point2D(2.5, 1), pt);
    }

    [Test]
    public void DistancePointPlaneTest()
    {
        var result = DistancePointPlane(4, -4, 3, 2, -2, 5, -8);
        Assert.AreEqual(6.78902858, result, 1e-6);
    }

    [Test]
    public void LinesParallelTest()
    {
        var ans = LinesParallel(new Point2D(1, 1), new Point2D(3, 5), new Point2D(2, 1), new Point2D(4, 5));
        Assert.IsTrue(ans);

        ans = LinesParallel(new Point2D(1, 1), new Point2D(3, 5), new Point2D(2, 0), new Point2D(4, 5));
        Assert.IsFalse(ans);

        ans = LinesParallel(new Point2D(1, 1), new Point2D(3, 5), new Point2D(5, 9), new Point2D(7, 13));
        Assert.IsTrue(ans);
    }

    [Test]
    public void LinesCollinearTest()
    {
        // expected: 0 0 1
        var ans = LinesCollinear(new Point2D(1, 1), new Point2D(3, 5), new Point2D(2, 1), new Point2D(4, 5));
        Assert.IsFalse(ans);
        ans = LinesCollinear(new Point2D(1, 1), new Point2D(3, 5), new Point2D(2, 0), new Point2D(4, 5));
        Assert.IsFalse(ans);
        ans = LinesCollinear(new Point2D(1, 1), new Point2D(3, 5), new Point2D(5, 9), new Point2D(7, 13));
        Assert.IsTrue(ans);
    }

    [Test]
    public void SegmentsIntersectTest()
    {
        var ans = SegmentsIntersect(new Point2D(0, 0), new Point2D(2, 4), new Point2D(3, 1), new Point2D(-1, 3));
        Assert.IsTrue(ans);
        ans = SegmentsIntersect(new Point2D(0, 0), new Point2D(2, 4), new Point2D(4, 3), new Point2D(0, 5));
        Assert.IsTrue(ans);
        ans = SegmentsIntersect(new Point2D(0, 0), new Point2D(2, 4), new Point2D(2, -1), new Point2D(-2, 1));
        Assert.IsTrue(ans);
        ans = SegmentsIntersect(new Point2D(0, 0), new Point2D(2, 4), new Point2D(5, 5), new Point2D(1, 7));
        Assert.IsFalse(ans);
    }

    [Test]
    public void ComputeLineIntersectionTest()
    {
        var pt = (ComputeLineIntersection(new Point2D(0, 0), new Point2D(2, 4), new Point2D(3, 1), new Point2D(-1, 3)));
        Assert.AreEqual(new Point2D(1, 2), pt);
    }

    [Test]
    public void ComputeCircleCenterTest()
    {
        var pt = (ComputeCircleCenter(new Point2D(-3, 4), new Point2D(6, 1), new Point2D(4, 5)));
        Assert.AreEqual(new Point2D(1, 1), pt);
    }

    [Test]
    public void InPolygonTest()
    {
        var v = new Point2D[]
        {
            new Point2D(0, 0),
            new Point2D(5, 0),
            new Point2D(5, 5),
            new Point2D(0, 5)
        };

        Assert.AreEqual(true, new Point2D(2, 2).InPolygon(v,false));
        Assert.AreEqual(null, new Point2D(2, 0).InPolygon(v,false));
        Assert.AreEqual(null, new Point2D(0, 2).InPolygon(v,false));
        Assert.AreEqual(null, new Point2D(2, 5).InPolygon(v, true));
        Assert.AreEqual(null, new Point2D(0, 0).InPolygon(v, true));
        Assert.AreEqual(false, new Point2D(0, 6).InPolygon(v, true)); ;
        Assert.AreEqual(false, new Point2D(6, 6).InPolygon(v, true)); ;
    }

    [Test]
    public void InPolygonWindingTest()
    {
        var v = new Point2D[]
        {
            new Point2D(0, 0),
            new Point2D(5, 0),
            new Point2D(5, 5),
            new Point2D(0, 5)
        };

        Assert.AreEqual(true, new Point2D(2, 2).InPolygon(v, true));
        Assert.AreEqual(null, new Point2D(2, 0).InPolygon(v, true));
        Assert.AreEqual(null, new Point2D(0, 2).InPolygon(v, true));
        Assert.AreEqual(null, new Point2D(5, 2).InPolygon(v, true));
        Assert.AreEqual(null, new Point2D(2, 5).InPolygon(v, true));
        Assert.AreEqual(null, new Point2D(0, 0).InPolygon(v, true));
        Assert.AreEqual(false, new Point2D(0, 6).InPolygon(v, true)); ;
        Assert.AreEqual(false, new Point2D(6, 6).InPolygon(v, true)); ;
    }

    [Test]
    public void CircleLineIntersectionTest()
    {
        var u = CircleLineIntersection(new Point2D(0, 6), new Point2D(2, 6), new Point2D(1, 1), 5);
        u = u.ConvertAll(pt => new Point2D(Math.Round(pt.X, 4), Math.Round(pt.Y, 4)));
        Assert.AreEqual(1, u.Count);
        Assert.AreEqual(new Point2D(1, 6), u[0]);

        u = CircleLineIntersection(new Point2D(0, 9), new Point2D(9, 0), new Point2D(1, 1), 5);
        u = u.ConvertAll(pt => new Point2D(Math.Round(pt.X, 4), Math.Round(pt.Y, 4)));
        Assert.AreEqual(2, u.Count);
        Assert.Contains(new Point2D(5, 4), u);
        Assert.Contains(new Point2D(4, 5), u);

        u = CircleCircleIntersection(new Point2D(1, 1), new Point2D(10, 10), 5, 5);
        u = u.ConvertAll(pt => new Point2D(Math.Round(pt.X, 4), Math.Round(pt.Y, 4)));
        Assert.AreEqual(0, u.Count);

        u = CircleCircleIntersection(new Point2D(1, 1), new Point2D(8, 8), 5, 5);
        u = u.ConvertAll(pt => new Point2D(Math.Round(pt.X, 4), Math.Round(pt.Y, 4)));
        Assert.AreEqual(2, u.Count);
        Assert.Contains(new Point2D(4, 5), u);
        Assert.Contains(new Point2D(5, 4), u);

        u = CircleCircleIntersection(new Point2D(1, 1), new Point2D(4.5, 4.5), 10, Sqrt(2.0) / 2.0);
        u = u.ConvertAll(pt => new Point2D(Math.Round(pt.X, 4), Math.Round(pt.Y, 4)));
        Assert.AreEqual(0, u.Count);

        u = CircleCircleIntersection(new Point2D(1, 1), new Point2D(4.5, 4.5), 5, Sqrt(2.0) / 2.0);
        u = u.ConvertAll(pt => new Point2D(Math.Round(pt.X, 4), Math.Round(pt.Y, 4)));
        Assert.AreEqual(2, u.Count);
        Assert.Contains(new Point2D(4, 5), u);
        Assert.Contains(new Point2D(5, 4), u);
    }

    [Test]
    public void CentroidTest()
    {
        var pa = new[] { new Point2D(0, 0), new Point2D(5, 0), new Point2D(1, 1), new Point2D(0, 5) };
        var p = pa.ToList();
        var c = ComputeCentroid(p);

        Assert.AreEqual(5.0, SignedArea(p));
        Assert.AreEqual(1.16666666, c.X, 1e-6);
        Assert.AreEqual(1.16666666, c.Y, 1e-6);
    }

    [Test]
    public void LatLongTest()
    {
        var x = -1.0;
        var y = 2.0;
        var z = -3.0;

        Spherical(x, y, z, out double r, out double lat, out double lon);
        Console.WriteLine(r + " " + lat + " " + lon);

        Rectangular(r, lat, lon, out x, out y, out z);
        Console.WriteLine(x + " " + y + " " + z);
    }
}