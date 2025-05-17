namespace Algorithms.Graphs;

public class MosAlgorithm
{
    public List<Task> Tasks;

    public MosAlgorithm(int n=0)
    {
        Tasks = new List<Task>(n);
    }

    public Task AddTask(int start, int end, Action<Task> action)
    {
        if (start > end)
        {
            var tmp = start;
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

    public Action<int, bool> Add;

    public Action<int, bool> Remove;

    public void Execute()
    {
        int max = Tasks.Max(t => t.End);
        int min = Tasks.Min(t => t.Start);

        int s = min;
        int e = s-1;

        int n = max - s + 1;
        int sqrt = (int) Math.Ceiling(Math.Sqrt(n));

        Tasks.Sort((x,y)=> x.Start/sqrt == y.Start/sqrt
            ? x.End.CompareTo(y.End)
            : x.Start.CompareTo(y.Start));

        // POSTPONE: 
        // One optimization we can make is to take advantage of any gaps
        // with no overlapping so we can start from the latest position

        foreach (var task in Tasks)
        {
            while (e < task.End) Add(++e, true);
            while (e > task.End) Remove(e--, true);
            while (s < task.Start) Remove(s++, false);
            while (s > task.Start) Add(--s, false);
            task.Action(task);
        }

        Tasks.Clear();
    }

    public class Task
    {
        public int Start;
        public int End;
        public Action<Task> Action;
        public object Tag;

        public override string ToString()
        {
            return $"{Tag} [{Start},{End}]";
        }
    }

}
