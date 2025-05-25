namespace Algorithms;

/// <summary>
///     Summary description for StringTools.
/// </summary>
public static class StringTools
{
    //[DebuggerStepThrough]
    public static string CamelCase(this string s) => PascalCase(s, false);

    //[DebuggerStepThrough]
    public static string PascalCase(this string s, bool upCase = true)
    {
        if (s.IsNullOrEmpty())
            return s;

        var sb = new StringBuilder(s.Length);
        for (int i = 0; i < s.Length;) {
            char ch = s[i];
            int start = i++;
            if (!char.IsLetterOrDigit(ch))
                continue;

            if (char.IsDigit(ch)) {
                if (sb.Length == 0)
                    sb.Append('_');
                sb.Append(ch);
                while (i < s.Length && char.IsDigit(s, i))
                    sb.Append(s[i++]);

                upCase = true;
                continue;
            }

            sb.Append(upCase ? char.ToUpper(ch) : char.ToLower(ch));
            upCase = true;

            if (char.IsUpper(ch)) {
                while (i < s.Length && char.IsUpper(s, i))
                    sb.Append(s[i++]);

                if (i - start > 1 && i < s.Length && char.IsLower(s, i)) {
                    i--;
                    sb.Length--;
                } else {
                    while (i < s.Length && char.IsLower(s, i))
                        sb.Append(s[i++]);
                }
            } else {
                while (i < s.Length) {
                    char ch2 = s[i];
                    if (char.IsLower(ch2))
                        sb.Append(ch2);
                    else if (ch2 != '\'')
                        break;
                    i++;
                }
            }
        }

        return sb.ToString();
    }

    [DebuggerStepThrough]
    public static string TitleCase(this string s)
    {
        if (s.IsNullOrEmpty())
            return s;

        bool upCase = true;
        var b = new StringBuilder(s);
        int write = 0;
        foreach (char ch in s)
            if (char.IsLetter(ch)) {
                b[write++] = upCase
                    ? char.ToUpper(ch)
                    : char.ToLower(ch);
                upCase = false;
            } else {
                upCase = true;
                b[write++] = ch;
            }

        return b.ToString(0, write);
    }

    public static string ToLowerFirst(this string? s)
    {
        if (string.IsNullOrEmpty(s) || !char.IsUpper(s, 0))
            return s;
        return char.ToLower(s[0]) + s.Substring(1);
    }

    public static string ToUpperFirst(this string? s)
    {
        if (string.IsNullOrEmpty(s) || !char.IsLower(s, 0))
            return s;
        return char.ToUpper(s[0]) + s.Substring(1);
    }

    public static bool IsNullOrEmpty(this string? s) =>
        // ReSharper disable ReplaceWithStringIsNullOrEmpty
        s == null || s.Length == 0;
    // ReSharper restore ReplaceWithStringIsNullOrEmpty

    public static bool IsNonempty(this string? s) =>
        // ReSharper disable ReplaceWithStringIsNullOrEmpty
        s != null && s.Length > 0;
    // ReSharper restore ReplaceWithStringIsNullOrEmpty

    public static string Chomp(this string? line)
    {
        if (line == null) return null;

        int len = line.Length;
        while (len > 0) {
            int ch = line[len - 1];
            if (ch != '\n' && ch != '\r')
                break;
            len--;
        }

        return line.Length == len ? line : line.Substring(0, len);
    }

    public static string Format(string message, params object[] args)
    {
        if (args == null || args.Length == 0)
            return message;
        return string.Format(message, args);
    }

    public static string Repeat(string data, int count)
    {
        if (count <= 0 || data == null || data.Length == 0)
            return "";
        if (count == 1)
            return data;
        var sb = new StringBuilder(data.Length * count);
        for (int i = 0; i < count; i++)
            sb.Append(data);
        return sb.ToString();
    }

    public static string Reverse(this string s)
    {
        if (s == null || s.Length < 2)
            return s;

        var sb = new StringBuilder(s);
        int last = s.Length - 1;
        for (int i = s.Length / 2 - 1; i >= 0; i--) {
            char tmp = sb[i];
            sb[i] = sb[last - i];
            sb[last - i] = tmp;
        }

        return sb.ToString();
    }

    public static string Replace(string path, char chFrom, char chTo)
    {
        if (path != null && path.IndexOf(chFrom) >= 0)
            path = path.Replace(chFrom, chTo);
        return path;
    }

    public static string Indent(this string message, string tab = "\t", int indent = 1)
    {
        tab = Repeat(tab, indent);
        var sb = new StringBuilder(message.Length + tab.Length + 6);
        if (!message.StartsWith("\n")) sb.Append(tab);
        sb.Append(message);
        sb.Replace("\n", "\n" + tab);
        return sb.ToString();
    }

    public static int Dec(string number) => Convert.ToInt32(number, 16);

    public static string Hex(this int number) => number.ToString("x");

    public static string Wrap(string text, int columns = 65)
    {
        if (text.Length <= columns)
            return text;

        var sb = new StringBuilder();
        int read = 0;
        int write = 0;
        int lastSpace = -1;

        while (read < text.Length) {
            char ch = text[read];
            sb.Append(ch);

            if (read - write > columns || ch == '\n') {
                if (lastSpace > write && ch != '\n')
                    read = lastSpace;

                sb.Append(text, write, read - write);
                sb.AppendLine();

                while (read < text.Length) {
                    char c2 = text[read];
                    if (!char.IsWhiteSpace(c2))
                        break;
                    if (c2 == '\n') {
                        read++;
                        break;
                    }
                }

                write = read;
                continue;
            }

            if (char.IsWhiteSpace(ch))
                lastSpace = read;

            read++;
        }

        sb.Append(text, write, read - write);
        return sb.ToString();
    }

    [DebuggerStepThrough]
    public static string Before(this string s, char match,
        string defaultResult = null)
    {
        int index = s.IndexOf(match);
        if (index < 0)
            return defaultResult;
        return s.Substring(0, index);
    }

    [DebuggerStepThrough]
    public static string BeforeLast(this string s, char match,
        string defaultResult = null)
    {
        int index = s.LastIndexOf(match);
        if (index < 0)
            return defaultResult;
        return s.Substring(0, index);
    }

    [DebuggerStepThrough]
    public static string AfterLast(this string s, char match, string defaultResult = null)
    {
        int index = s.LastIndexOf(match);
        if (index < 0)
            return defaultResult;
        return s.Substring(index + 1);
    }

    [DebuggerStepThrough]
    public static string After(this string s, char match, string defaultResult = null)
    {
        int index = s.IndexOf(match);
        if (index < 0)
            return defaultResult;
        return s.Substring(index + 1);
    }

    [DebuggerStepThrough]
    public static string PrefixByteOrderMark(string s)
    {
        byte[] preamble = Encoding.Unicode.GetPreamble();
        string byteOrderMark = Encoding.Unicode.GetString(preamble, 0, preamble.Length);
        return byteOrderMark + s;
    }

    [DebuggerStepThrough]
    public static bool CheckStart(ref string s, string prefix, bool caseSensitive)
    {
        if (s.StartsWith(prefix, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)) {
            s = s.Substring(prefix.Length);
            return true;
        }

        return false;
    }

    [DebuggerStepThrough]
    public static bool CheckEnd(ref string s, string suffix, bool caseSensitive)
    {
        if (s.EndsWith(suffix, caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase)) {
            s = s.Substring(0, s.Length - suffix.Length);
            return true;
        }

        return false;
    }

    [DebuggerStepThrough]
    public static string Normalize(string s, bool removeSpaces = true)
    {
        var b = new StringBuilder(s);

        int write = 0;
        for (int read = 0; read < s.Length; read++) {
            char ch = s[read];
            if (removeSpaces && (ch < '0' || ch == '_'))
                continue;
            ch = char.ToLower(ch);
            b[write++] = ch;
        }

        return b.ToString(0, write);
    }

    public static IEnumerable<int> Instances(string text, string p)
    {
        int i = 0;
        while (i < text.Length) {
            int newi = text.IndexOf(p, i);
            if (newi < 0) break;
            yield return newi;
            i = newi + 1;
        }
    }

    public static int CountInstancesOf(string text, string p)
    {
        int i = 0;
        int count = 0;
        while (i < text.Length) {
            int newi = text.IndexOf(p, i);
            if (newi < 0) break;
            count++;
            i = newi + 1;
        }

        return count;
    }

    #region Normalization

    /// <summary>
    ///     Reduces all linebreaks to '\r'
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    // ReSharper disable InconsistentNaming
    public static string NormalizeCRLF(string text)
        // ReSharper restore InconsistentNaming
    {
        if (text == null) return "";

        int index = text.IndexOf('\r');
        if (index == -1)
            return text;

        var sb = new StringBuilder(text.Length);
        for (int i = 0; i < text.Length; i++) {
            char ch = text[i];
            if (ch != '\r')
                sb.Append(ch);
            else if (i + 1 < text.Length && text[i + 1] != '\n')
                sb.Append('\n');
        }

        return sb.ToString();
    }

    #endregion

    #region Quotes

    public static string QuoteSpaces(string s)
    {
        if (s.Contains(" ") || s.Contains("\""))
            s = "\"" + s.Replace("\"", "") + "\"";
        return s;
    }

    #endregion

    #region Cyclic Shifts

    public static int LongestCommonPrefix(string a, string b)
    {
        if (a == null || b == null) return 0;
        int len = Math.Min(a.Length, b.Length);
        int i;
        for (i = 0; i < len && a[i] == b[i]; i++) { }

        return i;
    }

    // SOURCE: http://stackoverflow.com/questions/3459509/minimal-cyclic-shift-algorithm-explanation
    // Unclear whether this is maximal or minimal

    // Also, known as LyndonWord
    // can also be solved by Duval's algorithm
    public static int MinimalCyclicShift(string w)
    {
        int i = 0, n = w.Length;
        for (int j = 1; j < n; ++j) {
            int c, k = 0;
            while ((c = w[(i + k) % n].CompareTo(w[(j + k) % n])) == 0 && k != n) k++;
            j += c > 0 ? k / (j - i) * (j - i) : k;
            i = c > 0 ? j : i;
        }

        return i;
    }

    #endregion

    // TIP: When doing in-place, pair modifications with reads
    // Perform: Initialize values and perform rotations

    #region Index

    [DebuggerStepThrough]
    public static int IndexOf(this StringBuilder text, char findChar, int start = 0) =>
        text.IndexOf(findChar, start, text.Length - start);

    [DebuggerStepThrough]
    public static int IndexOf(this StringBuilder text, char findChar, int start, int count)
    {
        while (count-- > 0) {
            if (text[start] == findChar)
                return start;
            start++;
        }

        return -1;
    }

    [DebuggerStepThrough]
    public static int IndexOf(this StringBuilder text, string findText, int start = 0) =>
        IndexOf(text, findText, start, text.Length - start);

    [DebuggerStepThrough]
    public static int IndexOf(this StringBuilder text, string findText, int start, int count)
    {
        int textLen = text.Length;
        int findLen = findText.Length;

        if (start > textLen - count)
            count = textLen - start;

        count -= findLen;
        while (count-- >= 0) {
            int i;
            for (i = 0; i < findText.Length; i++)
                if (text[start + i] != findText[i])
                    break;
            if (i == findText.Length)
                return start;
            start++;
        }

        return -1;
    }

    [DebuggerStepThrough]
    public static int LastIndexOf(this StringBuilder b, string text)
    {
        int currentIndex = -1;
        while (true) {
            if (currentIndex + text.Length > b.Length)
                return currentIndex;

            int newIndex = b.IndexOf(text, currentIndex + 1);
            if (newIndex < 0)
                return currentIndex;

            currentIndex = newIndex;
        }
    }

    #endregion

    #region Split

    public static void Split(string src,
        out string first, out string second, out string third, char sep)
    {
        Split(src, out first, out string tmp, sep);
        Split(tmp, out second, out third, sep);
    }

    [DebuggerStepThrough]
    public static bool Split(string s, char ch, out string before, out string after)
    {
        int index = s.IndexOf(ch);
        if (index < 0) {
            before = s;
            after = "";
            return false;
        }

        before = s.Substring(0, index);
        after = s.Substring(index + 1);
        return true;
    }

    public static bool Split(string src, out string left, out string right, string sep)
    {
        int index = src.IndexOf(sep);
        if (index == -1) {
            left = src;
            right = string.Empty;
            return false;
        }

        left = src.Substring(0, index);
        right = src.Substring(index + sep.Length);
        return true;
    }

    public static char Split(string src, out string left, out string right, params char[] sep)
    {
        int index = src.IndexOfAny(sep);
        if (index == -1) {
            left = src;
            right = string.Empty;
            return '\0';
        }

        left = src.Substring(0, index);
        right = src.Substring(index + 1);
        return src[index];
    }

    public static bool Split(string src, out string left, out string right, char sep)
    {
        int index = src.IndexOf(sep);
        if (index == -1) {
            left = src;
            right = string.Empty;
            return false;
        }

        left = src.Substring(0, index);
        right = src.Substring(index + 1);
        return true;
    }

    public static bool SplitLast(string src, out string left, out string right, char sep)
    {
        int index = src.LastIndexOf(sep);
        if (index == -1) {
            left = src;
            right = string.Empty;
            return false;
        }

        left = src.Substring(0, index);
        right = src.Substring(index + 1);
        return true;
    }

    #endregion

    #region Bump

    public static long RemoveNumber(StringBuilder builder)
    {
        int value = 0;
        int index = builder.Length - 1;
        int start = Math.Max(0, builder.Length - 18);

        int factor = 1;
        while (index >= 0) {
            int d = builder[index] - '0';
            if (d < 0 || d > 9)
                break;

            value += d * factor;
            factor *= 10;
            index--;
        }

        index++;
        if (builder.Length == index)
            return 0;

        builder.Length = index;
        return value;
    }

    public static string Bump(string s, bool underscore = false) => Bump(s, underscore ? '_' : '\0');

    public static string Bump(string s, bool underscore, Func<string, bool> collision)
    {
        string cur = s;
        while (collision(cur))
            cur = Bump(cur, underscore);
        return cur;
    }

    public static string Bump(string s, char underscore)
    {
        var builder = new StringBuilder(s);
        long number = RemoveNumber(builder);

        if (underscore != '\0') {
            int length = builder.Length;
            if (length <= 0 || builder[length - 1] != underscore) {
                builder.Append(underscore);
                number = 0;
            }
        }

        if (number <= 0)
            number = 2;
        else
            number = number + 1;

        builder.Append(number);
        return builder.ToString();
    }

    public static int HexDigit(int ch)
    {
        if ((uint)(ch - '0') < 10)
            return ch - '0';
        if ((uint)(ch - 'a') < 6)
            return ch - 'a' + 10;
        if ((uint)(ch - 'A') < 6)
            return ch - 'A' + 10;
        return -1;
    }

    public static int Digit(int ch)
    {
        unchecked {
            int digit = ch - '0';
            if ((uint)digit <= 9)
                return digit;
            return -1;
        }
    }

    public static bool IsLetter(int ch)
    {
        unchecked {
            uint ch2 = (uint)(ch - 'A');
            return ch2 <= 'z' - 'A'
                   && (ch2 & ~32) <= 'Z' - 'A';
        }
    }

    [DebuggerStepThrough]
    public static bool IsDigit(int ch) => unchecked((uint)(ch - '0') <= 9);

    public static bool IsWord(int ch)
    {
        if (ch < 'A')
            return IsDigit(ch);

        if (ch <= 'Z')
            return true;

        if (ch < 'a')
            return ch == '_';

        return ch <= 'z';
    }

    public static bool IsWordUnicode(int ch)
    {
        unchecked {
            return char.IsLetterOrDigit((char)ch) || ch == '_';
        }
    }

    #endregion

    #region PrintF

    public static ulong? ParseInteger(string text, ref int i)
    {
        int j = i;
        ulong number = 0;

        for (; j < text.Length; j++) {
            char ch = text[j];
            if (ch < '0' || ch > '9') break;
            ulong digit = (ulong)(ch - '0');
            ulong oldnumber = number;
            number = unchecked(number * 10 + digit);
            if (number < oldnumber)
                return null;
        }

        if (i == j)
            return null;

        i = j;
        return number;
    }

    public static string EncodeSql(this string s,
        bool quoteFully = true)
    {
        var sb = new StringBuilder(s);
        sb.Replace("'", "''");
        if (quoteFully) {
            sb.Insert(0, '\'');
            sb.Append('\'');
        }

        return sb.ToString();
    }

    #endregion

    #region Diff

    public static bool SimpleDiff(string string1, string string2,
        out int start, out int charsFromEnd, bool ignoreCase = false)
    {
        int s;
        int e;

        start = 0;
        charsFromEnd = 0;

        if (string1 == null
            || string2 == null
            || string1.Length == 0
            || string2.Length == 0)
            return false;

        int last1 = string1.Length - 1;
        int last2 = string2.Length - 1;
        int count = Math.Min(last1, last2) + 1;
        if (count == 0)
            return false;

        for (s = 0; s < count; s++) {
            char ch1 = string1[s];
            char ch2 = string2[s];
            if (ch1 != ch2
                || (ignoreCase && IsSameLetter(ch1, ch2)))
                break;
        }

        count = Math.Min(count, count - s);
        for (e = 0; e < count; e++) {
            char ch1 = string1[last1 - e];
            char ch2 = string2[last2 - e];
            if (string1[last1 - e] != string2[last2 - e]
                || (ignoreCase && IsSameLetter(ch1, ch2)))
                break;
        }

        start = s;
        charsFromEnd = e;
        return true;
    }

    public static IEnumerable<string> TextToLines(string text)
    {
        var reader = new StringReader(text);
        string? line;
        while ((line = reader.ReadLine()) != null) yield return line;
    }

    public static bool IsSameLetter(char ch1, char ch2) => char.ToUpper(ch1) == char.ToUpper(ch2);

    #endregion

    #region Unicode

    public static void FixUnicode(string text, TextWriter writer, int codepage = 1252) // Latin 1
    {
        var textFixer = new TextFixer(codepage);
        textFixer.Process(text, writer);
    }

    class TextFixer
    {
        readonly Dictionary<int, int> _u2a;

        string _text;

        public TextFixer(int codepage)
        {
            byte[] ansi = new byte[32];
            for (int i = 0; i < 32; i++)
                ansi[i] = (byte)(i + 128);

            var win1252 = Encoding.GetEncoding(codepage);
            string converted = win1252.GetString(ansi);

            _u2a = new Dictionary<int, int>();
            for (int i = 0; i < 32; i++)
                _u2a[converted[i]] = i + 128;
        }

        public void Process(string text, TextWriter writer)
        {
            _text = text;
            int i = 0;

            // Skip BOM
            if (text.Length >= 3 && text[0] == 0xEF && text[1] == 0xBB && text[2] == 0xBF)
                i += 3;

            while (i < text.Length) {
                // Fast path
                char ch = text[i];
                if (ch < 0x80) {
                    writer.Write(ch);
                    i += 1;
                    continue;
                }

                if (ProcessChar(i, out int shift, out int value) && value <= 0xFFFF) {
                    if (value != 0xFEFF)
                        writer.Write((char)value);
                    i += shift;
                } else {
                    writer.Write(ch);
                    i += 1;
                }
            }
        }

        bool ProcessChar(int i, out int shift, out int value)
        {
            value = 0;
            shift = 0;

            int codeUnit2, codeUnit3;
            if (!ReadPseudo(i, out int codeUnit1) || codeUnit1 < 0xC2) return false;
            if (codeUnit1 < 0xE0) {
                /* 2-byte sequence */
                if (!ReadContinuation(i + 1, out codeUnit2)) return false;
                shift = 2;
                value = (codeUnit1 << 6) + codeUnit2 - 0x3080;
                return true;
            }

            if (codeUnit1 < 0xF0) {
                /* 3-byte sequence */
                if (!ReadContinuation(i + 1, out codeUnit2)) return false;
                if (codeUnit1 == 0xE0 && codeUnit2 < 0xA0) return false; /* overlong */
                if (!ReadContinuation(i + 2, out codeUnit3)) return false;
                shift = 3;
                value = (codeUnit1 << 12) + (codeUnit2 << 6) + codeUnit3 - 0xE2080;
                return true;
            }

            if (codeUnit1 < 0xF5) {
                /* 4-byte sequence */
                if (!ReadContinuation(i + 1, out codeUnit2)) return false;
                if (codeUnit1 == 0xF0 && codeUnit2 < 0x90) return false; /* overlong */
                if (codeUnit1 == 0xF4 && codeUnit2 >= 0x90) return false; /* > U+10FFFF */
                if (!ReadContinuation(i + 2, out codeUnit3)) return false;
                if (!ReadContinuation(i + 3, out int codeUnit4)) return false;
                shift = 4;
                value = (codeUnit1 << 18) + (codeUnit2 << 12) + (codeUnit3 << 6) + codeUnit4 - 0x3C82080;
                return true;
            }

            return false;
        }

        bool ReadPseudo(int i, out int result)
        {
            if (i >= _text.Length) {
                result = 0;
                return false;
            }

            int ch = _text[i];
            result = ch;
            if (ch < 0x80)
                return false;

            // The characters is this range are the same in ANSI and UNICODE
            // except for 0x80 to 0x9f where the characters are undefined in
            // UNICODE.
            if (ch <= 0x100)
                return true;

            return _u2a.TryGetValue(ch, out result);
        }

        bool ReadContinuation(int i, out int result) => ReadPseudo(i, out result) && (result & 0xc0) == 0x80;
    }

    #endregion

    #region Last Char

    public static char LastChar(string s) => s.Length > 0 ? s[s.Length - 1] : '\0';

    public static char FirstChar(string s) => s.Length > 0 ? s[0] : '\0';

    #endregion
}