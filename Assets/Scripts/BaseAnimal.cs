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


    // initialize at
    public virtual void InitializeFromNpc(NpcController npc)
    {
        grid = npc.grid;
        wallTilemap = npc.wallTilemap;
        obstacleTilemap = npc.obstacleTilemap;

        cellPos = npc.GetCellPosition();
        transform.position = grid.GetCellCenterWorld(cellPos);
    }
    
    // define movement pattern and unique behaviors in specific animal files
    public abstract void OnPlayerStep();

    protected bool IsWall(Vector3Int c) => wallTilemap != null && wallTilemap.HasTile(c);
    protected bool IsObstacle(Vector3Int c) => obstacleTilemap != null && obstacleTilemap.HasTile(c);
    public Vector3Int GetCellPosition() => cellPos;
    // add pits if we are gonna make the snake

    private void OnDestroy()
    {
        if (GameManager.I != null)
            GameManager.I.UnregisterAnimal(this);
    }

}
