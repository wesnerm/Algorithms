namespace Algorithms.Collections;

public class ArraySearch
{
    /// <summary>
    ///     Finds the maximum of a unimodal function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">The array.</param>
    /// <param name="start">The start.</param>
    /// <param name="count">The count.</param>
    /// <returns></returns>
    public static int TernarySearch(int left, int right, Func<int, int> func)
    {
        while (left < right) {
            int third = (right - left) / 3;
            int leftThird = left + third;
            int rightThird = right - third;
            ;
            if (func(leftThird) < func(rightThird))
                left = leftThird;
            else
                right = rightThird;
        }

        return left;
    }

    public static double TernarySearchMinimum(int left, int right,
        Func<int, double> f)
    {
        while (left < right) {
            int mid = (left + right) >> 1;
            if (f(mid) >= f(mid + 1))
                left = mid + 1;
            else
                right = mid;
        }

        return left;
    }

    public static double TernarySearchMaximum(double left, double right,
        Func<double, double> f,
        double precision = 1e-14)
    {
        while (Math.Abs(right - left) >= precision) {
            double leftThird = (2 * left + right) / 3;
            double rightThird = (left + 2 * right) / 3;
            if (f(leftThird) < f(rightThird))
                left = leftThird;
            else
                right = rightThird;
        }

        return (left + right) / 2;
    }

    public static int UpperBound(int[] nums, int left, int right, int k)
    {
        while (left <= right) {
            int mid = (left + right) / 2;
            if (k >= nums[mid])
                left = mid + 1;
            else
                right = mid - 1;
        }

        return left;
    }

    public static int LowerBound(int[] nums, int left, int right, int k)
    {
        while (left <= right) {
            int mid = (left + right) / 2;
            if (k > nums[mid])
                left = mid + 1;
            else
                right = mid - 1;
        }

        return left;
    }

    public static int MooresVotingAlgorithm(int[] a)
    {
        int maj_index = 0;
        int count = 1;

        for (int i = 1; i < a.Length; i++) {
            if (a[maj_index] == a[i])
                count++;
            else
                count--;
            if (count == 0) {
                maj_index = i;
                count = 1;
            }
        }

        return a[maj_index];
    }

    public static int MooresVotingAlgorithm3(int[] a)
    {
        int maj_index = 0;
        int count = 1;

        for (int i = 1; i < a.Length; i++) {
            if (a[maj_index] == a[i])
                count++;
            else
                count--;
            if (count == 0) {
                maj_index = i;
                count = 1;
            }
        }

        return a[maj_index];
    }

    // https://discuss.leetcode.com/topic/17396/boyer-moore-majority-vote-algorithm-generalization
    // TODO: Looks: buggy
    List<int> MajorityElementFor2(int[] a)
    {
        int y = 0, z = 1, cy = 0, cz = 0;
        foreach (int x in a)
            if (x == y) {
                cy++;
            } else if (x == z) {
                cz++;
            } else if (cy == 0) {
                y = x;
                cy = 1;
            } else if (cz == 0) {
                z = x;
                cz = 1;
            } else {
                cy--;
                cz--;
            }

        cy = cz = 0;
        foreach (int x in a)
            if (x == y) cy++;
            else if (x == z) cz++;

        var r = new List<int>();
        if (cy > a.Length / 3) r.Add(y);
        if (cz > a.Length / 3) r.Add(z);
        return r;
    }

    public static int JumpSearch1Based(Func<int, bool> test, int n)
    {
        int left = 1;
        int right = n;

        while (left < right)
            if (test(left) == false) {
                if (left * 2 < right) {
                    left = left * 2;
                } else {
                    left = left + 1;
                    break;
                }
            } else {
                right = left - 1;
                left = (left >> 1) + 1;
                break;
            }

        while (left <= right) {
            int mid = left + ((right - left) >> 1);
            if (test(mid) == false)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return left;
    }

    public static int JumpSearch(Func<int, bool> test, int left, int right)
    {
        return JumpSearch1Based(x => test(left + x - 1), right - left + 1);
    }

    public static int BinarySearch(int left, int right, Func<int, bool> test)
    {
        while (left <= right) {
            int mid = left + ((right - left) >> 1);
            if (test(mid) == false)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return left;
    }
}