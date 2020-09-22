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
        consumed,       // when pacman consumes the ghost and turns it into eyes
    }

    public enum Type
    {
        blinky,
        clyde,
        pinky,
        inky
    }

    [Header("Type")]
    [SerializeField] Type ghostType;
    [SerializeField] float startReleaseTimer = 0;
    [SerializeField] float consumedReleaseTimer = 0;

    [Header("Movement settings")]
    [SerializeField] float moveSpeed = 3;
    [SerializeField] float frightenedMoveSpeed = 1.5f;
    [SerializeField] float consumedMoveSpeed = 5;
    [SerializeField] float frightenedModeDuration = 20;

    [Header("Animation Names")]
    [SerializeField] string upAnim;
    [SerializeField] string downAnim;
    [SerializeField] string leftAnim;
    [SerializeField] string rightAnim;

    [Header("Eye sprites")]
    [SerializeField] Sprite eyesUp;
    [SerializeField] Sprite eyesDown;
    [SerializeField] Sprite eyesLeft;
    [SerializeField] Sprite eyesRight;

    // caches
    protected Transform trans;
    protected Rigidbody2D rb;
    protected Animator anim;
    protected SpriteRenderer spriteRenderer;
    protected Node currentNode;
    protected Node targetNode;
    protected Node homeNode;
    protected Node ghostHouse;

    // state
    protected State currentState;
    protected Vector2 currentPos;
    protected Vector2 currentDirection;
    protected float currentMoveSpeed;

    // mode change timings
    protected GhostStateTiming[] timings;
    protected float stateChangeTimer;
    protected float frightenedTimer;
    protected float releaseTimer;
    protected int stateChangeIndex;
    protected bool stateChangeSequenceComplete;

    // values to keep track of the movement from the current to the target node
    protected float distanceFromCurrent = 0;
    protected float distanceToTarget = 0;

    // public getters
    public Type GhostType { get { return ghostType; } }
    public State CurrentState { get { return currentState; } }
    public Vector2 CurrentDirection { get { return currentDirection; } }
    public Vector2 CurrentPosition { get { return new Vector2(Mathf.RoundToInt(currentPos.x), Mathf.RoundToInt(currentPos.y)); } }

    /// <summary>
    /// Sets up the ghost at the node specified
    /// </summary>
    /// <param name="startingnode">The node to start at</param>
    public void Init(Node ghostHouse, Node homeNode, GhostStateTiming[] timings)
    {
        trans = transform;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        this.homeNode = homeNode;
        this.ghostHouse = ghostHouse;
        currentNode = ghostHouse;
        currentPos = currentNode.pos;
        currentDirection = Vector2.zero;

        rb.MovePosition(currentPos);

        StateInit(timings);
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

                if (currentState == State.consumed && currentNode == ghostHouse)
                {
                    releaseTimer = consumedReleaseTimer;
                    ChangeState(State.inHouse);
                }
                else
                {
                    // go through any portals
                    if (currentNode.nodeType == Node.NodeType.portal)
                    {
                        trans.position = currentNode.neighbours[0].pos;
                        currentNode = currentNode.neighbours[0];
                    }

                    MoveToNextNode();
                }
            }
        }
    }

    /// <summary>
    /// Picks and then instructs the ghost to move to a new node
    /// </summary>
    void MoveToNextNode(bool ignoreOppositeCheck = false)
    {
        Vector2 nextDirection = Vector2.zero;
        Node next = ChooseNextNode(ref nextDirection, ignoreOppositeCheck);

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
    /// Gets the position the ghost should be moving towards at any given time
    /// </summary>
    /// <returns>The position to target</returns>
    protected virtual Vector2 GetTargetTile()
    {
        if (currentState == State.consumed)
            return ghostHouse.pos;
        if (currentState == State.frightened)
            return GetRandomTile();
        if (currentState == State.scatter)
            return homeNode.pos;

        return GameLogic.instance.Pacman.CurrentPositionRounded;
    }

    /// <summary>
    /// Picks the next node for the ghost to move
    /// This is called only on init and when the ghost has just reached a node
    /// </summary>
    /// <param name="nextDir">A reference to the next direction that the ghost going to move</param>
    /// <returns>The next node that the ghost is moving to</returns>
    Node ChooseNextNode(ref Vector2 nextDir, bool ignoreOppositeCheck = false)
    {
        Vector2 targetTile = GetTargetTile();

        // get a list of nodes that we could possibly move to
        // note that we cant move in the opposite direction
        List<Node> validNodes = new List<Node>();
        List<Vector2> validDirections = new List<Vector2>();
        for (int i = 0; i < currentNode.neighbours.Length; i++)
        {
            if (currentNode.directions[i] != currentDirection * -1 || ignoreOppositeCheck)
            {
                // dont navigate to the ghost house if not in the consumed state
                if (currentState != State.consumed && currentNode.neighbours[i].nodeType == Node.NodeType.ghostHouse)
                    continue;

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
        releaseTimer = startReleaseTimer;
        stateChangeSequenceComplete = false;

        ChangeState(State.inHouse);
    }

    /// <summary>
    /// Update loop to determine the current state of the ghost based on the timings
    /// </summary>
    void StateUpdate()
    {
        if (currentState == State.consumed)
            return;
        else if (currentState == State.inHouse)
        {
            if (stateChangeTimer >= releaseTimer)
            {
                ChangeState(timings[stateChangeIndex].state);
                MoveToNextNode(true);
                stateChangeTimer = 0;
            }
            else
                stateChangeTimer += Time.deltaTime;
        }
        else if (currentState == State.frightened) // dont go through the normal mode sequence when they are frightened
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
        if (currentState == State.consumed)
            anim.enabled = true;

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
                currentState = State.inHouse; // force the anim and not the eyes
                Rotate(Vector2.up);
                stateChangeTimer = 0;
                break;
            case State.consumed:
                anim.enabled = false;
                currentMoveSpeed = consumedMoveSpeed;

                currentState = State.consumed; // hack
                Rotate(currentDirection);
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
        if (currentState == State.consumed)
        {
            if (direction == Vector2.left)
            {
                spriteRenderer.sprite = eyesLeft;
            }
            else if (direction == Vector2.right)
            {
                spriteRenderer.sprite = eyesRight;
            }
            else if (direction == Vector2.down)
            {
                spriteRenderer.sprite = eyesDown;
            }
            else if (direction == Vector2.up)
            {
                spriteRenderer.sprite = eyesUp;
            }
        }
        else
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
    }
    #endregion
}
