using Algorithms.Mathematics;

namespace Algorithms.ComputationalGeometry;

[TestFixture]
public class Matrix2DTest
{
    #region Variables
    Matrix2D identity;
    Matrix2D sample;
    Matrix2D rot90;
    Matrix2D scale3;
    Matrix2D trans34;
    Matrix2D combine;
    Matrix2D zero;

    HashSet<Matrix2D> Objects = new HashSet<Matrix2D>();

    #endregion

    #region Setup And Teardown
    [SetUp]
    public void Setup()
    {
        identity = new Matrix2D();
        sample = new Matrix2D();
        rot90 = Matrix2D.RotationMatrix(90 *MathUtil.Degrees);
        scale3 = Matrix2D.ScaleMatrix(3, 3);
        trans34 = Matrix2D.TranslationMatrix(4,5);
        combine = new Matrix2D();
        zero = new Matrix2D();

        Objects.Add(sample);
        Objects.Add(rot90);
        Objects.Add(scale3);
        Objects.Add(trans34);
        Objects.Add(combine);
        Objects.Add(zero);
    }
    
    #endregion

    #region Tests

    /// <summary>
    /// Test for Identity
    /// </summary>
    [Test] 
    public void IdentityTest()
    {
        Matrix2D obj = Matrix2D.Identity;
        AreEqual(1, obj.E11);
        AreEqual(0, obj.E12);
        AreEqual(0, obj.E21);
        AreEqual(1, obj.E22);
        AreEqual(0, obj.OffsetX);
        AreEqual(0, obj.OffsetY);
        IsTrue(obj.IsIdentity);
    }

    /// <summary>
    /// Test for IsInvertible
    /// </summary>
    [Test] 
    public void IsInvertibleTest()
    {
        IsFalse(zero.IsInvertible);
        IsTrue(sample.IsInvertible);
        IsTrue(rot90.IsInvertible);
        IsTrue(scale3.IsInvertible);
        IsTrue(trans34.IsInvertible);
        IsTrue(combine.IsInvertible);
    }        

    /// <summary>
    /// Test for IsIdentity
    /// </summary>
    [Test] 
    public void IsIdentityTest()
    {
        IsTrue(new Matrix2D().IsIdentity);
        IsTrue(identity.IsIdentity);
        IsFalse(sample.IsIdentity);
        IsFalse(rot90.IsIdentity);
        IsFalse(scale3.IsIdentity);
        IsFalse(trans34.IsIdentity);
        IsFalse(combine.IsIdentity);
        IsFalse(zero.IsIdentity);
    }        

    /// <summary>
    /// Test for Determinant
    /// </summary>
    [Test] 
    public void DeterminantTest()
    {
        AreEqual(1, identity.Determinant);
        AreEqual(0, zero.Determinant);
        AreEqual(1, trans34.Determinant);
     }        

    /// <summary>
    /// Test for Inverse
    /// </summary>
    [Test] 
    public void InverseTest()
    {
        foreach (Matrix2D mat in Objects)
        {
            if (!(mat.IsInvertible))
                continue;

            var m1 = mat;
            var matInv = mat.Inverse;
            var m2 = matInv;
            m1.Multiply(m2);
            IsTrue(m1.IsIdentity);

        }
    }        

    /// <summary>
    /// Test for Invert()
    /// </summary>
    [Test] 
    public void InvertTest()
    {
//            foreach (Matrix2D mat in objects)
//            {
//                if (!(mat.IsInvertible))
//                    continue;
//
//                Matrix m1 = mat;
//                Matrix matInv = mat.Inverse;
//                Matrix m2 = matInv;
//                m1.Multiply(m2);
//                IsTrue(m1.IsIdentity);
//                m1.Dispose();
//                m2.Dispose();
//            }
    }        

    /// <summary>
    /// Test for Multiply(Matrix2D matrix)
    /// </summary>
    [Test] 
    [Ignore("Multiply is not yet implemented")]
    public void MultiplyTest()
    {
        // Matrix2D obj = new Matrix2D();
        //  obj.Multiply(matrix);
        Assert.Fail("Multiply is inconclusive");
    }        

    /// <summary>
    /// Test for Assign(Matrix2D matrix)
    /// </summary>
    [Test] 
    [Ignore("Assign is not yet implemented")]
    public void AssignTest()
    {
        // Matrix2D obj = new Matrix2D();
        //  obj.Assign(matrix);
        Assert.Fail("Assign is inconclusive");
    }        

    /// <summary>
    /// Test for Reset()
    /// </summary>
    [Test] 
    [Ignore("Reset is not yet implemented")]
    public void ResetTest()
    {
        foreach (Matrix2D m in Objects)
        {
            m.Reset();
            IsTrue(m.IsIdentity);
        }
        // Matrix2D obj = new Matrix2D();
        //  obj.Reset();
        Assert.Fail("Reset is inconclusive");
    }        

    /// <summary>
    /// Test for Rotate(float angle)
    /// </summary>
    [Test] 
    [Ignore("Rotate is not yet implemented")]
    public void RotateTest()
    {
        // Matrix2D obj = new Matrix2D();
        //  obj.Rotate(angle);
        Assert.Fail("Rotate is inconclusive");
    }        

    /// <summary>
    /// Test for Scale(Vector2D vector)
    /// </summary>
    [Test] 
    [Ignore("Scale is not yet implemented")]
    public void Scale()
    {
        // Matrix2D obj = new Matrix2D();
        //  obj.Scale(vector);
        Assert.Fail("Scale is inconclusive");
    }        

    /// <summary>
    /// Test for Skew(Vector2D angleVector)
    /// </summary>
    [Test] 
    [Ignore("Skew is not yet implemented")]
    public void SkewTest()
    {
        // Matrix2D obj = new Matrix2D();
        //  obj.Skew(angleVector);
        Assert.Fail("Skew is inconclusive");
    }        

    /// <summary>
    /// Test for Transform(Point2D [] points)
    /// </summary>
    [Test] 
    [Ignore("Transform is not yet implemented")]
    public void TransformTest()
    {
        // Matrix2D obj = new Matrix2D();
        //  obj.Transform([] points);
        Assert.Fail("Transform is inconclusive");
    }        

    /// <summary>
    /// Test for Translate(Vector2D vector)
    /// </summary>
    [Test] 
    [Ignore("Translate is not yet implemented")]
    public void TranslateTest()
    {
        // Matrix2D obj = new Matrix2D();
        //  obj.Translate(vector);
        Assert.Fail("Translate is inconclusive");
    }        

    /// <summary>
    /// Test for Transpose3x3()
    /// </summary>
    [Test] 
    [Ignore("Transpose3x3 is not yet implemented")]
    public void Transpose3x3Test()
    {
        // Matrix2D obj = new Matrix2D();
        //  obj.Transpose3x3();
        Assert.Fail("Transpose3x3 is inconclusive");
    }        

    /// <summary>
    /// Test for RotationMatrix(Vector2D axis, float angle, 
    /// </summary>
    [Test] 
    [Ignore("RotationMatrix is not yet implemented")]
    public void RotationMatrixTest()
    {
        // Matrix2D obj = new Matrix2D();
        // Matrix2D expected = obj.RotationMatrix(axis,angle, ;
        // Matrix2D actual = default(Matrix2D);
        // Assert.AreEqual(expected, actual, "RotationMatrix");
        Assert.Fail("RotationMatrix is inconclusive");
    }        

    /// <summary>
    /// Test for ScaleMatrix(Vector2D scale, Point2D center)
    /// </summary>
    [Test] 
    [Ignore("ScaleMatrix is not yet implemented")]
    public void ScaleMatrix()
    {
        // Matrix2D obj = new Matrix2D();
        // Matrix2D expected = obj.ScaleMatrix(scale,center);
        // Matrix2D actual = default(Matrix2D);
        // Assert.AreEqual(expected, actual, "ScaleMatrix");
        Assert.Fail("ScaleMatrix is inconclusive");
    }        

    /// <summary>
    /// Test for SkewMatrix(SizeF angleVector, 
    /// </summary>
    [Test] 
    [Ignore("SkewMatrix is not yet implemented")]
    public void SkewMatrix()
    {
        // Matrix2D obj = new Matrix2D();
        // Matrix2D expected = obj.SkewMatrix(angleVector, ;
        // Matrix2D actual = default(Matrix2D);
        // Assert.AreEqual(expected, actual, "SkewMatrix");
        Assert.Fail("SkewMatrix is inconclusive");
    }        

    /// <summary>
    /// Test for TranslationMatrix(Vector2D offset)
    /// </summary>
    [Test] 
    [Ignore("TranslationMatrix is not yet implemented")]
    public void TranslationMatrix()
    {
        // Matrix2D obj = new Matrix2D();
        // Matrix2D expected = obj.TranslationMatrix(offset);
        // Matrix2D actual = default(Matrix2D);
        // Assert.AreEqual(expected, actual, "TranslationMatrix");
        Assert.Fail("TranslationMatrix is inconclusive");
    }        
    #endregion
}

