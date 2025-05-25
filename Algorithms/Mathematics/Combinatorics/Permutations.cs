namespace Algorithms.Mathematics;

public static partial class Permutations
{
    #region Permutation Functions

    public static int GetRank(string word)
    {
        if (word == null || word.Length <= 1)
            return 0;

        char[] charsSorted = word.ToCharArray().OrderBy(x => x).ToArray();
        int[] letterCounts = new int[26];

        for (int i = 0; i < charsSorted.Length; i++)
            letterCounts[charsSorted[i] - 'a']++;

        int rank = 0;
        int permCount = CountPermutations(letterCounts);
        for (int j = 0; j < word.Length; j++) {
            int ch = word[j] - 'a';
            int pos = 0;
            for (int i = 0; i < ch; i++)
                pos += letterCounts[i];
            permCount = Mult(permCount, Inverse(word.Length - j));
            rank = (rank + Mult(permCount, pos)) % MOD;
            permCount = Mult(permCount, letterCounts[ch]--);
        }

        return rank;
    }

    // TESTS: https://leetcode.com/problems/permutation-sequence/#/description
    // Returns kth (1-based) permutation of 1...n
    public static List<int> GetPermutation(int n, long k)
    {
        var list = new List<int>(n);
        for (int i = 0; i < n; i++) list.Add(i + 1);

        long perm = 1;
        for (int i = 1; i <= n; i++) perm *= i;

        k--;
        k %= perm;
        int start = 0;
        for (; k > 0 && start < n; start++) {
            perm /= n - start;

            // if (k<perm) continue;

            int i = (int)(k / perm);
            k %= perm;

            int x = list[start + i];
            list.RemoveAt(start + i);
            list.Insert(start, x);
        }

        return list;
    }

    public static int CountPermutations(int[] letterCounts)
    {
        int total = 0;
        int divisor = 1;
        for (int i = 0; i < letterCounts.Length; i++)
            if (letterCounts[i] != 0) {
                total += letterCounts[i];
                divisor = Mult(divisor, Fact(letterCounts[i]));
            }

        return Mult(Fact(total), Inverse(divisor));
    }

    [DebuggerStepThrough]
    [Pure]
    public static IEnumerable<T[]> PermutationsOf<T>(IEnumerable<T> array)
    {
        T[] clone = array.ToArray();
        Array.Sort(clone);
        Comparison<T> comparison = Comparer<T>.Default.Compare;
        do
            yield return clone;
        while (NextPermutation(clone, comparison));
    }

    [DebuggerStepThrough]
    [Pure]
    public static IEnumerable<T[]> CombinationsOf<T>(int size, IEnumerable<T> array)
    {
        T[] clone = array.ToArray();
        var combo = new T[size];
        Comparison<T> comparison = Comparer<T>.Default.Compare;
        Array.Sort(clone);
        do {
            Array.Copy(clone, combo, size);
            yield return combo;
        } while (NextCombination(size, clone, comparison));
    }

    public static bool NextPermutation<T>(T[] array, Comparison<T> comparison = null)
    {
        if (comparison == null)
            comparison = Comparer<T>.Default.Compare;

        // Look for first sorted index
        int position;
        for (position = array.Length - 2; position >= 0; position--)
            if (comparison(array[position], array[position + 1]) < 0)
                break;

        // Place before -- if we want to wrap around
        if (position < 0)
            return false;

        Array.Reverse(array, position + 1, array.Length - (position + 1));

        // Swap pivot value with next largest value
        int min = position + 1;
        int max = array.Length - 1;
        T pivot = array[position];
        while (min <= max) {
            int mid = (min + max) >> 1;
            int cmp = comparison(pivot, array[mid]);
            if (cmp >= 0)
                min = mid + 1;
            else
                max = mid - 1;
        }

        array[position] = array[min];
        array[min] = pivot;
        return true;
    }

    /// <summary>
    ///     Generates the next combination of a presorted array
    /// </summary>
    [DebuggerStepThrough]
    public static bool NextCombination<T>(int size, T[] array, Comparison<T> comparison = null)
    {
        if (comparison == null)
            comparison = Comparer<T>.Default.Compare;

        while (size > 0) {
            // Swap pivot value with next largest value
            T pivot = array[size - 1];
            int min = size;
            int max = array.Length - 1;
            while (min <= max) {
                int mid = (min + max) >> 1;
                int cmp = comparison(pivot, array[mid]);
                if (cmp >= 0)
                    min = mid + 1;
                else
                    max = mid - 1;
            }

            if (min < array.Length) {
                array[size - 1] = array[min];
                array[min] = pivot;
                return true;
            }

            // Rotate
            Array.Copy(array, size, array, size - 1, array.Length - size);
            array[array.Length - 1] = pivot;
            size--;
        }

        return false;
    }

    #endregion

    #region Backtracking

    public class SubsetIterator
    {
        readonly Action<List<int>> _action;
        readonly bool _handleDupes;
        readonly int[] _nums;
        readonly List<int> _tempList;

        public SubsetIterator(int[] nums, Action<List<int>> action, bool handleDupes = true)
        {
            _action = action;
            _tempList = new List<int>();
            _nums = nums;
            _handleDupes = handleDupes;
            if (handleDupes) Array.Sort(nums);
            BacktrackSubset();
        }

        void BacktrackSubset(int start = 0)
        {
            _action(_tempList);
            for (int i = start; i < _nums.Length; i++) {
                if (_handleDupes && i > start && _nums[i] == _nums[i - 1]) continue; // skip duplicates
                _tempList.Add(_nums[i]);
                BacktrackSubset(i + 1);
                _tempList.RemoveAt(_tempList.Count - 1);
            }
        }
    }

    public class PermutationIterator
    {
        readonly Action<List<int>> _action;
        readonly bool _handleDupes;
        readonly int[] _nums;
        readonly List<int> _tempList;

        public PermutationIterator(int[] nums, Action<List<int>> action, bool handleDupes = true)
        {
            _action = action;
            _tempList = new List<int>();
            _nums = nums;
            _handleDupes = handleDupes;
            if (handleDupes) Array.Sort(nums);
            BacktrackPermute();
        }

        void BacktrackPermute(long used = 0)
        {
            if (_tempList.Count == _nums.Length) _action(_tempList);
            else
                for (int i = 0; i < _nums.Length; i++) {
                    if (used << ~i < 0 || (_handleDupes && i > 0 && _nums[i] == _nums[i - 1]
                                           && used << ~(i - 1) >= 0)) continue;
                    _tempList.Add(_nums[i]);
                    BacktrackPermute(used | (1L << i));
                    _tempList.RemoveAt(_tempList.Count - 1);
                }
        }
    }

    public class Permutation2Iterator
    {
        readonly Action<int> _action;
        readonly int _nums;

        public Permutation2Iterator(int nums, Action<int> action)
        {
            _action = action;
            _nums = nums;
            BacktrackPermute();
        }

        void BacktrackPermute(long used = 0, int n = 0, int depth = 0)
        {
            if (depth == _nums) _action(n);
            else
                for (int i = 1; i <= _nums; i++)
                    if (used << ~i >= 0)
                        BacktrackPermute(used | (1L << i), n * 10 + i, depth + 1);
        }
    }

    public class CombinationIterator
    {
        readonly Action<List<int>> _action;
        readonly bool _handleDupes;
        readonly int _k;
        readonly int[] _nums;
        readonly List<int> _tempList;

        public CombinationIterator(int[] nums, int k, Action<List<int>> action, bool handleDupes = true)
        {
            _action = action;
            _tempList = new List<int>();
            _nums = nums;
            _handleDupes = handleDupes;
            _k = k;

            if (k > nums.Length) return;
            if (handleDupes) Array.Sort(nums);
            BacktrackCombination(k);
        }

        void BacktrackCombination(int start = 0)
        {
            if (_tempList.Count >= _k) _action(_tempList);
            else
                for (int i = start; i < _nums.Length; i++) {
                    if (_handleDupes && i > start && _nums[i] == _nums[i - 1]) continue; // skip duplicates
                    _tempList.Add(_nums[i]);
                    BacktrackCombination(i + 1);
                    _tempList.RemoveAt(_tempList.Count - 1);
                }
        }
    }

    public class CombinationSumIterator
    {
        readonly Action<List<int>> _action;
        readonly bool _handleDupes;
        readonly int[] _nums;
        readonly List<int> _tempList;

        public CombinationSumIterator(int[] nums, int target, Action<List<int>> action, bool handleDupes = true)
        {
            _action = action;
            _tempList = new List<int>();
            _nums = nums;
            _handleDupes = handleDupes;
            if (handleDupes) Array.Sort(nums);
            BacktrackCombination(target);
        }

        void BacktrackCombination(int remain, int start = 0)
        {
            if (remain < 0) return;
            if (remain == 0) _action(_tempList);
            else
                for (int i = start; i < _nums.Length; i++) {
                    if (_handleDupes && i > start && _nums[i] == _nums[i - 1]) continue; // skip duplicates
                    _tempList.Add(_nums[i]);
                    BacktrackCombination(remain - _nums[i], i + 1);
                    _tempList.RemoveAt(_tempList.Count - 1);
                }
        }
    }

    public static long InitCombination(int n, int k) => ((1L << k) - 1) << (n - k);

    public static long NextCombination(long comb)
    {
        int bitcount = 0;
        long bit = 0;
        while (comb != 0) {
            bitcount++;
            bit = comb & -comb;
            if (bit >= 1L << bitcount)
                break;
            comb -= bit;
        }

        if (comb == 0) return 0;

        comb -= bit;
        comb += ((1L << bitcount) - 1) << (BitTools.Log2(bit) - bitcount);
        return comb;
    }

    #endregion

    #region Shuffling

    /// <summary> Fischer Yates random shuffling </summary>
    public static void Shuffle<T>(this IList<T> list, int start, int count)
    {
        int n = start + count;
        var random = new Random();
        for (int i = start; i + 1 < n; i++) {
            int j = random.Next(i, n); // random # from [i,n)
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        Shuffle(list, 0, list.Count);
    }

    /// <summary> Random cyclic permutation using Sattolos </summary>
    public static void ShuffleCycle<T>(this IList<T> list, int start, int count)
    {
        int n = start + count;
        var random = new Random();
        for (int i = start; i + 1 < n; i++) {
            int j = random.Next(i + 1, n); // random # from [i+1,n)
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    public static int[] ShuffleDerangement(int n)
    {
        int[] derangement = new int[n];

        var r = new Random();
        for (int i = 0; i < n; i++)
            derangement[i] = i;

        // There are no derangements for n<2
        if (n < 2) return derangement;

        for (int i = 0; i + 1 < n; i++) {
            int j = r.Next(i, n); // random # from [i,n)
            int tmp = derangement[j];
            if (tmp != i) {
                derangement[j] = derangement[i];
                derangement[i] = tmp;
            } else {
                i = -1;
            }
        }

        return derangement;
    }

    #endregion
}