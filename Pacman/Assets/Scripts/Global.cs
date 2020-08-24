using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global
{
    public static int HighScore
    {
        get
        {
            return PlayerPrefs.GetInt("HighScore", 0);
        }
        set
        {
            PlayerPrefs.SetInt("HighScore", value);
        }
    }
}
