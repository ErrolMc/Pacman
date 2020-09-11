using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewHighScore : Panel
{
    [SerializeField] TextMeshProUGUI newHighScoreText;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] GameObject continueButton;

    string inputName;
    int score;

    public override void OnShow()
    {
        score = GameLogic.instance.score;
        newHighScoreText.text = "New High Score: " + score;
        nameInput.text = "";

        continueButton.SetActive(false);
    }

    public void OnNameChanged(string str)
    {
        inputName = str;
        continueButton.SetActive(inputName.Length > 0);
    }

    public void OnClick_Continue()
    {
        HighScoreContainer highScores = HighScoreContainer.Load();
        highScores.AddHighScore(new HighScore(inputName, score));
        highScores.Save();

        PanelManager.instance.ShowPanel(PanelID.GameOver);
    }
}
