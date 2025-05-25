namespace Algorithms.Collections.Functional;

public struct VersionVector
    : IComparable<VersionVector>,
        IEquatable<VersionVector>
{
    int[] _ids;
    int[] _timestamps;

    void LazyInit()
    {
        if (_ids == null) {
            _ids = Array.Empty<int>();
            _timestamps = Array.Empty<int>();
        }
    }

    public VersionVector Bump(int id)
    {
        int i = Array.BinarySearch(_ids, id);
        if (i >= 0)
            return new VersionVector
            {
                _ids = _ids,
                _timestamps = _timestamps.ReplaceAt(i, _timestamps[i] + 1),
            };
        i = ~i;
        return new VersionVector
        {
            _ids = _ids.Insert(i, id),
            _timestamps = _timestamps.Insert(i, 1),
        };
    }

    /// <summary>
    ///     Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <returns>
    ///     true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
    /// </returns>
    /// <param name="other">An object to compare with this object.</param>
    public bool Equals(VersionVector other) =>
        ArrayTools.ArrayEqual(ref _ids, ref other._ids)
        && ArrayTools.ArrayEqual(ref _timestamps, ref _timestamps);

    /// <summary>
    ///     Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <returns>
    ///     true if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, false.
    /// </returns>
    /// <param name="obj">The object to compare with the current instance. </param>
    public override bool Equals(object obj) => obj is VersionVector && Equals((VersionVector)obj);

    public override int GetHashCode()
    {
        int hashcode = 0;

        if (_ids != null) {
            foreach (int id in _ids) hashcode = HashCode.Combine(hashcode, id);

            foreach (int timestamp in _timestamps) hashcode = HashCode.Combine(hashcode, timestamp);
        }

        return hashcode;
    }

    /// <summary>
    ///     Compares the current instance with another object of the same type and returns an integer that indicates whether
    ///     the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
    /// </summary>
    /// <returns>
    ///     A value that indicates the relative order of the objects being compared. The return value has these meanings: Value
    ///     Meaning Less than zero This instance precedes <paramref name="other" /> in the sort order.  Zero This instance
    ///     occurs in the same position in the sort order as <paramref name="other" />. Greater than zero This instance follows
    ///     <paramref name="other" /> in the sort order.
    /// </returns>
    /// <param name="other">An object to compare with this instance. </param>
    public int CompareTo(VersionVector other)
    {
        if ((object)other == null)
            return 1;
        int cmp = _ids.Length.CompareTo(other._ids.Length);
        if (cmp != 0)
            return cmp;

        for (int i = 0; i < _timestamps.Length; i++) {
            cmp = _ids[i] - other._ids[i];
            if (cmp < 0)
                return -cmp; // implied zero on the right side

            cmp = _timestamps[i] - other._timestamps[i];
            if (cmp != 0)
                return cmp;
        }

        return 0;
    }

    static bool CheckLess(VersionVector t1, VersionVector t2, bool orEqual)
    {
        if (t1 == null) return t2 != null;
        if (t2 == null) return false;

        bool lesser = false;
        int[] ts1 = t1._timestamps;
        int[] ts2 = t2._timestamps;

        int cmp = ts1.Length - ts2.Length;
        if (cmp == 0) {
            if (!ArrayTools.ArrayEqual(ref t1._ids, ref t2._ids))
                return false;

            if (ts1 != ts2)
                for (int i = ts1.Length - 1; i >= 0; i--) {
                    cmp = ts1[i] - ts2[i];
                    if (cmp > 0)
                        return false;
                    if (cmp < 0)
                        lesser = true;
                }
        } else {
            int i = ts1.Length - 1;
            int j = ts2.Length - 1;
            int[] ids1 = t1._ids;
            int[] ids2 = t2._ids;
            while (i >= 0 && j >= i) {
                cmp = ids1[i] - ids2[j];
                if (cmp > 0)
                    return false;
                j--;
                if (cmp < 0)
                    continue;

                cmp = ts1[i] - ts2[j];
                if (cmp > 0)
                    return false;
                if (cmp < 0)
                    lesser = true;
                i--;
            }

            if (i >= 0)
                return false;
        }

        return lesser || orEqual;
    }

    public static bool operator <(VersionVector t1, VersionVector t2) => CheckLess(t1, t2, false);

    public static bool operator >(VersionVector t1, VersionVector t2) => CheckLess(t2, t1, false);

    public static bool operator <=(VersionVector t1, VersionVector t2) => CheckLess(t1, t2, true);

    public static bool operator >=(VersionVector t1, VersionVector t2) => CheckLess(t2, t1, true);

    public static bool operator !=(VersionVector t1, VersionVector t2) => !Equals(t1, t2);

    public static bool operator ==(VersionVector t1, VersionVector t2) => Equals(t1, t2);

    public static bool IsConcurrent(VersionVector t1, VersionVector t2) => !(t1 >= t2) && !(t2 >= t1);

    public static VersionVector Merge(VersionVector t1, VersionVector t2)
    {
        int i = t1._ids.Length - 1;
        if (ArrayTools.ArrayEqual(ref t1._ids, ref t2._ids)) {
            int[] ids = t1._ids;
            int[] timestamps = new int[ids.Length];
            for (i = t1._ids.Length - 1; i > 0; i--)
                timestamps[i] = Math.Max(t1._timestamps[i],
                    t2._timestamps[i]);

            return new VersionVector
            {
                _ids = ids,
                _timestamps = timestamps,
            };
        }

        int j = t2._ids.Length - 1;
        var list = new List<int>(Math.Max(t1._ids.Length, t2._ids.Length));
        var tslist = new List<int>(list.Capacity);
        while (i >= 0 && j >= 0) {
            int cmp = t1._ids[i] - t2._ids[j];
            if (cmp > 0) {
                list.Add(t2._ids[j]);
                tslist.Add(t2._timestamps[j]);
                j--;
            } else if (cmp < 0) {
                list.Add(t1._ids[i]);
                tslist.Add(t1._timestamps[i]);
                i--;
            } else {
                list.Add(t1._ids[i]);
                tslist.Add(Math.Max(t1._timestamps[i], t2._timestamps[j]));
                i--;
                j--;
            }
        }

        while (i >= 0)
            list.Add(t1._ids[i--]);
        while (j >= 0)
            list.Add(t2._ids[j--]);
        list.Reverse();
        tslist.Reverse();
        return new VersionVector
        {
            _ids = list.ToArray(),
            _timestamps = tslist.ToArray(),
        };
    }
}