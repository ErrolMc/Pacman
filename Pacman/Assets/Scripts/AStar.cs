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

        hCost = 0;
        gCost = 0;
    }
}

public class AStar : MonoBehaviour
{
    public static AStar instance;

    List<AStarNode> nodes;

    void Awake()
    {
        instance = this;
    }

    public void GetGraph(Level level)
    {
        nodes = new List<AStarNode>();

        Node[] levelNodes = level.Nodes;

        // first pass, get all the nodes first
        for (int i = 0; i < levelNodes.Length; i++)
        {
            if (levelNodes[i].nodeType != Node.NodeType.ghostHouse && levelNodes[i].nodeType != Node.NodeType.portal && levelNodes[i].nodeType != Node.NodeType.home)
                nodes.Add(levelNodes[i].SetupAStar());
        }

        // second pass, get all the neighbours
        for (int i = 0; i < levelNodes.Length; i++)
        {
            if (levelNodes[i].nodeType != Node.NodeType.ghostHouse && levelNodes[i].nodeType != Node.NodeType.portal && levelNodes[i].nodeType != Node.NodeType.home)
                levelNodes[i].PopulateAStarNeighbours();
        }

        foreach (AStarNode node in nodes)
        {
            Debug.Log(node.position + " - " + node.neigbours.Count);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Vector2 start = new Vector2(6, 10);
            Vector2 end = new Vector2(21, 29);

            path = FindPath(start, end);
            Debug.LogError(path.Count);
        }
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

    public List<AStarNode> FindPath(Vector2 startPos, Vector2 endPos)
    {
        AStarNode start = GetNodeAtPos(startPos);
        AStarNode end = GetNodeAtPos(endPos);

        List<AStarNode> openList = new List<AStarNode>();
        List<AStarNode> closedList = new List<AStarNode>();

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

            // remove current from the open list
            openList.Remove(current);

            // if current is goal, end
            if (current == end)
            {
                int i = 0;

                List<AStarNode> output = new List<AStarNode>();
                while (current.parent != null && i < 10)
                {
                    output.Add(current.parent);
                    current = current.parent;

                    Debug.Log(current.position);
                    i++;
                }

                Debug.LogError(i);

                return output;
            }

            Debug.Log(current.neigbours.Count);

            // go through the current neighbours
            foreach (AStarNode neighbour in current.neigbours)
            {
                float tentative_gCost = current.gCost + Vector2.Distance(neighbour.position, current.position);

                if (openList.Contains(neighbour))
                {
                    if (neighbour.gCost <= tentative_gCost)
                        continue;
                }
                else if (closedList.Contains(neighbour))
                {
                    if (neighbour.gCost <= tentative_gCost)
                        continue;
                    closedList.Remove(neighbour);
                    openList.Add(neighbour);
                }
                else
                {
                    openList.Add(neighbour);
                    neighbour.hCost = Vector2.Distance(neighbour.position, end.position);
                }

                neighbour.gCost = tentative_gCost;
                neighbour.parent = current;
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
