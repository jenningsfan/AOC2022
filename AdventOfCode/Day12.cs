using System.Collections.Generic;

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
        List<Tuple<int, int>> visited = new();
        List<int> decisions = new();
        List<int> results = new();

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

        TraverseGraph(map, location, endLocation, visited, results);

        int result = results.Min();
        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        throw new NotImplementedException();
    }

    private void TraverseGraph(char[][] map, int[] location, int[] endLocation, List<Tuple<int, int>> visited, List<int> results)
    {
        int result = 0;
        bool backwards = false;
        Stack<int[]> moves = new();
        List<int> desicions = new();

        while (location[0] != endLocation[0] || location[1] != endLocation[1])
        {
            Console.WriteLine($"{location[0]}, {location[1]}");
            Console.WriteLine($"{result}");
            Console.WriteLine($"{backwards}");

            if (!backwards)
            {
                var locationTuple = Tuple.Create(location[0], location[1]);
                int x = (int)locationTuple.Item1;
                int y = (int)locationTuple.Item2;

                Tuple<int, int> convertedTuple = new Tuple<int, int>(x, y);

                visited.Add(convertedTuple);
            }
            else
            {
                var locationTuple = Tuple.Create(location[0], location[1]);
                int x = (int)locationTuple.Item1;
                int y = (int)locationTuple.Item2;
                Tuple<int, int> convertedTuple = new Tuple<int, int>(x, y);

                visited.Remove(convertedTuple);
            }
                
            int[][] directions = ValidDirections(map, location, visited);
            int[] direction = new int[] { 0, 0 };
                
            if (directions.Length == 0)
            {
                backwards = true;
            }
            else if (directions.Length == 1)
            {
                direction = directions[0];
            }
            else
            {
                if (backwards)
                {
                    backwards = false;

                    direction = directions[desicions.Last() + 1];
                    desicions.Add(desicions.Last() + 1);
                    desicions.RemoveAt(desicions.Count - 1);
                }
                else
                {
                    direction = directions[0];
                    desicions.Add(0);
                }
            }

            if (backwards)
            {
                direction = moves.Pop();
                direction = new int[] { -direction[0], -direction[1] };
                visited.Remove(Tuple.Create(location[0], location[1]));
            }
            else
            {
                moves.Push(direction);
            }
                
            location[0] += direction[0];
            location[1] += direction[1];
                
            if (backwards)
            {
                visited.Remove(Tuple.Create(location[0], location[1]));
            }

            if (!backwards)
            {
                result++;
            }
            else
            {
                result--;
            }
        }

        Console.WriteLine("Finished");

        results.Add(result);
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
