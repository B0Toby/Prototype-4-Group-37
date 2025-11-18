using UnityEngine;
using UnityEngine.Tilemaps;

public class NpcController : MonoBehaviour
{
    [Header("Grid reference")]
    public Grid grid;

    [Header("Tilemaps")]
    public Tilemap wallTilemap;

    [Header("Patrol between two cells")]
    public Vector3Int patrolCellA;
    public Vector3Int patrolCellB;
    private Vector3Int currentTarget;

    private Vector3Int cellPos;

    private void Start()
    {
        if (grid == null)
            grid = FindFirstObjectByType<Grid>();

        cellPos = patrolCellA;
        transform.position = grid.GetCellCenterWorld(cellPos);

        currentTarget = patrolCellB;
    }

    public void OnPlayerStep()
    {
        Vector3Int step = GetStepToward(currentTarget);
        Vector3Int nextCell = cellPos + step;

        if (IsBlocked(nextCell))
            return;

        cellPos = nextCell;
        transform.position = grid.GetCellCenterWorld(cellPos);

        if (cellPos == currentTarget)
        {
            currentTarget = (currentTarget == patrolCellA) ? patrolCellB : patrolCellA;
        }
    }

    private Vector3Int GetStepToward(Vector3Int target)
    {
        Vector3Int diff = target - cellPos;

        if (diff.x > 0) return Vector3Int.right;
        if (diff.x < 0) return Vector3Int.left;

        if (diff.y > 0) return Vector3Int.up;
        if (diff.y < 0) return Vector3Int.down;

        return Vector3Int.zero;
    }

    private bool IsBlocked(Vector3Int cell)
    {
        if (wallTilemap == null) return false;
        return wallTilemap.HasTile(cell);
    }

    public Vector3Int GetCellPosition()
    {
        return cellPos;
    }
}
