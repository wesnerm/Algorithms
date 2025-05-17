using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using BigInt = System.Numerics.BigInteger;

namespace Algorithms.Mathematics
{
    class PiAlgorithms
    {
        const string PiString = "1415926535897932384626433832795028841971693993751";
        static BigInt PiDenominator = BigInt.Pow(10, PiString.Length);
        static BigInt PiNumerator = BigInt.Parse(PiString);
        static BigInt PiNumerator3 = PiNumerator + 3 * PiDenominator;

        public static IEnumerable<Rational> EnumerateConvergents(bool all = true)
        {
            bool s = false;
            long pn = 2;
            long pd = 1;
            long qn = 4;
            long qd = 1;

            while (true)
            {
                var mn = pn + qn;
                var md = pd + qd;
                var gcd = Gcd(mn, md);
                mn /= gcd;
                md /= gcd;

                var olds = s;
                if (mn * PiDenominator < PiNumerator3 * md)
                {
                    s = true;
                    pn = mn;
                    pd = md;
                }
                else
                {
                    s = false;
                    qn = mn;
                    qd = md;
                }

                if (all || s != olds)
                    yield return new Rational(mn, md);
            }

            // all
            // 3/1 7/2 10/3 13/4 16/5 19/6 22/7 25/8 47/15 69/22
            // 91/29 113/36 135/43

            // convergents
            // 3/1 22/7 333/106 355/113 103993/33102 104348/33215
            // 208341/66317 312689/99532
        }

        public static long Gcd(long a, long b)
        {
            if (a == 0) return b;
            return Gcd(b % a, a);
        }

    }

    public class Rational : IComparable<Rational>
    {
        public readonly long Numerator;
        public readonly long Denominator;

        public Rational(long n, long d, bool reduce = false)
        {
            Numerator = n;
            Denominator = d;
            if (reduce)
            {
                long g = Gcd(n, d);
                Numerator /= g;
                Denominator /= g;
            }
        }

        public int CompareTo(Rational other)
        {
            int cmp = Denominator.CompareTo(other.Denominator);
            if (cmp != 0)
                return cmp;
            return Numerator.CompareTo(other.Numerator);
        }

        public static long Gcd(long a, long b)
        {
            if (a == 0) return b;
            return Gcd(b % a, a);
        }

    }

}
