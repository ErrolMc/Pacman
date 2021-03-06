﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that houses all the behaivour for pacman
/// </summary>
public class Pacman : MonoBehaviour
{
    enum InputDirection
    {
        up,
        down,
        left,
        right
    }

    public enum State
    {
        idle,
        moving,
        dead
    }

    [SerializeField] float moveSpeed = 5;
    [SerializeField] bool invulerable = false;

    // caches
    Transform trans;
    Rigidbody2D rb;
    Animator anim;
    Node currentNode;
    Node targetNode;
    float currentMoveSpeed;

    // directions
    Vector2 currentDirection;
    Vector2 nextDirection;

    // values to keep track of the movement from the current to the target node
    float distanceFromCurrent = 0;
    float distanceToTarget = 0;

    // state
    State currentState;
    Vector2 currentPos;
    int player;

    // public getters
    public int Player { get { return player; } }
    public State CurrentState {  get { return currentState; } }
    public Vector2 CurrentDirection { get { return currentDirection; } }
    public Vector2 CurrentPositionRounded { get { return new Vector2(Mathf.RoundToInt(currentPos.x), Mathf.RoundToInt(currentPos.y)); } }
    public Vector2 CurrentPosition { get { return currentPos; } }
    public Node CurrentNode { get { return currentNode; } }
    public Node TargetNode { get { return targetNode; } }

    /// <summary>
    /// Sets up pacman
    /// </summary>
    /// <param name="startingNode">The node to start at</param>
    public void Init(Node startingNode, int player)
    {
        trans = transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        this.player = player;
        currentNode = startingNode;
        targetNode = currentNode;
        currentPos = currentNode.pos;
        currentMoveSpeed = moveSpeed * GameSettings.instance.PacmanSpeedMultiplier;

        rb.MovePosition(currentPos);

        ChangeState(State.idle);
        currentDirection = Vector2.zero;
    }

    // Ran every frame (refresh rate of monitor)
    void Update()
    {
        if (currentState != State.dead)
            HandleInput();
    }

    // Ran 60 times a second (fixed)
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
            distanceFromCurrent += currentMoveSpeed * Time.deltaTime;                                  // move based on the movespeed
            float progress01 = Mathf.InverseLerp(0, distanceToTarget, distanceFromCurrent);     // calculate the normalised progress to the next node
            currentPos = Vector2.Lerp(currentNode.pos, targetNode.pos, progress01);             // get the current position

            rb.MovePosition(currentPos);

            // if we have reached the target node, try to keep moving
            if (progress01 == 1)
            {
                currentNode = targetNode;

                // if the current node is a portal, teleport through it
                if (currentNode.nodeType == Node.NodeType.portal)
                {
                    trans.position = currentNode.neighbours[0].pos;
                    currentNode = currentNode.neighbours[0];
                }

                bool canMove = MoveFromNode();
                if (!canMove)
                    ChangeState(State.idle);
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
        ChangeState(State.moving);

        currentDirection = direction;

        Rotate(currentDirection);

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
        if (HasInput(InputDirection.up))
        {
            nextDirection = Vector2.up;
            MoveInput();
        }
        else if (HasInput(InputDirection.down))
        {
            nextDirection = Vector2.down;
            MoveInput();
        }
        else if (HasInput(InputDirection.left))
        {
            nextDirection = Vector2.left;
            MoveInput();
        }
        else if (HasInput(InputDirection.right))
        {
            nextDirection = Vector2.right;
            MoveInput();
        }
    }

    /// <summary>
    /// Changes pacman to a new state, executing any code that needs to be ran on a state change
    /// </summary>
    /// <param name="newState">Pacmans new state</param>
    void ChangeState(State newState)
    {
        currentState = newState;

        switch (newState)
        {
            case State.idle:
                anim.StartPlayback();
                break;
            case State.moving:
                anim.StopPlayback();
                break;
            case State.dead:
                trans.rotation = Quaternion.identity;
                anim.StopPlayback();
                anim.Play("Pacman_Death");

                // check if all players are dead
                if (GameLogic.instance.Pacman.CurrentState == State.dead)
                    GameLogic.instance.EndGame(3, false);
                break;
        }
    }

    /// <summary>
    /// Rotates pacman to a direction
    /// </summary>
    void Rotate(Vector2 direction)
    {
        if (direction == Vector2.right)
        {
            trans.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector2.up)
        {
            trans.localRotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == Vector2.left)
        {
            trans.localRotation = Quaternion.Euler(0, 0, 180);
        }
        else if (direction == Vector2.down)
        {
            trans.localRotation = Quaternion.Euler(0, 0, 270);
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
            if (currentNode.neighbours[i].nodeType != Node.NodeType.ghostHouse && currentNode.directions[i] == direction)
                return currentNode.neighbours[i];
        }
        return null;
    }

    /// <summary>
    /// Checks if there is input in a given direction, this function takes into account the player
    /// </summary>
    /// <param name="inputDirection">The input direction to test for</param>
    /// <returns>If there is input in the specified direction</returns>
    bool HasInput(InputDirection inputDirection)
    {
        switch (inputDirection)
        {
            case InputDirection.up:
                if (player == 1)
                    return Input.GetKeyDown(KeyCode.UpArrow);
                else
                    return Input.GetKeyDown(KeyCode.W);
            case InputDirection.down:
                if (player == 1)
                    return Input.GetKeyDown(KeyCode.DownArrow);
                else
                    return Input.GetKeyDown(KeyCode.S);
            case InputDirection.left:
                if (player == 1)
                    return Input.GetKeyDown(KeyCode.LeftArrow);
                else
                    return Input.GetKeyDown(KeyCode.A);
            case InputDirection.right:
                if (player == 1)
                    return Input.GetKeyDown(KeyCode.RightArrow);
                else
                    return Input.GetKeyDown(KeyCode.D);
        }

        return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.collider.tag;

        switch (tag)
        {
            case "Pellet":
                // TODO: play sound
                collision.gameObject.SetActive(false);
                GameLogic.instance.AddScore(GameSettings.instance.PelletScore);
                break;
            case "SuperPellet":
                // TODO: play sound
                GameLogic.instance.AddScore(GameSettings.instance.SuperPelletScore);

                // Set all the ghosts to frightened
                List<Ghost> ghosts = GameLogic.instance.GetAllGhosts();
                for (int i = 0; i < ghosts.Count; i++)
                {
                    Ghost.State ghostState = ghosts[i].CurrentState;
                    if (ghostState != Ghost.State.consumed && ghostState != Ghost.State.inHouse)
                        ghosts[i].ChangeState(Ghost.State.frightened);
                }

                collision.gameObject.SetActive(false);
                break;
            case "Fruit":
                collision.gameObject.SetActive(false);
                int score = GameSettings.instance.FruitScore;
                GameLogic.instance.AddScore(score);

                GameLogic.instance.SpawnScoreText(currentPos, 1, score);
                break;
            case "Ghost":
                if (currentState != State.dead)
                {
                    Ghost ghost = collision.gameObject.GetComponent<Ghost>();
                    if (ghost.CurrentState == Ghost.State.frightened)
                    {
                        int scoreToAdd = GameSettings.instance.GhostEatScore;

                        GameLogic.instance.AddScore(scoreToAdd);
                        ghost.ChangeState(Ghost.State.consumed);

                        GameLogic.instance.SpawnGhostEatText(this, ghost, 1, scoreToAdd);
                    }
                    else if (ghost.CurrentState != Ghost.State.consumed && !invulerable)
                    {
                        ChangeState(State.dead);
                    }
                }
                break;
        }
    }
}
