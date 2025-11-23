using UnityEngine;
using UnityEngine.Tilemaps;

public abstract class BaseAnimal : MonoBehaviour
{
    protected Grid grid;
    protected Tilemap wallTilemap;
    protected Tilemap obstacleTilemap;
    protected Vector3Int cellPos;

    private void Start()
    {
        GameManager.I.RegisterAnimal(this);
    }


    public virtual void InitializeFromNpc(NpcController npc)
    {
        grid = npc.grid;
        wallTilemap = npc.wallTilemap;
        obstacleTilemap = npc.obstacleTilemap;

        cellPos = npc.GetCellPosition();
        transform.position = grid.GetCellCenterWorld(cellPos);
    }
    
    // movement logic when pacified
    public abstract void OnPlayerStep();

    // movement logic when enraged
    public abstract Vector3Int RageStep(NpcController npc);

    // use if animal should follow player ever
    protected Vector3Int GetStepToward(Vector3Int start, Vector3Int target)
    {
        Vector3Int diff = target - start;

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

    protected bool IsWall(Vector3Int c) => wallTilemap != null && wallTilemap.HasTile(c);
    protected bool IsObstacle(Vector3Int c) => obstacleTilemap != null && obstacleTilemap.HasTile(c);
    // add pits if we are gonna make the snake
    public Vector3Int GetCellPosition() => cellPos;

    private void OnDestroy()
    {
        if (GameManager.I != null)
            GameManager.I.UnregisterAnimal(this);
    }

}
