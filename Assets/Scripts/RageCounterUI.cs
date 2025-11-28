using UnityEngine;
using TMPro;

public class RageCounterUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    private RectTransform rec;
    private Camera cam;

    private void Awake()
    {
        rec = GetComponent<RectTransform>();
        cam = Camera.main;
    }


    public void SetValue(int value)
    {
        text.text = value.ToString();
    }

    public void UpdatePosition(Vector3 worldPos)
    {
        rec.position = cam.WorldToScreenPoint(worldPos);
    }

}
