public static class GridVisualization
{
    public static void DisplayGrid<T>(int x1, int x2, int y1, int y2,
        Func<int, int, T> func,
        Action<object> write, int spacing = 8)
    {
        const string NL = "\r\n";
        write(new string(' ', spacing + 1));

        for (int j = y1; j < y2; j++) {
            write(j.ToString().PadRight(spacing));
            write(" ");
        }

        write(NL);

        for (int i = x1; i < x2; i++) {
            write(i.ToString().PadRight(spacing));
            write(" ");
            for (int j = y1; j < y2; j++) {
                write(func(i, j).ToString().PadRight(spacing));
                write(" ");
            }

            write(NL);
        }
    }

    public static void DisplayGridAsArray(int x1, int x2, int y1, int y2,
        Func<int, int, long> func,
        Action<object> write, int spacing = 8)
    {
        const string NL = "\r\n";
        write(NL);

        for (int i = x1; i <= x2; i++) {
            write("{");
            for (int j = y1; j <= y2; j++) {
                write(func(i, j).ToString());
                write(", ");
            }

            write("},\n");
        }
    }

    public static void DisplayGrid<T>(T[,] grid,
        Action<object> write, int spacing = 8)
    {
        DisplayGrid(0, grid.GetLength(0), 0, grid.GetLength(1),
            (x, y) => grid[x, y],
            write, spacing);
    }

    public static void DisplayGrid<T>(T[][] grid,
        Action<object> write, int spacing = 8)
    {
        DisplayGrid(0, grid.Length, 0, grid[0].Length,
            (x, y) => grid[x][y],
            write, spacing);
    }
}