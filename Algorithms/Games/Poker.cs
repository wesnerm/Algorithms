namespace Algorithms.Mathematics.Games;

public class Poker
{
    const int BitShift = 24;
    const int Shift = 1 << BitShift;

    public const int HighCard = 0;
    public const int Pair = 1;
    public const int TwoPairs = 2;
    public const int ThreeOfAKind = 3;
    public const int Straight = 4;
    public const int Flush = 5;
    public const int FullHouse = 6;
    public const int FourOfAKind = 7;
    public const int FourOfAKindFlush = 8;
    public const int StraightFlush = 9;
    public const int RoyalFlush = 10;

    public const int Ace = 14;
    public const int King = 13;
    public const int Queen = 12;
    public const int Jack = 11;

    public const int Diamonds = 0;
    public const int Spades = 1;
    public const int Clubs = 2;
    public const int Hearts = 3;

    public static int Value(string s)
    {
        switch (s[0]) {
            case 'A': return Ace;
            case 'K': return King;
            case 'Q': return Queen;
            case 'J': return Jack;
            case 'T': return 10;
            default: return s[0] - '0';
        }
    }

    public static int Suit(string s)
    {
        switch (s[s.Length - 1]) {
            case 'D': return Diamonds;
            case 'C': return Clubs;
            case 'S': return Spades;
            default: return Hearts;
        }
    }

    public static long RankIt(string[] array)
    {
        array = array.ToArray();
        Array.Sort(array, (a, b) => Value(a).CompareTo(Value(b)));

        int vFlags = 0;
        int sFlags = 0;
        int pairFlags = 0;
        int tripleFlags = 0;
        int fourFlags = 0;

        foreach (string s in array) {
            int v = Value(s);
            int suit = Suit(s);
            fourFlags |= tripleFlags & (1 << v);
            tripleFlags |= pairFlags & (1 << v);
            pairFlags |= vFlags & (1 << v);
            vFlags |= 1 << v;
            sFlags |= 1 << suit;
        }

        if (vFlags == ((1 << Ace) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5)))
            vFlags = (1 << 1) | (1 << 2) | (1 << 3) | (1 << 4) | (1 << 5);

        int tmp = vFlags;
        while (tmp > 0 && (tmp & 1) == 0) tmp >>= 1;

        bool straight = tmp == (1 << 5) - 1;
        bool flush = (sFlags & (sFlags - 1)) == 0;

        long rank = HighCard * Shift;

        if (pairFlags != 0) {
            rank = Pair * Shift + pairFlags;

            if ((pairFlags & (pairFlags - 1)) > 0)
                rank = TwoPairs * Shift + pairFlags;
        }

        if (tripleFlags != 0)
            rank = ThreeOfAKind * Shift + tripleFlags;

        if (straight)
            rank = Straight * Shift + vFlags;

        if (flush)
            rank = Flush * Shift + tripleFlags;

        if (tripleFlags != 0 && (pairFlags & ~tripleFlags) != 0)
            rank = FullHouse * Shift + tripleFlags;

        if (fourFlags != 0)
            rank = (flush ? FourOfAKindFlush : FourOfAKind) * Shift + fourFlags;

        if (straight && flush) {
            rank = StraightFlush * Shift + vFlags;
            if ((vFlags & ((1 << Ace) | (1 << King))) == ((1 << Ace) | (1 << King)))
                rank = RoyalFlush * Shift;
        }

        for (int i = array.Length - 1; i >= 0; i--)
            rank = rank * 16 + Value(array[i]);

        return rank;
    }

    public static int Hand(long rank, int cardCount = 5) => (int)(rank >> (cardCount * 4 + BitShift));
}