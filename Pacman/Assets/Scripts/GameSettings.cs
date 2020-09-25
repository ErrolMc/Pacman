using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static GameSettings instance;

    // fields
    public float defaultPacmanSpeedMultiplier = 1;
    public float defaultGhostSpeedMultiplier = 1;
    public float defaultFruitAmountMultiplier = 1;
    public int defaultGhostFrightenedDuration = 10;
    public float defaultGhostFrightenedSpeedMultiplier = 1;
    public float defaultGhostConsumedSpeedMultiplier = 1;
    public int defaultPelletScore = 1;
    public int defaultFruitScore = 10;
    public int defaultGhostEatScore = 20;
    public int defaultSuperPelletScore = 5;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Resets all the custom settings to defaults listed on the prefab
    /// </summary>
    public void ResetToDefaults()
    {
        PacmanSpeedMultiplier = defaultPacmanSpeedMultiplier;
        GhostSpeedMultiplier = defaultGhostSpeedMultiplier;
        FruitAmountMultiplier = defaultFruitAmountMultiplier;
        GhostFrightenedDuration = defaultGhostFrightenedDuration;
        GhostFrightenedSpeedMultiplier = defaultGhostFrightenedSpeedMultiplier;
        GhostConsumedSpeedMultiplier = defaultGhostConsumedSpeedMultiplier;
        PelletScore = defaultPelletScore;
        FruitScore = defaultFruitScore;
        GhostEatScore = defaultGhostEatScore;
        SuperPelletScore = defaultSuperPelletScore;
    }

    public float PacmanSpeedMultiplier
    {
        get
        {
            return PlayerPrefs.GetFloat("PacmanSpeedMultiplier", defaultPacmanSpeedMultiplier);
        }
        set
        {
            PlayerPrefs.SetFloat("PacmanSpeedMultiplier", value);
        }
    }

    public float GhostSpeedMultiplier
    {
        get
        {
            return PlayerPrefs.GetFloat("GhostSpeedMultiplier", defaultGhostSpeedMultiplier);
        }
        set
        {
            PlayerPrefs.SetFloat("GhostSpeedMultiplier", value);
        }
    }

    public float FruitAmountMultiplier
    {
        get
        {
            return PlayerPrefs.GetFloat("FruitAmountMultiplier", defaultFruitAmountMultiplier);
        }
        set
        {
            PlayerPrefs.SetFloat("FruitAmountMultiplier", value);
        }
    }

    public float GhostFrightenedSpeedMultiplier
    {
        get
        {
            return PlayerPrefs.GetFloat("GhostFrightenedSpeedMultiplier", defaultGhostFrightenedSpeedMultiplier);
        }
        set
        {
            PlayerPrefs.SetFloat("GhostFrightenedSpeedMultiplier", value);
        }
    }

    public float GhostConsumedSpeedMultiplier
    {
        get
        {
            return PlayerPrefs.GetFloat("GhostConsumedSpeedMultiplier", defaultGhostConsumedSpeedMultiplier);
        }
        set
        {
            PlayerPrefs.SetFloat("GhostConsumedSpeedMultiplier", value);
        }
    }

    public int GhostFrightenedDuration
    {
        get
        {
            return PlayerPrefs.GetInt("GhostFrightenedTime", defaultGhostFrightenedDuration);
        }
        set
        {
            PlayerPrefs.SetInt("GhostFrightenedTime", value);
        }
    }

    public int PelletScore
    {
        get
        {
            return PlayerPrefs.GetInt("PelletScore", defaultPelletScore);
        }
        set
        {
            PlayerPrefs.SetInt("PelletScore", value);
        }
    }

    public int FruitScore
    {
        get
        {
            return PlayerPrefs.GetInt("FruitScore", defaultFruitScore);
        }
        set
        {
            PlayerPrefs.SetInt("FruitScore", value);
        }
    }

    public int GhostEatScore
    {
        get
        {
            return PlayerPrefs.GetInt("GhostEatScore", defaultGhostEatScore);
        }
        set
        {
            PlayerPrefs.SetInt("GhostEatScore", value);
        }
    }

    public int SuperPelletScore
    {
        get
        {
            return PlayerPrefs.GetInt("SuperPelletScore", defaultSuperPelletScore);
        }
        set
        {
            PlayerPrefs.SetInt("SuperPelletScore", value);
        }
    }

}
