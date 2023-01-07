namespace AdventOfCode;

using Coordinate = Tuple<long, long>;
public class Day17 : BaseDay
{
    private readonly int[] _input;
    private readonly int[][][] rocks;

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

        for (int i = 0; i < 2022; i++)
        {
            offset = MoveRock(rocks[i % 5], ref chamber, new Coordinate(HighestRock(chamber) - 3, 1 + rocks[i % 5][0].Length), offset);
        }

        return new((8086 - HighestRock(chamber)).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        bool[,] chamber = new bool[2000000000000, 7];
        int offset = 0;

        for (long i = 0; i < 1000000000000; i++)
        {
            offset = MoveRock(rocks[i % 5], ref chamber, new Coordinate(HighestRock(chamber) - 3, 1 + rocks[i % 5][0].Length), offset);
        }

        return new((3999999999998 - HighestRock(chamber)).ToString());
    }

    private int MoveRock(int[][] rock, ref bool[,] chamber, Coordinate rc, int offset, bool debug = false)
    {
        Coordinate down = new(1, 0);
        Coordinate left = new(0, -1);
        Coordinate right = new(0, 1);
        Coordinate[] jets = new Coordinate[] { left, right };

        while (true)
        {
            if (offset >= _input.Length) offset = 0;
            if (CanMove(jets[_input[offset]], rc, rock, chamber) == 2)
            {
                rc = new(rc.Item1 + jets[_input[offset]].Item1, rc.Item2 + jets[_input[offset]].Item2);
            }
            if (CanMove(down, rc, rock, chamber) == 1)
            {
                offset++;
                break;
            }
            else { rc = new(rc.Item1 + down.Item1, rc.Item2 + down.Item2); }

            offset++;
        }

        UpdateChamber(rock, ref chamber, rc);

        return offset;
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

    private int CanMove(Coordinate direction, Coordinate rc, int[][] rock, bool[,] chamber)
    {
        Coordinate newPositon = new(rc.Item1 + direction.Item1, rc.Item2 + direction.Item2);

        if (newPositon.Item2 - rock[0].Length > 6 || newPositon.Item2 - rock[0].Length < -1 || newPositon.Item2 > 6 || newPositon.Item2 < 0) return 0; // Continue but don't go sideways
        if (newPositon.Item1 >= 8087) return 1;  // Stop
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
