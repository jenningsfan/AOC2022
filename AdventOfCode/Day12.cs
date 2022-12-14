using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode;

public class Day12 : BaseDay
{
    private readonly char[][] _input;
    private int _tasksRunning = 1;

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
        List<Tuple<int, int>> visited = new();
        List<int> decisions = new();
        ConcurrentQueue<int> results = new();

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

        TraverseGraph(map, location, endLocation, visited, results, 0);

        while (_tasksRunning != 0)
        {

        }

        int result = results.Min();
        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        throw new NotImplementedException();
    }

    private void TraverseGraph(char[][] map, int[] location, int[] endLocation, List<Tuple<int, int>> visited, ConcurrentQueue<int> results, int result)
    {
        Stack<int[]> moves = new();
        List<int> desicions = new();

        while (location[0] != endLocation[0] || location[1] != endLocation[1])
        {
            visited.Add(Tuple.Create(location[0], location[1]));

            int[][] directions = ValidDirections(map, location, visited);
            int[] direction = new int[] { 0, 0 };
                
            if (directions.Length == 0)
            {
                Interlocked.Decrement(ref _tasksRunning);
                return;
            }
            else if (directions.Length == 1)
            {
                direction = directions[0];
            }
            else
            {
                foreach (int[] directionThread in directions)
                {
                    int[] locationCopy = (int[])location.Clone();
                    locationCopy[0] += directionThread[0];
                    locationCopy[1] += directionThread[1];

                    if (locationCopy[0] == endLocation[0] && locationCopy[1] == endLocation[1])
                    {
                        location = locationCopy;
                    }

                    List<Tuple<int, int>> visitedClone = new();
                    visitedClone.AddRange(visited.ToArray());
                    visitedClone.Add(Tuple.Create(locationCopy[0], locationCopy[1]));

                    var t = Task.Run(() => TraverseGraph(map, locationCopy, endLocation, visitedClone, results, result + 1));
                    //t.Wait();
                    Interlocked.Increment(ref _tasksRunning);
                }
                Interlocked.Decrement(ref _tasksRunning);
                return;
            }
                
            location[0] += direction[0];
            location[1] += direction[1];

            result++;
            if (!results.IsEmpty)
            {
                if (result >= results.Min())
                {
                    Interlocked.Decrement(ref _tasksRunning);
                    return;
                }
            }          
        }

        results.Enqueue(result);
        Interlocked.Decrement(ref _tasksRunning);
    }

    private int[][] ValidDirections(char[][] map, int[] location, List<Tuple<int, int>> visited)
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
                Tuple<int, int> xy = new(newX, newY);

                if (map[newX][newY] <= map[location[0]][location[1]] + 1 && !visited.Contains(xy))
                {
                    directions.Add(moves[i]);
                }
            }
            catch (System.IndexOutOfRangeException) { }
        }

        return directions.ToArray();
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
