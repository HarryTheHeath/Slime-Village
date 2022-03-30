public class Pathnode
{
    private Grid<Pathnode> grid;
    public int x;
    public int y;

    // 3 A* values that are needed to be calculated
    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public bool isTargetPoint;

    public Pathnode originNode;
    
    //constructor
    public Pathnode(Grid<Pathnode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
        isTargetPoint = false;
    }

    public override string ToString()
    {
        return x + "," + y;
    }

    // F Cost simple equation
    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
    }
    
    public void SetIsTargetPoint(bool isTargetPoint)
    {
        this.isTargetPoint = isTargetPoint;
    }
}
