using System.Collections;

namespace AdventOfCode;

public class Day09 : BaseDay
{
    private readonly int[][] _input;
    private readonly int length = 1000;

    public Day09()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        List<Tuple<int, int>> result = new List<Tuple<int, int>>();

        int[] head = new int[2] { length - 1, 0 };
        int[] tail = new int[2] { length - 1, 0 };

        result.Add(Tuple.Create(tail[0], tail[1]));

        foreach (var direction in _input)
        {
            for (int i = 0; i < direction[1]; i++) {
                switch (direction[0])
                {
                    case 0:
                        head[0] -= 1;
                        break;
                    case 1:
                        head[0] += 1;
                        break;
                    case 2:
                        head[1] -= 1;
                        break;
                    case 3:
                        head[1] += 1;
                        break;
                }

                tail = FixTail(head, tail);
                result.Add(Tuple.Create(tail[0], tail[1]));            
            }
        }

        return new(result.Distinct().ToList().Count.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        List<Tuple<int, int>> result = new List<Tuple<int, int>>();

        int[][] knots = new int[10][];

        for (int i = 0; i < 10; i++)
        {
            knots[i] = new int[2] { length - 1, 0 };
        }

        result.Add(Tuple.Create(knots[knots.Length - 1][0], knots[knots.Length - 1][1]));

        foreach (var direction in _input)
        {
            for (int i = 0; i < direction[1]; i++)
            {
                switch (direction[0])
                {
                    case 0:
                        knots[0][0] -= 1;
                        break;
                    case 1:
                        knots[0][0] += 1;
                        break;
                    case 2:
                        knots[0][1] -= 1;
                        break;
                    case 3:
                        knots[0][1] += 1;
                        break;
                }

                for (int j = 1; j < knots.Length; j++)
                {
                    knots[j] = FixTail(knots[j - 1], knots[j]);
                }
                
                result.Add(Tuple.Create(knots[knots.Length - 1][0], knots[knots.Length - 1][1]));
            }
        }

        return new(result.Distinct().ToList().Count.ToString());
    }

    private bool Adjancent(int[] head, int[] tail)
    {
        return Math.Abs(head[0] - tail[0]) < 2 && Math.Abs(head[1] - tail[1]) < 2;
    }

    private int[] FixTail(int[] head, int[] tail)
    {
        if (Adjancent(head, tail)) return tail;
        else
        {
            if (tail[0] - head[0] == 2 && tail[1] == head[1])
            {
                return new int[] { tail[0] - 1, tail[1] };
            }
            else if (head[0] - tail[0] == 2 && tail[1] == head[1])
            {
                return new int[] { tail[0] + 1, tail[1] };
            }
            else if (tail[1] - head[1] == 2 && tail[0] == head[0])
            {
                return new int[] { tail[0], tail[1] - 1 };
            }
            else if (head[1] - tail[1] == 2 && tail[0] == head[0])
            {
                return new int[] { tail[0], tail[1] + 1 };
            }
            else
            {
                if (tail[0] != head[0] && tail[1] != head[1])
                {
                    if (head[0] > tail[0] && head[1] < tail[1])
                    {
                        return new int[] { tail[0] + 1, tail[1] - 1 };
                    }
                    else if (head[0] < tail[0] && head[1] > tail[1])
                    {
                        return new int[] { tail[0] - 1, tail[1] + 1 };
                    }
                    else if (tail[0] > head[0] && tail[1] > head[1])
                    {
                        return new int[] { tail[0] - 1, tail[1] - 1 };
                    }
                    else if (tail[0] < head[0] && tail[1] < head[1])
                    {
                        return new int[] { tail[0] + 1, tail[1] + 1 };
                    }
                }
            }
        }

        return tail;
    }

    private int[][] ParseInput()
    {
        string[] rope = File.ReadAllText(InputFilePath).Split("\n");
        int[][] input = new int[rope.Length][];
        Hashtable directions = new()
        {
            ['U'] = 0,
            ['D'] = 1,
            ['L'] = 2,
            ['R'] = 3,
        };

        for (int j = 0; j < rope.Length; j++)
        {
            string direction = rope[j];
            input[j] = new int[] { (int)directions[direction[0]], Convert.ToInt32(direction.Substring(1)) };
        }

        return input;
    }
}
