using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class Crab : BaseAnimal
{
    private Vector3Int moveDir = Vector3Int.right;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip obstacleBreakClip;

    [Header("Animation")]
    public Animator animator;
    private SpriteRenderer sr;
    private static readonly int HashDoWalk = Animator.StringToHash("DoWalk");

    private void Awake()
    {
        // reference the AudioSource already on the prefab
        audioSource = GetComponent<AudioSource>();

        if (animator == null)
            animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();

        animalName = "Crab";
        behaviorNote = "Moves only left & right\r\nBreaks obstacles in its path";

        waterTilemap = GameObject.Find("Tilemap_Water")?.GetComponent<Tilemap>();
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

        BaseAnimal hitAnimal = GetAnimalAtCell(nextCell);
        if (hitAnimal != null)
        {
            if (hitAnimal.bounce)
            {
                hitAnimal.OnBounced(step);

                Vector3Int bounceCell = npcCell - step;
                if (GetAnimalAtCell(bounceCell) == null)
                    nextCell = bounceCell;
            }
        }

        // walls and obstacles both block in rage phase
        if (npc.IsWall(nextCell) || npc.IsObstacle(nextCell))
        {
            return npcCell;
        }

        // water blocks crab only if it cannot traverse water
        if (npc.IsWater(nextCell) && !CanTraverseWater)
        {
            return npcCell;
        }

        return nextCell;
    }

    public override void OnPlayerStep()
    {
        Vector3Int startCell = cellPos;
        Vector3Int nextCell = cellPos + moveDir;

        // animals: bounce in new direction if pig in nextCell
        BaseAnimal hitAnimal = GetAnimalAtCell(nextCell);
        if (hitAnimal != null)
        {
            if (hitAnimal.bounce)
            {
                hitAnimal.OnBounced(moveDir);

                if (moveDir == Vector3Int.left || moveDir == Vector3Int.right)
                {
                    moveDir = (Random.value < 0.5f) ? Vector3Int.up : Vector3Int.down;
                }
                else
                {
                    moveDir = (Random.value < 0.5f) ? Vector3Int.left : Vector3Int.right;
                }

                Vector3Int bounceCell = cellPos + moveDir;
                if (GetAnimalAtCell(bounceCell) == null)
                    nextCell = bounceCell;
            }
        }

        // walls: bounce immediately in this turn
        if (IsWall(nextCell))
        {
            moveDir = -moveDir;
            nextCell = cellPos + moveDir;

            if (IsWall(nextCell))
                return; // blocked on both sides
        }

        // water: bounce immediately if cannot traverse
        if (IsWater(nextCell) && !CanTraverseWater)
        {
            moveDir = -moveDir;
            nextCell = cellPos + moveDir;

            if (IsWall(nextCell) || IsWater(nextCell))
                return;
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

        if (cellPos != startCell)
        {
            PlayWalkOnce(moveDir);
        }
    }

    // crab can traverse water
    protected override bool CanTraverseWater => true;

    private void PlayWalkOnce(Vector3Int dir)
    {
        if (sr != null)
        {
            if (dir.x > 0) sr.flipX = false;
            else if (dir.x < 0) sr.flipX = true;
        }

        if (animator != null)
        {
            animator.ResetTrigger(HashDoWalk);
            animator.SetTrigger(HashDoWalk);
        }
    }
}
