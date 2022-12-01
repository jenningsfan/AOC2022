namespace AdventOfCode;

public class Day01 : BaseDay
{
    private readonly int[][] _input;

    public Day01()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        int result = 0;
        
        foreach(int[] calorieList in _input)
        {
            int calories = calorieList.Sum();
            if (calories > result) result = calories;
        }
        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int result = 0;
        int[] biggest = {0, 0, 0};

        foreach (int[] calorieList in _input)
        {
            int calories = calorieList.Sum();
            for (int i = 0; i < 3; i++)
            {
                if (calories > biggest[i])
                {
                    biggest[i] = calories;
                    break;
                }
            }
        }

        result = biggest.Sum();
        return new(result.ToString());
    }

    private int[][] ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n\n");
        int[][] calories = new int[input.Length][];

        for (int i = 0; i < input.Length; i++)
        {
            calories[i] = Array.ConvertAll(input[i].Split("\n"), int.Parse);
        }

        return calories;
    }
}
