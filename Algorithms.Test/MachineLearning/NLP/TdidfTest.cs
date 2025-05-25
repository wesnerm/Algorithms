namespace Algorithms.MachineLearning;

[TestFixture]
public class TdidfTest
{
    [Test]
    public void QueryTest()
    {
        Tdidf tdidf = Load();

        MatchDocument(tdidf, 2, 0);
        MatchDocument(tdidf, 0, 1);
        MatchDocument(tdidf, 1, 2);
        MatchDocument(tdidf, 3, 3);
        MatchDocument(tdidf, 4, 4);
    }

    public void MatchDocument(Tdidf tdidf, int d, int title)
    {
        string query = titles[title];
        double[] vector = tdidf.Query(query);
        int argMax = ArgMax(vector);
        AreEqual(d, argMax);
    }

    public int ArgMax(double[] results)
    {
        int argMax = 0;
        for (int i = 1; i < results.Length; i++)
            if (results[i] > results[argMax])
                argMax = i;
        return argMax;
    }

    Tdidf Load()
    {
        var tdidf = new Tdidf();
        foreach (string d in docs)
            tdidf.AddDocument(d);
        return tdidf;
    }

    static readonly string[] titles =
    {
        "How to Be a Domestic Goddess : Baking and the Art of Comfort Cooking(Paperback)",
        "Embedded / Real-Time Systems 1st Edition(Paperback)",
        "The Merchant of Venice(Paperback)",
        "Lose a Kilo a Week(Paperback)",
        "Few Things Left Unsaid(Paperback)",
    };

    static readonly string[] docs =
    {
        "Today the embedded systems are ubiquitous in occurrence, most significant in function and project an absolutely promising picture of developments in the near future.",
        "The Merchant Of Venice is one of Shakespeare's best known plays.",
        "How To Be A Domestic Goddess: Baking and the Art of Comfort Cooking is a bestselling cookbook by the famous chef Nigella Lawson who aims to introduce the art of baking through text with an emphasis.",
        "Lose A Kilo A Week is a detailed diet and weight loss plan, and also shows how to maintain the ideal weight after reaching it.",
        "Few Things Left Unsaid is a story of love, romance, and heartbreak.",
    };
    // private Statistics sample;
}