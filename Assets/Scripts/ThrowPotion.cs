using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Collections.Generic;

// use for inventory later but for now i made them infinite and just all available potions in the list
[System.Serializable]
public class PotionStack
{
    public PotionType type;
    public int count = 3;
}

public class ThrowPotion : MonoBehaviour
{
    [Header("References")]
    public Grid grid;
    public Tilemap wallTilemap;
    public Transform playerOrgin;
    public GameObject highlightPrefab;

    // add additional potions in the editor
    [Header("Potions")]
    public PotionType[] potions;
    private int selectedIndex = 0;

    public int SelectedIndex => selectedIndex;

    [Header("Throw Settings")]
    // made range potion dependent for now
    // could make a consistent thing instead
    public float travelTime = 0.5f;

    [Header("Audio")]
    public AudioClip throwSFX;
    public AudioSource audioSource;

    PotionType selectedPotion;
    private Vector3Int facingDirection = Vector3Int.up;
    private int currentRange = 1;
    private List<GameObject> activeHighlights = new List<GameObject>();

    void Start()
    {
        selectedPotion = potions[selectedIndex];
    }

    void Update()
    {
        UpdateFacing();
        HandlePotionSelection();
        HandleRangeSelection();
        HandleThrow();
    }

    void UpdateFacing()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            facingDirection = Vector3Int.up;
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            facingDirection = Vector3Int.down;
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            facingDirection = Vector3Int.left;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            facingDirection = Vector3Int.right;
    }

    // select potion type by pressing number 1 - 9
    void HandlePotionSelection()
    {
        for (int i = 0; i < potions.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                selectedIndex = i;
                selectedPotion = potions[selectedIndex];
            }
        }
    }

    // increase range with E, reduce range with Q
    void HandleRangeSelection()
    {
        if (Input.GetKeyDown(KeyCode.E))
            currentRange = Mathf.Clamp(currentRange + 1, selectedPotion.minRange, selectedPotion.maxRange);
        if (Input.GetKeyDown(KeyCode.Q))
            currentRange = Mathf.Clamp(currentRange - 1, selectedPotion.minRange, selectedPotion.maxRange);

        ShowRangeHighlights();
    }

    void ShowRangeHighlights()
    {
        foreach (var h in activeHighlights)
            if (h != null) Destroy(h);
        activeHighlights.Clear();

        if (highlightPrefab == null) return;

        Vector3Int originCell = grid.WorldToCell(playerOrgin.position);

        for (int i = 1; i <= currentRange; i++)
        {
            Vector3Int targetCell = originCell + facingDirection * i;
            if (wallTilemap != null && wallTilemap.HasTile(targetCell))
                break;

            Vector3 pos = grid.GetCellCenterWorld(targetCell);
            GameObject h = Instantiate(highlightPrefab, pos, Quaternion.identity);
            activeHighlights.Add(h);
        }
    }

    // SPACE to throw potion to currently displayed range (might change this control cause its a little weird)
    void HandleThrow()
    {
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        if (throwSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(throwSFX);
        }

        Vector3Int originCell = grid.WorldToCell(playerOrgin.position);

        Vector3Int targetCell = originCell;
        for (int i = 1; i <= currentRange; i++)
        {
            Vector3Int nextCell = originCell + facingDirection * i;
            if (wallTilemap != null && wallTilemap.HasTile(nextCell))
                break;
            targetCell = nextCell;
        }

        Vector3 targetWorldPos = grid.GetCellCenterWorld(targetCell);

        // launch potion visibly
        if (selectedPotion.potionPrefab != null)
        {
            GameObject proj = Instantiate(selectedPotion.potionPrefab, playerOrgin.position, Quaternion.identity);
            StartCoroutine(MoveBottle(proj.transform, targetWorldPos));
        }
    }

    IEnumerator MoveBottle(Transform proj, Vector3 target)
    {
        Vector3 start = proj.position;
        float t = 0f;

        // moves it in a little arc
        while (t < 1f)
        {
            t += Time.deltaTime / travelTime;
            float arc = Mathf.Sin(t * Mathf.PI) * 0.3f;
            proj.position = Vector3.Lerp(start, target, t) + new Vector3(0, arc, 0);
            yield return null;
        }

        ApplyPotionEffect(target);
        Destroy(proj.gameObject);
    }

    
    // just logs stuff for now, start effect here vv
    void ApplyPotionEffect(Vector3 targetWorldPos)
    {
        Vector3Int targetCell = grid.WorldToCell(targetWorldPos);
        Vector3 cellCenter = grid.GetCellCenterWorld(targetCell);

        float radius = 0.4f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(cellCenter, radius);

        if (hits.Length == 0)
        {
            Debug.Log($"{selectedPotion.potionName} landed on EMPTY tile at cell {targetCell}");
            return;
        }

        foreach (var hit in hits)
        {
            Debug.Log($"{selectedPotion.potionName} HIT entity: {hit.gameObject.name} at tile {targetCell}");

            NpcController npc = hit.GetComponent<NpcController>();
            if (npc != null)
            {
                npc.ApplyPotion(selectedPotion);
            }
        }
    }
}
