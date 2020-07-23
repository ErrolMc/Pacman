using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
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
    public Node currentNode;
    public Node targetNode;

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
                currentState = State.idle;

                currentNode = targetNode;
                bool canMove = Move();
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
        progress01 = Mathf.InverseLerp(0, distanceToTarget, distanceFromCurrent);
    }

    bool Move()
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

    bool MoveInput()
    {
        if (currentState == State.moving)
        {
            // we are going in the opposite direction
            if (Vector2.Dot(nextDirection, currentDirection) == -1)
            {
                StartMove(targetNode, currentNode, nextDirection, distanceToTarget - distanceFromCurrent);
                return true;
            }
            return false;
        }

        Node nextNode = CanMove(nextDirection);
        if (nextNode != null)
        {
            StartMove(currentNode, nextNode, nextDirection);
            return true;
        }

        return false;
        
    }
    
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            nextDirection = Vector2.up;
            MoveInput();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            nextDirection = Vector2.down;
            MoveInput();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            nextDirection = Vector2.left;
            MoveInput();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            nextDirection = Vector2.right;
            MoveInput();
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
