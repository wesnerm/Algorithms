namespace Algorithms.Mathematics;

public static class LinearFactorization
{
    // Linear and sublinear prime sieves

    // "An Introduction to Prime Number Sieves" Johnathan Sorenson (1990)
    // http://research.cs.wisc.edu/techreports/1990/TR909.pdf

    // The linear algorithms are slower than the standard n log log n algorithms.

    // Using a wheel may given a double speedup

    public static bool[] LinearPrimesExplanatory(int n)
    {
        n++;
        int[] next = new int[n];
        int[] prev = new int[n];

        next[2] = 3;
        prev[3] = 2;
        for (int i = 3; i < n; i += 2) next[i] = i + 2;

        for (int i = 5; i < n; i += 2) prev[i] = i - 2;

        int p = 3;

        while (p * p < n) {
            // Remove multiples
            int f = p;
            while (p * next[f] < n) f = next[f];

            // Loop down through the values
            do {
                int pf = p * f;
                int prevpf = prev[pf];
                int nextpf = next[pf];
                next[prevpf] = nextpf;
                if (nextpf < n) prev[nextpf] = prevpf;

                f = prev[f];
            } while (f >= p);

            // Find the next prime
            p = next[p];
        }

        bool[] isPrime = new bool[n];
        p = 2;
        while (p < n) {
            isPrime[p] = true;
            p = next[p];
        }

        return isPrime;
    }

    public static bool[] LinearPrimes(int n)
    {
        int[] next = LinearPrimesLinkedList(n);

        n++;
        bool[] isPrime = new bool[n];
        int p = 2;
        while (p < n) {
            isPrime[p] = true;
            Console.Error.WriteLine(p);
            p = next[p];
        }

        return isPrime;
    }

    public static int[] LinearPrimesList(int n)
    {
        int[] next = LinearPrimesLinkedList(n);

        int count = 0;
        for (int p = 2; p <= n; p = next[p]) count++;

        int[] result = new int[count];

        count = 0;
        for (int p = 2; p <= n; p = next[p]) result[count++] = p;

        return result;
    }

    public static int[] LinearPrimesLinkedList(int n)
    {
        n++;
        int[] next = new int[n];

        next[2] = 3;
        next[3] = 5;

        for (int i = 5; i < n; i += 2) {
            next[i - 1] = i - 2;
            next[i] = i + 2;
        }

        int p = 3;
        while (p * p < n) {
            // Remove multiples
            int f = p;
            while (p * next[f] < n) f = next[f];

            // Loop down through the values
            while (true) {
                int pf = p * f;
                int prevpf = next[pf - 1];
                int nextpf = next[pf];
                next[prevpf] = nextpf;
                if (nextpf < n) next[nextpf - 1] = prevpf;

                if (f == p) break;

                f = next[f - 1];
            }

            ;

            // Find the next prime
            //var oldp = p;
            p = next[p];
            //next[oldp] = oldp;
        }

        return next;
    }

    public static int[] LinearLowestPrimeFactor(int n, bool full = true)
    {
        n++;
        int[] next = new int[n];

        next[2] = 3;
        for (int i = 3; i < n; i += 2) next[i] = i + 2;

        for (int i = 5; i < n; i += 2) next[i - 1] = i - 2;

        int p = 3;
        while (p * p < n) {
            // Remove multiples
            int f = p;
            while (p * next[f] < n) f = next[f];

            // Loop down through the values
            while (true) {
                int pf = p * f;
                int prevpf = next[pf - 1];
                int nextpf = next[pf];
                next[prevpf] = nextpf;
                if (nextpf < n) next[nextpf - 1] = prevpf;

                next[pf] = p;
                if (f == p) break;

                f = next[f - 1];
            }

            ;

            // Find the next prime
            //var oldp = p;
            p = next[p];
            //next[oldp] = oldp;
        }

        // At this point, if next[p]>p, then p is prime, otherwise p is non-prime

        if (full) {
            next[0] = 0;
            next[1] = 1;
            for (int i = 2; i < n; i += 2) next[2] = 2;

            p = 3;
            while (p < n) {
                int nextp = next[p];
                next[p] = p;
                p = nextp;
            }
        }

        return next;
    }
}