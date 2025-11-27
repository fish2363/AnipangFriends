using System;
using UnityEngine;

namespace Assets.Work.CDH.Code.Maps
{
    public class Tile : MonoBehaviour
    {
        public Vector2Int GridPos { get; private set; } // MapDataSO에 저장되고 UI에서 사용할 위치.
        public TileType MapType { get; private set; }

        internal void Initialize(Vector2Int pos, TileType mapType)
        {
            GridPos = pos;
            MapType = mapType;
        }
    }
}