using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<int, TileData> tileDatas { get; set; } = new();
        private int curTileKey = 0;

        public List<TileData> GetTileDatas()
        {
            return tileDatas.Values.ToList();
        }

        public void AddTileData(TileData tileData)
        {
            int tileKey = GenerateTileKey();
            tileDatas.Add(tileKey, tileData);
        }

        public void Clear()
        {
            tileDatas.Clear();
        }

        private int GenerateTileKey()
        {
            return curTileKey++;
        }
    }
}
