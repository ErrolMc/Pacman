﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that houses all the behaivour for pacman
/// </summary>
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
    Rigidbody2D rb;
    public Node currentNode;
    public Node targetNode;

    // directions
    Vector2 currentDirection;
    Vector2 nextDirection;

    // values to keep track of the movement from the current to the target node
    float distanceFromCurrent = 0;
    float distanceToTarget = 0;

    // state
    public State currentState;
    public Vector2 currentPos;

    /// <summary>
    /// Sets up pacman
    /// </summary>
    /// <param name="startingNode">The node to start at</param>
    public void Init(Node startingNode)
    {
        trans = transform;
        rb = GetComponent<Rigidbody2D>();

        currentNode = startingNode;
        currentPos = currentNode.pos;

        rb.MovePosition(currentPos);

        currentState = State.idle;
        currentDirection = Vector2.zero;
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    /// <summary>
    /// The movement update loop, called every fixed time interval
    /// </summary>
    void HandleMovement()
    {
        // if we are moving, do the move stuff
        if (currentState == State.moving)
        {
            distanceFromCurrent += moveSpeed * Time.deltaTime;                                  // move based on the movespeed
            float progress01 = Mathf.InverseLerp(0, distanceToTarget, distanceFromCurrent);     // calculate the normalised progress to the next node
            currentPos = Vector2.Lerp(currentNode.pos, targetNode.pos, progress01);             // get the current position

            rb.MovePosition(currentPos);

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

    /// <summary>
    /// Sets up and starts pacman moving from 1 node to another
    /// </summary>
    /// <param name="a">The node we are moving from</param>
    /// <param name="b">The node we are moving to</param>
    /// <param name="direction">The direction we are moving</param>
    /// <param name="startingDist">An optional paramater to specify pacmans starting distance from a to b</param>
    void StartMove(Node a, Node b, Vector2 direction, float startingDist = 0)
    {
        // TODO: animation triggers

        currentState = State.moving;

        currentDirection = direction;

        currentNode = a;
        targetNode = b;

        distanceToTarget = Vector2.Distance(currentNode.pos, targetNode.pos);       // first calculate the distance required
        distanceFromCurrent = startingDist;
    }

    /// <summary>
    /// Tries to move pacman from a node (when he has just reached a node or is idle)
    /// </summary>
    /// <returns>If we can move or not</returns>
    bool MoveFromNode()
    {
        // first see if we can move in the next/cached direction
        Node nextNode = CanMove(nextDirection);
        if (nextNode != null)
        {
            StartMove(currentNode, nextNode, nextDirection);
            return true;
        }
        else if (currentState != State.idle) // now check the current direction if we arent idle (if we are idle the directions must be the same)
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

    /// <summary>
    /// Try to move pacman from the input
    /// </summary>
    /// <returns>If we can move or not</returns>
    bool MoveInput()
    {
        // if we are moving, check only for going in opposite direction, if not return false
        if (currentState == State.moving)
        {
            // we are going in the opposite direction
            if (Vector2.Dot(nextDirection, currentDirection) == -1)
            {
                StartMove(targetNode, currentNode, nextDirection, distanceToTarget - distanceFromCurrent); // move the opposite direction, making sure we reverse the progress
                return true;
            }
            return false; // not going in opposite direction, dont do anything
        }

        // we are still here, set both directions as the same and try to move the normal way
        currentDirection = nextDirection;
        return MoveFromNode();
    }
    
    /// <summary>
    /// Handles input (duh)
    /// </summary>
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.collider.tag;
        if (tag == "Pellet")
        {
            collision.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// If we are able to move in a given direction
    /// NOTE: this only works if we are idle (because it uses the current node and pacman could be inbetween nodes)
    /// </summary>
    /// <param name="direction">The direction to query</param>
    /// <returns>Returns a node if we can move in the given direction, returns null if we cant move</returns>
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
