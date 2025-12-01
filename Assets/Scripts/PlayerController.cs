using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Grid reference")]
    public Grid grid;

    [Header("Tilemaps")]
    public Tilemap wallTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap waterTilemap; // new reference to water

    [Header("Animation")]
    public Animator animator;
    private SpriteRenderer sr;

    private static readonly int HashWalkSide   = Animator.StringToHash("DoWalkSide");
    private static readonly int HashWalkUp     = Animator.StringToHash("DoWalkUp");
    private static readonly int HashWalkDown   = Animator.StringToHash("DoWalkDown");
    private static readonly int HashThrowSide  = Animator.StringToHash("DoThrowSide");
    private static readonly int HashThrowUp    = Animator.StringToHash("DoThrowUp");
    private static readonly int HashThrowDown  = Animator.StringToHash("DoThrowDown");

    private Vector3Int cellPos;

    private void Start()
    {
        cellPos = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(cellPos);

        if (animator == null)
            animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameManager.I != null && GameManager.I.IsGameEnded)
            return;

        Vector3Int dir = ReadInput();
        if (dir == Vector3Int.zero) return;

        Vector3Int targetCell = cellPos + dir;

        if (IsBlocked(targetCell))
        {
            PlayWalk(dir);
            return;
        }

        cellPos = targetCell;
        transform.position = grid.GetCellCenterWorld(cellPos);

        if (GameManager.I != null)
        {
            // store last move direction for animals like the parrot
            GameManager.I.lastPlayerMoveDir = dir;

            GameManager.I.OnPlayerMoved(cellPos);
        }

        PlayWalk(dir);
    }

    private Vector3Int ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            return Vector3Int.up;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            return Vector3Int.down;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            return Vector3Int.left;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            return Vector3Int.right;

        return Vector3Int.zero;
    }

    private bool IsBlocked(Vector3Int cell)
    {
        bool wallBlocked = wallTilemap != null && wallTilemap.HasTile(cell);
        bool obstacleBlocked = obstacleTilemap != null && obstacleTilemap.HasTile(cell);
        bool waterBlocked = waterTilemap != null && waterTilemap.HasTile(cell); // new water check
        return wallBlocked || obstacleBlocked || waterBlocked;
    }

    public Vector3Int GetCellPosition()
    {
        return cellPos;
    }

    private void ResetAllTriggers()
    {
        if (animator == null) return;

        animator.ResetTrigger(HashWalkSide);
        animator.ResetTrigger(HashWalkUp);
        animator.ResetTrigger(HashWalkDown);
        animator.ResetTrigger(HashThrowSide);
        animator.ResetTrigger(HashThrowUp);
        animator.ResetTrigger(HashThrowDown);
    }

    public void PlayWalk(Vector3Int dir)
    {
        if (animator == null) return;

        ResetAllTriggers();

        if (dir.y > 0)
        {
            animator.SetTrigger(HashWalkUp);
        }
        else if (dir.y < 0)
        {
            animator.SetTrigger(HashWalkDown);
        }
        else
        {
            animator.SetTrigger(HashWalkSide);

            if (sr != null)
            {
                if (dir.x > 0) sr.flipX = true;
                else if (dir.x < 0) sr.flipX = false;
            }
        }
    }

    public void PlayThrow(Vector3Int dir)
    {
        if (animator == null) return;

        ResetAllTriggers();

        if (dir.y > 0)
        {
            animator.SetTrigger(HashThrowUp);
        }
        else if (dir.y < 0)
        {
            animator.SetTrigger(HashThrowDown);
        }
        else
        {
            animator.SetTrigger(HashThrowSide);

            if (sr != null)
            {
                if (dir.x > 0) sr.flipX = true;
                else if (dir.x < 0) sr.flipX = false;
            }
        }
    }
}
