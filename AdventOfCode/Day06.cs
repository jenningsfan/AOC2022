namespace AdventOfCode;

public class Day06 : BaseDay
{
    private readonly string _input;

    public Day06()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        int result = 0;

        for (int i = 0; i < _input.Length; i++)
        {
            if (new HashSet<char>(_input.Substring(i, 4).ToCharArray()).Count == 4)
            {
                result = i + 4;
                break;
            }
        }

        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int result = 0;

        for (int i = 0; i < _input.Length; i++)
        {
            if (new HashSet<char>(_input.Substring(i, 14).ToCharArray()).Count == 14)
            {
                result = i + 14;
                break;
            }
        }

        return new(result.ToString());
    }

    private string ParseInput()
    {
        string input = File.ReadAllText(InputFilePath);

        return input;
    }
}
