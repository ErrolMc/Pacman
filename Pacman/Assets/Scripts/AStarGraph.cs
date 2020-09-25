using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public int id;
    public List<int> neigbourIDs;

    public Vector2 position;
    public AStarNode parent;
    public List<AStarNode> neigbours;

    public float fCost { get { return gCost + hCost; } } // gCost + hCost
    public float hCost; // heurestic cost from here to end (straight line)
    public float gCost; // cost from starting point to here

    public AStarNode(int id, Vector2 position, List<int> neigbourIDs)
    {
        this.neigbourIDs = neigbourIDs;
        neigbours = new List<AStarNode>();

        this.position = position;
        this.id = id;

        Reset();
    }

    public void Reset()
    {
        parent = null;
        hCost = 0;
        gCost = float.MaxValue;
    }
}

public class AStarNodeLink
{
    AStarNode front;
    AStarNode back;
    AStarNode middle;

    public AStarNodeLink(AStarNode front, AStarNode back, AStarNode middle)
    {
        this.front = front;
        this.back = back;
        this.middle = middle;

        front.neigbours.Remove(back);
        front.neigbours.Add(middle);

        back.neigbours.Remove(front);
        back.neigbours.Add(middle);

        middle.neigbours.Add(front);
        middle.neigbours.Add(back);
    }

    public void Reset()
    {
        middle.neigbours.Clear();

        front.neigbours.Remove(middle);
        front.neigbours.Add(back);

        back.neigbours.Remove(middle);
        back.neigbours.Add(front);
    }
}

public class AStarGraph
{
    List<AStarNode> nodes;
    List<AStarNodeLink> nodeLinks;

    /// <summary>
    /// Class that wholes
    /// </summary>
    /// <param name="level"></param>
    public AStarGraph(Level level)
    {
        nodes = new List<AStarNode>();
        nodeLinks = new List<AStarNodeLink>();

        Node[] levelNodes = level.Nodes;

        // first pass, get all the nodes first
        foreach (Node node in levelNodes)
        {
            if (node.nodeType != Node.NodeType.home)
            {
                // get the neighbour ids so we can do the second pass to get the references
                List<int> neigbourIDs = new List<int>();
                foreach (Node neighbour in node.neighbours)
                    neigbourIDs.Add(neighbour.id);
                
                AStarNode aStarNode = new AStarNode(node.id, node.pos, neigbourIDs);
                nodes.Add(aStarNode);
            }
        }

        // set all the neigbour references
        foreach (AStarNode node in nodes)
        {
            foreach (int neighbourID in node.neigbourIDs)
            {
                AStarNode neighbour = GetNodeFromID(neighbourID);
                node.neigbours.Add(neighbour);
            }
        }
    }

    AStarNode GetNodeFromID(int id)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].id == id)
                return nodes[i];
        }
        return null;
    }

    AStarNode GetPacmanNode(Pacman pacman)
    {
        if (pacman.CurrentState == Pacman.State.idle)
            return GetNodeFromID(pacman.CurrentNode.id);

        AStarNode front = GetNodeFromID(pacman.TargetNode.id);
        AStarNode back = GetNodeFromID(pacman.CurrentNode.id);
        AStarNode middle = new AStarNode(-1, pacman.CurrentPosition, null);

        nodeLinks.Add(new AStarNodeLink(front, back, middle));

        return middle;
    }

    AStarNode GetGhostNode(Ghost ghost)
    {
        if (ghost.CurrentState == Ghost.State.inHouse)
            return GetNodeFromID(ghost.CurrentNode.id);

        AStarNode front = GetNodeFromID(ghost.TargetNode.id);
        AStarNode back = GetNodeFromID(ghost.CurrentNode.id);
        AStarNode middle = new AStarNode(-1, ghost.CurrentPosition, null);

        nodeLinks.Add(new AStarNodeLink(front, back, middle));

        return middle;
    }

    public List<AStarNode> FindPath(Ghost ghost, Node node)
    {
        AStarNode start = GetGhostNode(ghost);
        AStarNode end = GetNodeFromID(node.id);

        return FindPath(start, end);
    }

    public List<AStarNode> FindPath(Ghost ghost, Pacman pacman)
    {
        AStarNode start = GetGhostNode(ghost);
        AStarNode end = GetPacmanNode(pacman);

        return FindPath(start, end);
    }

    public List<AStarNode> FindPath(AStarNode start, AStarNode end)
    {
        for (int i = 0; i < nodes.Count; i++)
            nodes[i].Reset();

        start.hCost = Vector2.Distance(start.position, end.position);
        start.gCost = 0;

        List<AStarNode> openList = new List<AStarNode>();

        AStarNode current = start;
        openList.Add(current);

        while (openList.Count != 0)
        {
            // get the node with the lowest f cost
            int lowestInd = 0;
            float lowestF = float.MaxValue;
            for (int i = 0; i < openList.Count; i++)
            {
                float curF = openList[i].fCost;
                if (curF < lowestF)
                {
                    lowestInd = i;
                    lowestF = curF;
                }
            }
            current = openList[lowestInd];

            // if current is goal, end
            if (current == end)
            {
                List<AStarNode> output = new List<AStarNode>() { current };
                while (current.parent != null)
                {
                    current = current.parent;
                    output.Add(current);
                }
                output.RemoveAt(output.Count - 1);

                if (nodeLinks.Count > 0)
                {
                    for (int i = 0; i < nodeLinks.Count; i++)
                        nodeLinks[i].Reset();
                    nodeLinks.Clear();
                }

                return output;
            }

            // remove current from the open list
            openList.Remove(current);

            // go through the current neighbours
            foreach (AStarNode neighbour in current.neigbours)
            {
                float tentative_gCost = current.gCost + Vector2.Distance(neighbour.position, current.position);

                if (tentative_gCost < neighbour.gCost)
                {
                    neighbour.parent = current;
                    neighbour.gCost = tentative_gCost;
                    neighbour.hCost = Vector2.Distance(neighbour.position, end.position);
                    if (!openList.Contains(neighbour))
                        openList.Add(neighbour);
                }
            }
        }

        return null;
    }
}
