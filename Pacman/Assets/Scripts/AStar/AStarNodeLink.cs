using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that holds a link between a new node and the 2 nodes that are on either side of it
/// </summary>
public class AStarNodeLink
{
    AStarNode front;
    AStarNode back;
    AStarNode middle;

    /// <summary>
    /// Creates the node link between the front/back/middle nodes
    /// </summary>
    /// <param name="front">The node to the front of the middle node</param>
    /// <param name="back">The node to the back of the middle node</param>
    /// <param name="middle">The middle node to create a link for</param>
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

    /// <summary>
    /// Resets the link so the front and back nodes are connecting again
    /// </summary>
    public void Reset()
    {
        middle.neigbours.Clear();

        front.neigbours.Remove(middle);
        front.neigbours.Add(back);

        back.neigbours.Remove(middle);
        back.neigbours.Add(front);
    }
}
