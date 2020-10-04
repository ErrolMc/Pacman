using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Class that manages the logic for the NewHighScore panel
/// </summary>
public class NewHighScore : Panel
{
    [SerializeField] TextMeshProUGUI newHighScoreText;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] GameObject continueButton;

    string inputName;
    int score;

    /// <summary>
    /// Called when the panel first opens
    /// Sets up some UI thats on the panel
    /// </summary>
    public override void OnShow()
    {
        score = GameLogic.instance.score;
        newHighScoreText.text = "New High Score: " + score;
        nameInput.text = "";

        continueButton.SetActive(false);
    }

    /// <summary>
    /// Callback for when the input field text changes
    /// Enables the continue button if the input name length is greater than 0
    /// </summary>
    /// <param name="str"></param>
    public void OnNameChanged(string str)
    {
        inputName = str;
        continueButton.SetActive(inputName.Length > 0);
    }

    /// <summary>
    /// Adds the high score to the highscores list and goes to the GameOver panel
    /// </summary>
    public void OnClick_Continue()
    {
        HighScoreManager.instance.AddHighScore(inputName, score);
        PanelManager.instance.ShowPanel(PanelID.GameOver);
    }
}
