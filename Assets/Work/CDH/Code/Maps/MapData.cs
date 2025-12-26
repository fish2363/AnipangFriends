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

    [DefaultExecutionOrder(-10)]
    public class MapData : MonoBehaviour
    {
        public delegate void AddTileDataDelegate(TileData newTile);
        public AddTileDataDelegate OnAddTileData;

        private Dictionary<int, TileData> tileDatas;

        // CellPos -> tileKey 인덱스 (핵심)
        private Dictionary<Vector2Int, int> tileKeyByCellPos;

        private int curTileKey;

        private void Awake()
        {
            tileDatas = new();
            tileKeyByCellPos = new();
            curTileKey = 0;
        }

        public List<TileData> GetTileDatas()
        {
            return tileDatas.Values.ToList();
        }

        // HashSet 대신 인덱스로 Contains 처리 (중복 데이터/동기화 문제 방지)
        public bool ContainsCellPos(Vector2Int targetPos)
        {
            return tileKeyByCellPos.ContainsKey(targetPos);
        }

        public TileData GetTileDataByTileKey(int tileKey)
        {
            tileDatas.TryGetValue(tileKey, out TileData tileData);
            return tileData;
        }

        // CellPos로 바로 TileData 얻기 (다리 생성에 필요)
        public bool TryGetTileDataByCellPos(Vector2Int cellPos, out TileData tileData)
        {
            if (tileKeyByCellPos.TryGetValue(cellPos, out int key))
                return tileDatas.TryGetValue(key, out tileData);

            tileData = default;
            return false;
        }

        public int AddTileData(Vector2Int cellPos, Vector2 anchoredPos)
        {
            TileData tileData = new TileData
            {
                CellPos = cellPos,
                AnchoredPos = anchoredPos
            };

            return AddTileData(tileData);
        }

        public int AddTileData(TileData tileData)
        {
            // 중복 방지(선택): 이미 있으면 -1 반환(또는 기존 키 반환 등으로 정책 선택 가능)
            if (tileKeyByCellPos.ContainsKey(tileData.CellPos))
            {
                Debug.LogWarning($"이미 존재하는 CellPos: {tileData.CellPos}");
                return -1;
            }

            int tileKey = curTileKey++;
            tileDatas.Add(tileKey, tileData);
            tileKeyByCellPos.Add(tileData.CellPos, tileKey);

            OnAddTileData?.Invoke(tileData);
            return tileKey;
        }

        public void Clear()
        {
            tileDatas.Clear();
            tileKeyByCellPos.Clear();
        }
    }
}
