using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem.XR;

public class Pig : BaseAnimal
{
    //[Header("Audio")]
    //private AudioSource audioSource;
    //public AudioClip bounceClip;

    [Header("Animation")]
    public Animator animator;
    private SpriteRenderer sr;

    private static readonly int HashDoWalkSide = Animator.StringToHash("DoWalkSide");
    private static readonly int HashDoWalkUp   = Animator.StringToHash("DoWalkUp");
    private static readonly int HashDoWalkDown = Animator.StringToHash("DoWalkDown");

    private void Awake()
    {
        animalName = "Pig";
        behaviorNote = "Other animals will bounce off of it";
        bounce = true;

        if (animator == null)
            animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();
    }

    // copied from crab, adjust in a minute maybe
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
        if (GameManager.I == null)
            return;

        Vector3Int dir = GameManager.I.lastPlayerMoveDir;

        if (dir == Vector3Int.zero)
            return;

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
