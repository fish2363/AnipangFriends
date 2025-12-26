using System;
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

        private HashSet<EdgeKey> placedBridges;

        private void Awake()
        {
            tileDatas = new();
            tileKeyByCellPos = new();
            placedBridges = new();
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

        /// <summary>
        /// CellPos로 바로 TileData 얻기 (다리 생성에 필요)
        /// CellPos -> Key -> tileData. CellPos로 된 타일이 이미 있으면 true, 없으면 false.
        /// </summary>
        /// <param name="cellPos"></param>
        /// <param name="tileData"></param>
        /// <returns></returns>
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

        /// <summary>
        /// cellA에서 cellB로 엣지키로 연결.
        /// 이미 있으면 false, 
        /// 새로 추가되면 true.
        /// </summary>
        /// <param name="cellA"></param>
        /// <param name="cellB"></param>
        /// <returns></returns>
        public bool TryRegisterBridge(Vector2Int cellA, Vector2Int cellB)
        {
            EdgeKey key = new EdgeKey(cellA, cellB);
            if (placedBridges.Contains(key))
                return false;

            placedBridges.Add(key);
            return true;
        }

        public void Clear()
        {
            tileDatas.Clear();
            tileKeyByCellPos.Clear(); 
            placedBridges.Clear();
        }

        /// <summary>
        /// 다리 연결 중복 방지용
        /// a부터 b까지 연결되어있다는 뜻
        /// Equal로 
        /// </summary>
        private readonly struct EdgeKey
        {
            private readonly Vector2Int a;
            private readonly Vector2Int b;

            public EdgeKey(Vector2Int p1, Vector2Int p2)
            {
                if (p1.x < p2.x || (p1.x == p2.x && p1.y <= p2.y))
                {
                    a = p1;
                    b = p2;
                }
                else
                {
                    a = p2;
                    b = p1;
                }
            }

            // 아래 두개의 함수 GetHashCode와 Equals는 HashSet/Dictionary에서 사용할 때 호출되는 함수로 override해서 사용자가 어떤식으로 동작할지 정의한 거임.
            // 가장 좋은건 IEquatable<>까지 구현하는 것. 간단하게 오버라이드만 해서 사용해도 됨
            public override int GetHashCode()
            {
                // // unchecked는 이 안에서 오버플로우가 발생해도 처음부터 시작하도록 만들어줌 ex) Int.Max + 1 = Int.Min
                // unchecked
                // {
                //     return (a.GetHashCode() * 397) ^ b.GetHashCode();
                // }
                // 근데 요즘 C#엔 HashCode.Combine가 있다
                return HashCode.Combine(a, b);
            }

            public override bool Equals(object obj) // 굳이 오버라이드해서 사용할 필요가 있을까? 그냥 EdgeKey를 받는 함수로 만들면 안되는건 아니겠지만 오버라이드 했을 때의 장점이 있나?
            {
                return obj is EdgeKey other && a == other.a && b == other.b;
            }
        }
    }
}
