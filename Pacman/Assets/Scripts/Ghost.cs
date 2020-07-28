using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public enum State
    {
        inHouse,
        
        chase,          // actively chasing pacman
        scatter,        // give pacman a short break
        frightened,     // when pacman consumes super pellet
    }

    public enum Type
    {
        blinky,
        clyde,
        pinky,
        inky
    }

    [SerializeField] float moveSpeed = 3;
    [SerializeField] float frightenedMoveSpeed = 1.5f; 
    [SerializeField] float frightenedModeDuration = 20;

    [Header("Animation Names")]
    [SerializeField] string upAnim;
    [SerializeField] string downAnim;
    [SerializeField] string leftAnim;
    [SerializeField] string rightAnim;

    // caches
    Transform trans;
    Rigidbody2D rb;
    Animator anim;
    [HideInInspector] public Node currentNode;
    [HideInInspector] public Node targetNode;
    [HideInInspector] public Node homeNode;
    [HideInInspector] public Vector2 currentDirection;

    // state
    public State currentState;
    [HideInInspector] public Vector2 currentPos;
    float currentMoveSpeed;

    // mode change timings
    GhostStateTiming[] timings;
    float stateChangeTimer;
    float frightenedTimer;
    int stateChangeIndex;
    bool stateChangeSequenceComplete;

    // values to keep track of the movement from the current to the target node
    float distanceFromCurrent = 0;
    float distanceToTarget = 0;

    /// <summary>
    /// Sets up the ghost at the node specified
    /// </summary>
    /// <param name="startingnode">The node to start at</param>
    public void Init(Node startingnode, Node homeNode, GhostStateTiming[] timings)
    {
        trans = transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        this.homeNode = homeNode;
        currentNode = startingnode;
        currentPos = currentNode.pos;
        currentDirection = Vector2.zero;

        rb.MovePosition(currentPos);

        StateInit(timings);

        MoveToNextNode();
    }

    void Update()
    {
        StateUpdate();
    }

    void FixedUpdate()
    {
        if (currentState != State.inHouse)
        {
            distanceFromCurrent += currentMoveSpeed * Time.deltaTime;                           // move based on the movespeed
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

        if (currentState != State.frightened)
            Rotate(currentDirection);

        distanceToTarget = Vector2.Distance(currentNode.pos, targetNode.pos);       // first calculate the distance required
        distanceFromCurrent = 0;
    }

    /// <summary>
    /// Gets a random tile position
    /// </summary>
    /// <returns>A random tile position on the board</returns>
    Vector2 GetRandomTile()
    {
        int x = UnityEngine.Random.Range(0, 28);
        int y = UnityEngine.Random.Range(0, 36);

        return new Vector2(x, y);
    }

    /// <summary>
    /// Gets the position this ghost wants to move to, this is different depending on the ghost type
    /// </summary>
    /// <returns>The position to target</returns>
    Vector2 GetTargetTile()
    {
        if (currentState == State.frightened)
            return GetRandomTile();
        if (currentState == State.scatter)
            return homeNode.pos;
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

    #region state stuff
    /// <summary>
    /// Initialises the state timings
    /// </summary>
    /// <param name="timings">The timings of when to change the states</param>
    void StateInit(GhostStateTiming[] timings)
    {
        this.timings = timings;
        stateChangeIndex = 0;
        stateChangeTimer = 0;
        stateChangeSequenceComplete = false;

        ChangeState(timings[stateChangeIndex].state);
    }

    /// <summary>
    /// Update loop to determine the current state of the ghost based on the timings
    /// </summary>
    void StateUpdate()
    {
        // dont go through the normal mode sequence when they are frightened
        if (currentState == State.frightened)
        {
            // if we have gone through the frightened timer, go back to where we were
            if (frightenedTimer >= frightenedModeDuration)
                ChangeState(timings[stateChangeIndex].state);
            else
                frightenedTimer += Time.deltaTime;
        }
        else
        {
            if (stateChangeSequenceComplete == false)
            {
                // we have reached the end of the current state
                if (stateChangeTimer >= timings[stateChangeIndex].seconds)
                {
                    stateChangeIndex++;
                    stateChangeTimer = 0;
                    ChangeState(timings[stateChangeIndex].state);

                    // we are on the last timing
                    if (stateChangeIndex == timings.Length - 1)
                        stateChangeSequenceComplete = true;
                }
                else
                {
                    stateChangeTimer += Time.deltaTime;
                }
            }
        }
    }

    /// <summary>
    /// Changes the state of the ghost, executing any state transition logic
    /// </summary>
    /// <param name="newState">The new state to transition to</param>
    public void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.chase:
            case State.scatter:
                if (currentState == State.frightened)
                    Rotate(currentDirection);
                currentMoveSpeed = moveSpeed;
                break;
            case State.frightened:
                frightenedTimer = 0;
                currentMoveSpeed = frightenedMoveSpeed;
                anim.Play("Ghost_Blue");
                break;
            case State.inHouse:
                break;
        }

        currentState = newState;
    }

    /// <summary>
    /// Rotates the ghost in a given direction by changing the animations
    /// </summary>
    /// <param name="direction">The direction to face</param>
    void Rotate(Vector2 direction)
    {
        if (direction == Vector2.left)
        {
            anim.Play(leftAnim);
        }
        else if (direction == Vector2.right)
        {
            anim.Play(rightAnim);
        }
        else if (direction == Vector2.down)
        {
            anim.Play(downAnim);
        }
        else if (direction == Vector2.up)
        {
            anim.Play(upAnim);
        }
    }
    #endregion
}
