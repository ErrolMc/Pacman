using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Panel
{
    [SerializeField] GameObject highScoreButton;

    public override void OnShow()
    {
        highScoreButton.SetActive(HighScoreContainer.Load().GetSortedScores().Count > 0);
    }

    public void StartGame(int players)
    {
        PanelManager.instance.ShowPanel(PanelID.GameScreen);
        GameLogic.instance.StartLevel(1, players);
    }
}
