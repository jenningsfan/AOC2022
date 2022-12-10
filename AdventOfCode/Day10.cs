namespace AdventOfCode;

public class Day10 : BaseDay
{
    private readonly int[][] _input;

    public Day10()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        int result = 0;
        int cycles = 0;
        int x = 1;

        foreach (int[] command in _input)
        {
            switch (command[0])
            {
                case 0:
                    cycles++;
                    if ((cycles - 20) % 40 == 0)
                    {
                        result += cycles * x;
                    }
                    break;
                case 1:
                    for (int i = 0; i < 2; i++)
                    {
                        cycles++;
                        if ((cycles - 20) % 40 == 0)
                        {
                            result += cycles * x;
                        }
                    }  
                    x += command[1];
                    break;
            }
        }
        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int cycles = 1;
        int x = 1;
        int[] sprite = new int[3];
        sprite = new int[] { x - 1, x, x + 1 };

        foreach (int[] command in _input)
        {
            switch (command[0])
            {
                case 0:
                    if (sprite.Contains(cycles - 1))
                    {
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                    if ((cycles) % 40 == 0 && cycles >= 40)
                    {
                        Console.WriteLine();
                        x += 40;
                    }
                    cycles++;
                    break;
                case 1:
                    for (int i = 0; i < 2; i++)
                    {
                        if (sprite.Contains(cycles - 1))
                        {
                            Console.Write("#");
                        }
                        else
                        {
                            Console.Write(".");
                        }
                        if ((cycles) % 40 == 0 && cycles >= 40)
                        {
                            Console.WriteLine();
                            x += 40;
                            sprite = new int[] { x - 1, x, x + 1 };
                        }
                        cycles++;
                    }
                    x += command[1];
                    sprite = new int[] { x - 1, x, x + 1 };
                    break;
            }
        }
        return new("");
    }

    private int[][] ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        int[][] commands = new int[input.Length][];

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == "noop")
            {
                commands[i] = new int[] { 0, 0 };
            }
            else if (input[i].StartsWith("addx"))
            {
                commands[i] = new int[] { 1, Convert.ToInt32(input[i][5..]) };
            }
        }

        return commands;
    }
}
