using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode;

public class Day12 : BaseDay
{
    private readonly char[][] _input;

    public Day12()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        char[][] map = new char[_input.Length][];

        for (int i = 0; i < _input.Length; i++)
        {
            map[i] = (char[])_input[i].Clone();
        }

        int[] location = new int[2];
        int[] endLocation = new int[2];

        for (int i = 0; i < map.Length; i++)
        {
            int posLocation = Array.FindIndex(map[i], row => row == 'S');
            int posEndLocation = Array.FindIndex(map[i], row => row == 'E');
            
            if (posLocation != -1)
            {
                location = new int[] { i, posLocation };
            }
            if (posEndLocation != -1)
            {
                endLocation = new int[] { i, posEndLocation };
            }
        }

        map[location[0]][location[1]] = 'a';
        map[endLocation[0]][endLocation[1]] = 'z';

        int result = TraverseGraph(map, location, endLocation);

        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        char[][] map = new char[_input.Length][];

        for (int i = 0; i < _input.Length; i++)
        {
            map[i] = (char[])_input[i].Clone();
        }

        int[] location = new int[2];
        int[] endLocation = new int[2];

        for (int i = 0; i < map.Length; i++)
        {
            int posLocation = Array.FindIndex(map[i], row => row == 'S');
            int posEndLocation = Array.FindIndex(map[i], row => row == 'E');

            if (posLocation != -1)
            {
                location = new int[] { i, posLocation };
            }
            if (posEndLocation != -1)
            {
                endLocation = new int[] { i, posEndLocation };
            }
        }

        map[location[0]][location[1]] = 'a';
        map[endLocation[0]][endLocation[1]] = 'z';

        List<int> results = new();

        for (int i = 0; i < map.Length; i++)
        {
            for (int j = 0; j < map[i].Length; j++)
            {
                if (map[i][j] == 'a')
                {
                    try
                    { 
                        results.Add(TraverseGraph(map, new int[] { i, j }, endLocation)); 
                    } 
                    catch { }
                }
            }
        }
        return new(results.Min().ToString());
    }

    private int TraverseGraph(char[][] map, int[] location, int[] endLocation)
    {
        List<GridNode> open = new();
        List<GridNode> closed = new();

        open.Add(new GridNode(0, (int[])location.Clone(), null));

        while (true)
        { 
            open = open.OrderBy(x => x.FScore).ToList();
            GridNode current = open[0];
            location = current.Location;

            closed.Add(open[0]);
            open.RemoveAt(0);

            if (CompareArray(location, endLocation))
            {
                return current.PathLength();
            }

            int[][] directions = ValidDirections(map, location);

            foreach (int[] neighbour in directions)
            {
                int[] locationCopy = new int[2];
                locationCopy[0] = location[0] + neighbour[0];
                locationCopy[1] = location[1] + neighbour[1];

                if (closed.Any(tuple => CompareArray(tuple.Location, locationCopy)))
                {
                    continue;
                }

                int fScore = current.PathLength() + ManhattanDistance(locationCopy, endLocation);
                IEnumerable<GridNode> item = open.Where(tuple => CompareArray(tuple.Location, locationCopy));

                if (item.Count() == 0 || item.Any(tuple => tuple.FScore > fScore))
                {
                    for (int i = 0; i < open.Count; i++)
                    {
                        if (CompareArray(open[i].Location, locationCopy))
                        {
                            open.RemoveAt(i);
                        }
                    }

                    open.Add(new GridNode(fScore, locationCopy, current));
                }
            }
        }
    }

    private int[][] ValidDirections(char[][] map, int[] location)
    {
        List<int[]> directions = new() { };
        int[][] moves = new int[4][]
        {
            new int[2] { 0, 1 },
            new int[2] { 0, -1 },
            new int[2] { 1, 0 },
            new int[2] {-1, 0 },
        };

        for (int i = 0; i < 4; i++)
        {
            try
            {
                int newX = location[0] + moves[i][0];
                int newY = location[1] + moves[i][1];

                if (map[newX][newY] <= map[location[0]][location[1]] + 1)
                {
                    directions.Add(moves[i]);
                }
            }
            catch (System.IndexOutOfRangeException) { }
        }

        return directions.ToArray();
    }

    private bool CompareArray(int[] array1, int[] array2)
    {
        if (array1.Length != array2.Length)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }
        }

        return true;
    }

    private int ManhattanDistance(int[] a, int[] b)
    {
        return Math.Abs(a[0] - b[0]) + Math.Abs(a[1] - b[1]);
    }
    private char[][] ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        char[][] heights = new char[input.Length][];

        for (int i = 0; i < input.Length; i++)
        {
            heights[i] = input[i].ToCharArray();
        }

        return heights;
    }
}
