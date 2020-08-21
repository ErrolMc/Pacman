using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : Panel
{
    public static MainMenu instance;

    void Awake()
    {
        instance = this;
    }

    public override void OnShow()
    {

    }

    public override void OnHide()
    {
       
    }

    public void StartGame(int players)
    {
        if (players == 1)
        {
            PanelManager.instance.ShowPanel(PanelID.GameScreen);
            GameLogic.instance.StartLevel(1);
        }
        else
            Debug.Log("Multiplayer not supported yet");
    }
}
