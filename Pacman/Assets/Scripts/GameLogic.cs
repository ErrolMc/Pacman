using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] Pacman pacmanPrefab;
    [SerializeField] Node startingNode;

    [SerializeField] GameObject pelletParent;
    [SerializeField] GameObject nodeParent;

    Node[] nodes;

    void Start()
    {
        SetupNodes();
        SpawnPacman();
    }

    void SpawnPacman()
    {
        Pacman pacman = Instantiate(pacmanPrefab);
        pacman.Init(startingNode);
    }

    void SetupNodes()
    {
        nodes = nodeParent.GetComponentsInChildren<Node>();
        for (int i = 0; i < nodes.Length; i++)
            nodes[i].Setup();
    }
}
