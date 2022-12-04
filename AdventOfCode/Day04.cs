namespace AdventOfCode;

public class Day04 : BaseDay
{
    private readonly int[][][] _input;

    public Day04()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        int result = 0;

        foreach (var line in _input)
        {
            IEnumerable<Int32> first = Enumerable.Range(line[0][0], line[0][1] - line[0][0] + 1).ToArray();
            IEnumerable<Int32> second = Enumerable.Range(line[1][0], line[1][1] - line[1][0] + 1).ToArray();

            if (first.Intersect(second).Count() == first.Count() || second.Intersect(first).Count() == second.Count())
            {
                result++;
            }
        }

        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int result = 0;

        foreach (var line in _input)
        {
            IEnumerable<Int32> first = Enumerable.Range(line[0][0], line[0][1] - line[0][0] + 1).ToArray();
            IEnumerable<Int32> second = Enumerable.Range(line[1][0], line[1][1] - line[1][0] + 1).ToArray();

            if (first.Intersect(second).Count() != 0 && second.Intersect(first).Count() != 0)
            {
                result++;
            }
        }

        return new(result.ToString());
    }

    private int[][][] ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        int[][][] jobs = new int[input.Length][][];

        for (int i = 0; i < input.Length; i++)
        {
            string line = input[i];
            string[] sections = line.Split(",");
            jobs[i] = new int[sections.Length][];

            for (int j = 0; j < sections.Length; j++)
            {
                jobs[i][j] = Array.ConvertAll(sections[j].Split("-"), int.Parse);
            }
        }

        return jobs;
    }
}
