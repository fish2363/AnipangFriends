using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


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
        [SerializeField] private float cellSize = 10f;

        private CancellationTokenSource _cts;

        private Vector2Int _previewCellPos;
        private Vector2 _previewAnchoredPos;
        private Image curImage;

        private void Awake()
        {
            mapDataSO.Clear();

            TileData tile = new();
            tile.CellPos = new Vector2Int(0, 0);
            tile.AnchoredPos = new Vector2(0, 0);
            mapDataSO.AddTileData(tile);
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
                curImage.GetComponent<RectTransform>().position = _previewAnchoredPos;

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
            Vector2 screenSize = new(Screen.width, Screen.height);
            Vector2 mouseLocal = mouseScreenPos - screenSize * 0.5f;

            TileData bestTile = mapDataSO.GetTileDatas()[0];

            if (mapDataSO.GetTileDatas().Count > 1)
            {
                float bestSqr = float.MaxValue;

                Vector2Int mouseQuad = new Vector2Int(mouseLocal.x >= 0f ? 1 : -1, mouseLocal.y >= 0f ? 1 : -1);
                bool foundInQuad = false;

                foreach (var tileData in mapDataSO.GetTileDatas())
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
                    foreach (var tileData in mapDataSO.GetTileDatas())
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

            Vector2Int tertiary = -secondary;
            Vector2Int quaternary = -primary;

            Vector2Int basePos = bestTile.CellPos;

            Vector2Int[] dirs = { primary, secondary, tertiary, quaternary };
            foreach (var dir in dirs)
            {
                Vector2Int p = basePos + dir;
                if (!HasTileAt(p))
                {
                    outCellPos = p;
                    outAnchoredPos = (screenSize / 2) + bestTile.AnchoredPos + (Vector2)dir * cellSize;
                    return;
                }
            }

            outCellPos = basePos;
            outAnchoredPos = (screenSize / 2) + bestTile.AnchoredPos;
        }
    }
}
