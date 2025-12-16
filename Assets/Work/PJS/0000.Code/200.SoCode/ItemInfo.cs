using UnityEngine;

[CreateAssetMenu(fileName = "ItemInfo", menuName = "SO/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    public int Cost;
    public string Name;
    public string Description;
}
