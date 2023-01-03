using System.Collections;
using System.Text.RegularExpressions;

namespace AdventOfCode;

using CacheKey = Tuple<int, int, int, int, int, bool>;

class ValveNode
{
    public ValveNode(int flow, string name, List<int> neighbourIds)
    {
        Flow = flow;
        Name = name;
        NeighbourIds = neighbourIds;
    }

    public int Flow { get; set; }
    public string Name { get; set; }
    public List<int> NeighbourIds { get; set; }
}

public class Day16 : BaseDay
{
    private readonly List<ValveNode> _input;
    private Dictionary<CacheKey, int> cache;
    public Day16()
    {
        _input = ParseInput();
        cache = new Dictionary<CacheKey, int>();
    }

    public override ValueTask<string> Solve_1()
    {
        int currentNode = _input.FindIndex(x => x.Name == "AA");

        int result = MaxRelief(_input, currentNode, new bool[_input.Count], 30);
        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        int currentNode = _input.FindIndex(x => x.Name == "AA");

        int result = MaxRelief(_input, currentNode, new bool[_input.Count], 26, true);
        return new(result.ToString());
    }

    private int MaxRelief(List<ValveNode> nodes, int nodeId, bool[] openedNodes, int timeLeft, bool elephant = false)
    {
        if (timeLeft <= 0)
        {
            if (elephant)
            {
                bool[] openedCopy = new bool[openedNodes.Length];
                openedNodes.CopyTo(openedCopy, 0);

                return MaxRelief(nodes, _input.FindIndex(x => x.Name == "AA"), openedCopy, 26);
            }
            else
            {
                return 0;
            }
        }

        int maxRelief = 0;
        ValveNode node = nodes[nodeId];

        // Caching Code
        int[] openedIntTemp = new int[3];
        new BitArray(openedNodes).CopyTo(openedIntTemp, 0);

        CacheKey stateCache = new(nodeId, openedIntTemp[0], openedIntTemp[1], openedIntTemp[2], timeLeft, elephant);

        if (cache.ContainsKey(stateCache))
        {
            return cache[stateCache];
        }

        for (int i = 0; i < node.NeighbourIds.Count; i++)
        {
            maxRelief = Math.Max(maxRelief, MaxRelief(nodes, node.NeighbourIds[i], openedNodes, timeLeft - 1, elephant));
        }

        if (openedNodes[nodeId] == false && node.Flow > 0)
        {
            bool[] openedCopy = new bool[openedNodes.Length];
            openedNodes.CopyTo(openedCopy, 0);

            openedCopy[nodeId] = true;
            timeLeft--;

            int totalReleased = timeLeft * node.Flow;

            for (int i = 0; i < node.NeighbourIds.Count; i++)
            {
                maxRelief = Math.Max(maxRelief, totalReleased + MaxRelief(nodes, node.NeighbourIds[i], openedCopy, timeLeft - 1, elephant));
            }
        }

        cache[stateCache] = maxRelief;

        return maxRelief;
    }

    private int FindValveNode(string valve, List<ValveNode> valveList)
    {
        for (int i = 0; i < valveList.Count; i++)
        {
            if (valveList[i].Name == valve)
            {
                return i;
            }
        }

        return -1;
    }

    private List<ValveNode> ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        List<ValveNode> nodes = new();
        Regex flowRegex = new Regex("([0-9]+)", RegexOptions.Compiled);
        Regex connectionRegex = new Regex("([A-Z][A-Z])", RegexOptions.Compiled);

        for (int i = 0; i < input.Length; i++)
        {
            nodes.Add(new ValveNode(Convert.ToInt32(flowRegex.Matches(input[i])[0].ToString()), connectionRegex.Matches(input[i])[0].ToString(), new List<int>()));
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            MatchCollection matches = connectionRegex.Matches(input[i]);

            for (int j = 1; j < matches.Count; j++)
            {
                nodes[i].NeighbourIds.Add(FindValveNode(matches[j].ToString(), nodes));
            }
        }

        return nodes;
    }
}
