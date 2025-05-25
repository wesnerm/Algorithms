namespace Algorithms.Tools;

public class CycleDetection
{
    /// <summary>
    ///     Floyd's cycle algorithm.
    /// </summary>
    /// <param name="head">The head.</param>
    /// <param name="func"></param>
    /// <returns></returns>
    public T DetectCycle<T>(T head, Func<T, T> func)
    {
        var comparer = EqualityComparer<T>.Default;
        T slow = head;
        T fast = head;

        while (fast != null && func(fast) != null) {
            fast = func(func(fast));
            slow = func(slow);
            if (comparer.Equals(fast, slow))
                break;
        }

        if (fast == null || func(fast) == null)
            return default;

        slow = head;
        int cycleStart = 0;
        while (!comparer.Equals(fast, slow)) {
            fast = func(fast);
            slow = func(slow);
            cycleStart++;
        }

        /*
        int cycleLength = 0;
        do
        {
            fast = fast.next;
            cycleLength++;
        }
        while (slow != fast);
        */

        return slow;
    }

    /*
     * You are given a circular linked-list of n nodes. Two pointers start from the same node. One moves a steps at a time and other moves b steps at a time. On which node (with respect to start node) will they meet for the first time.?
     * Let's assume a > b. The problem means that we need to find a number time
     * that satisfy time * (a - b) % n == 0.
     *
     * Find the largest common divisor d for a - b and n.
     * time = n / d
     */

    public T BrentsCycleDetection<T>(T head, Func<T, T> f)
    {
        // main phase: search successive powers of two
        var comparer = EqualityComparer<T>.Default;
        int power = 1;
        int cycleLength = 1;
        T tortoise = head;
        T hare = f(head); // f(x0) is the element/node next to x0.
        while (!comparer.Equals(tortoise, hare)) {
            if (power == cycleLength) // time to start a new power of two?
            {
                tortoise = hare;
                power *= 2;
                cycleLength = 0;
            }

            hare = f(hare);
            cycleLength += 1;
        }

        //Find the position of the first repetition of length λ
        tortoise = hare = head;
        for (int i = 0; i < cycleLength; i++)
            hare = f(hare);

        // The distance between the hare and tortoise is now λ.

        // Next, the hare and tortoise move at same speed until they agree
        int cycleStart;
        for (cycleStart = 0; !comparer.Equals(tortoise, hare); cycleStart++) {
            tortoise = f(tortoise);
            hare = f(hare);
        }

        return hare;
    }
}