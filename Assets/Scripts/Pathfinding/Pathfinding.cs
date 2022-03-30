using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    // basic movement costs
    private const int MoveStraightCoast = 10;
    private const int MoveDiagonallyCost = 14; // math value (square root of 200)

    // made pathfinding into a singleton to be easily accessible
    public static Pathfinding Instance { get; private set; }

    // grid and pathnode lists
    private Grid<Pathnode> grid;
    private List<Pathnode> openList;
    private HashSet<Pathnode> closedList;

    //constructor that uses grid.cs and sets the above singleton
    public Pathfinding(int width, int height)
    {
        Instance = this;
        grid = new Grid<Pathnode>(width, height, 10f, Vector3.zero,
            (Grid<Pathnode> g, int x, int y) => new Pathnode(g, x, y));
    }
    
    // returns grid
    public Grid<Pathnode> GetGrid() 
    {
        return grid;
    }
    
    // find path method for AIs
    // returns a list of vector3 values
    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)
    {
        // convert world positions into grid positions
        grid.GetXY(startWorldPos, out int startX, out int startY);
        grid.GetXY(endWorldPos, out int endX, out int endY);

        // uses node function to find path, with a check to see if we have found one
        List<Pathnode> path = FindPath(startX, startY, endX, endY);
        if (path == null)
            return null;
        
        // if a path is found
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            // cycles through all pathnodes
            foreach (Pathnode pathnode in path)
            {
                // the path nodes are converted into a list of Vector 3 values
                vectorPath.Add(new Vector3(pathnode.x, pathnode.y) * grid.GetCellSize() 
                               + Vector3.one * grid.GetCellSize() *0.5f);
            }
            return vectorPath;
        }
    }

    // implementing the algorithm
    // returns a list of path nodes for entire path
    public List<Pathnode> FindPath(int startX, int startY, int endX, int endY)
    {
        // get grid object's starting x and y coordinates
        Pathnode startnode = grid.GetGridObject(startX, startY);
        Pathnode endnode = grid.GetGridObject(endX, endY);
        openList = new List<Pathnode> {startnode};
        
        // uses hashset since only need to look to see if contains given node or not
        closedList = new HashSet<Pathnode>();

        // finding path logic
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Pathnode pathnode = grid.GetGridObject(x, y);
                // initalized by setting G cost to infinite
                pathnode.gCost = int.MaxValue;
                // calculate F cost
                pathnode.CalculateFCost();
                // set back to null to avoid data from previous path being pulled up
                pathnode.originNode = null;

            }
        }

        // calculates starting data for our algorithm
        // g is 0 as it's our start location
        startnode.gCost = 0;
        startnode.hCost = CalculateDistanceCost(startnode, endnode);
        startnode.CalculateFCost();

        // cycles while there are still nodes on the openlist
        while (openList.Count > 0)
        {
            Pathnode currentNode = GetLowestFCost(openList);
            // check if it's the final node
            if (currentNode == endnode)
                // Reached our goal - calculates the path we took to reach it
                return CalculatePath(endnode);

            // current node already been searched so is closed
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // cycle through neighbour nodes of current node
            foreach (Pathnode neighbourNode in GetNeighbourList(currentNode))
            {
                // check if neighbour node is already on the closed lost (so has been searched)
                if (closedList.Contains(neighbourNode))
                    continue;

                // ignore non-walkable nodes from algorithm
                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                var tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);

                // else need to search for it
                // and see if there's a faster path from current node to neighbour node than previous
                // if so all neighbour node values are updated

                if (tentativeGCost >= neighbourNode.gCost) continue;
                neighbourNode.originNode = currentNode;
                neighbourNode.gCost = tentativeGCost;
                neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endnode);
                neighbourNode.CalculateFCost();

                // check if it's not already on the open list, or else add it
                if (!openList.Contains(neighbourNode))
                    openList.Add(neighbourNode);
            }
        }
        // out of nodes on the open list (means we have searched whole path and not found a path)
        return null;
    }


    // uses origin node to retrace steps
    private List<Pathnode> CalculatePath(Pathnode endnode)
    {
        List<Pathnode> path = new List<Pathnode>();
        
        // start path from end
        path.Add(endnode);
        Pathnode currentNode = endnode;
        // while the current node has a parent add to the path and update current node
        // cycle through nodes until we reach that one doesn't have a parent i.e. the start node
        while (currentNode.originNode != null)
        {
            path.Add(currentNode.originNode);
            currentNode = currentNode.originNode;
        }
        // reverses so end is now the beginning
        path.Reverse();
        return path;
    }

    // function logic for H cost, calculating distance while ignoring blocking areas
    private int CalculateDistanceCost(Pathnode a, Pathnode b)
    {
        // calulate common distance cost (quickest direct path)
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MoveDiagonallyCost * Mathf.Min(xDistance, yDistance) + MoveStraightCoast * remaining;
    }

    // gets the node with the lowest f cost (current node)
    private Pathnode GetLowestFCost(List<Pathnode> pathnodeList)
    {
        Pathnode lowestFCostNode = pathnodeList[0];
        // cycle through entire node list to get the one with the lowest value
        for (int i = 1; i < pathnodeList.Count; i++)
        {
            if (pathnodeList[i].fCost < lowestFCostNode.fCost)
                lowestFCostNode = pathnodeList[i];
        }
        return lowestFCostNode;
    }

    public Pathnode GetNode(int x, int y) 
    {
        return grid.GetGridObject(x, y);
    }
    
    // function to get neighbours of the current node
    private List<Pathnode> GetNeighbourList(Pathnode currentNode)
    {
        var neighbourList = new List<Pathnode>();
        
        // checks eight neighbour positions (every surrounding node)
        // need to perform validity checks on each node by checking above zero
        if (currentNode.x - 1 >= 0) {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            // Left Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            // Left Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        
        if (currentNode.x + 1 < grid.GetWidth()) {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            // Right Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            // Right Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        
        // Down
        if (currentNode.y - 1 >= 0) 
            neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) 
            neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;
    }


    public List<Pathnode> GetLeftRightNeighbours(Pathnode currentNode)
    {
        var leftRightNeighbours = new List<Pathnode>();
        
        // validity check for out of bounds left and then right
        if (currentNode.x - 1 >= 0 && currentNode.x + 1 < grid.GetWidth())
        {
            var leftNeighbour = GetNode(currentNode.x - 1, currentNode.y);
            leftRightNeighbours.Add(leftNeighbour);
            
            var rightNeighbour = GetNode(currentNode.x + 1, currentNode.y);
            leftRightNeighbours.Add(rightNeighbour);
            
            return leftRightNeighbours;
        }
        
        // if invalid, nodes are marked as O,0 for easy exception law-keeping
        else
        {
            var leftNeighbour = GetNode(0, 0);
            leftRightNeighbours.Add(leftNeighbour);
            
            var rightNeighbour = GetNode(0, 0);
            leftRightNeighbours.Add(rightNeighbour);
            
            return leftRightNeighbours;
        }
    }
}
