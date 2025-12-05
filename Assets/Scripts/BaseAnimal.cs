using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public abstract class BaseAnimal : MonoBehaviour, TooltipInterface
{
    protected Grid grid;
    protected Tilemap wallTilemap;
    protected Tilemap obstacleTilemap;
    protected Tilemap waterTilemap;
    protected Vector3Int cellPos;

    protected string animalName;
    protected string behaviorNote;
    public bool bounce = false;

    private bool initializedFromNpc = false;

    protected void Start()
    {
        if (!initializedFromNpc)
        {
            if (grid == null)
                grid = FindFirstObjectByType<Grid>();

            if (wallTilemap == null || obstacleTilemap == null || waterTilemap == null)
            {
                var npc = FindFirstObjectByType<NpcController>();
                if (npc != null)
                {
                    if (wallTilemap == null)      wallTilemap      = npc.wallTilemap;
                    if (obstacleTilemap == null)  obstacleTilemap  = npc.obstacleTilemap;
                    if (waterTilemap == null)     waterTilemap     = npc.waterTilemap;
                }
            }

            if (grid != null)
            {
                cellPos = grid.WorldToCell(transform.position);
                transform.position = grid.GetCellCenterWorld(cellPos);
            }
        }

        GameManager.I.RegisterAnimal(this);
    }

    public virtual void InitializeFromNpc(NpcController npc)
    {
        initializedFromNpc = true;

        grid = npc.grid;
        wallTilemap = npc.wallTilemap;
        obstacleTilemap = npc.obstacleTilemap;
        waterTilemap = npc.waterTilemap;

        cellPos = npc.GetCellPosition();
        transform.position = grid.GetCellCenterWorld(cellPos);
    }

    // movement logic when pacified
    public abstract void OnPlayerStep();

    // movement logic when enraged
    public abstract Vector3Int RageStep(NpcController npc);

    // called when another animal "bounces" off this one
    public virtual void OnBounced(Vector3Int fromDir)
    {
        // default: no special reaction
    }

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

    protected BaseAnimal GetAnimalAtCell(Vector3Int c)
    {
        Debug.Log("Checking cell: " + c);

        foreach (var a in GameManager.I.animals)
        {
            if (a != this && a.GetCellPosition() == c)
            { 
                Debug.Log("HIT "+a.animalName + " at " + a.GetCellPosition());
                return a;
            }
        }
        Debug.Log("No animal found at " + c);
        return null;
    }

    protected bool IsWall(Vector3Int c) => wallTilemap != null && wallTilemap.HasTile(c);
    protected bool IsObstacle(Vector3Int c) => obstacleTilemap != null && obstacleTilemap.HasTile(c);
    protected bool IsWater(Vector3Int c) => waterTilemap != null && waterTilemap.HasTile(c);

    // determines if this animal ignores water
    protected virtual bool CanTraverseWater => false;

    public Vector3Int GetCellPosition() => cellPos;

    private void OnDestroy()
    {
        if (GameManager.I != null)
            GameManager.I.UnregisterAnimal(this);
    }

    public string GetTooltipText()
    {
        return $"{animalName}\n<size=80%><i>{behaviorNote}</i></size>";
    }
}
