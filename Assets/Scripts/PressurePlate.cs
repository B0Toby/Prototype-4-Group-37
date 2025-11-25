using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour
{
    [Header("Grid / Tilemaps")]
    public Grid grid;
    public Tilemap plateTilemap;
    public Tilemap wallTilemap;

    [Header("Plate Tiles")]
    public TileBase plateIdleTile;
    public TileBase platePressedTile;

    [Header("Door Tile")]
    public TileBase doorClosedTile;

    [Header("Cells (grid coordinates)")]
    public Vector3Int plateCell;
    public Vector3Int doorCell;

    [Header("Detection")]
    public LayerMask characterMask;

    private bool isPressed = false;

    private void Start()
    {
        if (grid == null)
            grid = FindFirstObjectByType<Grid>();

        if (plateCell == Vector3Int.zero)
        {
            plateCell = grid.WorldToCell(transform.position);
        }

        UpdatePlateVisual(false);
        SetDoorClosed();
    }

    private void Update()
    {
        if (GameManager.I != null && GameManager.I.IsGameEnded)
            return;

        bool pressedNow = CheckPressed();

        if (pressedNow == isPressed)
            return;

        isPressed = pressedNow;

        UpdatePlateVisual(isPressed);

        if (isPressed)
            SetDoorOpen();
        else
            SetDoorClosed();
    }

    bool CheckPressed()
    {
        Vector3 center = grid.GetCellCenterWorld(plateCell);
        Vector3 size = grid.cellSize * 0.5f;

        Collider2D hit = Physics2D.OverlapBox(center, size, 0f, characterMask);
        return hit != null;
    }

    void UpdatePlateVisual(bool pressed)
    {
        if (plateTilemap == null) return;

        TileBase tile = pressed ? platePressedTile : plateIdleTile;
        plateTilemap.SetTile(plateCell, tile);
    }

    void SetDoorClosed()
    {
        if (wallTilemap == null || doorClosedTile == null) return;

        wallTilemap.SetTile(doorCell, doorClosedTile);
    }

    void SetDoorOpen()
    {
        if (wallTilemap == null) return;

        wallTilemap.SetTile(doorCell, null);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (grid == null) return;

        Gizmos.color = Color.yellow;
        Vector3 plateCenter = grid.CellToWorld(plateCell) + new Vector3(0.5f, 0.5f, 0);
        Gizmos.DrawWireCube(plateCenter, grid.cellSize * 0.5f);

        Gizmos.color = Color.cyan;
        Vector3 doorCenter = grid.CellToWorld(doorCell) + new Vector3(0.5f, 0.5f, 0);
        Gizmos.DrawWireCube(doorCenter, grid.cellSize * 0.5f);
    }
#endif
}
