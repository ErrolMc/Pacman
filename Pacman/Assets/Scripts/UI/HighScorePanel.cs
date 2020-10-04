using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Class that manages the logic for the HighScore panel
/// </summary>
public class HighScorePanel : Panel
{
    [SerializeField] TextMeshProUGUI scoreTemplate;

    List<TextMeshProUGUI> scoreObjects;

    /// <summary>
    /// Called when the panel first opens
    /// Spawns all the highscores to the screen
    /// </summary>
    public override void OnShow()
    {
        scoreObjects = new List<TextMeshProUGUI>();
        scoreTemplate.gameObject.SetActive(false);

        List<HighScore> highScores = HighScoreManager.instance.GetSortedScores();

        Transform templateTrans = scoreTemplate.transform;
        for (int i = 0; i < highScores.Count; i++)
        {
            HighScore curScore = highScores[i];

            TextMeshProUGUI curScoreObject = Instantiate(scoreTemplate);
            
            Transform curScoreObjectTrans = curScoreObject.transform;
            curScoreObjectTrans.localScale = Vector3.one;
            curScoreObjectTrans.parent = templateTrans.parent;

            curScoreObject.text = (i + 1) + ": " + curScore.name + " - " + curScore.score;

            curScoreObject.gameObject.SetActive(true);
            scoreObjects.Add(curScoreObject);
        }
    }

    /// <summary>
    /// Called when the panel closes
    /// Despawns the highscore objects that are currently on the screen
    /// </summary>
    public override void OnHide()
    {
        for (int i = 0; i < scoreObjects.Count; i++)
            Destroy(scoreObjects[i].gameObject);
        scoreObjects.Clear();
    }
}
