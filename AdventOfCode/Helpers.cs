namespace AdventOfCode;

public class GridNode
{
    public GridNode(int fScore, int[] location, GridNode parent)
    {
        FScore = fScore;
        Location = location;
        Parent = parent;
    }

    public int PathLength()
    {
        if (Parent == null)
        {
            return 0;
        }
        else
        {
            return Parent.PathLength() + 1;
        }
    }

    public int FScore { get; set; }
    public int[] Location { get; set; }
    public GridNode Parent { get; set; }
}

public class AStarNode
{
    public AStarNode(int fScore, AStarNode pathParent, List<AStarNode> neighbours)
    {
        FScore = fScore;
        PathParent = pathParent;
        Neighbours = neighbours;
    }

    public int PathLength()
    {
        if (PathParent == null)
        {
            return 0;
        }
        else
        {
            return PathParent.PathLength() + 1;
        }
    }

    public int FScore { get; set; }
    public AStarNode PathParent { get; set; }
    public List<AStarNode> Neighbours { get; set; }
}