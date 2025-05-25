namespace Algorithms;

[TestFixture]
public class StringToolsTest
{
    [Test]
    public void Chomp()
    {
        AreEqual("", "".Chomp());
        AreEqual(null, StringTools.Chomp(null));
        AreEqual("", "\n".Chomp());
        AreEqual("", "\r\n".Chomp());
        AreEqual("", "\r".Chomp());
        AreEqual("a", "a\r".Chomp());
        AreEqual("a", "a\r\n".Chomp());
        AreEqual("a", "a\n".Chomp());
    }

    [Test]
    public void CamelCase()
    {
        AreEqual("camelCase", "camel case".CamelCase());
        AreEqual("_123", "123".CamelCase());
        AreEqual("camelCase", "CamelCase".CamelCase());
        AreEqual("camelCase", "Camel Case".CamelCase());
        AreEqual("theWar", " The War ".CamelCase());
        AreEqual("_4IsEnough", " 4 is enough".CamelCase());
    }

    [Test]
    public void PascalCase()
    {
        AreEqual("PascalCase", "pascal case".PascalCase());
        AreEqual("PascalCase", "pascal case".PascalCase());
        AreEqual("camelCase", "camel case".PascalCase(false));
        AreEqual("CamelCase", "CamelCase".PascalCase());
        AreEqual("CamelCase", "Camel Case".PascalCase());
        AreEqual("TheWar", " The War ".PascalCase());
        AreEqual("_4IsEnough", " 4 is enough".PascalCase());
    }

    [Test]
    public void TitleCase()
    {
        AreEqual("Title Case", "title case".TitleCase());
        AreEqual("Camelcase", "CamelCase".TitleCase());
        AreEqual("Camel Case", "Camel Case".TitleCase());
        AreEqual(" The War ", " the war ".TitleCase());
        AreEqual(" 4 Is Enough", " 4 is enough".TitleCase());
    }

    [Test]
    public void ToLowerFirst()
    {
        AreEqual("aLPHABET", "ALPHABET".ToLowerFirst());
        AreEqual("alphabet", "alphabet".ToLowerFirst());
        AreEqual("", "".ToLowerFirst());
        AreEqual(null, StringTools.ToLowerFirst(null));
        AreEqual("", "".ToLowerFirst());
        AreEqual(" Lower", " Lower".ToLowerFirst());
        AreEqual("lower", "Lower".ToLowerFirst());
        AreEqual("lower", "lower".ToLowerFirst());
    }

    [Test]
    public void ToUpperFirst()
    {
        AreEqual("ALPHABET", "ALPHABET".ToUpperFirst());
        AreEqual("Alphabet", "alphabet".ToUpperFirst());
        AreEqual("", "".ToUpperFirst());
        AreEqual(null, StringTools.ToUpperFirst(null));
        AreEqual("", "".ToUpperFirst());
        AreEqual(" word", " word".ToUpperFirst());
        AreEqual("Word", "Word".ToUpperFirst());
        AreEqual("Word", "word".ToUpperFirst());
        AreEqual("Word break", "word break".ToUpperFirst());
    }

    [Test]
    public void IsNullOrEmpty()
    {
        IsTrue("".IsNullOrEmpty());
        IsFalse("a".IsNullOrEmpty());
        IsTrue(StringTools.IsNullOrEmpty(null));
        IsTrue("".IsNullOrEmpty());
        IsFalse("x".IsNullOrEmpty());
    }

    [Test]
    public void IsNonempty()
    {
        IsFalse("".IsNonempty());
        IsTrue("a".IsNonempty());
        IsFalse(StringTools.IsNonempty(null));
        IsFalse("".IsNonempty());
        IsTrue("x".IsNonempty());
    }

    [Test]
    public void FormatTest()
    {
        AreEqual(null, StringTools.Format(null));
        AreEqual("", StringTools.Format(""));
        AreEqual("1", StringTools.Format("{0}", 1));
    }

    [Test]
    public void Repeat()
    {
        AreEqual("", StringTools.Repeat("", 2));
        AreEqual("", StringTools.Repeat(null, 2));
        AreEqual("", StringTools.Repeat("abc", 0));
        AreEqual("abc", StringTools.Repeat("abc", 1));
        AreEqual("abcabc", StringTools.Repeat("abc", 2));
    }

    [Test]
    public void ReverseTest()
    {
        AreEqual(null, StringTools.Reverse(null));
        AreEqual("", "".Reverse());
        AreEqual("a", "a".Reverse());
        AreEqual("ba", "ab".Reverse());
        AreEqual("abcde", "edcba".Reverse());
    }

    [Test]
    [Ignore("NYI")]
    public void SplitTest() { }

    [Test]
    [Ignore("NYI")]
    public void IndexOfTest() { }

    [Test]
    public void IndentTest()
    {
        AreEqual("\ta", "a".Indent());
        AreEqual("\ta\n\t", "a\n".Indent());
    }

    [Test]
    public void DecTest()
    {
        AreEqual(0, StringTools.Dec("0"));
        AreEqual(1, StringTools.Dec("1"));
        AreEqual(10, StringTools.Dec("A"));
        AreEqual(10, StringTools.Dec("a"));
        AreEqual(16, StringTools.Dec("10"));
        AreEqual(255, StringTools.Dec("fF"));
    }

    [Test]
    public void HexTest()
    {
        AreEqual("0", 0.Hex());
        AreEqual("1", 1.Hex());
        AreEqual("ff", 255.Hex());
    }
}