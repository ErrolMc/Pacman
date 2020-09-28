using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A node class which has data specific for the astar algorithim
/// </summary>
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

    /// <summary>
    /// Initialises the AStarNode with its position and neighbour ids
    /// </summary>
    /// <param name="id">The node id that links to this AStarNode</param>
    /// <param name="position">This nodes position</param>
    /// <param name="neigbourIDs">The node ids of the neighbouring nodes</param>
    public AStarNode(int id, Vector2 position, List<int> neigbourIDs)
    {
        this.neigbourIDs = neigbourIDs;
        neigbours = new List<AStarNode>();

        this.position = position;
        this.id = id;

        Reset();
    }

    /// <summary>
    /// Resets the astar variables for this node so the algorithim can be run again
    /// </summary>
    public void Reset()
    {
        parent = null;
        hCost = 0;
        gCost = float.MaxValue;
    }
}
