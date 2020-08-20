﻿using System.Collections;
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
        Debug.Log("Starting: " + players);
    }
}
