namespace AdventOfCode;

public class Day03 : BaseDay
{
    private readonly string[] _input;

    public Day03()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        int result = 0;

        string[][] split = new string[_input.Length][];

        for (int i = 0; i < _input.Length; i++)
        {
            string line = _input[i];
            split[i] = new string[2] { line.Substring(0, line.Length / 2), line.Substring(line.Length / 2, line.Length / 2) };
        }

        foreach (var line in split)
        {
           result += GetPriority(line[0].Intersect(line[1]).First());
        }

        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int result = 0;

        for (int i = 0; i < _input.Length; i += 3)
        {
            result += GetPriority(_input[i].Intersect(_input[i + 1]).Intersect(_input[i + 2]).First());
        }

        return new(result.ToString());
    }

    private int GetPriority(char item)
    {
        if (item <= 'Z')
        {
            return item - 38;
        }
        else if (item <= 'z')
        {
            return item - 96;
        }
        return -1;
    }

    private string[] ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
       
        return input;
    }
}
