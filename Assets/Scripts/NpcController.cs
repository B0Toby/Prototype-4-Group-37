using UnityEngine;
using UnityEngine.Tilemaps;

public class NpcController : MonoBehaviour
{
    [Header("Grid reference")]
    public Grid grid;

    [Header("Tilemaps")]
    public Tilemap wallTilemap;       // permanent walls
    public Tilemap obstacleTilemap;   // boxes (Tilemap_Obstacles)

    [Header("Pacified visuals")]
    public Sprite pacifiedSprite;     // sprite for animal phase

    [Header("Animal abilities")]
    public bool canBreakObstacles = true; // later you can change per animal

    // state
    private bool infected = false;    // potion hit, counting down
    private bool pacified = false;    // animal phase
    private int turnsLeft = 0;        // turns before becoming pacified

    // movement
    private Vector3Int cellPos;
    private Vector3Int moveDir = Vector3Int.right;

    private SpriteRenderer sr;

    private void Start()
    {
        if (grid == null)
            grid = FindFirstObjectByType<Grid>();

        cellPos = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(cellPos);

        sr = GetComponent<SpriteRenderer>();
    }

    // called once per player step from GameManager
    public void OnPlayerStep()
    {
        // Stage 0: not infected, not pacified -> idle
        if (!infected && !pacified)
            return;

        // Stage 1: infected, counting down
        if (infected && !pacified)
        {
            turnsLeft--;

            if (turnsLeft <= 0)
            {
                EnterPacifiedState();
            }

            return;
        }

        // Stage 2: pacified animal movement
        if (pacified)
        {
            MoveAsAnimal();
        }
    }

    // called by ThrowPotion when potion hits this NPC
    public void ApplyPotion(int turns)
    {
        if (pacified) return; // already animal

        infected = true;
        turnsLeft = turns;
    }

    private void EnterPacifiedState()
    {
        infected = false;
        pacified = true;

        if (sr != null && pacifiedSprite != null)
        {
            sr.sprite = pacifiedSprite;
        }

        moveDir = Vector3Int.right;
    }

    private void MoveAsAnimal()
    {
        // try current direction
        Vector3Int tryDir = moveDir;
        Vector3Int nextCell = cellPos + tryDir;

        // check walls first
        if (IsWall(nextCell))
        {
            // flip direction and try opposite in same turn
            tryDir = -moveDir;
            nextCell = cellPos + tryDir;

            // both sides blocked by walls -> stay
            if (IsWall(nextCell))
            {
                return;
            }

            moveDir = tryDir;
        }

        // check obstacles (boxes)
        if (IsObstacle(nextCell))
        {
            if (canBreakObstacles)
            {
                // destroy the box and move through
                obstacleTilemap.SetTile(nextCell, null);
            }
            else
            {
                // cannot pass obstacles
                return;
            }
        }

        // move one cell
        cellPos = nextCell;
        transform.position = grid.GetCellCenterWorld(cellPos);
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
