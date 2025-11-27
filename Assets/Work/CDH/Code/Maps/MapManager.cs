using UnityEngine;

namespace Assets.Work.CDH.Code.Maps
{
    public enum TileType
    {
        Default,
        Boss,
        Store,
        // etc...
    }

    public class MapManager : MonoBehaviour
    {
        [SerializeField] private MapDataSO mapDataSO;
    }
}
