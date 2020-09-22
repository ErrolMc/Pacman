using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode
{
    public Vector2 position;
    public AStarNode parent;
    public List<AStarNode> neigbours;

    public float fCost { get { return gCost + hCost; } } // gCost + hCost
    public float hCost; // heurestic cost from here to end (straight line)
    public float gCost; // cost from starting point to here

    public AStarNode(Vector2 position)
    {
        neigbours = new List<AStarNode>();
        this.position = position;

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

public class AStar : MonoBehaviour
{
    public static AStar instance;

    List<AStarNode> nodes;
    List<AStarNodeLink> nodeLinks;

    void Awake()
    {
        instance = this;
    }

    public void GetGraph(Level level)
    {
        nodes = new List<AStarNode>();
        nodeLinks = new List<AStarNodeLink>();

        Node[] levelNodes = level.Nodes;

        // first pass, get all the nodes first
        for (int i = 0; i < levelNodes.Length; i++)
        {
            if (levelNodes[i].nodeType != Node.NodeType.home)
                nodes.Add(levelNodes[i].SetupAStar());
        }

        // second pass, get all the neighbours
        for (int i = 0; i < levelNodes.Length; i++)
        {
            if (levelNodes[i].nodeType != Node.NodeType.home)
                levelNodes[i].PopulateAStarNeighbours();
        }
    }

    public void DoAstar(Ghost ghost)
    {
        path = FindPath(ghost);
    }

    AStarNode GetNodeAtPos(Vector2 pos)
    {
        foreach (AStarNode node in nodes)
        {
            if (Vector2.Distance(node.position, pos) < Mathf.Epsilon)
                return node;
        }
        return null;
    }

    AStarNode GetPacmanNode()
    {
        Pacman pacman = GameLogic.instance.Pacman;
        if (pacman.CurrentState == Pacman.State.idle)
            return GetNodeAtPos(pacman.CurrentPosition);

        AStarNode front = GetNodeAtPos(pacman.TargetNode.pos);
        AStarNode back = GetNodeAtPos(pacman.CurrentNode.pos);
        AStarNode middle = new AStarNode(pacman.CurrentPosition);

        nodeLinks.Add(new AStarNodeLink(front, back, middle));

        return middle;
    }

    AStarNode GetGhostNode(Ghost ghost)
    {
        if (ghost.CurrentState == Ghost.State.inHouse)
            return GetNodeAtPos(ghost.CurrentPosition);

        AStarNode front = GetNodeAtPos(ghost.TargetNode.pos);
        AStarNode back = GetNodeAtPos(ghost.CurrentNode.pos);
        AStarNode middle = new AStarNode(ghost.CurrentPosition);

        nodeLinks.Add(new AStarNodeLink(front, back, middle));

        return middle;
    }

    public List<AStarNode> FindPath(Ghost ghost)
    {
        for (int i = 0; i < nodes.Count; i++)
            nodes[i].Reset();

        AStarNode end = GetPacmanNode();
        AStarNode start = GetGhostNode(ghost);
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
                    output.Add(current.parent);
                    current = current.parent;
                }

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

    List<AStarNode> path;

    void OnDrawGizmos()
    {
        if (path != null && path.Count > 1)
        {
            AStarNode prev = path[0];
            for (int i = 1; i < path.Count; i++)
            {
                AStarNode cur = path[i];
                Debug.DrawLine(prev.position, cur.position, Color.white);

                prev = cur;
            }
        }
    }
}
