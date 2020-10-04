using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that manages the logic for the MainMenu panel
/// </summary>
public class MainMenu : Panel
{
    [SerializeField] GameObject highScoreButton;

    /// <summary>
    /// Called when the panel first opens
    /// </summary>
    public override void OnShow()
    {
        highScoreButton.SetActive(HighScoreManager.instance.GetSortedScores().Count > 0);
    }

    /// <summary>
    /// Spawns the first level with 1/2 players and opens the game screen panel
    /// </summary>
    /// <param name="players">The amount of players to play the game with</param>
    public void StartGame(int players)
    {
        PanelManager.instance.ShowPanel(PanelID.GameScreen);
        GameLogic.instance.StartLevel(1, players);
    }
}
