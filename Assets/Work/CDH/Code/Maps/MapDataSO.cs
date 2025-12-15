using System.Collections.Generic;
using UnityEngine;

namespace Assets.Work.CDH.Code.Maps
{
    public struct TileData
    {
        public Vector2Int CellPos;
        public Vector2 AnchoredPos;
    }

    /// <summary>
    /// 타일들의 정보를 저장하는 SO
    /// </summary>
    [CreateAssetMenu(fileName = "MapDataSO", menuName = "SO/CDH/MapData")]
    public class MapDataSO : ScriptableObject
    {
        public Dictionary<int, TileData> TileDatas { get; set; } = new();
    }
}
