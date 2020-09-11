using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelID
{
    MainMenu,
    GameScreen,
    GameOver,
    HighScores,
    NewHighScore
}

public class Panel : MonoBehaviour
{
    public PanelID panelID;
    [HideInInspector] public bool isActive = false;

    public virtual void OnShow()
    {

    }

    public virtual void OnHide()
    {

    }
}
