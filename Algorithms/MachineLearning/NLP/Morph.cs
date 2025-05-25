#region

/////////////////////////////////////////////////////////////////////////////
// This source code may not be reviewed, copied, or redistributed without
// the expressed permission of Wesner Moise.
// 
// Copyright (C) 2010-2011, Wesner Moise.
//////////////////////////////////////////////////////////////////////////////

using System.Text.RegularExpressions;
using Algorithms.Collections;

#endregion

namespace Algorithms.NLP;

public class Morph
{
    #region Variables

    static readonly Dictionary<string, string> _sepsHash = new()
    {
        { " ", "Space" },
        { ".", "Dot" },
        { ". ", "DotSpace" },
        { "-", "Dash" },
        { "##", "Empty" },
        { "/", "Slash" },
        { "...", "Ellipsis" },
        { ".-", "DotDash" },
    };

    static readonly HashSet<string> _error = new();

    static readonly HashSet<string> _ignoreMap = new();

    static readonly HashSet<string> _prepositions = new(StringComparer.OrdinalIgnoreCase);
    static readonly HashSet<string> _noiseWords = new(StringComparer.OrdinalIgnoreCase);
    static bool _initedNoiseWords;

    #endregion

    #region Helper Methods

    public static string AdoptCase(string from, string to)
    {
        bool foundUpper = false;
        foreach (char ch in from) {
            if (ch >= 'a' && ch <= 'z')
                return to;
            if (ch >= 'A' && ch <= 'Z')
                foundUpper = true;
        }

        return foundUpper ? to.ToUpper() : to;
    }

    public static string Inflect(string word, string ending, int subtract = 0)
    {
        ending = AdoptCase(word, ending);
        if (subtract > 0)
            word = word.Substring(0, word.Length - subtract);
        return word + ending;
    }

    public static string ToPlural(string word)
    {
        if (word == null)
            return null;

        if (Regex.Match(word, @"[szx]$|[cs]h$|zz$", RegexOptions.IgnoreCase).Success)
            return Inflect(word, "es");
        if (Regex.Match(word, @"[^aeiou]y$", RegexOptions.IgnoreCase).Success)
            return Inflect(word, "ies", 1);
        if (Regex.Match(word, @"[^f]f$", RegexOptions.IgnoreCase).Success)
            return Inflect(word, "ves", 1);
        return Inflect(word, "s");
    }

    public static string ToPast(string word) => ToSuffix(word, "ed");

    public static string ToPresentPart(string word) => ToSuffix(word, "ing");

    public static string ToSuffix(string word, string ending)
    {
        var io = RegexOptions.IgnoreCase;
        if (Regex.IsMatch(ending, @"^e", io)) {
            if (Regex.IsMatch(word, @"[^aeiou]y$", io)) return Inflect(word, "i" + ending, 1);
            if (Regex.IsMatch(word, @"e$", io)) return Inflect(word, ending, 1);
        } else {
            if (Regex.IsMatch(word, @"[^e]e$", io)) return Inflect(word, ending, 1);
        }

        if (Regex.IsMatch(word, @"[aiouhwxy]$", io)) return Inflect(word, ending);

        // Double the consonant
        if (Regex.IsMatch(word, @"([^aeiou]|qu)[aeiouy][^aeiou]$", io)) {
            string doubl = word.Substring(word.Length - 1);
            if (Regex.IsMatch(doubl, @"^c$", io)) doubl = "k";
            return Inflect(word, doubl + ending);
        }

        return Inflect(word, ending);
    }

    static string GetEnding(string line)
    {
        if (line.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            return "s";
        if (line.EndsWith("ing", StringComparison.OrdinalIgnoreCase))
            return "ing";
        if (line.EndsWith("er", StringComparison.OrdinalIgnoreCase))
            return "er";
        if (line.EndsWith("est", StringComparison.OrdinalIgnoreCase))
            return "est";
        if (line.EndsWith("ly", StringComparison.OrdinalIgnoreCase))
            return "ly";
        return "ed";
    }

    static string GetBaseHelper(string word, string ending)
    {
        bool verb = Regex.IsMatch(ending, @"ed|ing$", RegexOptions.IgnoreCase);
        int off = ending.Length;
        if (off > 0)
            word = word.Substring(0, word.Length - off);

        off = 0;

        if (Regex.IsMatch(word, @"([bcdfghjkmnpqrstvwxy])\1$", RegexOptions.IgnoreCase)) {
            off = 1;
        } else if (verb && Regex.IsMatch(word, @"..(en|[ao]w|at|[^ch][eo]r)$")) { } else if (Regex.IsMatch(word,
            @"[^aeiou][aeiou][^aeiouyx]$", RegexOptions.IgnoreCase)) {
            word += "e";
        } else if (Regex.IsMatch(word, @"i$", RegexOptions.IgnoreCase)) {
            word = Regex.Replace(word, @"i$", @"y", RegexOptions.IgnoreCase);
        } else if (verb && Regex.IsMatch(word, @"u$")) {
            word += "e";
        }

        if (off > 0)
            word = word.Substring(0, word.Length - off);
        return word;
    }

    static void SetupIgnoreMap()
    {
        if (_ignoreMap.Count > 0)
            return;

        // TODO: Fix this file issue
        foreach (string line in File.ReadLines("/dict/obj/lexexceptions.txt"))
            _ignoreMap.Add(line);

        Debug.Assert(_ignoreMap.Count > 0);
    }

    public static string GetBase(string word)
    {
        SetupIgnoreMap();
        if (_ignoreMap.Contains(word))
            return word;
        return GetBaseCore(word);
    }

    public static string GetBaseCore(string word)
    {
        if (word.EndsWith("ed", StringComparison.OrdinalIgnoreCase))
            return GetBaseHelper(word, "ed");
        if (word.EndsWith("er", StringComparison.OrdinalIgnoreCase))
            return GetBaseHelper(word, "er");
        if (word.EndsWith("est", StringComparison.OrdinalIgnoreCase))
            return GetBaseHelper(word, "est");
        if (word.EndsWith("ing", StringComparison.OrdinalIgnoreCase))
            return GetBaseHelper(word, "ing");

        var io = RegexOptions.IgnoreCase;
        if (!word.EndsWith("'s", StringComparison.OrdinalIgnoreCase)) {
            word = Regex.Replace(word, "'$", "", io);
            word = Regex.Replace(word, "lves$", "lf", io);
            if (!word.EndsWith("ss"))
                word = Regex.Replace(word, "s$", "", io);
        } else {
            word = Regex.Replace(word, "(.)'s$", "$1", io);
        }

        return word;
    }

    public static string GetBaseType(string word)
    {
        if (word.EndsWith("ed", StringComparison.OrdinalIgnoreCase))
            return "ED";
        if (word.EndsWith("er", StringComparison.OrdinalIgnoreCase))
            return "ER";
        if (word.EndsWith("'s", StringComparison.OrdinalIgnoreCase))
            return "POSS";
        if (word.EndsWith("s'", StringComparison.OrdinalIgnoreCase))
            return "PLPOSS";
        if (word.EndsWith("est", StringComparison.OrdinalIgnoreCase))
            return "EST";
        if (word.EndsWith("ing", StringComparison.OrdinalIgnoreCase))
            return "ING";
        if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase))
            return "S";
        return "";
    }

    public static string ToWord(string word)
    {
        word = word.Replace('_', ' '); // Convert underscores to spaces
        word = Regex.Replace(word, @"\s+", @" "); // Reduce spaces
        word = word.Trim(); //Trim edges    
        return word;
    }

    public static string ToLemma(string word)
    {
        word = word.ToLower();
        word = word.Replace('_', ' '); // Convert underscores to spaces
        word = Regex.Replace(word, @"[^\w']+", @" "); // Convert symbols to spaces
        word = Regex.Replace(word, @"\s+", @" "); // Reduce spaces
        word = word.Trim(); //Trim edges
        word = Regex.Replace(word, @"(\d)([a-z])", @"$1 $2"); // Introduce spaces between numbers and letters
        return word;
    }

    public static string ToVar(string word)
    {
        word = word.Replace("_", " ");
        word = Regex.Replace(word, @"\b([a-z])", x => x.Groups[1].Value.ToUpper());
        word = Regex.Replace(word, @"[^\w]", "");
        if (Regex.IsMatch(word, @"^\d")) word = "_" + word;
        if (word != "") return word;

        word = "_Empty";
        if (!_error.Contains(word))
            Console.Error.Write(word + " could not be made into a variable\n");
        _error.Add(word);
        return word;
    }

    public static string GetSyntax(string word)
    {
        List<string> array = Regex.Split(word, @"([^\w']+)").ToList();
        // Console.Write(string.Join("/", array), "\n");
        var result = new List<string>();
        string capTest = Casing(array[0]);
        string sepTest = "";

        if (array.Count > 1 && array[array.Count - 1] == "")
            array.Pop();

        if (array.Count > 1)
            sepTest = _sepsHash[array[1]];

        bool wordMode = false;
        foreach (string part in array) {
            wordMode = !wordMode;

            if (wordMode) {
                string cap = Casing(part);
                result.Add(cap);
                if (capTest != cap) capTest = "";
            } else {
                string sep = _sepsHash[part];
                if (string.IsNullOrEmpty(sep))
                    Console.Error.Write("\t\t'" + part + "' (", part[0], ") is not a known separator\n");
                result.Add(sep);
                if (sepTest != sep) sepTest = "";
            }
        }

        int pos;
        for (pos = result.Count - 1; pos > 2; pos--)
            if (result[pos] != result[pos - 2])
                break;

        if (pos < result.Count - 1)
            result.Trim(pos + 1);

        if (capTest == "Normal" && (array.Count == 1 || sepTest == "Space"))
            result.Clear();
        else if (array.Count > 1
                 && !string.IsNullOrEmpty(capTest)
                 && !string.IsNullOrEmpty(sepTest))
            result = new List<string> { capTest, sepTest };

        string tmp = string.Join(" ", result);
        return "(Variant " + tmp + ")";
    }

    static string Casing(string word)
    {
        if (!Regex.IsMatch(word, @"[A-Z]"))
            return "Normal";
        if (!Regex.IsMatch(word, @"[a-z]"))
            return "CAP";
        return "Cap";
    }

    static void InitNoise()
    {
        var array = new List<string>
        {
            "on",
            "of",
            "at",
            "in",
            "for",
            "from",
            "to",
            "out",
            "away",
            "through",
            "up",
            "down",
            "back",
            "forward",
            "against",
            "with",
            "as",
            "by",
        };
        _prepositions.UnionWith(array);

        array.AddRange(new[] { "the", "and", "a", "he", "she", "him", "her", "it", "no" });
        _noiseWords.UnionWith(array);
        _initedNoiseWords = true;
    }

    public static bool IsNoise(string word)
    {
        if (!_initedNoiseWords)
            InitNoise();

        return _noiseWords.Contains(word);
    }

    public static bool IsPreposition(string word)
    {
        if (!_initedNoiseWords)
            InitNoise();
        return _prepositions.Contains(word);
    }

    static bool IsLatin(string word) => Regex.IsMatch(word, @"(um|a|ae|is|i|es|as|on)$", RegexOptions.IgnoreCase);

    public static int GetKeyIndex(params string[] lines)
    {
        string key = GetKeyword(lines);
        for (int i = 0; i < lines.Length; i++)
            if (key == lines[i])
                return i;

        throw new InvalidOperationException("Could not find key");
    }

    public static string GetKeyword(params string[] lines)
    {
        List<string> array = lines.Where(line => !IsNoise(line)).ToList();
        if (array.Count == 0)
            return lines[lines.Length - 1];

        for (int i = array.Count - 1; i >= 0; i--) {
            string word = array[i];
            if (IsPreposition(word) && i > 0) {
                array.RemoveRange(i, array.Count - i);
                continue;
            }

            if (IsNoise(word) && array.Count > 1) array.RemoveRange(i, 1);
        }

        if (array.Count == 2)
            if (IsLatin(array[0]) && IsLatin(array[1]))
                array.Pop();

        return array[array.Count - 1];
    }

    #endregion
}