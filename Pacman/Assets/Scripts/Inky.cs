using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The blue ghost
/// </summary>
public class Inky : Ghost
{

    /// <summary>
    /// Gets the target tile for inky:
    /// 1. Select the position two tiles in front of pacman
    /// 2. Draw vector from blinky to that position
    /// 3. Double the length of that vector
    /// </summary>
    /// <returns>Inky's target tile</returns>
    protected override Vector2 GetTargetTile()
    {
        if (currentState == State.chase)
        {
            Vector2 pacmanPos = GameLogic.instance.Pacman.CurrentPosition;
            Vector2 pacmanDirection = GameLogic.instance.Pacman.CurrentDirection;

            Vector2 targetTile = pacmanPos + (2 * pacmanDirection);

            Ghost blinky = GameLogic.instance.GetGhost(Type.blinky);
            if (blinky == null)
            {
                Debug.LogError("Blinky is null");
                return base.GetTargetTile();
            }

            Vector2 dirVec = targetTile - blinky.CurrentPosition;

            return dirVec * 2;
        }

        return base.GetTargetTile();
    }
}
