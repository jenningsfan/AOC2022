namespace AdventOfCode;

public class Day08 : BaseDay
{
    private readonly int[][] _input;

    public Day08()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        int result = 0;
        List<Tuple<int, int>> matches = new List<Tuple<int, int>>  ();
        
        for (int i = 0; i < _input.Length; i++)
        {
            for (int j = 0; j < _input[0].Length; j++)
            {
                if (i == 0 || i == _input.Length - 1 || j == 0 || j == _input[0].Length - 1)
                {
                    matches.Add(Tuple.Create( i, j ));
                }
                else
                {
                    int value = _input[i][j];

                    if (value > _input[i].Where((x, i) => i < j).Max())
                    {
                        matches.Add(Tuple.Create(i, j));
                    }
                    if (value > _input[i].Where((x, i) => i > j).Max())
                    {
                        matches.Add(Tuple.Create(i, j));
                    }

                    int highest = 0;
                    for (int x = 0; x < i; x++)
                    {
                        if (_input[x][j] > highest)
                        {
                            highest = _input[x][j];
                        }
                    }
                    if (value > highest)
                    {
                        matches.Add(Tuple.Create(i, j));
                    }

                    highest = 0;
                    for (int x = i + 1; x < _input.Length; x++)
                    {
                        if (_input[x][j] > highest)
                        {
                            highest = _input[x][j];
                        }
                    }
                    if (value > highest)
                    {
                        matches.Add(Tuple.Create(i, j));
                    }
                }
            }
        }

        return new(matches.Distinct().ToList().Count.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        List<int> scores = new List<int>();

        for (int i = 0; i < _input.Length; i++)
        {
            for (int j = 0; j < _input[0].Length; j++)
            {
                if (i == 0 || i == _input.Length - 1 || j == 0 || j == _input[0].Length - 1)
                {
                    scores.Add(0);
                }
                else
                {
                    int value = _input[i][j];
                    int up = 0;
                    int down = 0;
                    int left = 0;
                    int right = 0;

                    for (int k = j - 1; k > -1; k--)
                    {
                        left++;
                        if (value <= _input[i][k])
                        {
                            break;
                        }
                    }
                    for (int k = j + 1; k < _input[0].Length; k++)
                    {
                        right++;
                        if (value <= _input[i][k])
                        {
                            break;
                        }
                    }

                    for (int x = i - 1; x > -1; x--)
                    {
                        up++;
                        if (value <= _input[x][j])
                        {
                            break;
                        }
                    }

                    for (int x = i + 1; x < _input.Length; x++)
                    {
                        down++;
                        if (value <= _input[x][j])
                        {
                            break;
                        }
                    }

                    scores.Add(up * down * left * right);
                }
            }
        }

        return new(scores.Max().ToString());
    }

    private int[][] ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        int[][] heights = new int[input.Length][];

        for (int i = 0; i < input.Length; i++)
        {
            heights[i] = Array.ConvertAll(input[i].ToCharArray(), c => (int)char.GetNumericValue(c));
        }

        return heights;
    }
}
