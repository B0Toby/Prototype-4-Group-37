using UnityEngine;
using UnityEngine.UI;

public class PotionIconUI : MonoBehaviour
{
    public Image background;
    public Image iconSprite;

    public Sprite selectedBackground;
    public Sprite unselectedBackground;

    private Sprite potionIcon;
    private bool isSelected;

    public void RefreshUI()
    {
        // show the background
        if (background != null)
        {
            background.enabled = true;
            background.sprite = isSelected ? selectedBackground : unselectedBackground;
        }

        // update potion icon
        if (iconSprite != null)
        {
            iconSprite.sprite = potionIcon;
            iconSprite.enabled = potionIcon != null;
            iconSprite.preserveAspect = true;
        }
    }

    public void SetSprite(Sprite s)
    {
        potionIcon = s;
        RefreshUI();
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;
        RefreshUI();
    }
}