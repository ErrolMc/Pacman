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
    [SerializeField] Ghost ghostPrefab;

    [Header("Nodes")]
    [SerializeField] Node startingNode;
    [SerializeField] Node ghostHouse;
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
        SetupNodes();
        SpawnPacman();

        ghosts = new List<Ghost>();
        SpawnGhost(Ghost.Type.blinky);
    }

    void SpawnGhost(Ghost.Type type)
    {
        switch (type)
        {
            case Ghost.Type.blinky:
                Ghost ghost = Instantiate(ghostPrefab, ghostHouse.pos, Quaternion.identity);
                ghost.Init(ghostHouse, blinkyHomeNode, ghostTimings);
                ghosts.Add(ghost);
                break;
        }
    }

    void SpawnPacman()
    {
        pacman = Instantiate(pacmanPrefab, startingNode.pos, Quaternion.identity);
        pacman.Init(startingNode);
    }

    void SetupNodes()
    {
        nodes = nodeParent.GetComponentsInChildren<Node>();
        for (int i = 0; i < nodes.Length; i++)
            nodes[i].Setup();
    }

    public void AddScore()
    {
        // TODO: play sound

        score++;
        scoreDisplay.text = score.ToString();
    }

    public void CollectSuperPellet()
    {
        // TODO: play sound

        score += 5;
        for (int i = 0; i < ghosts.Count; i++)
            ghosts[i].ChangeState(Ghost.State.frightened);
    }
}
