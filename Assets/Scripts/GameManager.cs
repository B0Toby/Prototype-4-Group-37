using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("End Cells")]
    public List<Vector3Int> endCells = new List<Vector3Int>();

    [Header("NPCs in this level")]
    public List<NpcController> npcs = new List<NpcController>();

    private bool gameEnded = false;

    private void Awake()
    {
        I = this;
    }

    public void OnPlayerMoved(Vector3Int playerCell)
    {
        if (gameEnded) return;

        foreach (var cell in endCells)
        {
            if (playerCell == cell)
            {
                EndGame();
            }
        }

        foreach (var npc in npcs)
        {
            if (npc != null)
            {
                npc.OnPlayerStep();
            }
        }
    }

    private void EndGame()
    {
        gameEnded = true;
        Debug.Log("GG");
    }
}
