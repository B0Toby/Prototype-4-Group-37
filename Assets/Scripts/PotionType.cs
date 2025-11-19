using UnityEngine;

[CreateAssetMenu(fileName = "PotionType", menuName = "Scriptable Objects/PotionType")]
public class PotionType : ScriptableObject
{
    public string potionName;
    public GameObject potionPrefab;

    public int turns = 3;

    public int minRange = 1;
    public int maxRange = 3;

    // can add stuff about potion effect here if necessary ?

}
