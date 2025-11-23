using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("Grid reference")]
    public Grid grid;

    [Header("Tilemaps")]
    public Tilemap wallTilemap;
    public Tilemap obstacleTilemap;

    private Vector3Int cellPos;

    private void Start()
    {
        cellPos = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(cellPos);
    }

    private void Update()
    {
        if (GameManager.I != null && GameManager.I.IsGameEnded)
            return;

        Vector3Int dir = ReadInput();
        if (dir == Vector3Int.zero) return;

        Vector3Int targetCell = cellPos + dir;

        if (IsBlocked(targetCell))
        {
            return;
        }

        cellPos = targetCell;
        transform.position = grid.GetCellCenterWorld(cellPos);

        if (GameManager.I != null)
        {
            GameManager.I.OnPlayerMoved(cellPos);
        }
    }

    private Vector3Int ReadInput()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            return Vector3Int.up;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            return Vector3Int.down;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            return Vector3Int.left;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            return Vector3Int.right;

        return Vector3Int.zero;
    }

    private bool IsBlocked(Vector3Int cell)
    {
        bool wallBlocked = wallTilemap != null && wallTilemap.HasTile(cell);
        bool obstacleBlocked = obstacleTilemap != null && obstacleTilemap.HasTile(cell);
        return wallBlocked || obstacleBlocked;
    }

    public Vector3Int GetCellPosition()
    {
        return cellPos;
    }
}
