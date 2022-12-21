using System.Text.RegularExpressions;
using System.Diagnostics;

namespace AdventOfCode;

using Coordinate = Tuple<int, int>;

public class Day15 : BaseDay
{
    private readonly List<Tuple<Coordinate, Coordinate>> _input;

    public Day15()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        int resultRow = 2000000;
        HashSet<int> result = new();
        HashSet<int> beaconsX = new();

        foreach (Tuple<Coordinate, Coordinate> line in _input)
        {
            if (line.Item2.Item2 == resultRow)
            {
                beaconsX.Add(line.Item2.Item1);
            }

            if (Math.Abs((resultRow - line.Item1.Item2)) < ManhattanDistance(line.Item1, line.Item2))
            {
                int manhattan = ManhattanDistance(line.Item1, line.Item2);
                int length = (manhattan - Math.Abs((resultRow - line.Item1.Item2))) * 2 + 1;
                int lengthSide = length / 2;
                int middle = line.Item1.Item1;
                int left = middle - lengthSide;
                int right = middle + lengthSide;


                for (int i = left; i < right; i++)
                {
                    result.Add(i);
                }
            }
        }

        return new(result.Count.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int resultSize = 4000000;

        //long frequency = Stopwatch.Frequency;
        //long nanosecPerTick = (1000L * 1000L * 1000L) / frequency;

        for (int i = 0; i < resultSize; i++)
        {
            //var watch = new System.Diagnostics.Stopwatch();
            //watch.Start();

            List<List<int>> possible = new();

            foreach (Tuple<Coordinate, Coordinate> line in _input)
            {
                int manhattan = ManhattanDistance(line.Item1, line.Item2);
                if (Math.Abs((i - line.Item1.Item2)) < manhattan)
                {
                    int length = (manhattan - Math.Abs((i - line.Item1.Item2))) * 2 + 1;
                    int lengthSide = length / 2;
                    int middle = line.Item1.Item1;
                    int left = middle - lengthSide;
                    int right = middle + lengthSide;

                    possible.Add(new List<int> { Math.Max(0, left), Math.Min(resultSize, right) });
                }

                //if (possible.Count == resultSize) break;
            }

            possible = MergeIntervals(possible);

            if (possible.Count == 2)
            {
                long x = possible[0][1] + 1;
                //Console.WriteLine($"x: {x}, y: {i}");
                return new((x * 4000000L + i).ToString());
            }

            //watch.Stop();
            //Console.WriteLine($"{watch.ElapsedTicks * nanosecPerTick}");
        }

        return new(0.ToString());
    }

    private List<List<int>> MergeIntervals(List<List<int>> intervals)
    {
        List<List<int>> sortedIntervals = intervals.OrderBy(t => t[0]).ToList();
        List<List<int>> stack = new();
        stack.Add(sortedIntervals[0]);

        for (int i = 1; i < sortedIntervals.Count; i++) // 1 is right. Already added to stack first one.
        {
            List<int> interval = sortedIntervals[i];

            // Check for overlapping interval
            if (stack[^1][0] <= interval[0] && interval[0] <= stack[^1][^1])
            {
                stack[^1][^1] = Math.Max(stack[^1][^1], interval[^1]);
            }
            else
            {
                stack.Add(interval);
            }
        }

        return stack;
    }

    private int ManhattanDistance(Coordinate a, Coordinate b)
    {
        return Math.Abs(a.Item1 - b.Item1) + Math.Abs(a.Item2 - b.Item2);
    }

    private List<Tuple<Coordinate, Coordinate>> ParseInput()
    {
        string input = File.ReadAllText(InputFilePath);
        string pattern = "Sensor at x=(-?[0-9]+), y=(-?[0-9]+): closest beacon is at x=(-?[0-9]+), y=(-?[0-9]+)";

        Regex regex = new Regex(pattern, RegexOptions.Compiled);
        MatchCollection matches = regex.Matches(input);

        List<Tuple<Coordinate, Coordinate>> result = new();

        for (int i = 0; i < matches.Count; i++)
        {
            var line = matches[i].Groups;
            Coordinate sensor = Tuple.Create(Convert.ToInt32(line[1].ToString()), Convert.ToInt32(line[2].ToString()));
            Coordinate beacon = Tuple.Create(Convert.ToInt32(line[3].ToString()), Convert.ToInt32(line[4].ToString()));

            result.Add(Tuple.Create(sensor, beacon));
        }

        return result;
    }
}
