using System.Text.RegularExpressions;

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
        HashSet<Coordinate> possible = new();
        HashSet<Coordinate> notPossible = new();

        foreach (Tuple<Coordinate, Coordinate> line in _input)
        {
            int manhattan = ManhattanDistance(line.Item1, line.Item2);

            for (int i = line.Item1.Item1 - manhattan / 2; i < line.Item1.Item1 + manhattan / 2; i++)
            {
                if (Math.Abs((i - line.Item1.Item2)) < manhattan)
                {
                    int length = (manhattan - Math.Abs((i - line.Item1.Item2))) * 2 + 1;
                    int lengthSide = length / 2;
                    int middle = line.Item1.Item1;
                    int left = middle - lengthSide;
                    int right = middle + lengthSide;

                    if (left > 0 && right < resultSize)
                    {
                        Coordinate position = Tuple.Create(i, right + 1);
                        if (!notPossible.Contains(position))
                        {
                            possible.Add(position);
                        }                 
                    }

                    for (int j = left; j < right; j++)
                    {
                        //possible.Remove(Tuple.Create(i, j));
                        notPossible.Add(Tuple.Create(i, j));
                    }

                    possible.RemoveWhere(n => n.Item2 >= left && n.Item2 <= right && n.Item1 == i);
                }
            }
        }

        Coordinate location = possible.ToArray()[0];
        int result = location.Item2 * 4000000 + location.Item1;
        return new(result.ToString());
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
