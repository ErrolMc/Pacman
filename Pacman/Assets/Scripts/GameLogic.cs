﻿using System.Collections;
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

    [Header("Level")]
    [SerializeField] Level level;

    [Header("Timings")]
    [SerializeField] GhostStateTiming[] ghostTimings;

    int score;
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
        level.SetupNodes();
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
        Node ghostHouseLeft = level.GhostHouseLeft;
        Node ghostHouseRight = level.GhostHouseRight;

        switch (type)
        {
            case Ghost.Type.blinky:
                Ghost blinky = Instantiate(blinkyPrefab);
                blinky.Init(ghostHouseLeft, level.BlinkyHomeNode, ghostTimings);
                ghosts.Add(blinky);
                break;
            case Ghost.Type.pinky:
                Ghost pinky = Instantiate(pinkyPrefab);
                pinky.Init(ghostHouseRight, level.PinkyHomeNode, ghostTimings);
                ghosts.Add(pinky);
                break;
            case Ghost.Type.inky:
                Ghost inky = Instantiate(inkyPrefab);
                inky.Init(ghostHouseRight, level.InkyHomeNode, ghostTimings);
                ghosts.Add(inky);
                break;
            case Ghost.Type.clyde:
                Ghost clyde = Instantiate(clydePrefab);
                clyde.Init(ghostHouseLeft, level.ClydeHomeNode, ghostTimings);
                ghosts.Add(clyde);
                break;
        }
    }

    /// <summary>
    /// Spawns pacman
    /// </summary>
    void SpawnPacman()
    {
        Node startingNode = level.StartingNode;
        pacman = Instantiate(pacmanPrefab, startingNode.pos, Quaternion.identity);
        pacman.Init(startingNode);
    }

    /// <summary>
    /// Adds score to the score counter
    /// </summary>
    /// <param name="value">The amount of score to add</param>
    public void AddScore(int value)
    {
        score += value;
        scoreDisplay.text = score.ToString();

        if (level.CheckGameComplete() && pacman.CurrentState != Pacman.State.dead)
        {
            for (int i = 0; i < ghosts.Count; i++)
                ghosts[i].gameObject.SetActive(false);

            ResetGame(3);
        }
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

    /// <summary>
    /// Public getter for ghost list
    /// </summary>
    /// <returns>The list of ghostys</returns>
    public List<Ghost> GetAllGhosts()
    {
        return ghosts;
    }

    /// <summary>
    /// Resets the game
    /// </summary>
    /// <param name="delay">A delay before resetting the board</param>
    public void ResetGame(float delay = 0)
    {
        StartCoroutine(ResetGameSequence(delay));
    }

    /// <summary>
    /// The timing sequence for resetting the game
    /// </summary>
    /// <param name="delay">The delay before resetting the board</param>
    IEnumerator ResetGameSequence(float delay = 0)
    {
        yield return new WaitForSeconds(delay);

        level.EnablePellets(false);

        // despawn pacman and the ghosts
        Destroy(pacman.gameObject);
        foreach (Ghost ghost in ghosts)
            Destroy(ghost.gameObject);
        pacman = null;
        ghosts = null;

        yield return new WaitForSeconds(0.5f);

        level.EnablePellets(true);

        // respawn pacman and the ghosts
        SpawnPacman();

        ghosts = new List<Ghost>();
        SpawnGhost(Ghost.Type.blinky);
        SpawnGhost(Ghost.Type.pinky);
        SpawnGhost(Ghost.Type.inky);
        SpawnGhost(Ghost.Type.clyde);

        score = 0;
        scoreDisplay.text = score.ToString();
    }
}
