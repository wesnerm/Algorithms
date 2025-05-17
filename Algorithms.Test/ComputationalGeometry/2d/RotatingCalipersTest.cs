#region Using

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2002-2010, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using Algorithms.Mathematics;

#endregion

namespace Algorithms.ComputationalGeometry;

[TestFixture]
public static class RotatingCalipersTest
{
    [Test]
    public static void GenerateTest()
    {
        var pts = new[]
        {
            new Point2D(0,0),
            new Point2D(1,0),
            new Point2D(1,1),
            new Point2D(0,1),
        };

        var result = RotatingCalipers.Generate2(pts).ToArray();

        Debugger.Break();

    }
}