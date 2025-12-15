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

    /// <summary>
    /// 타일의 설치, UI와 연결
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private MapDataSO mapDataSO;
        [SerializeField] private Transform tilesParent;
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private Renderer renderer;

        public Vector2Int TestGenerateTilePos;

        private float tileSize;
        private int tileKey;

        private float testTileSize => renderer.bounds.size.x;


        private void Awake()
        {
            tileKey = 0;

            var renderer = tilePrefab.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                tileSize = renderer.bounds.size.x;
            }
            else
            {
                Debug.LogWarning("tilePrefab에 Renderer X, use default value");
                tileSize = 1;
            }
        }

        [ContextMenu("GenerateTile")]
        private void TestGenerate()
        {
            GenerateTile(TestGenerateTilePos);
        }

        public void GenerateTile(Vector2Int gridPos)
        {
            Vector3 worldPos = GridPosToWorldPos(gridPos);
            Instantiate(tilePrefab, worldPos, Quaternion.identity, tilesParent);
            // mapDataSO.TileDatas.Add(tileKey++, );
        }

        public Vector3 GridPosToWorldPos(Vector2Int gridPos)
        {
            print($"[gridPos] x:{gridPos.x} z:{gridPos.y}");
            float worldX = gridPos.x * testTileSize;
            float worldZ = gridPos.y * testTileSize;

            print($"[WorldPos] x:{worldX} z:{worldZ}");
            return new Vector3(worldX, 0f, worldZ);
        }
    }
}
