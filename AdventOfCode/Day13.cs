namespace AdventOfCode;

public class PacketComparer : IComparer<object>
{
    public int Compare(object left, object right)
    {
        if (left is int && right is int)
        {
            return (int)left - (int)right;
        }
        else if (left is List<object> && right is List<object>)
        {
            for (int i = 0; i < ((List<object>)left).Count; i++)
            {
                try
                {
                    int result = Compare(((List<object>)left)[i], ((List<object>)right)[i]);

                    if (result <= -1)
                    {
                        return -1;   // Right order
                    }
                    else if (result >= 1)
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
                return -1;   // Right order
            }
            else
            {
                return 0;
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

}

public class Day13 : BaseDay
{
    private readonly List<List<object>> _input;

    public Day13()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        int result = 0;

        for (int i = 0; i < _input.Count / 2; i++)
        {
            if (ComparePair(Tuple.Create(_input[i * 2], _input[i * 2 + 1])))
            {
                result += i + 1;
            }
        }
        
        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        List<object> dividers = new();
        object divider1 = (new List<object> { 2 });
        object divider2 = (new List<object> { 6 });
        dividers.Add(divider1);
        dividers.Add(divider2);

        foreach (object item in _input)
        {
            dividers.Add(item);
        }

        dividers.Sort(new PacketComparer());

        return new(((dividers.IndexOf(divider1) + 1) * (dividers.IndexOf(divider2) + 1)).ToString());
    }

    private bool ComparePair(Tuple<List<object>, List<object>> pair)
    {
        if (new PacketComparer().Compare(pair.Item1, pair.Item2) == -1)
        {
            return true;
        }
        else { return false; }
    }

    private List<List<object>> ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        List<List<object>> packets = new();

        for (int i = 0; i < input.Length; i+=3)
        {
            for (int j = 0; j < 2; j++)
            {
                List<object> packet = new List<object>();
                List<object> currList = new();
                List<List<object>> oldList = new();
                List<char> number = new();
                packet = currList;

                oldList.Add(currList);

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

                packets.Add(packet);
            }
        }

        return packets;
    }
}
