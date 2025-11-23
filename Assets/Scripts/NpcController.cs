using UnityEngine;
using UnityEngine.Tilemaps;

public class NpcController : MonoBehaviour
{
    public enum NpcState
    {
        Idle,
        Rage,
        Pacify
    }

    [Header("Grid reference")]
    public Grid grid;

    [Header("Tilemaps")]
    public Tilemap wallTilemap;
    public Tilemap obstacleTilemap;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite rageSprite;
    public Sprite pacifySprite;

    [Header("Rage settings")]
    public int rageMoveInterval = 2;  // move once every X player turns

    [Header("Pacify settings")]
    public bool canBreakObstacles = true;

    // current state
    public NpcState state = NpcState.Idle;

    // rage state data
    private int rageTurnsLeft = 0;
    private int rageMoveCounter = 0;

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
                MoveAsPacifiedAnimal();
                break;
        }
    }

    // called by ThrowPotion when hit
    public void ApplyPotion(int turns)
    {
        if (state == NpcState.Pacify)
            return;

        state = NpcState.Rage;
        rageTurnsLeft = turns;
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
            EnterPacifyState();
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

    private void EnterPacifyState()
    {
        state = NpcState.Pacify;

        if (sr != null && pacifySprite != null)
        {
            sr.sprite = pacifySprite;
        }

        pacifyMoveDir = Vector3Int.right;
    }

    private void MoveAsPacifiedAnimal()
    {
        Vector3Int tryDir = pacifyMoveDir;
        Vector3Int nextCell = cellPos + tryDir;

        // walls: bounce immediately in this turn
        if (IsWall(nextCell))
        {
            tryDir = -pacifyMoveDir;
            nextCell = cellPos + tryDir;

            if (IsWall(nextCell))
            {
                return; // blocked on both sides
            }

            pacifyMoveDir = tryDir;
        }

        // obstacles: break if allowed
        if (IsObstacle(nextCell))
        {
            if (canBreakObstacles && obstacleTilemap != null)
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
