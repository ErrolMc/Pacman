using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Ghost : MonoBehaviour
{
    public enum State
    {
        inHouse,
        chase,
        scatter
    }

    [SerializeField] float moveSpeed = 3;

    Transform trans;
    Rigidbody2D rb;

    public Node currentNode;
    public Node targetNode;
    public Vector2 currentDirection;

    // values to keep track of the movement from the current to the target node
    float distanceFromCurrent = 0;
    float distanceToTarget = 0;

    public State currentState;
    public Vector2 currentPos;

    public Pacman pacman;

    public void Init(Node startingnode)
    {
        trans = transform;
        rb = GetComponent<Rigidbody2D>();

        currentNode = startingnode;
        currentPos = currentNode.pos;
        currentDirection = Vector2.zero;

        rb.MovePosition(currentPos);

        currentState = State.chase;

        pacman = GameLogic.instance.pacman;

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

            if (progress01 == 1)
            {
                currentNode = targetNode;
                MoveToNextNode();
            }
        }
    }

    void MoveToNextNode()
    {
        Node next = ChooseNextNode(ref currentDirection);
        targetNode = next;

        distanceToTarget = Vector2.Distance(currentNode.pos, targetNode.pos);       // first calculate the distance required
        distanceFromCurrent = 0;
    }

    Vector2 GetTargetTile()
    {
        return pacman.currentPos;
    }

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

        Node nextNode = null;
        Vector2 nextDirection = Vector2.zero;

        if (validNodes.Count == 1)
            nextNode = validNodes[0];
        else // > 1
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

        nextDir = nextDirection;

        return nextNode;
    }
}
