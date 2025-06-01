namespace Algorithms.Collections;

using static NUnit.Framework.Assert;

[TestFixture]
public class BitSet256Test
{
    [Test]
    public void IsEmpty_NewBitSet_ReturnsTrue()
    {
        // Arrange
        var bitSet = new BitSet256();
        
        // Assert
        IsTrue(bitSet.IsEmpty);
    }
    
    [Test]
    public void IsFull_FullSet_ReturnsTrue()
    {
        // Arrange
        var bitSet = BitSet256.FullSet;
        
        // Assert
        IsTrue(bitSet.IsFull);
    }
    
    [Test]
    public void Indexer_SetAndGetBit_WorksCorrectly()
    {
        // Arrange
        var bitSet = new BitSet256();
        
        // Act & Assert
        bitSet[5] = true;
        IsTrue(bitSet[5]);
        
        bitSet[5] = false;
        IsFalse(bitSet[5]);
        
        // Test boundary cases
        bitSet[0] = true;
        IsTrue(bitSet[0]);
        
        bitSet[255] = true;
        IsTrue(bitSet[255]);
    }
    
    [Test]
    public void Union_TwoBitSets_CombinesBitsCorrectly()
    {
        // Arrange
        var bitSet1 = new BitSet256();
        var bitSet2 = new BitSet256();
        
        bitSet1[1] = true;
        bitSet2[2] = true;
        
        // Act
        var result = bitSet1 | bitSet2;
        
        // Assert
        IsTrue(result[1]);
        IsTrue(result[2]);
        IsFalse(result[0]);
        IsFalse(result[3]);
    }
    
    [Test]
    public void Intersection_TwoBitSets_RetainsCommonBits()
    {
        // Arrange
        var bitSet1 = new BitSet256();
        var bitSet2 = new BitSet256();
        
        bitSet1[1] = true;
        bitSet1[2] = true;
        bitSet2[2] = true;
        bitSet2[3] = true;
        
        // Act
        var result = bitSet1 & bitSet2;
        
        // Assert
        IsFalse(result[1]);
        IsTrue(result[2]);
        IsFalse(result[3]);
    }
    
    [Test]
    public void Difference_TwoBitSets_RemovesCommonBits()
    {
        // Arrange
        var bitSet1 = new BitSet256();
        var bitSet2 = new BitSet256();
        
        bitSet1[1] = true;
        bitSet1[2] = true;
        bitSet2[2] = true;
        bitSet2[3] = true;
        
        // Act
        var result = bitSet1 - bitSet2;
        
        // Assert
        IsTrue(result[1]);
        IsFalse(result[2]);
        IsFalse(result[3]);
    }
    
    [Test]
    public void XOR_TwoBitSets_TogglesCommonBits()
    {
        // Arrange
        var bitSet1 = new BitSet256();
        var bitSet2 = new BitSet256();
        
        bitSet1[1] = true;
        bitSet1[2] = true;
        bitSet2[2] = true;
        bitSet2[3] = true;
        
        // Act
        var result = bitSet1 ^ bitSet2;
        
        // Assert
        IsTrue(result[1]);
        IsFalse(result[2]);
        IsTrue(result[3]);
    }
    
    [Test]
    public void Not_BitSet_InvertsAllBits()
    {
        // Arrange
        var bitSet = new BitSet256();
        bitSet[1] = true;
        
        // Act
        var result = ~bitSet;
        
        // Assert
        IsFalse(result[1]);
        IsTrue(result[0]);
        IsTrue(result[2]);
        IsTrue(result[255]);
    }
    
    [Test]
    public void Contains_SubsetBitSet_ReturnsTrue()
    {
        // Arrange
        var bitSet1 = new BitSet256();
        var bitSet2 = new BitSet256();
        
        bitSet1[1] = true;
        bitSet1[2] = true;
        bitSet2[1] = true;
        
        // Assert
        IsTrue(bitSet1.Contains(bitSet2));
        IsFalse(bitSet2.Contains(bitSet1));
    }
    
    [Test]
    public void Overlaps_IntersectingBitSets_ReturnsTrue()
    {
        // Arrange
        var bitSet1 = new BitSet256();
        var bitSet2 = new BitSet256();
        
        bitSet1[1] = true;
        bitSet1[2] = true;
        bitSet2[2] = true;
        bitSet2[3] = true;
        
        // Assert
        IsTrue(bitSet1.Overlaps(bitSet2));
        
        bitSet2[2] = false;
        IsFalse(bitSet1.Overlaps(bitSet2));
    }
    
    [Test]
    public void ToString_BitSet_ReturnsHexString()
    {
        // Arrange
        var bitSet = new BitSet256();
        bitSet[0] = true; // Sets lowest bit
        
        // Act
        string result = bitSet.ToString();
        
        // Assert
        IsTrue(result.Length > 0);
        IsTrue(result.EndsWith("1")); // Lowest bit should be 1
    }
    
    [Test]
    public void Equals_SameBitSets_ReturnsTrue()
    {
        // Arrange
        var bitSet1 = new BitSet256();
        var bitSet2 = new BitSet256();
        
        bitSet1[1] = true;
        bitSet2[1] = true;
        
        // Assert
        IsTrue(bitSet1.Equals(bitSet2));
        IsTrue(bitSet1.Equals((object)bitSet2));
        
        bitSet2[2] = true;
        IsFalse(bitSet1.Equals(bitSet2));
    }
    
    [Test]
    public void GetHashCode_SameBitSets_ReturnsSameHash()
    {
        // Arrange
        var bitSet1 = new BitSet256();
        var bitSet2 = new BitSet256();
        
        bitSet1[1] = true;
        bitSet2[1] = true;
        
        // Assert
        AreEqual(bitSet1.GetHashCode(), bitSet2.GetHashCode());
    }
} 