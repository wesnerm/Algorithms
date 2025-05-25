namespace Algorithms.Graphs;

public class MosAlgorithmCum
{
    public Action<int, bool> Add;

    public Action<int, bool> Remove;
    public List<Task> Tasks;

    public MosAlgorithmCum(int n = 0) => Tasks = new List<Task>(n);

    public Task AddTask(int start, int end, Action<Task> action)
    {
        if (start > end) {
            int tmp = start;
            start = end;
            end = tmp;
        }

        var task = new Task
        {
            Start = start,
            End = end,
            Action = action,
        };

        Tasks.Add(task);
        return task;
    }

    public void ExecuteCumulative()
    {
        int max = Tasks.Max(t => t.End);

        int s = 0;
        int e = s;

        int n = max - s + 1;
        int sqrt = (int)Math.Ceiling(Math.Sqrt(n));

        Tasks.Sort((x, y) => x.Start / sqrt == y.Start / sqrt
            ? x.End.CompareTo(y.End)
            : x.Start.CompareTo(y.Start));

        foreach (Task task in Tasks) {
            while (s < task.Start) Add(++s, false);
            while (e < task.End) Add(++e, true);
            while (e > task.End) Remove(e--, true);
            while (s > task.Start) Remove(s--, false);
            task.Action(task);
        }

        Tasks.Clear();
    }

    public class Task
    {
        public Action<Task> Action;
        public int End;
        public int Start;
        public object Tag;

        public override string ToString() => $"{Tag} [{Start},{End}]";
    }
}