using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class NpcController : MonoBehaviour
{
    public enum NpcState
    {
        Idle,
        Rage,
        Pacify      // transition state, initiate spawn animal
    }

    [Header("Grid reference")]
    public Grid grid;

    [Header("Tilemaps")]
    public Tilemap wallTilemap;
    public Tilemap obstacleTilemap;
    public Tilemap waterTilemap;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite rageSprite;
    //public Sprite pacifySprite;

    [Header("Rage UI")]
    public Canvas uiCanvas;
    public GameObject rageTurnCounterPrefab;
    private RageCounterUI rageCounterUI;

    //audio for transformation
    [Header("Audio")]
    public AudioClip rageLoopClip;

    [Header("Animation")]
    public Animator animator;

    private static readonly int HashWalkSide = Animator.StringToHash("DoWalkSide");
    private static readonly int HashWalkUp   = Animator.StringToHash("DoWalkUp");
    private static readonly int HashWalkDown = Animator.StringToHash("DoWalkDown");

    // current state
    public NpcState state = NpcState.Idle;

    // rage state data
    private PotionType hitPotion;           // specified when hit with potion
    private GameObject animalPrefab;        // specified when hit with potion
    private BaseAnimal animalData;
    private int rageTurnsLeft = 0;
    private int rageMoveCounter = 0;
    private int rageMoveInterval = 0;       // move once every X player turns  

    // position & movement
    private Vector3Int cellPos;
    private Vector3Int pacifyMoveDir = Vector3Int.right;

    private SpriteRenderer sr;
    private AudioSource audioSource;

    private void Start()
    {
        if (grid == null)
            grid = FindFirstObjectByType<Grid>();

        cellPos = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(cellPos);

        sr = GetComponent<SpriteRenderer>();
        if (sr != null && idleSprite != null)
        {
            sr.sprite = idleSprite;
        }

        if (animator == null)
            animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();

        GameManager.I.RegisterNpc(this);
    }

    // called once per player step from GameManager
    public void OnPlayerStep()
    {
        if (GameManager.I != null && GameManager.I.IsGameEnded)
            return;

        switch (state)
        {
            case NpcState.Idle:
                break;

            case NpcState.Rage:
                HandleRageStep();
                break;

            case NpcState.Pacify:
                TransformIntoAnimal();
                break;
        }
    }

    // called by ThrowPotion when hit
    public void ApplyPotion(PotionType potion)
    {
        // set new properties
        hitPotion = potion;
        animalPrefab = potion.animalPrefab;
        animalData = animalPrefab.GetComponent<BaseAnimal>();

        if (state == NpcState.Pacify)
            return;

        state = NpcState.Rage;
        rageTurnsLeft = hitPotion.rageTurns;
        rageMoveInterval = hitPotion.moveInterval;
        rageMoveCounter = 0;

        // set sprite to rage sprite
        if (sr != null && rageSprite != null)
        {
            sr.sprite = rageSprite;
        }

        // have a rage counter appear
        if (rageTurnCounterPrefab != null)
        {
            GameObject rageUI = Instantiate(rageTurnCounterPrefab, uiCanvas.transform);
            rageCounterUI = rageUI.GetComponent<RageCounterUI>();
            rageCounterUI.SetValue(rageTurnsLeft);
            rageCounterUI.UpdatePosition(transform.position);
        }

        // potion bubble audio (plays during rage only)
        if (audioSource != null && rageLoopClip != null)
        {
            audioSource.clip = rageLoopClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void HandleRageStep()
    {
        // rage lasts a limited number of player turns
        rageTurnsLeft--;
        rageCounterUI.SetValue(rageTurnsLeft);
        if (rageTurnsLeft <= 0)
        {
            state = NpcState.Pacify;
      
            if (audioSource != null)
                audioSource.Stop();

            return;
        }

        // only move every rageMoveInterval turns
        rageMoveCounter++;
        if (rageMoveCounter < rageMoveInterval)
        {
            return;
        }
        rageMoveCounter = 0;

        // npc movement during rage state is based on animal properties
        if (animalData != null)
        {
            Vector3Int nextCell = animalData.RageStep(this);

            Vector3Int moveDir = nextCell - cellPos;

            cellPos = nextCell;
            transform.position = grid.GetCellCenterWorld(cellPos);

            PlayWalk(moveDir);
        }

        // make sure counter UI stays with the npc
        if (rageCounterUI != null)
        {
            rageCounterUI.UpdatePosition(transform.position);
        }

        if (GameManager.I == null)
            return;

        // if we step onto the player in rage state, player loses
        if (cellPos == GameManager.I.currentPlayerCell)
        {
            GameManager.I.TriggerLose();
        }
    }

    // instantiate animal prefab and destroy npc
    private void TransformIntoAnimal()
    {
        GameObject animalObj = Instantiate(animalPrefab, transform.position, Quaternion.identity);
        BaseAnimal animal = animalObj.GetComponent<BaseAnimal>();

        if (animal != null)
            animal.InitializeFromNpc(this);

        // remove UI when rage ends
        Destroy(rageCounterUI.gameObject);
        rageCounterUI = null;

        // remove NPC when rage ends
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (GameManager.I != null)
            GameManager.I.UnregisterNpc(this);
    }

    // left in here in case we want to use it later
    private Vector3Int GetStepToward(Vector3Int target)
    {
        Vector3Int diff = target - cellPos;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            if (diff.x > 0) return Vector3Int.right;
            if (diff.x < 0) return Vector3Int.left;
        }
        else
        {
            if (diff.y > 0) return Vector3Int.up;
            if (diff.y < 0) return Vector3Int.down;
        }

        return Vector3Int.zero;
    }

    private bool IsWall(Vector3Int cell)
    {
        return wallTilemap != null && wallTilemap.HasTile(cell);
    }

    private bool IsObstacle(Vector3Int cell)
    {
        return obstacleTilemap != null && obstacleTilemap.HasTile(cell);
    }

    public Vector3Int GetCellPosition()
    {
        return cellPos;
    }

    private void PlayWalk(Vector3Int dir)
    {
        if (animator == null) return;
        if (dir == Vector3Int.zero) return;

        animator.ResetTrigger(HashWalkSide);
        animator.ResetTrigger(HashWalkUp);
        animator.ResetTrigger(HashWalkDown);

        if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
        {
            if (dir.y > 0)
            {
                animator.SetTrigger(HashWalkUp);
            }
            else if (dir.y < 0)
            {
                animator.SetTrigger(HashWalkDown);
            }
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
}
