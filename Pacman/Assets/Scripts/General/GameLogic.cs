﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Center point for the game, contains and manages references for the current level/ghosts/pacman
/// </summary>
public class GameLogic : MonoBehaviour
{
    public static GameLogic instance;

    [Header("Prefabs")]
    [SerializeField] Pacman pacmanPrefab;
    [SerializeField] Ghost blinkyPrefab;
    [SerializeField] Ghost pinkyPrefab;
    [SerializeField] Ghost inkyPrefab;
    [SerializeField] Ghost clydePrefab;
    [SerializeField] ScoreText popupTextPrefab;

    [Header("Assets")]
    public Sprite[] fruitSprites;

    [Header("Other")]
    [SerializeField] int numLevels;

    // state caches
    int players;
    int currentLevelNumber;
    [HideInInspector] public int score;

    // component caches
    List<Ghost> ghosts;
    Level currentLevel;
    Pacman pacman1;
    Pacman pacman2;

    // getters
    public Pacman Pacman
    {
        get
        {
            if (players == 1)
                return pacman1;
            else
            {
                // if player 1 is dead and player 2 isnt, return player 2
                if (pacman1.CurrentState == Pacman.State.dead && pacman2.CurrentState != Pacman.State.dead)
                    return pacman2;
                return pacman1; // else return player 1
            }
        }
    }
    
    public Level CurrentLevel { get { return currentLevel; } }

    public int Players { get { return players; } }

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Starts a level with x amount of players
    /// </summary>
    /// <param name="levelNumber">The level to start</param>
    /// <param name="players">The amount of players</param>
    public void StartLevel(int levelNumber, int players, int startingScore = 0)
    {
        // spawn the level
        Level level = Instantiate(Resources.Load<Level>("Levels/Level_" + levelNumber));
        level.Setup();
        level.transform.position = Vector3.zero;
        currentLevel = level;
        currentLevelNumber = levelNumber;

        // spawn pacman
        this.players = players;
        if (players == 2)
            pacman2 = SpawnPacman(currentLevel.StartingNode_2, 2);
        pacman1 = SpawnPacman(currentLevel.StartingNode, 1);

        // spawn the ghosts
        ghosts = new List<Ghost>();
        SpawnGhost(Ghost.Type.blinky, true);
        SpawnGhost(Ghost.Type.pinky);
        SpawnGhost(Ghost.Type.inky);
        SpawnGhost(Ghost.Type.clyde);

        score = startingScore;
        GameScreen.instance.SetScoreText(score);
    }

    /// <summary>
    /// Spawns a ghost given the type
    /// </summary>
    /// <param name="type">The type of ghost to spawn</param>
    void SpawnGhost(Ghost.Type type, bool aStar = false)
    {
        Node ghostHouseLeft = currentLevel.GhostHouseLeft;
        Node ghostHouseRight = currentLevel.GhostHouseRight;

        switch (type)
        {
            case Ghost.Type.blinky:
                Ghost blinky = Instantiate(blinkyPrefab);
                blinky.Init(ghostHouseLeft, currentLevel.BlinkyHomeNode, currentLevel.GhostTimings, aStar);
                ghosts.Add(blinky);
                break;
            case Ghost.Type.pinky:
                Ghost pinky = Instantiate(pinkyPrefab);
                pinky.Init(ghostHouseRight, currentLevel.PinkyHomeNode, currentLevel.GhostTimings, aStar);
                ghosts.Add(pinky);
                break;
            case Ghost.Type.inky:
                Ghost inky = Instantiate(inkyPrefab);
                inky.Init(ghostHouseRight, currentLevel.InkyHomeNode, currentLevel.GhostTimings, aStar);
                ghosts.Add(inky);
                break;
            case Ghost.Type.clyde:
                Ghost clyde = Instantiate(clydePrefab);
                clyde.Init(ghostHouseLeft, currentLevel.ClydeHomeNode, currentLevel.GhostTimings, aStar);
                ghosts.Add(clyde);
                break;
        }
    }

    /// <summary>
    /// Spawns pacman
    /// </summary>
    Pacman SpawnPacman(Node startingNode, int player)
    {
        Pacman newPacman = Instantiate(pacmanPrefab, startingNode.pos, Quaternion.identity);
        newPacman.Init(startingNode, player);
        return newPacman;
    }

    /// <summary>
    /// Adds score to the score counter
    /// </summary>
    /// <param name="value">The amount of score to add</param>
    public void AddScore(int value)
    {
        score += value;
        GameScreen.instance.SetScoreText(score);

        if (currentLevel.CheckGameComplete() && Pacman.CurrentState != Pacman.State.dead)
        {
            for (int i = 0; i < ghosts.Count; i++)
                ghosts[i].gameObject.SetActive(false);

            EndGame(3, true);
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
    /// Clears the current level
    /// </summary>
    void ClearLevel()
    {
        // the level
        Destroy(currentLevel.gameObject);
        currentLevel = null;

        // pacman
        Destroy(pacman1.gameObject);
        pacman1 = null;
        if (players == 2)
        {
            Destroy(pacman2.gameObject);
            pacman2 = null;
        }

        // ghosts
        foreach (Ghost ghost in ghosts)
            Destroy(ghost.gameObject);
        ghosts = null;
    }

    /// <summary>
    /// Resets the game
    /// </summary>
    /// <param name="delay">A delay before resetting the board</param>
    public void EndGame(float delay, bool won)
    {
        StartCoroutine(EndGameSequence(delay, won));
    }

    /// <summary>
    /// The timing sequence for resetting the game
    /// </summary>
    /// <param name="delay">The delay before resetting the board</param>
    IEnumerator EndGameSequence(float delay, bool won)
    {
        yield return new WaitForSeconds(delay);

        ClearLevel();

        // if we have won and there are more levels, go to the next level
        if (won && currentLevelNumber < numLevels)
        {
            yield return new WaitForSeconds(0.5f);
            StartLevel(currentLevelNumber + 1, players, score);
        }
        else
        {
            if (HighScoreManager.instance.CanAddScore(score))
                PanelManager.instance.ShowPanel(PanelID.NewHighScore);
            else
                PanelManager.instance.ShowPanel(PanelID.GameOver);
        }
    }

    public void SpawnScoreText(Vector2 position, float duration, int score)
    {
        StartCoroutine(SpawnScoreText_Async(position, duration, score));
    }

    IEnumerator SpawnScoreText_Async(Vector2 position, float duration, int score)
    {
        ScoreText popupText = Instantiate(popupTextPrefab);
        popupText.Spawn(position, score);

        yield return new WaitForSeconds(duration);

        Destroy(popupText.gameObject);
    }

    /// <summary>
    /// Spawns the text for when pacman eats a ghost and waits a given duration
    /// </summary>
    /// <param name="pacman">The pacman responsible for where the text is going to be spawned</param>
    /// <param name="ghost">The ghost pacman ate</param>
    /// <param name="duration">The duration the game will pause for</param>
    /// <param name="score">The score on the text</param>
    public void SpawnGhostEatText(Pacman pacman, Ghost ghost, float duration, int score)
    {
        StartCoroutine(SpawnGhostEatText_Async(pacman, ghost, duration, score));
    }

    /// <summary>
    /// Coroutine for the spawn ghost eat text method
    /// </summary>
    /// <param name="pacman">The pacman responsible for where the text is going to be spawned</param>
    /// <param name="ghost">The ghost pacman ate</param>
    /// <param name="duration">The duration the game will pause for</param>
    /// <param name="score">The score on the text</param>
    IEnumerator SpawnGhostEatText_Async(Pacman pacman, Ghost ghost, float duration, int score)
    {
        yield return null; // wait a frame

        pacman.gameObject.SetActive(false);
        ghost.gameObject.SetActive(false);

        ScoreText popupText = Instantiate(popupTextPrefab);
        popupText.Spawn(pacman.CurrentPositionRounded, score);

        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1;

        pacman.gameObject.SetActive(true);
        ghost.gameObject.SetActive(true);

        Destroy(popupText.gameObject);
    }
}
