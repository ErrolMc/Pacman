using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Class that manages the logic for the GameScreen panel
/// </summary>
public class GameScreen : Panel
{
    public static GameScreen instance;

    [SerializeField] TextMeshProUGUI scoreText;

    /// <summary>
    /// Called when the panel first opens, initialises the instance variable
    /// </summary>
    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Updates the score text to a new value
    /// </summary>
    /// <param name="score">The number to set the score text to</param>
    public void SetScoreText(int score)
    {
        scoreText.text = score.ToString();
    }
}
