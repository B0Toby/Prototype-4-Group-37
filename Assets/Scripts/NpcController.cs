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

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite rageSprite;
    //public Sprite pacifySprite;

    // current state
    public NpcState state = NpcState.Idle;

    // rage state data
    private PotionType hitPotion;       // specified when hit with potion
    private GameObject transformAnimal; // specified when hit with potion
    private int rageTurnsLeft = 0;
    private int rageMoveCounter = 0;
    private int rageMoveInterval = 2;   // move once every X player turns  

    // position & movement
    private Vector3Int cellPos;
    private Vector3Int pacifyMoveDir = Vector3Int.right;

    private SpriteRenderer sr;

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
        transformAnimal = potion.animalPrefab;

        if (state == NpcState.Pacify)
            return;

        state = NpcState.Rage;
        rageTurnsLeft = hitPotion.rageTurns;
        rageMoveCounter = 0;

        if (sr != null && rageSprite != null)
        {
            sr.sprite = rageSprite;
        }
    }

    private void HandleRageStep()
    {
        // rage lasts a limited number of player turns
        rageTurnsLeft--;
        if (rageTurnsLeft <= 0)
        {
            state = NpcState.Pacify;
            return;
        }

        // only move every rageMoveInterval turns
        rageMoveCounter++;
        if (rageMoveCounter < rageMoveInterval)
        {
            return;
        }
        rageMoveCounter = 0;

        if (GameManager.I == null)
            return;

        // move 1 step toward player
        Vector3Int playerCell = GameManager.I.currentPlayerCell;
        Vector3Int step = GetStepToward(playerCell);

        if (step == Vector3Int.zero)
            return;

        Vector3Int nextCell = cellPos + step;

        // walls and obstacles both block in rage phase
        if (IsWall(nextCell) || IsObstacle(nextCell))
        {
            return;
        }

        cellPos = nextCell;
        transform.position = grid.GetCellCenterWorld(cellPos);

        // if we step onto the player in rage state, player loses
        if (cellPos == GameManager.I.currentPlayerCell)
        {
            GameManager.I.TriggerLose();
        }
    }

    // instantiate animal prefab and destroy npc
    private void TransformIntoAnimal()
    {
        GameObject animalObj = Instantiate(transformAnimal, transform.position, Quaternion.identity);
        BaseAnimal animal = animalObj.GetComponent<BaseAnimal>();

        if (animal != null)
            animal.InitializeFromNpc(this);

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (GameManager.I != null)
            GameManager.I.UnregisterNpc(this);
    }

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
}
