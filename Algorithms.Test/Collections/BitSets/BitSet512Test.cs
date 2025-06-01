namespace Algorithms.Collections.Mutable;

using static NUnit.Framework.Assert;

[TestFixture]
public class BitSet512Test
{
    [Test]
    public void IsEmpty_NewBitSet_ReturnsTrue()
    {
        // Arrange
        var bitSet = new BitSet512();
        
        // Assert
        IsTrue(bitSet.IsEmpty);
    }
    
    [Test]
    public void IsFull_AllBitsSet_ReturnsTrue()
    {
        // Arrange
        var bitSet = ~new BitSet512();
        
        // Assert
        IsTrue(bitSet.IsFull);
    }
    
    [Test]
    public void Count_EmptySet_ReturnsZero()
    {
        // Arrange
        var bitSet = new BitSet512();
        
        // Assert
        AreEqual(0, bitSet.Count);
    }
    
    [Test]
    public void Count_SingleBitSet_ReturnsOne()
    {
        // Arrange
        var bitSet = new BitSet512();
        bitSet.Add(5);
        
        // Assert
        AreEqual(1, bitSet.Count);
    }
    
    [Test]
    public void FirstElement_EmptySet_ReturnsMinusOne()
    {
        // Arrange
        var bitSet = new BitSet512();
        
        // Assert
        AreEqual(-1, bitSet.FirstElement);
    }
    
    [Test]
    public void LastElement_EmptySet_ReturnsMinusOne()
    {
        // Arrange
        var bitSet = new BitSet512();
        
        // Assert
        AreEqual(-1, bitSet.LastElement);
    }
    
    [Test]
    public void FirstElement_MultipleBits_ReturnsLowestSetBit()
    {
        // Arrange
        var bitSet = new BitSet512();
        bitSet.Add(100);
        bitSet.Add(50);
        bitSet.Add(200);
        
        // Assert
        AreEqual(50, bitSet.FirstElement);
    }
    
    [Test]
    public void LastElement_MultipleBits_ReturnsHighestSetBit()
    {
        // Arrange
        var bitSet = new BitSet512();
        bitSet.Add(100);
        bitSet.Add(50);
        bitSet.Add(200);
        
        // Assert
        AreEqual(200, bitSet.LastElement);
    }
    
    [Test]
    public void Union_TwoBitSets_CombinesBitsCorrectly()
    {
        // Arrange
        var bitSet1 = new BitSet512();
        var bitSet2 = new BitSet512();
        
        bitSet1.Add(1);
        bitSet2.Add(2);
        
        // Act
        var result = bitSet1 | bitSet2;
        
        // Assert
        IsTrue(result.Contains(1));
        IsTrue(result.Contains(2));
        IsFalse(result.Contains(0));
        IsFalse(result.Contains(3));
    }
    
    [Test]
    public void Intersection_TwoBitSets_RetainsCommonBits()
    {
        // Arrange
        var bitSet1 = new BitSet512();
        var bitSet2 = new BitSet512();
        
        bitSet1.Add(1);
        bitSet1.Add(2);
        bitSet2.Add(2);
        bitSet2.Add(3);
        
        // Act
        var result = bitSet1 & bitSet2;
        
        // Assert
        IsFalse(result.Contains(1));
        IsTrue(result.Contains(2));
        IsFalse(result.Contains(3));
    }
    
    [Test]
    public void Difference_TwoBitSets_RemovesCommonBits()
    {
        // Arrange
        var bitSet1 = new BitSet512();
        var bitSet2 = new BitSet512();
        
        bitSet1.Add(1);
        bitSet1.Add(2);
        bitSet2.Add(2);
        bitSet2.Add(3);
        
        // Act
        var result = bitSet1 - bitSet2;
        
        // Assert
        IsTrue(result.Contains(1));
        IsFalse(result.Contains(2));
        IsFalse(result.Contains(3));
    }
    
    [Test]
    public void XOR_TwoBitSets_TogglesCommonBits()
    {
        // Arrange
        var bitSet1 = new BitSet512();
        var bitSet2 = new BitSet512();
        
        bitSet1.Add(1);
        bitSet1.Add(2);
        bitSet2.Add(2);
        bitSet2.Add(3);
        
        // Act
        var result = bitSet1 ^ bitSet2;
        
        // Assert
        IsTrue(result.Contains(1));
        IsFalse(result.Contains(2));
        IsTrue(result.Contains(3));
    }
    
    [Test]
    public void Not_BitSet_InvertsAllBits()
    {
        // Arrange
        var bitSet = new BitSet512();
        bitSet.Add(1);
        
        // Act
        var result = ~bitSet;
        
        // Assert
        IsFalse(result.Contains(1));
        IsTrue(result.Contains(0));
        IsTrue(result.Contains(2));
        IsTrue(result.Contains(511));
    }
    
    [Test]
    public void Contains_SubsetBitSet_ReturnsTrue()
    {
        // Arrange
        var bitSet1 = new BitSet512();
        var bitSet2 = new BitSet512();
        
        bitSet1.Add(1);
        bitSet1.Add(2);
        bitSet2.Add(1);
        
        // Assert
        IsTrue(bitSet1.Contains(bitSet2));
        IsFalse(bitSet2.Contains(bitSet1));
    }
    
    [Test]
    public void Add_ValidIndex_SetsCorrectBit()
    {
        // Arrange
        var bitSet = new BitSet512();
        
        // Act & Assert
        bitSet.Add(0);
        IsTrue(bitSet.Contains(0));
        
        bitSet.Add(63);
        IsTrue(bitSet.Contains(63));
        
        bitSet.Add(64);
        IsTrue(bitSet.Contains(64));
        
        bitSet.Add(511);
        IsTrue(bitSet.Contains(511));
    }
    
    [Test]
    public void Add_InvalidIndex_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var bitSet = new BitSet512();
        
        // Act & Assert
        Throws<ArgumentOutOfRangeException>(() => bitSet.Add(512));
    }
    
    [Test]
    public void Remove_ExistingBit_RemovesBit()
    {
        // Arrange
        var bitSet = new BitSet512();
        bitSet.Add(100);
        
        // Act
        bitSet.Remove(100);
        
        // Assert
        IsFalse(bitSet.Contains(100));
    }
    
    [Test]
    public void GetEnumerator_MultipleBits_EnumeratesInOrder()
    {
        // Arrange
        var bitSet = new BitSet512();
        bitSet.Add(1);
        bitSet.Add(64);
        bitSet.Add(128);
        
        // Act
        var elements = bitSet.ToList();
        
        // Assert
        AreEqual(3, elements.Count);
        AreEqual(1, elements[0]);
        AreEqual(64, elements[1]);
        AreEqual(128, elements[2]);
    }
    
    [Test]
    public void ToString_MultipleBits_ReturnsCorrectString()
    {
        // Arrange
        var bitSet = new BitSet512();
        bitSet.Add(1);
        bitSet.Add(2);
        
        // Act
        string result = bitSet.ToString();
        
        // Assert
        AreEqual("1, 2", result);
    }
} 