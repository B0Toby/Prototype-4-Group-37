using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public GameObject tooltipPrefab;
    private RectTransform tooltipRect;
    private TextMeshProUGUI tooltipText;
    private Canvas uiCanvas;

    void Awake()
    {
        Instance = this;
        uiCanvas = FindFirstObjectByType<Canvas>();

        var tt = Instantiate(tooltipPrefab, uiCanvas.transform);
        tooltipRect = tt.GetComponent<RectTransform>();
        tooltipText = tt.GetComponentInChildren<TextMeshProUGUI>();
        tt.SetActive(false);
    }

    public void ShowTooltip(string text, Vector2 screenPos)
    {
        tooltipText.text = text;
        tooltipRect.gameObject.SetActive(true);
        tooltipRect.position = screenPos + new Vector2(150f, -50f);
    }

    public void HideTooltip()
    {
        tooltipRect.gameObject.SetActive(false);
    }
}
