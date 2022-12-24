using System.Text.RegularExpressions;

namespace AdventOfCode;

class ValveNode : AStarNode
{
    public ValveNode(int fScore, int flow, string name, AStarNode pathParent, List<AStarNode> neighbours) : base(fScore, pathParent, neighbours)
    {
        Flow = flow;
        Name = name;
    }

    public int Flow { get; set; }
    public string Name { get; set; }
}

public class Day16 : BaseDay
{
    private readonly List<ValveNode> _input;

    public Day16()
    {
        _input = ParseInput();
    }

    public override ValueTask<string> Solve_1()
    {
        List<ValveNode> working = new();    // Nodes that have a flow rate above 0
        ValveNode currentNode = _input.Find(x => x.Name == "AA");
        working.Add(currentNode);

        for (int i = 0; i < _input.Count; i++)
        {
            if (_input[i].Flow > 0)
            {
                working.Add(_input[i]);
            }  
        }

        int currentFlow = 0;
        int movingTimeLeft = 0;
        int result = 0;

        for (int i = 0; i < 30; i++) {      
            result += currentFlow;

            if (movingTimeLeft == 0 && working.Count > 0)
            {
                currentFlow += currentNode.Flow;
                movingTimeLeft = 1;
                if (working.Count > 0)
                {
                    currentNode = BestNode(currentNode, working, 30 - i);
                }
                movingTimeLeft += currentNode.PathLength();
                ResetNodes(_input);
                ResetNodes(working);

                if (working.Count == 1)
                {
                    currentFlow += currentNode.Flow;
                    movingTimeLeft = 1;
                    working.RemoveAt(0);
                }
            }

            /*if (working.Count > 0)
            {
                result += currentFlow;
            }
            else
            {
                result += currentFlow * (30 - i);
                break;
            }*/

            movingTimeLeft--;

            Console.WriteLine($"Minute: {i}");           
            Console.WriteLine($"Total: {result}");
            Console.WriteLine($"Current: {currentFlow}");
            Console.WriteLine($"Valve: {currentNode.Name}");          
        }

        return new(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        throw new NotImplementedException();
    }

    private void ResetNodes(List<ValveNode> nodes)
    {
        foreach (ValveNode node in nodes)
        {
            node.PathParent = null;
            node.FScore = 0;
        }
    }

    private ValveNode BestNode(ValveNode node, List<ValveNode> nodeList, int timeLeft)
    {
        List<int> paths = new();
        
        for (int i = 0; i < nodeList.Count; i++)
        {
            if (node.Name != nodeList[i].Name)
            {
                LinkNodes(node, nodeList[i]);
                paths.Add((timeLeft - nodeList[i].PathLength() - 1) * nodeList[i].Flow);
            }
            else
            {
                paths.Add(0);
            }
        }

        if (nodeList.Count == 1 && node.Name == nodeList[0].Name)
        {
            return node;
        }

        int closestId = paths.IndexOf(paths.Max());
        ValveNode best = nodeList[closestId];
        nodeList.RemoveAt(closestId);

        return best;
    }

    private ValveNode FindValveNode(string valve, List<ValveNode> valveList)
    {
        for (int i = 0; i < valveList.Count; i++)
        {
            if (valveList[i].Name == valve)
            {
                return valveList[i];
            }
        }

        return null;
    }

    private void LinkNodes(ValveNode node1, ValveNode node2)
    {
        List<ValveNode> open = new();
        List<ValveNode> closed = new();

        open.Add(node1);

        while (true)
        {
            open = open.OrderBy(x => x.FScore).ToList();
            ValveNode current = open[0];

            if (closed.Any(x => x.Name == current.Name))
            {
                open.RemoveAt(0);
                continue;
            }

            open.RemoveAt(0);
            closed.Add(current);

            if (current.Name == node2.Name)
            {
                return;
            }

            foreach (ValveNode neighbour in current.Neighbours)
            {
                if ((neighbour.FScore == 0 || current.PathLength() + 1 < neighbour.FScore) && !closed.Any(x => x.Name == neighbour.Name))
                {
                    neighbour.PathParent = current;
                    neighbour.FScore = neighbour.PathLength();   
                }

                open.Add(neighbour);
            }
        }
    }

    private List<ValveNode> ParseInput()
    {
        string[] input = File.ReadAllText(InputFilePath).Split("\n");
        List<ValveNode> nodes = new();
        Regex flowRegex = new Regex("([0-9]+)", RegexOptions.Compiled);
        Regex connectionRegex = new Regex("([A-Z][A-Z])", RegexOptions.Compiled);

        for (int i = 0; i < input.Length; i++)
        {
            nodes.Add(new ValveNode(0, Convert.ToInt32(flowRegex.Matches(input[i])[0].ToString()), connectionRegex.Matches(input[i])[0].ToString(), null, new List<AStarNode>()));
        }

        for (int i = 0; i < nodes.Count; i++)
        {
            MatchCollection matches = connectionRegex.Matches(input[i]);

            for (int j = 1; j < matches.Count; j++)
            {
                nodes[i].Neighbours.Add(FindValveNode(matches[j].ToString(), nodes));
            }
        }

        return nodes;
    }
}
