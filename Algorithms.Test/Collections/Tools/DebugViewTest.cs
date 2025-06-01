namespace Algorithms.Collections;

using static NUnit.Framework.Assert;

[TestFixture]
public class DebugViewTest
{
    [Test]
    public void Collection_NullCollection_ThrowsNotSupportedException()
    {
        // Act & Assert
        Throws<NotSupportedException>(() => new Collection<int>(null!));
    }
    
    [Test]
    public void Collection_ValidCollection_ReturnsItems()
    {
        // Arrange
        var list = new List<int> { 1, 2, 3 };
        var collection = new Collection<int>(list);
        
        // Act
        var items = collection.Items;
        
        // Assert
        AreEqual(3, items.Length);
        AreEqual(1, items[0]);
        AreEqual(2, items[1]);
        AreEqual(3, items[2]);
    }
    
    [Test]
    public void Collection_Enumeration_WorksCorrectly()
    {
        // Arrange
        var list = new List<int> { 1, 2, 3 };
        var collection = new Collection<int>(list);
        
        // Act
        var items = collection.ToList();
        
        // Assert
        AreEqual(3, items.Count);
        AreEqual(1, items[0]);
        AreEqual(2, items[1]);
        AreEqual(3, items[2]);
    }
    
    [Test]
    public void CollectionDebugView_NullCollection_ThrowsNotSupportedException()
    {
        // Act & Assert
        Throws<NotSupportedException>(() => new CollectionDebugView<int>(null!));
    }
    
    [Test]
    public void CollectionDebugView_ValidCollection_ReturnsItems()
    {
        // Arrange
        var list = new List<int> { 1, 2, 3 };
        var debugView = new CollectionDebugView<int>(list);
        
        // Act
        var items = debugView.Items;
        
        // Assert
        AreEqual(3, items.Length);
        AreEqual(1, items[0]);
        AreEqual(2, items[1]);
        AreEqual(3, items[2]);
    }
    
    [Test]
    public void SortedCollectionDebugView_NullCollection_ThrowsNotSupportedException()
    {
        // Act & Assert
        Throws<NotSupportedException>(() => new SortedCollectionDebugView<int>(null!));
    }
    
    [Test]
    public void SortedCollectionDebugView_ValidCollection_ReturnsSortedItems()
    {
        // Arrange
        var list = new List<int> { 3, 1, 2 };
        var debugView = new SortedCollectionDebugView<int>(list);
        
        // Act
        var items = debugView.Items;
        
        // Assert
        AreEqual(3, items.Length);
        AreEqual(1, items[0]);
        AreEqual(2, items[1]);
        AreEqual(3, items[2]);
    }
    
    [Test]
    public void DictionaryDebugView_NullDictionary_ThrowsNotSupportedException()
    {
        // Act & Assert
        Throws<NotSupportedException>(() => new DictionaryDebugView<int, string>(null!));
    }
    
    [Test]
    public void DictionaryDebugView_ValidDictionary_ReturnsItems()
    {
        // Arrange
        var dict = new Dictionary<int, string>
        {
            { 1, "one" },
            { 2, "two" },
            { 3, "three" }
        };
        var debugView = new DictionaryDebugView<int, string>(dict);
        
        // Act
        var items = debugView.Items;
        
        // Assert
        AreEqual(3, items.Length);
        AreEqual(1, items[0].Key);
        AreEqual("one", items[0].Value);
        AreEqual(2, items[1].Key);
        AreEqual("two", items[1].Value);
        AreEqual(3, items[2].Key);
        AreEqual("three", items[2].Value);
    }
    
    [Test]
    public void SortedDictionaryDebugView_NullDictionary_ThrowsNotSupportedException()
    {
        // Act & Assert
        Throws<NotSupportedException>(() => new SortedDictionaryDebugView<int, string>(null!));
    }
    
    [Test]
    public void SortedDictionaryDebugView_ValidDictionary_ReturnsSortedItems()
    {
        // Arrange
        var dict = new Dictionary<int, string>
        {
            { 3, "three" },
            { 1, "one" },
            { 2, "two" }
        };
        var debugView = new SortedDictionaryDebugView<int, string>(dict);
        
        // Act
        var items = debugView.Items;
        
        // Assert
        AreEqual(3, items.Length);
        AreEqual(1, items[0].Key);
        AreEqual("one", items[0].Value);
        AreEqual(2, items[1].Key);
        AreEqual("two", items[1].Value);
        AreEqual(3, items[2].Key);
        AreEqual("three", items[2].Value);
    }
    
    [Test]
    public void KeyValue_DebuggerDisplay_ShowsCorrectFormat()
    {
        // Arrange
        var keyValue = new KeyValue<int, string>(1, "one");
        
        // Act & Assert
        var debuggerDisplayAttribute = typeof(KeyValue<,>).GetCustomAttributes(false)
            .OfType<DebuggerDisplayAttribute>()
            .FirstOrDefault();
            
        IsNotNull(debuggerDisplayAttribute);
        AreEqual("{Value}", debuggerDisplayAttribute.Value);
        AreEqual("[{Key,nq}]", debuggerDisplayAttribute.Name);
        AreEqual("", debuggerDisplayAttribute.Type);
    }
} 