using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Work.CDH.Code.Maps
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private MapData mapData;
        [SerializeField] private Transform tilesParent;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private GameObject bridgePrefab;

        // optional: z-fighting 방지용
        [SerializeField] private float bridgeYOffset = 0.01f;

        private Renderer tileRenderer;
        private Renderer bridgeRenderer;

        // tile extents (half size) in world space
        private Vector2 tileExtentsXZ;

        // bridge size from renderer bounds (x, z)
        private Vector2 bridgeSizeXZ;

        // placed tile instances (optional, for later use)
        private Dictionary<Vector2Int, GameObject> tileInstancesByCell = new Dictionary<Vector2Int, GameObject>();

        /// <summary>
        /// 근처 4방향 배열
        /// </summary>
        private static readonly Vector2Int[] Neigh4 =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        private Vector2 tileSizeXZ;     // (size.x, size.z)
        private float bridgeLen;        // 다리 "길이"로 쓸 값
        private Vector2 stepXZ;         // 타일 중심 간격

        private void Awake()
        {
            tileRenderer = tilePrefab != null ? tilePrefab.GetComponentInChildren<Renderer>() : null;
            bridgeRenderer = bridgePrefab != null ? bridgePrefab.GetComponentInChildren<Renderer>() : null;

            tileSizeXZ = tileRenderer != null
                ? new Vector2(tileRenderer.bounds.size.x, tileRenderer.bounds.size.z)
                : new Vector2(1f, 1f);

            // 다리 길이 축이 X인지 Z인지 애매하면, 그냥 더 큰 쪽을 길이로 쓰면 대부분 맞음
            Vector2 bSize = bridgeRenderer != null
                ? new Vector2(bridgeRenderer.bounds.size.x, bridgeRenderer.bounds.size.z)
                : Vector2.one;

            bridgeLen = Mathf.Max(bSize.x, bSize.y);

            // 타일 중심 간격 = 타일 크기 + 다리 길이
            stepXZ = new Vector2(tileSizeXZ.x + bridgeLen, tileSizeXZ.y + bridgeLen);

            if (mapData != null)
                mapData.OnAddTileData += HandleAddTile;
        }


        private void OnDestroy()
        {
            if (mapData != null)
                mapData.OnAddTileData -= HandleAddTile;
        }

        private void HandleAddTile(TileData newTile)
        {
            GenerateBridgeThenTile(newTile);
        }

        private void GenerateBridgeThenTile(TileData newTile)
        {
            // anchoredPos를 CellPos 기반으로 고정
            Vector2 anchored = CellToAnchoredPos(newTile.CellPos);

            for (int i = 0; i < Neigh4.Length; i++)
            {
                Vector2Int neighborCell = newTile.CellPos + Neigh4[i];
                if (!mapData.TryGetTileDataByCellPos(neighborCell, out TileData neighborTile))
                    continue;

                if (!mapData.TryRegisterBridge(newTile.CellPos, neighborCell))
                    continue;

                // 이웃도 CellPos 기반으로 고정
                Vector2 neighborAnchored = CellToAnchoredPos(neighborTile.CellPos);

                CreateBridgeBetween(neighborAnchored, anchored);
            }

            Vector3 tileWorldPos = AnchoredToWorldXZ(anchored);
            Instantiate(tilePrefab, tileWorldPos, Quaternion.identity, tilesParent);
        }


        /// <summary>
        /// 다리 생성. anchoredA부터 anchoredB까지.
        /// </summary>
        /// <param name="anchoredA"></param>
        /// <param name="anchoredB"></param>
        private void CreateBridgeBetween(Vector2 anchoredA, Vector2 anchoredB)
        {
            // 월드 좌표로 변환
            Vector3 A = AnchoredToWorldXZ(anchoredA);
            Vector3 B = AnchoredToWorldXZ(anchoredB);

            // 방향
            Vector3 dir = B - A;
            dir.y = 0f;
            if (dir.sqrMagnitude <= 0.0000001f) return;

            // 다리는 타일 중심의 정확한 중간에 배치
            Vector3 pos = (A + B) * 0.5f;
            pos.y += bridgeYOffset;

            // 방향만 맞춰줌 (스케일은 건드리지 않음)
            Quaternion rot = Quaternion.LookRotation(dir.normalized, Vector3.up);

            Instantiate(bridgePrefab, pos, rot, tilesParent);
        }


        /// <summary>
        /// Vector2에서 Vector3로 변환해줌
        /// </summary>
        /// <param name="anchoredPos"></param>
        /// <returns></returns>
        private static Vector3 AnchoredToWorldXZ(Vector2 anchoredPos)
        {
            return new Vector3(anchoredPos.x, 0f, anchoredPos.y);
        }

        /// <summary>
        /// CellPos -> AnchoredPos
        /// </summary>
        /// <param name="cellPos"></param>
        /// <returns></returns>
        private Vector2 CellToAnchoredPos(Vector2Int cellPos)
        {
            return new Vector2(cellPos.x * stepXZ.x, cellPos.y * stepXZ.y);
        }
    }
}
