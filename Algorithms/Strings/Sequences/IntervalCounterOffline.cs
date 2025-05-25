#region Usings

using Algorithms.RangeQueries;

#pragma warning disable CS0675

#endregion

public class IntervalCounterOffline
{
    readonly List<Interval> intervals = new();
    readonly int size;

    public IntervalCounterOffline(int size) => this.size = size;

    public void Insert(int left, int right, int add = 1)
    {
        if (left <= right)
            intervals.Add(new Interval(left, right, add));
    }

    public void Query(int left, int right, Action<int> action)
    {
        intervals.Add(new Interval(left, right, action));
    }

    public void FindSupersets()
    {
        var ft = new FenwickTree(size);
        List<Interval> leftList = Sorted(intervals);
        int total = 0;
        foreach (Interval q in leftList)
            if (q.query) {
                q.answer = total - (int)ft.SumInclusive(q.right - 1);
                q.action?.Invoke(q.answer);
            } else {
                int add = q.add;
                ft.Add(q.right, add);
                total += add;
            }
    }

    public void FindSupersetsNoSort()
    {
        var ft = new FenwickTree(size);
        List<Interval> leftList = Sorted(intervals);
        int total = 0;
        foreach (Interval q in leftList)
            if (q.query) {
                q.answer = total - (int)ft.SumInclusive(q.right - 1);
                q.action?.Invoke(q.answer);
            } else {
                int add = q.add;
                ft.Add(q.right, add);
                total += add;
            }
    }

    public void FindSubsets()
    {
        var ft = new FenwickTree(size);
        List<Interval> list = Sorted(intervals, -1);
        list.Reverse();
        foreach (Interval q in list)
            if (!q.query) {
                ft.Add(q.right, q.add);
            } else {
                q.answer = (int)ft.SumInclusive(q.right);
                q.action?.Invoke(q.answer);
            }
    }

    public void FindOverlapping()
    {
        var ft = new FenwickTree(size);
        List<Interval> list = Sorted(intervals);

        int total = 0;
        foreach (Interval q in list)
            if (q.query) {
                q.answer += total - (int)ft.SumInclusive(q.left - 1);
            } else {
                ft.Add(q.right, q.add);
                total += q.add;
            }

        list.Reverse();
        ft.Clear();

        total = 0;
        foreach (Interval q in list)
            if (q.query) {
                q.answer += (int)ft.SumInclusive(q.left, q.right);
            } else {
                ft.Add(q.left, q.add);
                total += q.add;
            }

        InvokeActions();
    }

    public void FindOneCommonEndpoint()
    {
        var ft = new FenwickTree(size);
        List<Interval> list = Sorted(intervals, -1);

        foreach (Interval q in list)
            if (q.query)
                q.answer += (int)ft.SumInclusive(q.left, q.right);
            else
                ft.Add(q.right, q.add);

        list.Reverse();
        ft.Clear();

        foreach (Interval q in list)
            if (q.query) {
                q.answer += (int)ft.SumInclusive(q.left, q.right);
            } else {
                ft.Add(q.left, q.add);
                ft.Add(q.right, -q.add);
            }

        InvokeActions();
    }

    void InvokeActions()
    {
        foreach (Interval q in intervals)
            if (q.query)
                q.action?.Invoke(q.answer);
    }

    // Offline points queries  O(n)
    // Just paint sections of an array by adding 1 to A[s] and -1 to A[e+1].

    void Transfer(List<Interval> intervals, Interval[] buckets, bool query)
    {
        foreach (Interval iv in intervals) {
            if (iv.query != query) continue;
            int x = iv.left;
            iv.next = buckets[x];
            buckets[x] = iv;
        }
    }

    List<Interval> Sorted(List<Interval> intervals, int sign = 1)
    {
        var list = new List<Interval>(intervals.Count);
        if (true) {
            // Fast unsorted version
            var buckets = new Interval[size];
            if (sign > 0) Transfer(intervals, buckets, true);
            Transfer(intervals, buckets, false);
            if (sign < 0) Transfer(intervals, buckets, true);

            foreach (Interval iv in buckets)
                for (Interval? cur = iv; cur != null; cur = cur.next)
                    list.Add(cur);
        } else {
            // Sorted
            list.AddRange(intervals);
            if (sign == 0) list.RemoveAll(q => q.query);
            list.Sort((a, b) => {
                int cmp = a.left.CompareTo(b.left);
                if (cmp != 0) return cmp;
                return sign * a.query.CompareTo(b.query);
            });
        }

        return list;
    }

    class Interval
    {
        public readonly Action<int> action;
        public readonly int add;
        public readonly int left;
        public readonly int right;
        public int answer;
        public Interval next;

        public Interval(int left, int right, int add = 1)
        {
            this.left = left;
            this.right = right;
            this.add = add;
        }

        public Interval(int left, int right, Action<int> action = null) : this(left, right, 0) => this.action = action;

        public bool query => add == 0;

        public override string ToString() => $"[{left},{right}] Add={add}";
    }
}