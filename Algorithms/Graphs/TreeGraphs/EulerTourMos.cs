using static System.Math;

namespace Algorithms.Graphs;

// https://www.hackerrank.com/contests/w34/challenges/path-statistics/submissions/code/1302610431
// Euler trees are good for path operations, subtree sums, and lca operations

public class EulerTourMos
{
    public EulerTour Tour;
    public TreeGraph Tree;
    public List<Task> Tasks;
    bool[] flipped;
    Action<int, int> change;

    public EulerTourMos(List<int>[] graph, int root, int queryCount=0)
    {
        Tour = new EulerTour(graph, root);
        Tree = new TreeGraph(graph, root);
        Tasks = new List<Task>(queryCount);
    }

    public void ClearQueries()
    {
        Tasks.Clear();
    }

    public void AddQuery(int x, int y, Action action)
    {
        var lca = Tree.Lca(x, y);
        if (lca == y) Swap(ref x, ref y);

        int start, end, lcaSpecial;
        if (lca == x)
        {
            start = Tour.Begin[x];
            end = Tour.Begin[y];
            lcaSpecial = -1;
        }
        else
        {
            if (Tour.Begin[x] > Tour.Begin[y]) Swap(ref x, ref y);
            start = Tour.End[x];
            end = Tour.Begin[y];
            lcaSpecial = lca;
        }

        if (start > end) Swap(ref start, ref end);

        var task = new Task
        {
            Start = start,
            End = end,
            Lca = lcaSpecial,
            Action = action,
        };

        Tasks.Add(task);
    }

    public void AddPairwiseQuery(int x, int y, Action action)
    {
        throw new NotImplementedException();
    }

    void Flip(int x)
    {
        var node = Tour.Trace[x];
        var add = flipped[node] ^= true;
        change(node, add ? 1 : -1);
    }

    public void Execute(Action<int, int> change)
    {
        this.change = change;
        flipped = new bool[Tree.TreeSize];

        int n = Tour.Trace.Length;
        int sqrt = (int)Ceiling(Sqrt(n));
        Tasks.Sort((x, y) => x.Start / sqrt == y.Start / sqrt
            ? x.End.CompareTo(y.End)
            : x.Start.CompareTo(y.Start));

        CoreOptimized();
    }
    
    void PerformAction(Task task)
    {
        var lca = task.Lca;
        if (lca != -1) Flip(Tour.Begin[lca]);
        task.Action();
        if (lca != -1) Flip(Tour.Begin[lca]);
    }

    void CoreSimple()
    {
        int s = 0;
        int e = s - 1;

        foreach (var task in Tasks)
        {
            while (e < task.End) Flip(++e);
            while (e > task.End) Flip(e--);
            while (s < task.Start) Flip(s++);
            while (s > task.Start) Flip(--s);
            PerformAction(task);
        }
    }

    void CoreOptimized()
    {
        int s = 0;
        int e = s - 1;
        var tour = Tour;

        foreach (var task in Tasks)
        {
            int start = task.Start;
            int end = task.End;

            int end2 = end << 1;
            do
            {
                while (e < end)
                {
                    ++e;
                    var node = tour.Trace[e];
                    var next = tour.End[node];
                    if (e == next || next + e >= end2)
                        // if (e == next || next > end && next - end >= end - e)
                        // if (e == next || next > end)
                        Flip(e);
                    else
                        e = next;
                }
                for (; e > end; e--)
                {
                    var node = tour.Trace[e];
                    var prev = tour.Begin[node];
                    if (e == prev || end2 >= e + prev)
                        //if (e == prev || prev <= end && end - prev >= e - end)
                        //if (e == prev || prev <= end)
                        Flip(e);
                    else
                        e = prev;
                }
            }
            while (e != end);

            int start2 = start << 1;
            do
            {
                while (s > start)
                {
                    --s;
                    var node = tour.Trace[s];
                    var prev = tour.Begin[node];
                    if (s == prev || start2 >= s + prev)
                        // if (s == prev || prev < start && start - prev >= s - start)
                        // if (s == prev || prev < start )
                        Flip(s);
                    else
                        s = prev;
                }
                for (; s < start; s++)
                {
                    var node = tour.Trace[s];
                    var next = tour.End[node];
                    if (s == next || next + s >= start2)
                        //if (s == next || next >= start && next - start >= start - s)
                        //if (s == next || next >= start )
                        Flip(s);
                    else
                        s = next;
                }
            }
            while (s != start);

            PerformAction(task);
        }
    }

    public class Task
    {
        public int Start;
        public int End;
        public int Lca = -1;
        public Action Action;
        public object Tag;

        public override string ToString()
        {
            return $"{Tag} [{Start},{End}]";
        }
    }
}
