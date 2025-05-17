using System.Text;
using System.Threading.Tasks;
using static System.Math;

//namespace Algorithms.Stanford
namespace Algorithms.ComputationalGeometry;

public class SweepLineAlgorithm
{

    public List<Task> Tasks;

    public SweepLineAlgorithm(int n = 0)
    {
        Tasks = new List<Task>(n);
    }

    public Task AddTask(int x1, int x2, object tag)
    {
        if (x1 > x2)
        {
            var tmp = x1;
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

    public Action<Task> Add;

    public Action<Task> Remove;

    public void Execute(Action<State> action)
    {
        var rTasks = Tasks.ToList();
        Tasks.Sort((a, b) => a.X1.CompareTo(b.X2));
        rTasks.Sort((a, b) => a.X2.CompareTo(b.X2));

        var xset = new HashSet<int>();
        foreach(var t in Tasks)
        {
            xset.Add(t.X1);
            xset.Add(t.X2);
        }

        var pts = xset.ToList();
        pts.Sort();

        int left = 0;
        int right = 0;

        var state = new State();
        for (var i = 0; i < pts.Count; i++)
        {
            var x = pts[i];
            var xNext = i < pts.Count - 1 ? pts[i + 1] : x;

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
        public int X1;
        public int X2;
        public object Tag;

        public override string ToString()
        {
            return $"{Tag} [{X1},{X2}]";
        }
    }
}
