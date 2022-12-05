using System;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public class Day05 : BaseDay
{
    private readonly (LinkedList<char>[], int[][]) _input;

    public Day05()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        string result = "";

        List<LinkedList<char>> stacks = new List<LinkedList<char>> { };
        for (int i = 0; i < _input.Item1.Length; i++)
        {
            stacks.Add(new LinkedList<char> { });
            foreach (var item in _input.Item1[i])
            {
                stacks[i].AddLast(item);
            }
        }

        int[][] procedures = _input.Item2;

        foreach (var procedure in procedures)
        {
            for (int i = 0; i < procedure[0]; i++) {
                stacks[procedure[2] - 1].AddFirst(stacks[procedure[1] - 1].First());
                stacks[procedure[1] - 1].RemoveFirst();
            }
        }

        foreach (var stack in stacks)
        {
            result += stack.First();
        }

        return new(result);
    }

    public override ValueTask<string> Solve_2()
    {
        string result = "";

        List<LinkedList<char>> stacks = new List<LinkedList<char>> { };
        for (int i = 0; i < _input.Item1.Length; i++)
        {
            stacks.Add(new LinkedList<char> { });
            foreach (var item in _input.Item1[i])
            {
                stacks[i].AddLast(item);
            }
        }

        int[][] procedures = _input.Item2;

        foreach (var procedure in procedures)
        {
            LinkedList<char> temp = new LinkedList<char> { };

            for (int i = 0; i < procedure[0]; i++) {
                temp.AddLast(stacks[procedure[1] - 1].First());
                stacks[procedure[1] - 1].RemoveFirst();
            }

            for (int i = 0; i < procedure[0]; i++)
            {                
                stacks[procedure[2] - 1].AddFirst(temp.Last());
                temp.RemoveLast();
            }
        }

        foreach (var stack in stacks)
        {
            result += stack.First();
        }

        return new(result);
    }

    private (LinkedList<char>[], int[][]) ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        
        int procedureStart = 0;

        for (int i = 0; i < input.Length; i++)
        {
            if (input[i].Substring(0, Math.Min(4, input[i].Length)) == "move")
            {
                procedureStart = i;
                break;
            }
        }

        int[][] procedures = new int[input.Length - procedureStart][];
        Regex rx = new Regex(@"[0-9]+", RegexOptions.Compiled);

        for (int i = 0; i < input.Length - procedureStart; i++)
        {
            procedures[i] = new int[3];
            for (int j = 0; j < 3; j++) procedures[i][j] = Convert.ToInt32(rx.Matches(input[i + procedureStart])[j].Value);
        }

        LinkedList<char>[] stacks = new LinkedList<char>[rx.Matches(input[procedureStart - 2]).Count];

        for (int i = 0; i < stacks.Length; i++)
        {
            stacks[i] = new LinkedList<char> { };
        }

        for (int i = 0; i < procedureStart - 2; i++)
        {
            for (int j = 0; j < stacks.Length; j++)
            {
                if (input[i][j * 4 + 1] != ' ') stacks[j].AddLast(input[i][j * 4 + 1]);
            }
        }

        return (stacks, procedures);
    }
}