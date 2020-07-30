using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The pink ghost
/// </summary>
public class Pinky : Ghost
{
    /// <summary>
    /// The target tile for pinky is 4 tiles in front of pacman
    /// </summary>
    /// <returns>Pinky's target tile</returns>
    protected override Vector2 GetTargetTile()
    {
        if (currentState == State.chase)
        {
            Vector2 pacmanPos = GameLogic.instance.pacman.CurrentPosition;
            Vector2 pacmanDirection = GameLogic.instance.pacman.CurrentDirection;

            Vector2 targetTile = pacmanPos + (4 * pacmanDirection);

            return targetTile;
        }

        // if not chasing, do the base behaivour
        return base.GetTargetTile();
    }
}
