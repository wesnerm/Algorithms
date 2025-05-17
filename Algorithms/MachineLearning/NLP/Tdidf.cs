using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Math;

// TESTS
// https://www.hackerrank.com/challenges/stitch-the-torn-wiki/problem
// https://www.hackerrank.com/challenges/matching-book-names-and-descriptions

namespace Algorithms.MachineLearning;

public class Tdidf
{
    private Dictionary<string, int> collectionFrequency = new Dictionary<string, int>();
    private List<Document> documents = new List<Document>();
    public bool Distinct;

    public Tdidf()
    {
        
    }

    public Document AddDocument(string text)
    {
        return AddDocument(Words(text));    
    }

    public Document AddDocument(IEnumerable<string> terms)
    {
        var doc = new Document {Index = documents.Count};
        documents.Add(doc);

        var docFrequency = doc.Frequency;

        foreach (var t in terms)
        {
            if (!docFrequency.ContainsKey(t)) docFrequency[t] = 0;
            docFrequency[t]++;
        }

        foreach (var t in docFrequency.Keys)
        {
            int df;
            bool found = collectionFrequency.TryGetValue(t, out df);
            Debug.Assert(found || df == 0);
            collectionFrequency[t] = df + 1;
        }

        return doc;
    }

    public double[] Query(string text)
    {
        return Query(Words(text));
    }

    public double[] Query(IEnumerable<string> terms)
    {
        int N = documents.Count;
        var results = new double[documents.Count];

        var termSet = terms;

        if (Distinct)
            termSet = new HashSet<string>(termSet);

        double idfConstant = Log(N * 1d);
        foreach (var t in termSet)
        {
            int dft;
            if (!collectionFrequency.TryGetValue(t, out dft)) continue;

            double idf = idfConstant - Log(dft);
            foreach (var doc in documents)
            {
                int counts;
                if (!doc.Frequency.TryGetValue(t, out counts)) continue;
                double tf = ComputeTf(counts);
                double wtd = tf * idf;
                results[doc.Index] += wtd;
            }
        }
        return results;
    }

    IEnumerable<string> Words(string doc)
    {
        return Regex.Matches(doc, @"\w+").Cast<Match>().Select(x => x.Value.ToUpperInvariant());
    }

    public double DocumentSimilarity(Document d1, Document d2)
    {
        if (d1.Frequency.Count > d2.Frequency.Count)
            return DocumentSimilarity(d2, d1);

        int N = documents.Count;

        double sum = 0;

        foreach (var pair in d1.Frequency)
        {
            var t = pair.Key;
            var counts1 = pair.Value;

            int counts2;
            if (!d2.Frequency.TryGetValue(t, out counts2))
                continue;

            double tf1 = ComputeTf(counts1);
            double tf2 = ComputeTf(counts2);
            double idf = Log(N * 1d / collectionFrequency[t]);
            sum = (tf1 * idf) * (tf2 * idf);
        }

        double weights = DocumentWeight(d1) * DocumentWeight(d2);
        return sum / weights;
    }

    double DocumentWeight(Document doc)
    {
        int N = documents.Count;
        double weight = doc.Weight;
        if (weight != 0) return weight;

        foreach (var pair in doc.Frequency)
        {
            var t = pair.Key;
            double tf = ComputeTf(pair.Value);
            double idf = Log(N * 1d / collectionFrequency[t]);
            double wtd = tf * idf;
            weight += wtd * wtd;
        }

        weight = Sqrt(weight);
        doc.Weight = weight != 0 ? weight : 1.0;
        return weight;
    }

    double ComputeTf(int counts)
    {
        // Boolean frequencies: tf = Sign(tftd)
        // Adjusted for doc length: tf = tftd / doc.Values.Sum()
        // Plain: tf = tftd
        // Augmented frequency: tf = 0.5 + 0.5 * tftd / doc.Values.Max();
        return 1.0 + Log(counts);
    }
    
    public class Document
    {
        public Dictionary<string, int> Frequency = new Dictionary<string, int>();
        public int Index;
        public double Weight;
        public object Tag;
    }

}
