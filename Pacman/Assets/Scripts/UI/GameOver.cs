using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOver : Panel
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;

    public override void OnShow()
    {
        HighScoreContainer highScoreContainer = HighScoreContainer.Load();
        List<HighScore> highScores = highScoreContainer.GetSortedScores();

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

    public void OnClick_Retry()
    {
        PanelManager.instance.ShowPanel(PanelID.GameScreen);
        GameLogic.instance.StartLevel(1, GameLogic.instance.Players);
    }
}
