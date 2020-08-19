using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [Header("Starting Nodes")]
    [SerializeField] Node startingNode;
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

    Node[] nodes;
    List<SpriteRenderer> pellets;

    public Node GhostHouseLeft { get { return ghostHouseLeft; } }
    public Node GhostHouseRight { get { return ghostHouseRight; } }
    public Node StartingNode { get { return startingNode; } }

    public void SetupNodes()
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
