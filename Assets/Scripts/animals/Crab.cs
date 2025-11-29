using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Crab : BaseAnimal
{
    private Vector3Int moveDir = Vector3Int.right;

    private AudioSource audioSource;
    public AudioClip obstacleBreakClip;

    private void Awake()
    {
        // reference the AudioSource already on the prefab
        audioSource = GetComponent<AudioSource>();
        animalName = "Crab";
        behaviorNote = "Moves only left & right\r\nBreaks obstacles in its path";

    }

    // step functions mostly copied over from original npc controller
    public override Vector3Int RageStep(NpcController npc)
    {

        // move 1 step toward player
        Vector3Int npcCell = npc.GetCellPosition();
        Vector3Int playerCell = GameManager.I.currentPlayerCell;

        Vector3Int step = GetStepToward(npcCell, playerCell);

        if (step == Vector3Int.zero)
            return npcCell;

        Vector3Int nextCell = npcCell + step;

        // walls and obstacles both block in rage phase
        if (IsWall(nextCell) || IsObstacle(nextCell))
        {
            return npcCell;
        }

        return nextCell;
    }

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

                if (audioSource != null && obstacleBreakClip != null)
                {
                    audioSource.PlayOneShot(obstacleBreakClip);
                }
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
