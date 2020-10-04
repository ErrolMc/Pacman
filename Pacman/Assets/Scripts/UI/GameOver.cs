using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Class that manages the logic for the GameOver panel
/// </summary>
public class GameOver : Panel
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;

    /// <summary>
    /// Called when the panel first opens
    /// Sets up the UI thats on the panel
    /// </summary>
    public override void OnShow()
    {
        List<HighScore> highScores = HighScoreManager.instance.GetSortedScores();

        if (highScores.Count > 0)
        {
            HighScore highScore = highScores[0];
            highScoreText.text = "High Score (" + highScore.name + "): " + highScore.score.ToString();
            highScoreText.gameObject.SetActive(true);
        }
        else
            highScoreText.gameObject.SetActive(false);

        int score = GameLogic.instance.score;
        scoreText.text = "Score: " + score;
    }

    /// <summary>
    /// Resets the game to the first level with the same amount of players
    /// </summary>
    public void OnClick_Retry()
    {
        PanelManager.instance.ShowPanel(PanelID.GameScreen);
        GameLogic.instance.StartLevel(1, GameLogic.instance.Players);
    }
}
