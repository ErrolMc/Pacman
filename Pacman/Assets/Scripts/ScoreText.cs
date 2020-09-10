using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// A container class for in game score text objects
/// </summary>
public class ScoreText : MonoBehaviour
{
    [SerializeField] TextMeshPro text;

    /// <summary>
    /// Spawns the score text object at a position with a value
    /// </summary>
    /// <param name="positon">The position of the score text</param>
    /// <param name="score">The score to display</param>
    public void Spawn(Vector2 positon, int score)
    {
        Vector3 curPos = positon;
        curPos.z = -1;

        transform.position = curPos;
        text.text = score.ToString();
    }
}
