﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that manages the logic for the customisation panel
/// </summary>
public class Customisation : Panel
{
    [Header("Input fields")]
    [SerializeField] InputField pacmanSpeedMultiplier_IF;
    [SerializeField] InputField ghostSpeedMultiplier_IF;
    [SerializeField] InputField fruitAmountMultiplier_IF;
    [SerializeField] InputField ghostFrightenedDuration_IF;
    [SerializeField] InputField ghostFrightenedSpeedMultiplier_IF;
    [SerializeField] InputField ghostConsumedSpeedMultiplier_IF;
    [SerializeField] InputField pelletScore_IF;
    [SerializeField] InputField fruitScore_IF;
    [SerializeField] InputField ghostEatScore_IF;
    [SerializeField] InputField superPelletScore_IF;

    /// <summary>
    /// Called when the panel first opens
    /// Initialises the input fields with the numbers that are stored in GameSettings
    /// </summary>
    public override void OnShow()
    {
        InitInputField(pacmanSpeedMultiplier_IF, GameSettings.instance.PacmanSpeedMultiplier, GameSettings.instance.defaultPacmanSpeedMultiplier);
        InitInputField(ghostSpeedMultiplier_IF, GameSettings.instance.GhostSpeedMultiplier, GameSettings.instance.defaultGhostSpeedMultiplier);
        InitInputField(fruitAmountMultiplier_IF, GameSettings.instance.FruitAmountMultiplier, GameSettings.instance.defaultFruitAmountMultiplier);
        InitInputField(ghostFrightenedDuration_IF, GameSettings.instance.GhostFrightenedDuration, GameSettings.instance.defaultGhostFrightenedDuration);
        InitInputField(ghostFrightenedSpeedMultiplier_IF, GameSettings.instance.GhostFrightenedSpeedMultiplier, GameSettings.instance.defaultGhostFrightenedSpeedMultiplier);
        InitInputField(ghostConsumedSpeedMultiplier_IF, GameSettings.instance.GhostConsumedSpeedMultiplier, GameSettings.instance.defaultGhostConsumedSpeedMultiplier);
        InitInputField(pelletScore_IF, GameSettings.instance.PelletScore, GameSettings.instance.defaultPelletScore);
        InitInputField(fruitScore_IF, GameSettings.instance.FruitScore, GameSettings.instance.defaultFruitScore);
        InitInputField(ghostEatScore_IF, GameSettings.instance.GhostEatScore, GameSettings.instance.defaultGhostEatScore);
        InitInputField(superPelletScore_IF, GameSettings.instance.SuperPelletScore, GameSettings.instance.defaultSuperPelletScore);
    }

    /// <summary>
    /// Event callback for when the user has finished editing any of the input fields
    /// Gets the input field identifier and updates the value in GameSettings
    /// </summary>
    /// <param name="field">The setting identifier</param>
    public void OnEndEditField(string field)
    {
        switch (field)
        {
            case "PacmanSpeedMultiplier":
                GameSettings.instance.PacmanSpeedMultiplier = GetInputFieldValue(pacmanSpeedMultiplier_IF);
                break;
            case "GhostSpeedMultiplier":
                GameSettings.instance.GhostSpeedMultiplier = GetInputFieldValue(ghostSpeedMultiplier_IF);
                break;
            case "FruitAmountMultiplier":
                GameSettings.instance.FruitAmountMultiplier = GetInputFieldValue(fruitAmountMultiplier_IF);
                break;
            case "GhostFrightenedSpeedMultiplier":
                GameSettings.instance.GhostFrightenedSpeedMultiplier = GetInputFieldValue(ghostFrightenedSpeedMultiplier_IF);
                break;
            case "GhostConsumedSpeedMultiplier":
                GameSettings.instance.GhostConsumedSpeedMultiplier = GetInputFieldValue(ghostConsumedSpeedMultiplier_IF);
                break;
            case "GhostFrightenedDuration":
                GameSettings.instance.GhostFrightenedDuration = (int)GetInputFieldValue(ghostFrightenedDuration_IF);
                break;
            case "PelletScore":
                GameSettings.instance.PelletScore = (int)GetInputFieldValue(pelletScore_IF);
                break;
            case "FruitScore":
                GameSettings.instance.FruitScore = (int)GetInputFieldValue(fruitScore_IF);
                break;
            case "GhostEatScore":
                GameSettings.instance.GhostEatScore = (int)GetInputFieldValue(ghostEatScore_IF);
                break;
            case "SuperPelletScore":
                GameSettings.instance.SuperPelletScore = (int)GetInputFieldValue(superPelletScore_IF);
                break;
        }
    }

    /// <summary>
    /// Initialises an input field by setting the current and default values
    /// </summary>
    /// <param name="field">The input field to initialise</param>
    /// <param name="value">The current value of the input field to set</param>
    /// <param name="defaultValiue">The default value of the input field to set</param>
    void InitInputField(InputField field, float value, float defaultValiue)
    {
        field.text = value.ToString();
        field.placeholder.GetComponent<Text>().text = defaultValiue.ToString();
    }

    /// <summary>
    /// Gets the value from an input field as a float
    /// </summary>
    /// <param name="field">The inputfield to get the value from</param>
    /// <returns>The value assigned to that input field</returns>
    float GetInputFieldValue(InputField field)
    {
        if (string.IsNullOrEmpty(field.text))
            return float.Parse(field.placeholder.GetComponent<Text>().text);
        return float.Parse(field.text);
    }

    /// <summary>
    /// Resets all the settings back to their defaults
    /// </summary>
    public void OnClick_Reset()
    {
        GameSettings.instance.ResetToDefaults();
        OnShow();
    }
}
