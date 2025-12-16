using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfoSO", menuName = "SO/ItemInfoSO")]
public class ItemInfoSO : ScriptableObject
{
    public int Cost;
    public string Name;
    public string Description;
}
