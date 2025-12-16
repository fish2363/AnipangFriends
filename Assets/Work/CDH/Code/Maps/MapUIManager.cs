using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditorInternal.ReorderableList;



#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.Work.CDH.Code.Maps
{
    public class MapUIManager : MonoBehaviour
    {
        [SerializeField] private MapDataSO mapDataSO;
        [SerializeField] private Image tileImage;
        [SerializeField] private Transform tileImageParent;

        private CancellationTokenSource _cts;

        private Vector2Int _previewCellPos;
        private Vector2 _previewAnchoredPos;
        private Image curImage;

        private Vector2 screenSize;
        private Vector2 defaultPos;

        private float cellSize = 10f;

        private void Awake()
        {
            mapDataSO.Clear();
            SetScreenSize();

            TileData tile = new();
            tile.CellPos = new Vector2Int(0, 0);
            tile.AnchoredPos = new Vector2(0, 0);
            mapDataSO.AddTileData(tile);

            // GetWorldCorners로 정확한 타일 크기 가져오기
            Vector3[] corners = new Vector3[4];
            tileImage.rectTransform.GetWorldCorners(corners);
            cellSize = Vector3.Distance(corners[0], corners[3]); // 세로 길이 기준 (또는 corners[0] ↔ corners[1]으로 가로)
        }


        private void SetScreenSize()
        {
            screenSize = new(Screen.width, Screen.height);
            defaultPos = screenSize / 2;
        }

        private void Update()
        {
            if(Keyboard.current.bKey.wasPressedThisFrame)
            { 
                StartBuildTile();
            }
        }

        public void StartBuildTile()
        {
            Cancel();
            _cts = new CancellationTokenSource();
            curImage = Instantiate(tileImage, tileImageParent);
            _ = StartAwaitable(_cts.Token);
        }

        private void Cancel()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private void BuildTile()
        {
            TileData tile = new();
            tile.CellPos = _previewCellPos;
            tile.AnchoredPos = _previewAnchoredPos;
            mapDataSO.AddTileData(tile);
        }

        private async Awaitable StartAwaitable(CancellationToken ct)
        {
            while (true)
            {
                Vector2 mouse = Mouse.current.position.ReadValue();

                GetNearestAndBuildableTile(mouse, out _previewCellPos, out _previewAnchoredPos);
                print($"cellPos : {_previewCellPos}");
                print($"anchoredpos : {_previewAnchoredPos}");
                curImage.GetComponent<RectTransform>().position = _previewAnchoredPos + defaultPos;

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Cancel();
                    BuildTile();
                    return;
                }

                await Awaitable.NextFrameAsync(ct);
            }
        }

        private void GetNearestAndBuildableTile(Vector2 mouseScreenPos, out Vector2Int outCellPos, out Vector2 outAnchoredPos)
        {
            Vector2 mouseLocal = mouseScreenPos - screenSize * 0.5f;

            TileData bestTile = mapDataSO.GetTileDatas()[0];

            var tiles = mapDataSO.GetTileDatas();
            if (tiles.Count > 1)
            {
                float bestSqr = float.MaxValue;
                Vector2Int mouseQuad = new Vector2Int(mouseLocal.x >= 0f ? 1 : -1, mouseLocal.y >= 0f ? 1 : -1);
                bool foundInQuad = false;

                foreach (var tileData in tiles)
                {
                    Vector2Int tileQuad = new Vector2Int(tileData.AnchoredPos.x >= 0f ? 1 : -1, tileData.AnchoredPos.y >= 0f ? 1 : -1);
                    if (!mouseQuad.Equals(tileQuad)) continue;

                    foundInQuad = true;

                    float sqr = (mouseLocal - tileData.AnchoredPos).sqrMagnitude;
                    if (sqr < bestSqr)
                    {
                        bestSqr = sqr;
                        bestTile = tileData;
                    }
                }

                if (!foundInQuad)
                {
                    bestSqr = float.MaxValue;
                    foreach (var tileData in tiles)
                    {
                        float sqr = (mouseLocal - tileData.AnchoredPos).sqrMagnitude;
                        if (sqr < bestSqr)
                        {
                            bestSqr = sqr;
                            bestTile = tileData;
                        }
                    }
                }
            }

            bool HasTileAt(Vector2Int cellPos)
            {
                foreach (var t in mapDataSO.GetTileDatas())
                {
                    if (t.CellPos == cellPos)
                        return true;
                }
                return false;
            }

            Vector2 delta = mouseLocal - bestTile.AnchoredPos;

            Vector2Int primary, secondary;
            if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
            {
                primary = (delta.x >= 0f) ? Vector2Int.right : Vector2Int.left;
                secondary = (delta.y >= 0f) ? Vector2Int.up : Vector2Int.down;
            }
            else
            {
                primary = (delta.y >= 0f) ? Vector2Int.up : Vector2Int.down;
                secondary = (delta.x >= 0f) ? Vector2Int.right : Vector2Int.left;
            }

            Vector2Int[] directions = new Vector2Int[]
            {
        primary, secondary,
        -secondary, -primary,
        primary + secondary, primary - secondary,
        -primary + secondary, -primary - secondary
            };

            Vector2Int basePos = bestTile.CellPos;

            // 넓은 범위로 순차적으로 검사
            for (int distance = 1; distance <= 10; distance++) // 10은 최대 거리, 필요시 조절 가능
            {
                foreach (var dir in directions)
                {
                    Vector2Int p = basePos + dir * distance;
                    if (!HasTileAt(p))
                    {
                        outCellPos = p;
                        outAnchoredPos = bestTile.AnchoredPos + (Vector2)dir * cellSize * distance;
                        return;
                    }
                }
            }

            // fallback
            outCellPos = basePos;
            outAnchoredPos = bestTile.AnchoredPos;
        }
    }
}
