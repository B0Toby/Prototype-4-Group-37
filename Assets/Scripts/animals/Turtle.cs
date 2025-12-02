using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Turtle : BaseAnimal
{
    [Header("Animation")]
    public Animator animator;
    private SpriteRenderer sr;

    private static readonly int HashDoWalkSide = Animator.StringToHash("DoWalkSide");
    private static readonly int HashDoWalkUp   = Animator.StringToHash("DoWalkUp");
    private static readonly int HashDoWalkDown = Animator.StringToHash("DoWalkDown");

    [Header("Bridge Visual")]
    public Sprite bridgeSprite;

    [Header("Movement")]
    public int pacifyMoveInterval = 2;
    private int pacifyMoveCounter = 0;

    private bool isBridge = false;

    private void Awake()
    {
        animalName   = "Turtle";
        behaviorNote = "Moves toward player\nCreates a bridge on water";

        if (animator == null)
            animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();

        if (grid == null)
            grid = FindFirstObjectByType<Grid>();

        if (waterTilemap == null)
            waterTilemap = GameObject.Find("Tilemap_Water")?.GetComponent<Tilemap>();
    }

    public override Vector3Int RageStep(NpcController npc)
    {
        return npc.GetCellPosition();
    }

    public override void OnPlayerStep()
    {
        if (GameManager.I == null)
            return;

        if (isBridge)
            return;

        pacifyMoveCounter++;
        if (pacifyMoveCounter < pacifyMoveInterval)
            return;

        pacifyMoveCounter = 0;

        Vector3Int playerCell = GameManager.I.currentPlayerCell;
        Vector3Int step       = GetStepToward(cellPos, playerCell);

        if (step == Vector3Int.zero)
            return;

        Vector3Int nextCell = cellPos + step;

        if (IsWall(nextCell) || IsObstacle(nextCell))
            return;

        if (IsWater(nextCell))
        {
            BecomeBridge(nextCell);
            return;
        }

        cellPos = nextCell;
        transform.position = grid.GetCellCenterWorld(cellPos);

        PlayWalk(step);
    }

    private void BecomeBridge(Vector3Int waterCell)
    {
        if (waterTilemap != null)
        {
            waterTilemap.SetTile(waterCell, null);
        }

        cellPos = waterCell;
        if (grid != null)
            transform.position = grid.GetCellCenterWorld(cellPos);

        isBridge = true;

        if (animator != null)
            animator.enabled = false;

        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        if (sr != null && bridgeSprite != null)
            sr.sprite = bridgeSprite;
    }

    private void PlayWalk(Vector3Int dir)
    {
        if (animator == null || isBridge) 
            return;

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

    protected override bool CanTraverseWater => false;
}
