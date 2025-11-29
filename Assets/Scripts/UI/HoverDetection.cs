using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class HoverDetection : MonoBehaviour
{
    private GraphicRaycaster graphicRay;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    void Start()
    {
        Canvas uiCanvas = FindFirstObjectByType<Canvas>();
        graphicRay = uiCanvas.GetComponent<GraphicRaycaster>();

        eventSystem = EventSystem.current;
    }

    void Update()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // ui layer raycastt
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = mousePos;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRay.Raycast(pointerEventData, results);

        foreach (var result in results)
        {
            var tooltipTargetUI = result.gameObject.GetComponent<TooltipInterface>();
            if (tooltipTargetUI != null)
            {
                TooltipManager.Instance.ShowTooltip(
                    tooltipTargetUI.GetTooltipText(),
                    mousePos
                );
                return;
            }
        }

        // world raycast
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);

        if (hit.collider != null)
        {
            var tooltipTarget = hit.collider.GetComponent<TooltipInterface>();
            if (tooltipTarget != null)
            {
                TooltipManager.Instance.ShowTooltip(
                    tooltipTarget.GetTooltipText(),
                    mousePos
                );
                return;
            }
        }

        TooltipManager.Instance.HideTooltip();
    }
}