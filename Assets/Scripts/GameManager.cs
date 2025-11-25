using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager I { get; private set; }

    [Header("End Cells")]
    public List<Vector3Int> endCells = new List<Vector3Int>();

    [Header("Player")]
    public Vector3Int currentPlayerCell;

    [Header("Player movement")]
    public Vector3Int lastPlayerMoveDir;

    [Header("NPCs in this level")]
    public List<NpcController> npcs = new List<NpcController>();

    [Header("Animals in this level")]
    public List<BaseAnimal> animals = new List<BaseAnimal>();

    [Header("End Screen UI")]
    public GameObject endScreenPanel;
    public TextMeshProUGUI endScreenText;

    private bool gameEnded = false;
    public bool IsGameEnded => gameEnded;

    private void Awake()
    {
        I = this;

        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    public void OnPlayerMoved(Vector3Int playerCell)
    {
        if (gameEnded) return;

        // store player position for NPCs (rage stage)
        currentPlayerCell = playerCell;

        // check win condition (player reaches end cell)
        foreach (var cell in endCells)
        {
            if (playerCell == cell)
            {
                TriggerWin();
                return;
            }
        }

        // NPCs take their turns
        foreach (var npc in npcs)
        {
            if (npc != null)
                npc.OnPlayerStep();
        }

        // animals take their turns
        foreach (var animal in animals)
        {
            if (animal != null)
                animal.OnPlayerStep();
        }
    }

    private void EndGame(string message)
    {
        if (gameEnded) return;

        gameEnded = true;
        Debug.Log(message);

        if (endScreenPanel != null)
            endScreenPanel.SetActive(true);

        if (endScreenText != null)
            endScreenText.text = message;
    }

    public void TriggerWin()
    {
        int index = SceneManager.GetActiveScene().buildIndex;
        int lastIndex = SceneManager.sceneCountInBuildSettings - 1;

        if (index == lastIndex)
        {
            EndGame("You Win!");
        }
        else
        {
            SceneManager.LoadScene(index + 1);
        }
    }

    public void TriggerLose()
    {
        EndGame("You Lose!");
    }

    private void RestartLevel()
    {
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    // creature registration for holding them in the lists
    public void RegisterNpc(NpcController npc) => npcs.Add(npc);
    public void UnregisterNpc(NpcController npc) => npcs.Remove(npc);
    public void RegisterAnimal(BaseAnimal a) => animals.Add(a);
    public void UnregisterAnimal(BaseAnimal a) => animals.Remove(a);
}
