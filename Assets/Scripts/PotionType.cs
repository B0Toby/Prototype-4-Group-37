using UnityEngine;

[CreateAssetMenu(fileName = "PotionType", menuName = "Scriptable Objects/PotionType")]
public class PotionType : ScriptableObject
{
    public string potionName;
    public GameObject potionPrefab;
    public GameObject animalPrefab;

    public int rageTurns = 3;

    public int minRange = 1;
    public int maxRange = 3;
}
