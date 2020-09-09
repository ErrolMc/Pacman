using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Starting Nodes")]
    [SerializeField] Node startingNode;
    [SerializeField] Node startingNode_2;
    [SerializeField] Node ghostHouseLeft;
    [SerializeField] Node ghostHouseRight;

    [Header("Ghost Home Nodes")]
    [SerializeField] Node blinkyHomeNode;
    [SerializeField] Node clydeHomeNode;
    [SerializeField] Node pinkyHomeNode;
    [SerializeField] Node inkyHomeNode;

    [Header("Parent Objects")]
    [SerializeField] GameObject pelletParent;
    [SerializeField] GameObject nodeParent;

    [Header("Timings")]
    [SerializeField] GhostStateTiming[] ghostTimings;

    Node[] nodes;
    List<SpriteRenderer> pellets;

    public Node GhostHouseLeft { get { return ghostHouseLeft; } }
    public Node GhostHouseRight { get { return ghostHouseRight; } }
    public Node StartingNode { get { return startingNode; } }
    public Node StartingNode_2 { get { return startingNode_2; } }
    public Node BlinkyHomeNode { get { return blinkyHomeNode; } }
    public Node PinkyHomeNode { get { return pinkyHomeNode; } }
    public Node ClydeHomeNode { get { return clydeHomeNode; } }
    public Node InkyHomeNode { get { return inkyHomeNode; } }
    public GhostStateTiming[] GhostTimings { get { return ghostTimings; } }

    public void Setup()
    {
        nodes = nodeParent.GetComponentsInChildren<Node>();
        for (int i = 0; i < nodes.Length; i++)
            nodes[i].Setup();

        SpriteRenderer[] pellets1 = pelletParent.GetComponentsInChildren<SpriteRenderer>();
        SpriteRenderer[] pellets2 = pelletParent.GetComponentsInChildren<SpriteRenderer>();
        pellets = new List<SpriteRenderer>();
        for (int i = 0; i < pellets1.Length; i++)
            pellets.Add(pellets1[i]);
        for (int i = 0; i < pellets2.Length; i++)
            pellets.Add(pellets2[i]);
    }

    public bool CheckGameComplete()
    {
        for (int i = 0; i < pellets.Count; i++)
        {
            if (pellets[i].gameObject.activeInHierarchy)
                return false;
        }
        return true;
    }

    public void EnablePellets(bool state)
    {
        for (int i = 0; i < pellets.Count; i++)
            pellets[i].gameObject.SetActive(state);

        for (int i = 0; i < nodes.Length; i++)
            nodes[i].gameObject.SetActive(state);
    }
}
