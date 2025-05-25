using System.Text.RegularExpressions;
using static System.Math;

// TESTS
// https://www.hackerrank.com/challenges/stitch-the-torn-wiki/problem
// https://www.hackerrank.com/challenges/matching-book-names-and-descriptions

namespace Algorithms.MachineLearning;

public class Tdidf
{
    readonly Dictionary<string, int> collectionFrequency = new();
    readonly List<Document> documents = new();
    public bool Distinct;

    public Document AddDocument(string text) => AddDocument(Words(text));

    public Document AddDocument(IEnumerable<string> terms)
    {
        var doc = new Document { Index = documents.Count };
        documents.Add(doc);

        Dictionary<string, int> docFrequency = doc.Frequency;

        foreach (string t in terms) {
            if (!docFrequency.ContainsKey(t)) docFrequency[t] = 0;
            docFrequency[t]++;
        }

        foreach (string t in docFrequency.Keys) {
            int df;
            bool found = collectionFrequency.TryGetValue(t, out df);
            Debug.Assert(found || df == 0);
            collectionFrequency[t] = df + 1;
        }

        return doc;
    }

    public double[] Query(string text) => Query(Words(text));

    public double[] Query(IEnumerable<string> terms)
    {
        int N = documents.Count;
        double[] results = new double[documents.Count];

        IEnumerable<string> termSet = terms;

        if (Distinct)
            termSet = new HashSet<string>(termSet);

        double idfConstant = Log(N * 1d);
        foreach (string t in termSet) {
            int dft;
            if (!collectionFrequency.TryGetValue(t, out dft)) continue;

            double idf = idfConstant - Log(dft);
            foreach (Document doc in documents) {
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
        return Regex.Matches(doc, @"\w+").Select(x => x.Value.ToUpperInvariant());
    }

    public double DocumentSimilarity(Document d1, Document d2)
    {
        if (d1.Frequency.Count > d2.Frequency.Count)
            return DocumentSimilarity(d2, d1);

        int N = documents.Count;

        double sum = 0;

        foreach (KeyValuePair<string, int> pair in d1.Frequency) {
            string t = pair.Key;
            int counts1 = pair.Value;

            int counts2;
            if (!d2.Frequency.TryGetValue(t, out counts2))
                continue;

            double tf1 = ComputeTf(counts1);
            double tf2 = ComputeTf(counts2);
            double idf = Log(N * 1d / collectionFrequency[t]);
            sum = tf1 * idf * (tf2 * idf);
        }

        double weights = DocumentWeight(d1) * DocumentWeight(d2);
        return sum / weights;
    }

    double DocumentWeight(Document doc)
    {
        int N = documents.Count;
        double weight = doc.Weight;
        if (weight != 0) return weight;

        foreach (KeyValuePair<string, int> pair in doc.Frequency) {
            string t = pair.Key;
            double tf = ComputeTf(pair.Value);
            double idf = Log(N * 1d / collectionFrequency[t]);
            double wtd = tf * idf;
            weight += wtd * wtd;
        }

        weight = Sqrt(weight);
        doc.Weight = weight != 0 ? weight : 1.0;
        return weight;
    }

    double ComputeTf(int counts) =>
        // Boolean frequencies: tf = Sign(tftd)
        // Adjusted for doc length: tf = tftd / doc.Values.Sum()
        // Plain: tf = tftd
        // Augmented frequency: tf = 0.5 + 0.5 * tftd / doc.Values.Max();
        1.0 + Log(counts);

    public class Document
    {
        public Dictionary<string, int> Frequency = new();
        public int Index;
        public object Tag;
        public double Weight;
    }
}