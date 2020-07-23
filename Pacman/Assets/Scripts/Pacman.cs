using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Pacman : MonoBehaviour
{
    public enum State
    {
        idle,
        moving
    }

    [SerializeField] float moveSpeed = 5;

    // caches
    Transform trans;
    Node currentNode, targetNode;

    // directions
    Vector2 currentDirection;
    Vector2 nextDirection;

    // values to keep track of the movement from the current to the target node
    float progress01 = 0;
    float distanceFromCurrent = 0;
    float distanceToTarget = 0;

    // state
    public State currentState;
    public Vector2 currentPos;

    public void Init(Node node)
    {
        trans = transform;

        currentPos = node.pos;
        trans.position = currentPos;

        currentState = State.idle;
        currentNode = node;

        currentDirection = Vector2.zero;
    }

    void Update()
    {
        HandleInput();

        // if we are moving, move
        if (currentState == State.moving)
        {
            distanceFromCurrent += moveSpeed * Time.deltaTime;                          // move based on the movespeed
            progress01 = Mathf.InverseLerp(0, distanceToTarget, distanceFromCurrent);   // calculate the normalised progress to the next node
            currentPos = Vector2.Lerp(currentNode.pos, targetNode.pos, progress01);     // get the current position

            trans.position = currentPos;                                                // set the current position

            // if we have reached the target node, try to keep moving
            if (progress01 == 1) 
            {
                currentNode = targetNode;
                bool canMove = MoveFromNode();
                if (!canMove)
                    currentState = State.idle;
            }
        }
    }

    void StartMove(Node a, Node b, Vector2 direction, float currentDist = 0)
    {
        currentState = State.moving;

        currentDirection = direction;

        currentNode = a;
        targetNode = b;

        distanceToTarget = Vector2.Distance(currentNode.pos, targetNode.pos);
        distanceFromCurrent = currentDist;
        progress01 = 0;
        currentState = State.moving;
    }

    bool MoveFromNode()
    {
        Node nextNode = CanMove(nextDirection);
        if (nextNode != null)
        {
            StartMove(currentNode, nextNode, nextDirection);
            return true;
        }
        else
        {
            nextNode = CanMove(currentDirection);
            if (nextNode != null)
            {
                StartMove(currentNode, nextNode, currentDirection);
                return true;
            }
        }

        return false;
    }
    
    void TryMoveInput()
    {
        if (currentState == State.idle)
        {
            currentDirection = nextDirection;
            MoveFromNode();
        }
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            nextDirection = Vector2.up;
            TryMoveInput();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            nextDirection = Vector2.down;
            TryMoveInput();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            nextDirection = Vector2.left;
            TryMoveInput();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            nextDirection = Vector2.right;
            TryMoveInput();
        }
    }

    Node CanMove(Vector2 direction)
    {
        for (int i = 0; i < currentNode.neighbours.Length; i++)
        {
            if (currentNode.directions[i] == direction)
                return currentNode.neighbours[i];
        }
        return null;
    }
}
