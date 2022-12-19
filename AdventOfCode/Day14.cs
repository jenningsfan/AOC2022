namespace AdventOfCode;

using Coordinate = Tuple<int, int>;

public class Day14 : BaseDay
{
    private readonly List<List<Tuple<Coordinate, Coordinate>>> _input;

    public Day14()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        int result = 0;
        int[,] cave = new int[7000, 7000];

        for (int i = 0; i < _input.Count; i++)
        {
            foreach (Tuple<Coordinate, Coordinate> c in _input[i])
            {
                for (int x = c.Item1.Item1; x < c.Item2.Item1 + 1; x++)
                {
                    cave[x, c.Item1.Item2] = 1;
                }
                for (int x = c.Item1.Item2; x < c.Item2.Item2; x++)
                {
                    cave[c.Item1.Item1, x] = 1;
                }
                for (int x = c.Item1.Item1; x > c.Item2.Item1 - 1; x--)
                {
                    cave[x, c.Item1.Item2] = 1;
                }
                for (int x = c.Item1.Item2; x > c.Item2.Item2 - 1; x--)
                {
                    cave[c.Item1.Item1, x] = 1;
                }
            }
        }

        while (true)
        {
            Coordinate newSand = MoveSand(cave, Tuple.Create(500, 0));

            if (newSand.Item1 == -1 && newSand.Item2 == -1)
            {
                break;
            }
            else
            {
                cave[newSand.Item1, newSand.Item2] = 2;
                result++;
            }
        }

        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int result = 0;
        int[,] cave = new int[7000, 7000];
        int highestY = 0;

        for (int i = 0; i < _input.Count; i++)
        {
            foreach (Tuple<Coordinate, Coordinate> c in _input[i])
            {
                for (int x = c.Item1.Item1; x < c.Item2.Item1 + 1; x++)
                {
                    cave[x, c.Item1.Item2] = 1;
                }
                for (int x = c.Item1.Item2; x < c.Item2.Item2; x++)
                {
                    if (x > highestY) highestY = x;
                    cave[c.Item1.Item1, x] = 1;
                }
                for (int x = c.Item1.Item1; x > c.Item2.Item1 - 1; x--)
                {
                    cave[x, c.Item1.Item2] = 1;
                }
                for (int x = c.Item1.Item2; x > c.Item2.Item2 - 1; x--)
                {
                    if (x > highestY) highestY = x;
                    cave[c.Item1.Item1, x] = 1;
                }
            }
        }

        for (int i = 0; i < 7000; i++)
        {
            cave[i, highestY + 2] = 1;
        }

        while (true)
        {
            Coordinate newSand = MoveSand(cave, Tuple.Create(500, 0));

            if ((newSand.Item1 == 500 && newSand.Item2 == 0) || (newSand.Item1 == -1 && newSand.Item2 == -1))
            {
                result++;
                break;
            }
            else
            {
                cave[newSand.Item1, newSand.Item2] = 2;
                result++;
            }
        }

        return new(result.ToString());
    }

    private Coordinate MoveSand(int[,] grid, Coordinate sand)
    {
        bool resting = false;

        while (!resting)
        {
            try
            {
                if (grid[sand.Item1, sand.Item2 + 1] == 0)
                {
                    sand = Tuple.Create(sand.Item1, sand.Item2 + 1);
                }
                else if (grid[sand.Item1 - 1, sand.Item2 + 1] == 0)
                {
                    sand = Tuple.Create(sand.Item1 - 1, sand.Item2 + 1);
                }
                else if (grid[sand.Item1 + 1, sand.Item2 + 1] == 0)
                {
                    sand = Tuple.Create(sand.Item1 + 1, sand.Item2 + 1);
                }
                else
                {
                    resting = true;
                }
            }
            catch (System.IndexOutOfRangeException)
            {
                return Tuple.Create(-1, -1);
            }
        }

        return sand;
    }

    private List<List<Tuple<Coordinate, Coordinate>>> ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        List<List<Tuple<Coordinate, Coordinate>>> rocks = new();

        foreach (string line in input)
        {
            string[] path = line.Split(" -> ");
            List<Tuple<Coordinate, Coordinate>> pathList = new();

            for (int i = 0; i < path.Length - 1; i++)
            {
                string[] point1 = path[i].Split(",");
                string[] point2 = path[i + 1].Split(",");

                Coordinate point1Coords = Tuple.Create(Convert.ToInt32(point1[0]), Convert.ToInt32(point1[1]));
                Coordinate point2Coords = Tuple.Create(Convert.ToInt32(point2[0]), Convert.ToInt32(point2[1]));

                pathList.Add(Tuple.Create(point1Coords, point2Coords));
            }

            rocks.Add(pathList);
        }

        return rocks;
    }
}
