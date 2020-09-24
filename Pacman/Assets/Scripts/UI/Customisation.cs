﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public override void OnHide()
    {
        GameSettings.instance.PacmanSpeedMultiplier = GetInputFieldValue(pacmanSpeedMultiplier_IF);
        GameSettings.instance.GhostSpeedMultiplier = GetInputFieldValue(ghostSpeedMultiplier_IF);
        GameSettings.instance.FruitAmountMultiplier = GetInputFieldValue(fruitAmountMultiplier_IF);
        GameSettings.instance.GhostFrightenedDuration = (int)GetInputFieldValue(ghostFrightenedDuration_IF);
        GameSettings.instance.GhostFrightenedSpeedMultiplier = GetInputFieldValue(ghostFrightenedSpeedMultiplier_IF);
        GameSettings.instance.PelletScore = (int)GetInputFieldValue(pelletScore_IF);
        GameSettings.instance.FruitScore = (int)GetInputFieldValue(fruitScore_IF);
        GameSettings.instance.GhostEatScore = (int)GetInputFieldValue(ghostEatScore_IF);
        GameSettings.instance.SuperPelletScore = (int)GetInputFieldValue(superPelletScore_IF);
    }

    void InitInputField(InputField field, float value, float defaultValiue)
    {
        field.text = value.ToString();
        field.placeholder.GetComponent<Text>().text = defaultValiue.ToString();
    }

    float GetInputFieldValue(InputField field)
    {
        if (string.IsNullOrEmpty(field.text))
            return float.Parse(field.placeholder.GetComponent<Text>().text);
        return float.Parse(field.text);
    }

    public void OnClick_Reset()
    {
        GameSettings.instance.ResetToDefaults();
        OnShow();
    }
}
