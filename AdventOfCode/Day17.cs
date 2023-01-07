namespace AdventOfCode;

using CacheKey = Tuple<long, int, long, long, long, long>;
using Coordinate = Tuple<long, long>;

public class Day17 : BaseDay
{
    private readonly int[] _input;
    private readonly int[][][] rocks;
    private Dictionary<CacheKey, long> cache;

    public Day17()
    {
        _input = ParseInput();

        int[][] rock1 = new int[][]
        {
            new int[] { 1, 1, 1, 1 },
        };
        int[][] rock2 = new int[][]
        {
            new int[] { 0, 1, 0 },
            new int[] { 1, 1, 1 },
            new int[] { 0, 1, 0 },
        };
        int[][] rock3 = new int[][]
        {
            new int[] { 0, 0, 1 },
            new int[] { 0, 0, 1 },
            new int[] { 1, 1, 1 },
        };
        int[][] rock4 = new int[][]
        {
            new int[] { 1 },
            new int[] { 1 },
            new int[] { 1 },
            new int[] { 1 },
        };
        int[][] rock5 = new int[][]
        {
            new int[] { 1, 1 },
            new int[] { 1, 1 },
        };

        rocks = new int[5][][] { rock1, rock2, rock3, rock4, rock5 };

        cache = new Dictionary<CacheKey, long>();
    }

    public override ValueTask<string> Solve_1()
    {
        bool[,] chamber = new bool[8088, 7];
        int offset = 0;

        for (int i = 0; i < 2022; i++)
        {
            offset = MoveRock(i % 5, ref chamber, new Coordinate(HighestRock(chamber) - 3, 1 + rocks[i % 5][0].Length), offset);
        }

        return new((8086 - HighestRock(chamber)).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        bool[,] chamber = new bool[400000, 7];
        int offset = 0;

        for (long i = 0; i < 1000; i++)
        {
            offset = MoveRock(i % 5, ref chamber, new Coordinate(HighestRock(chamber) - 3, 1 + rocks[i % 5][0].Length), offset);
        }

        PrintChamber(chamber);

        return new((3999999999998 - HighestRock(chamber)).ToString());
    }

    private int MoveRock(long rockId, ref bool[,] chamber, Coordinate rc, int offset)
    {
        int[][] rock = rocks[rockId];

        while (true)
        {
            Tuple<int, Coordinate> jetResult = MoveJets(offset, rc, rockId, chamber);
            offset = jetResult.Item1;
            rc = jetResult.Item2;

            Tuple<bool, int, Coordinate> downResult = MoveDown(offset, rc, rockId, chamber);
            offset = downResult.Item2;
            rc = downResult.Item3;

            if (downResult.Item1) break;
        }

        UpdateChamber(rock, ref chamber, rc);

        return offset;
    }

    private Tuple<int, Coordinate> MoveJets(int offset, Coordinate rc, long rockId, bool[,] chamber)
    {
        Coordinate left = new(0, -1);
        Coordinate right = new(0, 1);
        Coordinate[] jets = new Coordinate[] { left, right };

        int[][] rock = rocks[rockId];

        if (offset >= _input.Length) offset = 0;

        if (CanMove(jets[_input[offset]], rc, rock, chamber) == 2)
        {
            rc = new(rc.Item1 + jets[_input[offset]].Item1, rc.Item2 + jets[_input[offset]].Item2);
        }

        CacheKey cacheKey = CreateCacheKey(rockId, offset, chamber);
        Tuple<bool, long> cacheResult = CheckCache(cacheKey);

        if (cacheResult.Item1 is true)
        {
            Console.WriteLine("Cache hit");
            return Tuple.Create(offset, rc);
        }

        cache.Add(cacheKey, HighestRock(chamber));

        return Tuple.Create(offset, rc);
    }

    private Tuple<bool, int, Coordinate> MoveDown(int offset, Coordinate rc, long rockId, bool[,] chamber)
    {
        Coordinate down = new(1, 0);
        int[][] rock = rocks[rockId];
        bool stopped = false;

        if (CanMove(down, rc, rock, chamber) == 2) rc = new(rc.Item1 + down.Item1, rc.Item2 + down.Item2);
        else stopped = true;

        offset++;

        CacheKey cacheKey = CreateCacheKey(rockId, offset, chamber);
        Tuple<bool, long> cacheResult = CheckCache(cacheKey);

        if (cacheResult.Item1 is true)
        {
            Console.WriteLine("Cache hit");
            return Tuple.Create(stopped, offset, rc);
        }

        cache.Add(cacheKey, HighestRock(chamber));

        return Tuple.Create(stopped, offset, rc);
    }

    private int CanMove(Coordinate direction, Coordinate rc, int[][] rock, bool[,] chamber)
    {
        Coordinate newPositon = new(rc.Item1 + direction.Item1, rc.Item2 + direction.Item2);

        if (newPositon.Item2 - rock[0].Length > 6 || newPositon.Item2 - rock[0].Length < -1 || newPositon.Item2 > 6 || newPositon.Item2 < 0) return 0; // Continue but don't go sideways
        if (newPositon.Item1 >= chamber.GetLongLength(0) - 1) return 1;  // Stop
        if (direction.Item1 == 1 && direction.Item2 == 0)   // If going down
        {
            if (RockCollides(newPositon, rock, chamber)) return 1;  // Stop
        }
        else
        {
            if (RockCollides(newPositon, rock, chamber)) return 0;  // Continue but don't move across
        }
        return 2; // Continue
    }

    private bool RockCollides(Coordinate rc, int[][] rock, bool[,] chamber)
    {
        for (int i = rock.Length - 1; i >= 0; i--)
        {
            for (int j = rock[i].Length - 1; j >= 0; j--)
            {
                int r1 = rock.Length;
                int r2 = i;

                int c1 = rock[i].Length;
                int c2 = j;

                if (rock[i][j] == 1 && chamber[rc.Item1 - (r1 - r2) + 1, rc.Item2 - (c1 - c2) + 1] is true) return true;
            }
        }

        return false;
    }

    private long HighestRock(bool[,] chamber)
    {
        for (long row = chamber.GetLongLength(0) - 2; row > -1; row--)
        {
            bool allZeros = true;
            for (int col = 0; col < 7; col++)
            {
                if (chamber[row, col] is not false)
                {
                    allZeros = false;
                    break;
                }
            }

            if (allZeros)
            {
                return row;
            }
        }

        return 0;
    }


    private CacheKey CreateCacheKey(long rockId, int jetOffset, bool[,] chamber)
    {
        long chamberLastRow = chamber.GetLongLength(0) - 1;
        long[] values = new long[4];

        for (int i = 0; i < 4; i++)
        {
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    values[i] += (chamber[chamberLastRow - (r + i * 9), c] ? 1 : 0) << c;
                }
            }
        }

        return new CacheKey(rockId, jetOffset, values[0], values[1], values[2], values[3]);
    }

    private Tuple<bool, long> CheckCache(CacheKey cacheKey)
    {
        if (cache.ContainsKey(cacheKey))
        {
            return new Tuple<bool, long>(true, cache[cacheKey]);
        }
        else
        {
            return new Tuple<bool, long>(false, -1);
        }
    }
    private void UpdateChamber(int[][] rock, ref bool[,] chamber, Coordinate rc)
    {
        for (int i = rock.Length - 1; i >= 0; i--)
        {
            for (int j = rock[i].Length - 1; j >= 0; j--)
            {
                int r1 = rock.Length;
                int r2 = i;

                int c1 = rock[i].Length;
                int c2 = j;

                if (rock[i][j] == 1) chamber[rc.Item1 - (r1 - r2) + 1, rc.Item2 - (c1 - c2) + 1] = true;
            }
        }
    }

    private void PrintChamber(bool[,] chamber)
    {
        for (long row = chamber.GetLongLength(0) - 10; row < chamber.GetLongLength(0) - 1; row++)
        {
            Console.Write($"{row}:\t");
            Console.Write("|");
            for (int col = 0; col < 7; col++)
            {
                if (chamber[row, col] is false)
                {
                    Console.Write(".");
                }
                else
                {
                    Console.Write("#");
                }
            }
            Console.Write("|");
            Console.WriteLine();
        }
        Console.Write("8087:\t+-------+");
        Console.WriteLine();
    }

    private int[] ParseInput()
    {
        string input = File.ReadAllText(InputFilePath);
        int[] moves = new int[input.Length];

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '<')
            {
                moves[i] = 0;
            }
            else if (input[i] == '>')
            {
                moves[i] = 1;
            }
            else
            {
                throw new InvalidDataException();
            }
        }

        return moves;
    }
}
