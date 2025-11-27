using UnityEngine;
using UnityEngine.UI;

public class PotionHotbarUI : MonoBehaviour
{
    [Header("References")]
    public ThrowPotion throwPotion;
    public PotionIconUI slotPrefab;
    public Transform slotParent;

    private PotionIconUI[] slots;

    void Start()
    {
        BuildHotbar();
    }

    void Update()
    {
        UpdateSelection();
    }

    // builds UI slots based on potion array
    void BuildHotbar()
    {
        // delete old children
        foreach (Transform child in slotParent)
            Destroy(child.gameObject);

        PotionType[] potions = throwPotion.potions;
        slots = new PotionIconUI[potions.Length];

        for (int i = 0; i < potions.Length; i++)
        {
            PotionIconUI slot = Instantiate(slotPrefab, slotParent);
            slot.SetSprite(potions[i].iconSprite); // auto-assign icon
            slot.SetSelected(false);
            slots[i] = slot;
        }
    }

    // updates which slot is selected
    void UpdateSelection()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetSelected(i == throwPotion.SelectedIndex);
        }
    }
}