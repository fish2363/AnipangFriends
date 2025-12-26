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

        // optional: z-fighting ¹æÁö¿ë
        [SerializeField] private float bridgeYOffset = 0.01f;

        private Renderer tileRenderer;
        private Renderer bridgeRenderer;

        // tile extents (half size) in world space
        private Vector2 tileExtentsXZ;

        // bridge size from renderer bounds (x, z)
        private Vector2 bridgeSizeXZ;

        // placed tile instances (optional, for later use)
        private Dictionary<Vector2Int, GameObject> tileInstancesByCell = new Dictionary<Vector2Int, GameObject>();

        // prevent duplicate bridges
        private HashSet<EdgeKey> placedBridges = new HashSet<EdgeKey>();

        private static readonly Vector2Int[] Neigh4 =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        private void Awake()
        {
            if (tilePrefab != null)
                tileRenderer = tilePrefab.GetComponentInChildren<Renderer>();

            if (tileRenderer != null)
                tileExtentsXZ = new Vector2(tileRenderer.bounds.extents.x, tileRenderer.bounds.extents.z);
            else
                tileExtentsXZ = new Vector2(0.5f, 0.5f);

            if (bridgePrefab != null)
                bridgeRenderer = bridgePrefab.GetComponentInChildren<Renderer>();

            if (bridgeRenderer != null)
                bridgeSizeXZ = new Vector2(bridgeRenderer.bounds.size.x, bridgeRenderer.bounds.size.z);
            else
                bridgeSizeXZ = Vector2.one;

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
            // 1) bridges first (connect to any existing neighbor tiles)
            for (int i = 0; i < Neigh4.Length; i++)
            {
                Vector2Int neighborCell = newTile.CellPos + Neigh4[i];

                if (!mapData.TryGetTileDataByCellPos(neighborCell, out TileData neighborTile))
                    continue;

                EdgeKey edge = new EdgeKey(newTile.CellPos, neighborCell);
                if (placedBridges.Contains(edge))
                    continue;

                CreateBridgeBetween(neighborTile.AnchoredPos, newTile.AnchoredPos);

                placedBridges.Add(edge);
            }

            // 2) then place tile
            Vector3 tileWorldPos = AnchoredToWorldXZ(newTile.AnchoredPos);
            GameObject tileObj = Instantiate(tilePrefab, tileWorldPos, Quaternion.identity, tilesParent);
            tileInstancesByCell[newTile.CellPos] = tileObj;
        }

        private void CreateBridgeBetween(Vector2 anchoredA, Vector2 anchoredB)
        {
            Vector3 A = AnchoredToWorldXZ(anchoredA);
            Vector3 B = AnchoredToWorldXZ(anchoredB);

            Vector3 dir = B - A;
            dir.y = 0f;

            float distCenters = dir.magnitude;
            if (distCenters <= 0.0001f)
                return;

            Vector3 dirN = dir / distCenters;

            // trim so the bridge starts/ends near tile edges, not tile centers
            bool horizontal = Mathf.Abs(dirN.x) > Mathf.Abs(dirN.z);
            float trim = horizontal ? tileExtentsXZ.x : tileExtentsXZ.y;

            Vector3 start = A + dirN * trim;
            Vector3 end = B - dirN * trim;

            float len = Vector3.Distance(start, end);
            if (len <= 0.0001f)
                return;

            Vector3 pos = (start + end) * 0.5f;
            pos.y += bridgeYOffset;

            // assume bridge length axis is local Z
            Quaternion rot = Quaternion.LookRotation((end - start).normalized, Vector3.up);

            GameObject bridgeObj = Instantiate(bridgePrefab, pos, rot, tilesParent);

            // scale bridge along its length axis (local Z) using bridgeSizeXZ.y (bounds.size.z)
            float baseLen = Mathf.Max(bridgeSizeXZ.y, 0.0001f);
            float mul = len / baseLen;

            Vector3 s = bridgeObj.transform.localScale;
            s.z *= mul;
            bridgeObj.transform.localScale = s;
        }

        private static Vector3 AnchoredToWorldXZ(Vector2 anchoredPos)
        {
            return new Vector3(anchoredPos.x, 0f, anchoredPos.y);
        }

        // undirected edge key, so (a,b) and (b,a) are same
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

            public override int GetHashCode()
            {
                unchecked
                {
                    return (a.GetHashCode() * 397) ^ b.GetHashCode();
                }
            }

            public override bool Equals(object obj)
            {
                return obj is EdgeKey other && a == other.a && b == other.b;
            }
        }
    }
}
