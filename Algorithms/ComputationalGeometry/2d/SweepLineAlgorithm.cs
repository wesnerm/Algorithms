//namespace Algorithms.Stanford

namespace Algorithms.ComputationalGeometry;

public class SweepLineAlgorithm
{
    public Action<Task> Add;

    public Action<Task> Remove;

    public List<Task> Tasks;

    public SweepLineAlgorithm(int n = 0) => Tasks = new List<Task>(n);

    public Task AddTask(int x1, int x2, object tag)
    {
        if (x1 > x2) {
            int tmp = x1;
            x1 = x2;
            x2 = tmp;
        }

        var task = new Task
        {
            X1 = x1,
            X2 = x2,
            Tag = tag,
        };

        Tasks.Add(task);
        return task;
    }

    public void Execute(Action<State> action)
    {
        List<Task> rTasks = Tasks.ToList();
        Tasks.Sort((a, b) => a.X1.CompareTo(b.X2));
        rTasks.Sort((a, b) => a.X2.CompareTo(b.X2));

        var xset = new HashSet<int>();
        foreach (Task t in Tasks) {
            xset.Add(t.X1);
            xset.Add(t.X2);
        }

        List<int> pts = xset.ToList();
        pts.Sort();

        int left = 0;
        int right = 0;

        var state = new State();
        for (int i = 0; i < pts.Count; i++) {
            int x = pts[i];
            int xNext = i < pts.Count - 1 ? pts[i + 1] : x;

            // Add figures
            while (left < Tasks.Count && Tasks[left].X1 <= x)
                Add(Tasks[left++]);

            // Remove figures
            while (right < rTasks.Count && rTasks[right].X2 <= x)
                Remove(rTasks[right++]);

            state.X1 = x;
            state.X2 = xNext;
            action(state);
        }
    }

    public class State
    {
        public int X1;
        public int X2;
    }

    public class Task
    {
        public object Tag;
        public int X1;
        public int X2;

        public override string ToString() => $"{Tag} [{X1},{X2}]";
    }
}