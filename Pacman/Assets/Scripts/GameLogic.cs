using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameLogic : MonoBehaviour
{
    public static GameLogic instance;

    [SerializeField] TextMeshProUGUI scoreDisplay;

    [SerializeField] Pacman pacmanPrefab;
    [SerializeField] Node startingNode;

    [SerializeField] GameObject pelletParent;
    [SerializeField] GameObject nodeParent;

    int score;
    Node[] nodes;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetupNodes();
        SpawnPacman();
    }

    void SpawnPacman()
    {
        Pacman pacman = Instantiate(pacmanPrefab, startingNode.pos, Quaternion.identity);
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
        score++;
        scoreDisplay.text = score.ToString();
    }
}
