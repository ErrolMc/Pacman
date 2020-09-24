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

    [Header("Other")]
    [SerializeField] GhostStateTiming[] ghostTimings;
    [SerializeField] float fruitAmount = 0;

    Node[] nodes;
    List<SpriteRenderer> pellets;
    List<SpriteRenderer> superPellets;
    List<SpriteRenderer> fruits;

    public Node GhostHouseLeft { get { return ghostHouseLeft; } }
    public Node GhostHouseRight { get { return ghostHouseRight; } }
    public Node StartingNode { get { return startingNode; } }
    public Node StartingNode_2 { get { return startingNode_2; } }
    public Node BlinkyHomeNode { get { return blinkyHomeNode; } }
    public Node PinkyHomeNode { get { return pinkyHomeNode; } }
    public Node ClydeHomeNode { get { return clydeHomeNode; } }
    public Node InkyHomeNode { get { return inkyHomeNode; } }
    public GhostStateTiming[] GhostTimings { get { return ghostTimings; } }
    public Node[] Nodes { get { return nodes; } }

    public void Setup()
    {
        nodes = nodeParent.GetComponentsInChildren<Node>();
        for (int i = 0; i < nodes.Length; i++)
            nodes[i].Setup();

        pellets = new List<SpriteRenderer>();
        superPellets = new List<SpriteRenderer>();
        fruits = new List<SpriteRenderer>();
        ProcessSprites(pelletParent.GetComponentsInChildren<SpriteRenderer>());
        ProcessSprites(nodeParent.GetComponentsInChildren<SpriteRenderer>());

        int fruit = (int)(fruitAmount * GameSettings.instance.FruitAmountMultiplier);
        if (fruit > 0)
            AddFruits(fruit);
    }

    void AddFruits(int amount)
    {
        Sprite[] fruitSprites = GameLogic.instance.fruitSprites;

        for (int i = 0; i < amount; i++)
        {
            int ind = Random.Range(0, pellets.Count - 1);
            SpriteRenderer pellet = pellets[ind];
            pellets.RemoveAt(ind);

            pellet.sprite = fruitSprites[Random.Range(0, fruitSprites.Length - 1)];
            pellet.gameObject.tag = "Fruit";
            pellet.transform.localScale = Vector3.one * 2.5f;
            fruits.Add(pellet);
        }
    }

    void ProcessSprites(SpriteRenderer[] sprites)
    {
        foreach (SpriteRenderer sprite in sprites)
        {
            string tag = sprite.gameObject.tag;

            switch (tag)
            {
                case "Fruit":
                    fruits.Add(sprite);
                    break;
                case "SuperPellet":
                    superPellets.Add(sprite);
                    break;
                case "Pellet":
                    pellets.Add(sprite);
                    break;
            }
        }
    }

    public bool CheckGameComplete()
    {
        if (AnySpritesEnabled(pellets))
            return false;
        if (AnySpritesEnabled(superPellets))
            return false;
        if (AnySpritesEnabled(fruits))
            return false;
        return true;
    }

    bool AnySpritesEnabled(List<SpriteRenderer> sprites)
    {
        foreach (SpriteRenderer sprite in sprites)
        {
            if (sprite.gameObject.activeInHierarchy)
                return true;
        }
        return false;
    }

    public void EnableSprites(bool state)
    {
        foreach (SpriteRenderer pellet in pellets)
            pellet.gameObject.SetActive(state);
        foreach (SpriteRenderer fruit in fruits)
            fruit.gameObject.SetActive(state);
        foreach (SpriteRenderer superPellet in superPellets)
            superPellet.gameObject.SetActive(state);
    }
}
