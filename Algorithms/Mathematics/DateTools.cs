namespace Algorithms.Mathematics;

public class DateTools
{
    static int GregorianDay(int year, int month, int d)
    {
        int m = month;
        int y = year;
        if (m < 3) {
            y--;
            m += 12;
        }

        //int k = year % 100;
        //int j = year / 100;
        //var h = (d + 13 * (m+1) / 5 + k + k/4 + j/4 + 5*j) % 7; // Doesn't work
        int h = (d + 13 * (m + 1) / 5 + y + y / 4 - y / 100 + y / 400) % 7;
        h = (h + 6) % 7;

        //var date = new DateTime(year, month, d);
        //int h2 = (int)(date.DayOfWeek);
        //if (h2 != h)
        //    Console.Error.WriteLine($"Mismatch({date}) {h} {h2}");

        // Sunday is 0, Saturday is 6
        return h;
    }

    static int JulianDay(int year, int month, int d)
    {
        int m = month;
        int y = year;
        if (m < 3) {
            y--;
            m += 12;
        }

        int h = (d + 13 * (m + 1) / 5 + y + y / 4 + 5) % 7;
        h = (h + 6) % 7;

        // Sunday is 0, Saturday is 6
        return h;
    }
}