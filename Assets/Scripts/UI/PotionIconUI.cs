using UnityEngine;
using UnityEngine.UI;

public class PotionIconUI : MonoBehaviour, TooltipInterface
{
    public Image background;
    public Image iconSprite;

    public Sprite selectedBackground;
    public Sprite unselectedBackground;

    private Sprite potionIcon;
    private bool isSelected;

    private string potionName;
    private int potionRageTurns;

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

    public void SetPotion(string potion, int turns)
    {
        potionName = potion;
        potionRageTurns = turns;
    }

    public string GetTooltipText()
    {
        return $"{potionName}\n<size=80%><i>Takes {potionRageTurns} turns to complete transformation</i></size>";
    }
}