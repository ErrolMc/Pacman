using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PanelID
{
    MainMenu,
    GameScreen,
    GameOver,
    HighScores,
    NewHighScore,
    Customisation,
}

/// <summary>
/// Base class to represent a UI panel/screen
/// </summary>
public class Panel : MonoBehaviour
{
    public PanelID panelID;
    [HideInInspector] public bool isActive = false;

    /// <summary>
    /// Called when the panel first opens
    /// </summary>
    public virtual void OnShow()
    {

    }

    /// <summary>
    /// Called when the panel closes
    /// </summary>
    public virtual void OnHide()
    {

    }
}
