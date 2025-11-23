using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Crab : BaseAnimal
{
    private Vector3Int moveDir = Vector3Int.right;

    // mostly copied over from original npc controller
    public override void OnPlayerStep()
    {
        Vector3Int nextCell = cellPos + moveDir;
        // walls: bounce immediately in this turn
        if (IsWall(nextCell))
        {
            moveDir = -moveDir;
            nextCell = cellPos + moveDir;

            if (IsWall(nextCell))
            {
                return; // blocked on both sides
            }
        }

        // obstacles: break if allowed
        if (IsObstacle(nextCell))
        {
            if (obstacleTilemap != null)
            {
                obstacleTilemap.SetTile(nextCell, null);
            }
            else
            {
                return;
            }
        }

        cellPos = nextCell;
        transform.position = grid.GetCellCenterWorld(cellPos);
    }
}
