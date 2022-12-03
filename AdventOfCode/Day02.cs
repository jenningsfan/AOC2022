using System.Collections;

namespace AdventOfCode;

public class Day02 : BaseDay
{
    private readonly int[][] _input;
    private readonly Hashtable result;
    private readonly Hashtable win;
    private readonly Hashtable lose;

    public Day02()
    {
        _input = ParseInput();

        result = new()
        {
            [0] = 2,    // Rock
            [1] = 0,    // Paper
            [2] = 1,    // Scissors
        };

        win = new()
        {
            [0] = 1,    // Paper
            [1] = 2,    // Scissors
            [2] = 0,    // Rock
        };

        lose = new()
        {
            [0] = 2,    // Rock
            [1] = 0,    // Paper
            [2] = 1,    // Scissors
        };
    }

    public override ValueTask<string> Solve_1()
    {
        int total = 0;
        foreach (var row in _input)
        {
            int them = row[0];
            int us = row[1];

            total += us + 1;

            if ((int)result[them] == us) // Lose
            {
                total += 0;
            }
            else if (us == them) // Draw
            {
                total += 3;
            }
            else if ((int)result[us] == them) // Win
            {
                total += 6;
            }
        }

        return new(total.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int total = 0;

        foreach (var row in _input)
        {
            int them = row[0];
            int us = -1;

            int wanted = row[1];

            if (wanted == 0)    // Lose
            {
                us = (int)lose[them];
                total += 0;
            }
            else if (wanted == 1)   // Draw
            {
                us = them;
                total += 3;
            }
            else if (wanted == 2)   // Draw
            {
                us = (int)win[them];
                total += 6;
            }

            total += us + 1;
        }

        return new(total.ToString());
    }

    private int[][] ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        int[][] strategy = new int[input.Length][];

        Hashtable convert = new()
        {
            ["A"] = 0,
            ["B"] = 1,
            ["C"] = 2,

            ["X"] = 0,
            ["Y"] = 1,
            ["Z"] = 2,
        };

        for (int i = 0; i < input.Length; i++)
        {
            string[] round = input[i].Split(" ");

            strategy[i] = new int[] { (int)convert[round[0]], (int)convert[round[1]] };
        }

        return strategy;
    }
}
