using System.Collections;
using System.Text;

namespace Algorithms.Strings;

public abstract class CharSequence : IEnumerable<char>
{

    #region Properties
    public abstract char this[int index]
    {
        get;
    }

    public abstract object Data
    {
        get;
    }

    public abstract int Length
    {
        get;
    }
    #endregion

    #region Methods
    public override string ToString()
    {
        return Data.ToString();
    }
    #endregion

    #region Collection
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<char> GetEnumerator()
    {
        int length = Length;
        for (int i = 0; i < length; i++)
            yield return this[i];
    }
    #endregion

    #region Converters
    public static implicit operator CharSequence(char[] ch)
    {
        return new ArrayBased(ch);
    }

    public static implicit operator CharSequence(string str)
    {
        return new StringBased(str);
    }

    public static implicit operator CharSequence(StringBuilder str)
    {
        return new StringBuilderBased(str);
    }
    #endregion

    #region Helpers
    class StringBased : CharSequence
    {
        readonly string _data;

        public StringBased(string s)
        {
            _data = s;
        }

        public override char this[int index] => _data[index];
        public override int Length => _data.Length;
        public override object Data => _data;
    }

    class StringBuilderBased : CharSequence
    {
        readonly StringBuilder _data;

        public StringBuilderBased(StringBuilder s)
        {
            _data = s;
        }

        public override char this[int index] => _data[index];
        public override int Length => _data.Length;
        public override object Data => _data;
    }

    class ArrayBased : CharSequence
    {
        readonly char[] _data;

        public ArrayBased(char[] s)
        {
            _data = s;
        }

        public override char this[int index] => _data[index];
        public override int Length => _data.Length;
        public override object Data => _data;
        public override string ToString()
        {
            return new string(_data);
        }
    }
    #endregion

}
