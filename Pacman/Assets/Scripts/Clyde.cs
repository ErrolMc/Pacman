using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The orange ghost
/// </summary>
public class Clyde : Ghost
{
    /// <summary>
    /// Gets the target tile for clyde:
    /// 1. Calculate distance from Pacman
    /// 2. If distance is greater than eight tiles, targeting is same as blinky (target pacman)
    /// 3. If distance is less than eight, target his home node
    /// </summary>
    /// <returns></returns>
    protected override Vector2 GetTargetTile()
    {
        if (currentState == State.chase)
        {
            Vector2 pacmanPos = GameLogic.instance.Pacman.CurrentPosition;

            float distance = Vector2.Distance(CurrentPosition, pacmanPos);

            if (distance > 8)
                return pacmanPos;

            return homeNode.pos;
        }

        return base.GetTargetTile();
    }
}
