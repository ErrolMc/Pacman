using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public enum NodeType
    {
        regular,
        ghostHouse,
        portal
    }

    public Node[] neighbours;
    public NodeType nodeType = NodeType.regular;

    [HideInInspector] public Vector2[] directions;
    [HideInInspector] public Vector2 pos;

    public void Setup()
    {
        directions = new Vector2[neighbours.Length];
        for (int i = 0; i < neighbours.Length; i++)
        {
            Node neighbour = neighbours[i];
            Vector2 dir = neighbour.transform.position - transform.position;
            directions[i] = dir.normalized;
        }

        pos = new Vector2(transform.position.x, transform.position.y);
    }
}
