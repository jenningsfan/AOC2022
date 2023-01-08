namespace AdventOfCode;

using CacheKey = Tuple<long, int, long, long, long, long>;
using Coordinate = Tuple<long, long>;

public class Day17 : BaseDay
{
    private readonly int[] _input;
    private readonly int[][][] rocks;
    private Dictionary<CacheKey, Tuple<long, long>> cache;

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
    }

    public override ValueTask<string> Solve_1()
    {
        bool[,] chamber = new bool[8088, 7];
        int offset = 0;

        long height = 0;
        long remainingRocks = 0;

        cache = new Dictionary<CacheKey, Tuple<long, long>>();

        for (int i = 0; i < 2022; i++)
        {
            Tuple<int, long, long> result = MoveRock(i % 5, ref chamber, new Coordinate(HighestRock(chamber) - 3, 1 + rocks[i % 5][0].Length), offset, i, 2022);
            offset = result.Item1;

            if (result.Item2 != -1)
            {
                height = result.Item2;
                remainingRocks = result.Item3;
                break;
            }
        }

        long oldHighest = HighestRock(chamber);

        for (int i = 0; i < remainingRocks; i++)
        {
            Tuple<int, long, long> result = MoveRock(i % 5, ref chamber, new Coordinate(HighestRock(chamber) - 3, 1 + rocks[i % 5][0].Length), offset, i, 2022);
            offset = result.Item1;
        }

        return new((height + 8086 - (oldHighest - HighestRock(chamber))).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        bool[,] chamber = new bool[400000, 7];
        int offset = 0;

        long height = 0;
        long remainingRocks = 0;

        cache = new Dictionary<CacheKey, Tuple<long, long>>();

        for (int i = 0; i < 100000; i++)
        {
            Tuple<int, long, long> result = MoveRock(i % 5, ref chamber, new Coordinate(HighestRock(chamber) - 3, 1 + rocks[i % 5][0].Length), offset, i, 100000);
            offset = result.Item1;

            if (result.Item2 != -1)
            {
                height = result.Item2;
                remainingRocks = result.Item3;
                break;
            }
        }

        long oldHighest = HighestRock(chamber);

        for (int i = 0; i < remainingRocks; i++)
        {
            Tuple<int, long, long> result = MoveRock(i % 5, ref chamber, new Coordinate(HighestRock(chamber) - 3, 1 + rocks[i % 5][0].Length), offset, i, 100000);
            offset = result.Item1;
        }

        return new((height + 3999999999998 - (oldHighest - HighestRock(chamber))).ToString());
    }

    private Tuple<int, long, long> MoveRock(long rockId, ref bool[,] chamber, Coordinate rc, int offset, long rocksFallen, long totalRocks)
    {
        int[][] rock = rocks[rockId];

        while (true)
        {
            Coordinate jetResult = MoveJets(offset, rc, rockId, chamber, rocksFallen);
            rc = jetResult;

            Tuple<bool, Coordinate> downResult = MoveDown(offset, rc, rockId, chamber, rocksFallen);
            rc = downResult.Item2;

            offset++;
            if (offset >= _input.Length) offset = 0;

            CacheKey cacheKey = CreateCacheKey(rockId, offset, chamber);
            Tuple<bool, Tuple<long, long>> cacheResult = CheckCache(cacheKey);

            if (cacheResult.Item1 is true)
            {
                long remainingShapes = totalRocks - rocksFallen;
                long repeatsNeeded = remainingShapes / (rocksFallen - cacheResult.Item2.Item2); // Full repeats
                remainingShapes %= cacheResult.Item2.Item2; // Remaining individual shapes

                long heightDelta = ((8086 - HighestRock(chamber)) - (8086 - cacheResult.Item2.Item1)) * repeatsNeeded;
                long newHeight = (8086 - HighestRock(chamber)) + heightDelta;

                //Console.WriteLine(HighestRock(chamber));
                Console.WriteLine(newHeight);
                Console.WriteLine(remainingShapes);
                return Tuple.Create(offset, newHeight, remainingShapes);
            }
            else
            {
                cache.Add(cacheKey, Tuple.Create(HighestRock(chamber), rocksFallen));
            }
            
           
            if (downResult.Item1) break;
        }

        UpdateChamber(rock, ref chamber, rc);

        return Tuple.Create(offset, -1L, -1L);
    }

    private Coordinate MoveJets(int offset, Coordinate rc, long rockId, bool[,] chamber, long rocksDropped)
    {
        Coordinate left = new(0, -1);
        Coordinate right = new(0, 1);
        Coordinate[] jets = new Coordinate[] { left, right };

        int[][] rock = rocks[rockId];

        if (CanMove(jets[_input[offset]], rc, rock, chamber) == 2)
        {
            rc = new(rc.Item1 + jets[_input[offset]].Item1, rc.Item2 + jets[_input[offset]].Item2);
        }
        
        return rc;
    }

    private Tuple<bool, Coordinate> MoveDown(int offset, Coordinate rc, long rockId, bool[,] chamber, long rocksDropped)
    {
        Coordinate down = new(1, 0);
        int[][] rock = rocks[rockId];
        bool stopped = false;

        if (CanMove(down, rc, rock, chamber) == 2) rc = new(rc.Item1 + down.Item1, rc.Item2 + down.Item2);
        else stopped = true;

        return Tuple.Create(stopped, rc);
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

    private Tuple<bool, Tuple<long, long>> CheckCache(CacheKey cacheKey)
    {
        if (cache.ContainsKey(cacheKey))
        {
            return new Tuple<bool, Tuple<long, long>>(true, cache[cacheKey]);
        }
        else
        {
            return new Tuple<bool, Tuple<long, long>>(false, Tuple.Create(-1L, -1L));
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
