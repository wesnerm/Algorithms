namespace Algorithms.Strings;

public abstract class CharSequence : IEnumerable<char>
{
    #region Methods

    public override string ToString() => Data.ToString();

    #endregion

    #region Properties

    public abstract char this[int index] { get; }

    public abstract object Data { get; }

    public abstract int Length { get; }

    #endregion

    #region Collection

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<char> GetEnumerator()
    {
        int length = Length;
        for (int i = 0; i < length; i++)
            yield return this[i];
    }

    #endregion

    #region Converters

    public static implicit operator CharSequence(char[] ch) => new ArrayBased(ch);

    public static implicit operator CharSequence(string str) => new StringBased(str);

    public static implicit operator CharSequence(StringBuilder str) => new StringBuilderBased(str);

    #endregion

    #region Helpers

    class StringBased : CharSequence
    {
        readonly string _data;

        public StringBased(string s) => _data = s;

        public override char this[int index] => _data[index];
        public override int Length => _data.Length;
        public override object Data => _data;
    }

    class StringBuilderBased : CharSequence
    {
        readonly StringBuilder _data;

        public StringBuilderBased(StringBuilder s) => _data = s;

        public override char this[int index] => _data[index];
        public override int Length => _data.Length;
        public override object Data => _data;
    }

    class ArrayBased : CharSequence
    {
        readonly char[] _data;

        public ArrayBased(char[] s) => _data = s;

        public override char this[int index] => _data[index];
        public override int Length => _data.Length;
        public override object Data => _data;

        public override string ToString() => new(_data);
    }

    #endregion
}