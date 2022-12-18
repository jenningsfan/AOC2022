namespace AdventOfCode;

public class Day13 : BaseDay
{
    private readonly List<Tuple<List<object>, List<object>>> _input;

    public Day13()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        int result = 0;

        for (int i = 0; i < _input.Count; i++)
        {
            if (ComparePair(_input[i]))
            {
                result += i + 1;
            }
        }
        
        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        throw new NotImplementedException();
    }

    private bool ComparePair(Tuple<List<object>, List<object>> pair)
    {
        if (Compare(pair.Item1, pair.Item2) == 0)
        {
            return true;
        }
        else { return false; }
    }

    private int Compare(object left, object right)
    {
        if (left is int && right is int)
        {
            if ((int)left < (int)right)
            {
                return 0;   // Right order
            }
            else if ((int)left > (int)right)
            {
                return 1;   // Wrong order
            }
            else
            {
                return 2;   // Continue checking
            }
        }
        else if (left is List<object> && right is List<object>)
        {
            for (int i = 0; i < ((List<object>)left).Count; i++)
            {
                try
                {
                    int result = Compare(((List<object>)left)[i], ((List<object>)right)[i]);

                    if (result == 0)
                    {
                        return 0;   // Right order
                    }
                    else if (result == 1)
                    {
                        return 1;   // Wrong order
                    }
                }
                catch
                {
                    return 1;   // Right ran out first. Wrong order.
                }
            }
            if (((List<object>)left).Count < ((List<object>)right).Count)
            {
                return 0;   // Right order
            }
            else
            {
                return 2;
            }
        }
        else
        {
            if (left is int)
            {
                List<object> listLeft = new List<object>();
                listLeft.Add(left);
                return Compare(listLeft, right);
            }
            else
            {
                List<object> listRight = new List<object>();
                listRight.Add(right);
                return Compare(left, listRight);
            }
        }
    }

    private List<Tuple<List<object>, List<object>>> ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        List<Tuple<List<object>, List<object>>> packets = new();

        for (int i = 0; i < input.Length; i+=3)
        {
            List<object>[] pair = new List<object>[2];

            for (int j = 0; j < 2; j++)
            {
                List<object> packet = new List<object>();
                List<object> currList = new();
                List<List<object>> oldList = new();
                List<char> number = new();
                packet = currList;

                oldList.Add(currList);
                pair[j] = packet;

                foreach (char section in input[i + j])
                {
                    if (section == '[')
                    {
                        try
                        {
                            currList.Add(Convert.ToInt32(new string(number.ToArray())));
                            number = new();
                        }
                        catch (System.FormatException) { }                      

                        currList.Add(new List<object>());
                        oldList.Add(currList);
                        currList = (List<object>)currList.Last();                      
                    }
                    else if (section == ']')
                    {
                        try
                        {
                            currList.Add(Convert.ToInt32(new string(number.ToArray())));
                            number = new();
                        }
                        catch (System.FormatException) { }

                        currList = oldList.Last();
                        oldList.RemoveAt(oldList.Count - 1);
                    }
                    else if (section == ',')
                    {
                        try
                        {
                            currList.Add(Convert.ToInt32(new string(number.ToArray())));
                            number = new();
                        }
                        catch (System.FormatException) { }

                        number = new();
                    }
                    else
                    {
                        number.Add(section);
                    }                      
                }
            }
            packets.Add(Tuple.Create(pair[0], pair[1]));
        }

        return packets;
    }
}
