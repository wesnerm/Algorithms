namespace Algorithms.Collections;

using Algorithms.Misc;
using static NUnit.Framework.Assert;

[TestFixture]
public class RollingMedianTest
{
    [Test]
    public void Add_SingleValue_ReturnsCorrectMedian()
    {
        // Arrange
        var rollingMedian = new RollingMedian();
        
        // Act
        rollingMedian.Add(5);
        
        // Assert
        AreEqual(5.0, rollingMedian.GetMedian());
    }
    
    [Test]
    public void Add_MultipleValues_MaintainsCorrectMedian()
    {
        // Arrange
        var rollingMedian = new RollingMedian();
        
        // Act & Assert
        rollingMedian.Add(1);
        AreEqual(1.0, rollingMedian.GetMedian());
        
        rollingMedian.Add(3);
        AreEqual(2.0, rollingMedian.GetMedian());
        
        rollingMedian.Add(2);
        AreEqual(2.0, rollingMedian.GetMedian());
    }
    
    [Test]
    public void Add_EvenNumberOfValues_ReturnsAverageOfMiddleValues()
    {
        // Arrange
        var rollingMedian = new RollingMedian();
        
        // Act
        rollingMedian.Add(1);
        rollingMedian.Add(2);
        rollingMedian.Add(3);
        rollingMedian.Add(4);
        
        // Assert
        AreEqual(2.5, rollingMedian.GetMedian());
    }
    
    //[Test]
    //public void Remove_ExistingValue_UpdatesMedianCorrectly()
    //{
    //    // Arrange
    //    var rollingMedian = new RollingMedian();
    //    rollingMedian.Add(1);
    //    rollingMedian.Add(2);
    //    rollingMedian.Add(3);
    //    AreEqual(2.0, rollingMedian.GetMedian());
        
    //    // Act
    //    rollingMedian.Remove(2);
        
    //    // Assert
    //    AreEqual(2.0, rollingMedian.GetMedian());
    //}
    
    //[Test]
    //public void Remove_NonExistingValue_DoesNotAffectMedian()
    //{
    //    // Arrange
    //    var rollingMedian = new RollingMedian();
    //    rollingMedian.Add(1);
    //    rollingMedian.Add(2);
    //    double initialMedian = rollingMedian.GetMedian();
        
    //    // Act
    //    rollingMedian.Remove(3);
        
    //    // Assert
    //    AreEqual(initialMedian, rollingMedian.GetMedian());
    //}
    
    [Test]
    public void GetMedian_EmptyCollection_ThrowsInvalidOperationException()
    {
        // Arrange
        var rollingMedian = new RollingMedian();
        
        // Act & Assert
        Throws<InvalidOperationException>(() => rollingMedian.GetMedian());
    }
} 