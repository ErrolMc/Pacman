using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameScreen : Panel
{
    public static GameScreen instance;

    [SerializeField] TextMeshProUGUI scoreText;

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

    public void SetScoreText()
    {

    }
}
