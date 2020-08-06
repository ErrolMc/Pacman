using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLogic : MonoBehaviour
{
    public static GameLogic instance;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreDisplay;

    [Header("Prefabs")]
    [SerializeField] Pacman pacmanPrefab;
    [SerializeField] Ghost blinkyPrefab;
    [SerializeField] Ghost pinkyPrefab;
    [SerializeField] Ghost inkyPrefab;
    [SerializeField] Ghost clydePrefab;

    [Header("Nodes")]
    [SerializeField] Node startingNode;
    [SerializeField] Node ghostHouseLeft;
    [SerializeField] Node ghostHouseRight;
    [SerializeField] Node blinkyHomeNode;
    [SerializeField] Node clydeHomeNode;
    [SerializeField] Node pinkyHomeNode;
    [SerializeField] Node inkyHomeNode;

    [Header("Board parents")]
    [SerializeField] GameObject pelletParent;
    [SerializeField] GameObject nodeParent;

    [Header("Timings")]
    [SerializeField] GhostStateTiming[] ghostTimings;

    int score;
    Node[] nodes;
    List<Ghost> ghosts;

    [HideInInspector] public Pacman pacman;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Setup();
    }

    /// <summary>
    /// Sets up the game
    /// </summary>
    void Setup()
    {
        SetupNodes();
        SpawnPacman();

        // spawn the ghosts
        ghosts = new List<Ghost>();
        SpawnGhost(Ghost.Type.blinky);
        SpawnGhost(Ghost.Type.pinky);
        SpawnGhost(Ghost.Type.inky);
        SpawnGhost(Ghost.Type.clyde);
    }

    /// <summary>
    /// Spawns a ghost given the type
    /// </summary>
    /// <param name="type">The type of ghost to spawn</param>
    void SpawnGhost(Ghost.Type type)
    {
        switch (type)
        {
            case Ghost.Type.blinky:
                Ghost blinky = Instantiate(blinkyPrefab);
                blinky.Init(ghostHouseLeft, blinkyHomeNode, ghostTimings);
                ghosts.Add(blinky);
                break;
            case Ghost.Type.pinky:
                Ghost pinky = Instantiate(pinkyPrefab);
                pinky.Init(ghostHouseRight, pinkyHomeNode, ghostTimings);
                ghosts.Add(pinky);
                break;
            case Ghost.Type.inky:
                Ghost inky = Instantiate(inkyPrefab);
                inky.Init(ghostHouseRight, inkyHomeNode, ghostTimings);
                ghosts.Add(inky);
                break;
            case Ghost.Type.clyde:
                Ghost clyde = Instantiate(clydePrefab);
                clyde.Init(ghostHouseLeft, clydeHomeNode, ghostTimings);
                ghosts.Add(clyde);
                break;
        }
    }

    /// <summary>
    /// Spawns pacman
    /// </summary>
    void SpawnPacman()
    {
        pacman = Instantiate(pacmanPrefab, startingNode.pos, Quaternion.identity);
        pacman.Init(startingNode);
    }

    /// <summary>
    /// Sets up all the nodes so pacman/ghosts can path between them
    /// </summary>
    void SetupNodes()
    {
        nodes = nodeParent.GetComponentsInChildren<Node>();
        for (int i = 0; i < nodes.Length; i++)
            nodes[i].Setup();
    }

    /// <summary>
    /// Adds score to the score counter
    /// </summary>
    /// <param name="value">The amount of score to add</param>
    public void AddScore(int value)
    {
        score += value;
        scoreDisplay.text = score.ToString();
    }

    /// <summary>
    /// Gets a ghost given the type
    /// </summary>
    /// <param name="type">The type of the ghost to get</param>
    /// <returns>The ghost of the given type (or null if it doesnt exist)</returns>
    public Ghost GetGhost(Ghost.Type type)
    {
        for (int i = 0; i < ghosts.Count; i++)
        {
            if (ghosts[i].GhostType == type)
                return ghosts[i];
        }
        return null;
    }

    public List<Ghost> GetAllGhosts()
    {
        return ghosts;
    }
}
