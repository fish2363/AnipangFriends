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
    [DefaultExecutionOrder(-10)]
    public class MapData : MonoBehaviour
    {
        public delegate void AddTileDataDelegate(TileData newTile);

        public AddTileDataDelegate OnAddTileData;

        private Dictionary<int, TileData> tileDatas;
        private int curTileKey;

        private void Awake()
        {
            tileDatas = new();
            curTileKey = 0;
        }

        public List<TileData> GetTileDatas()
        {
            return tileDatas.Values.ToList();
        }

        public TileData GetTileDataByTileKey(int tileKey)
        {
            tileDatas.TryGetValue(tileKey, out TileData tileData);
            return tileData;
        }

        public int AddTileData(TileData tileData)
        {
            int GenerateTileKey()
            {
                return curTileKey++;
            }
            int tileKey = GenerateTileKey();
            tileDatas.Add(tileKey, tileData);
            OnAddTileData?.Invoke(tileData);
            return tileKey;
        }

        public void Clear()
        {
            tileDatas.Clear();
        }

    }
}
