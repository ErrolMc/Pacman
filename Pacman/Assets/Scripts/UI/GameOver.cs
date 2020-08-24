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
        int score = GameLogic.instance.score;
        scoreText.text = "Score: " + score;

        if (score > Global.HighScore)
            Global.HighScore = score;
        highScoreText.text = "High Score: " + Global.HighScore.ToString();
    }

    public void OnClick_Retry()
    {
        PanelManager.instance.ShowPanel(PanelID.GameScreen);
        GameLogic.instance.StartLevel(1);
    }
}
