using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum State
    {
        inHouse,
        chase,
        scatter
    }

    public enum Type
    {
        blinky,
        clyde,
        pinky,
        inky
    }

    [SerializeField] float moveSpeed = 3;

    // caches
    Transform trans;
    Rigidbody2D rb;
    [HideInInspector] public Node currentNode;
    [HideInInspector] public Node targetNode;
    [HideInInspector] public Vector2 currentDirection;

    // state
    [HideInInspector] public State currentState;
    [HideInInspector] public Vector2 currentPos;

    // values to keep track of the movement from the current to the target node
    float distanceFromCurrent = 0;
    float distanceToTarget = 0;

    /// <summary>
    /// Sets up the ghost at the node specified
    /// </summary>
    /// <param name="startingnode">The node to start at</param>
    public void Init(Node startingnode)
    {
        trans = transform;
        rb = GetComponent<Rigidbody2D>();

        currentNode = startingnode;
        currentPos = currentNode.pos;
        currentDirection = Vector2.zero;

        rb.MovePosition(currentPos);

        currentState = State.chase;

        MoveToNextNode();
    }

    void FixedUpdate()
    {
        if (currentState == State.chase)
        {
            distanceFromCurrent += moveSpeed * Time.deltaTime;                                  // move based on the movespeed
            float progress01 = Mathf.InverseLerp(0, distanceToTarget, distanceFromCurrent);     // calculate the normalised progress to the next node
            currentPos = Vector2.Lerp(currentNode.pos, targetNode.pos, progress01);             // get the current position

            rb.MovePosition(currentPos);

            // if we have reached the target node, pick a new one
            if (progress01 == 1)
            {
                currentNode = targetNode;
                MoveToNextNode();
            }
        }
    }

    /// <summary>
    /// Picks and then instructs the ghost to move to a new node
    /// </summary>
    void MoveToNextNode()
    {
        Vector2 nextDirection = Vector2.zero;
        Node next = ChooseNextNode(ref nextDirection);

        targetNode = next;
        currentDirection = nextDirection;

        distanceToTarget = Vector2.Distance(currentNode.pos, targetNode.pos);       // first calculate the distance required
        distanceFromCurrent = 0;
    }

    /// <summary>
    /// Gets the position this ghost wants to move to, this is different depending on the ghost type
    /// </summary>
    /// <returns>The position to target</returns>
    Vector2 GetTargetTile()
    {
        return GameLogic.instance.pacman.currentPos;
    }

    /// <summary>
    /// Picks the next node for the ghost to move
    /// This is called only on init and when the ghost has just reached a node
    /// </summary>
    /// <param name="nextDir">A reference to the next direction that the ghost going to move</param>
    /// <returns>The next node that the ghost is moving to</returns>
    Node ChooseNextNode(ref Vector2 nextDir)
    {
        Vector2 targetTile = GetTargetTile();

        // get a list of nodes that we could possibly move to
        // note that we cant move in the opposite direction
        List<Node> validNodes = new List<Node>();
        List<Vector2> validDirections = new List<Vector2>();
        for (int i = 0; i < currentNode.neighbours.Length; i++)
        {
            if (currentNode.directions[i] != currentDirection * -1)
            {
                validNodes.Add(currentNode.neighbours[i]);
                validDirections.Add(currentNode.directions[i]);
            }
        }

        // declare some values to return
        Node nextNode = null;
        Vector2 nextDirection = Vector2.zero;

        // if there is only 1 node to move to, just return that
        if (validNodes.Count == 1)
        {
            nextNode = validNodes[0];
            nextDirection = validDirections[0];
        }
        else // choose the node which is closest to the target (straight line distance) (maybe in the future calculate the actual distance?)
        {
            float leastDist = float.MaxValue;
            for (int i = 0; i < validNodes.Count; i++)
            {
                float dist = Vector2.Distance(validNodes[i].pos, targetTile);
                if (dist < leastDist)
                {
                    nextDirection = validDirections[i];
                    nextNode = validNodes[i];
                    leastDist = dist;
                }
            }
        }

        nextDir = nextDirection;    // "return" the next direction
        return nextNode;            // return the next node
    }
}
