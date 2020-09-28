using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A representation of the game world that the AStar algorithim understands to allow for pathfinding
/// </summary>
public class AStarGraph
{
    List<AStarNode> nodes;
    List<AStarNodeLink> nodeLinks;

    /// <summary>
    /// Creates the graph for a level
    /// </summary>
    /// <param name="level">The level to create the graph for</param>
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

    /// <summary>
    /// Gets a AStarNode based on its parents node id
    /// </summary>
    /// <param name="id">The node id of the node to get</param>
    /// <returns>The AStarNode that corresponds to the node id</returns>
    AStarNode GetNodeFromID(int id)
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].id == id)
                return nodes[i];
        }
        return null;
    }

    /// <summary>
    /// Returns an AStarNode representing where pacman is.
    /// Will create new AStarNodes if pacman isnt still on a node.
    /// </summary>
    /// <param name="pacman">The pacman to get the node from</param>
    /// <returns>The AStarNode that represents where pacman is on the board</returns>
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

    /// <summary>
    /// Returns an AStarNode represenmting where a ghost is.
    /// Will create new AStarNodes if the ghost isnt still on a node.
    /// </summary>
    /// <param name="ghost">The ghost to get the node from</param>
    /// <returns>The AStarNode that represents where the ghost is on the board</returns>
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

    /// <summary>
    /// Finds a path from a ghost to a node
    /// </summary>
    /// <param name="ghost">The ghost to path from</param>
    /// <param name="node">The node to path to</param>
    /// <returns>A path from the ghost to the node</returns>
    public List<AStarNode> FindPath(Ghost ghost, Node node)
    {
        AStarNode start = GetGhostNode(ghost);
        AStarNode end = GetNodeFromID(node.id);

        return FindPath(start, end);
    }

    /// <summary>
    /// Finds a path from a ghost to pacman
    /// </summary>
    /// <param name="ghost">The ghost to path from</param>
    /// <param name="pacman">The pacman to path to</param>
    /// <returns>A path from the ghost to pacman</returns>
    public List<AStarNode> FindPath(Ghost ghost, Pacman pacman)
    {
        AStarNode start = GetGhostNode(ghost);
        AStarNode end = GetPacmanNode(pacman);

        return FindPath(start, end);
    }

    /// <summary>
    /// Finds a path from a start and end node
    /// </summary>
    /// <param name="start">The start node in the path</param>
    /// <param name="end">The end node in the path</param>
    /// <returns>The path from the start to the end node</returns>
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
