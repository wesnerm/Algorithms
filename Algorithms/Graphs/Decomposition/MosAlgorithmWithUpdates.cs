namespace Algorithms.Graphs;

public class MosAlgorithmWithUpdates
{
    public Action<int, bool> Add;

    public Action<int, bool> Remove;
    public List<Task> Tasks;

    public MosAlgorithmWithUpdates(int n = 0) => Tasks = new List<Task>(n);

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

    public void Execute()
    {
        int max = Tasks.Max(t => t.End);
        int min = Tasks.Min(t => t.Start);

        int s = min;
        int e = s - 1;

        int n = max - s + 1;
        int f = (int)(Math.Pow(n, 2.0 / 3.0) + 1);

        Tasks.Sort((x, y) => {
            int cmp = x.Start / f - y.Start / f;
            if (cmp != 0) return cmp;
            cmp = x.End / f - y.End / f;
            if (cmp != 0) return cmp;
            return x.Time - y.Time;
        });

        foreach (Task task in Tasks) {
            // One optimization to take advantage of any gaps
            // if (task.Start > e)
            // {
            //     while (s < e) Remove(s++, false);
            //     s = e = task.Start;
            // }

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
        public Action<Task> Action;
        public int End;
        public int Start;
        public object Tag;
        public int Time;

        public override string ToString() => $"{Tag} [{Start},{End}]";
    }
}