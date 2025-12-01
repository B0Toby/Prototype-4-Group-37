using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Parrot : BaseAnimal
{
    [Header("Animation")]
    public Animator animator;

    private SpriteRenderer sr;

    private static readonly int HashDoWalkSide = Animator.StringToHash("DoWalkSide");
    private static readonly int HashDoWalkUp   = Animator.StringToHash("DoWalkUp");
    private static readonly int HashDoWalkDown = Animator.StringToHash("DoWalkDown");

    private void Awake()
    {
        animalName = "Parrot";
        behaviorNote = "Mimics player movement";

        if (animator == null)
            animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();
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

        if (IsWall(nextCell) || IsObstacle(nextCell) || IsWater(nextCell))
        {
            return;
        }

        cellPos = nextCell;
        transform.position = grid.GetCellCenterWorld(cellPos);

        PlayWalk(dir);
    }

    private void PlayWalk(Vector3Int dir)
    {
        if (animator == null) return;

        animator.ResetTrigger(HashDoWalkSide);
        animator.ResetTrigger(HashDoWalkUp);
        animator.ResetTrigger(HashDoWalkDown);

        if (dir.y > 0)
        {
            animator.SetTrigger(HashDoWalkUp);
        }
        else if (dir.y < 0)
        {
            animator.SetTrigger(HashDoWalkDown);
        }
        else
        {
            animator.SetTrigger(HashDoWalkSide);

            if (sr != null)
            {
                if (dir.x > 0) sr.flipX = true;
                else if (dir.x < 0) sr.flipX = false;
            }
        }
    }
}
