using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;
public class Parrot : BaseAnimal
{
    private void Awake()
    {
        animalName = "Parrot";
        behaviorNote = "Mimics player movement";

    }

    public override Vector3Int RageStep(NpcController npc)
    {
        return npc.GetCellPosition();
    }

    public override void OnPlayerStep()
    {
        if (GameManager.I == null)
            return;

        Vector3Int dir = GameManager.I.lastPlayerMoveDir;

        if (dir == Vector3Int.zero)
            return;

        Vector3Int nextCell = cellPos + dir;

        if (IsWall(nextCell) || IsObstacle(nextCell))
        {
            return;
        }

        cellPos = nextCell;
        transform.position = grid.GetCellCenterWorld(cellPos);
    }
}
