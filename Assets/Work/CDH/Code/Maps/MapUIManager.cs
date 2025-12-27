using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Assets.Work.CDH.Code.Maps
{
    public class MapUIManager : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int grid;
        [SerializeField] private float tileInterval;

        [Header("ETC...")]
        [SerializeField] private MapData mapData;
        [SerializeField] private Image tileImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Transform mapUIParent;
        [SerializeField] private Canvas canvas;
        [SerializeField] private Image tileBackground;

        private CancellationTokenSource _cts;

        private Vector2Int _previewCellPos;
        private Vector2 _previewAnchoredPos;
        private Image curImage;

        private Vector2 screenSize;
        private Vector2 defaultPos;

        #region UIs
        private Vector2 backgroundSize;
        private Transform tileUIParent;
        #endregion

        private float cellSize = 10f;

        // added
        private float step;      // cellSize + tileInterval
        private int minX, maxX;  // allowed cell range
        private int minY, maxY;

        private void Awake()
        {
            mapData.Clear();
            SetScreenSize();

            // GetWorldCorners로 정확한 타일 크기 가져오기
            Vector3[] corners = new Vector3[4];
            tileImage.rectTransform.GetWorldCorners(corners);
            cellSize = Vector3.Distance(corners[0], corners[3]);

            // spacing step
            step = cellSize + tileInterval;

            // grid range (centered at 0,0)
            minX = -grid / 2;
            maxX = minX + grid - 1;
            minY = -grid / 2;
            maxY = minY + grid - 1;

            // background size (interval count is grid-1)
            backgroundSize = new Vector2(
            cellSize* grid + tileInterval * (grid + 1),
            cellSize* grid + tileInterval * (grid + 1)
            );

            Image background = Instantiate(backgroundImage, mapUIParent);
            tileUIParent = background.transform;
            tileUIParent.localPosition = Vector3.zero;
            (tileUIParent as RectTransform).sizeDelta = backgroundSize;

            CreateTileBackgroundGrid();

            Image centerTile = Instantiate(tileImage, tileUIParent);
            centerTile.rectTransform.anchoredPosition = CellToAnchoredPos(Vector2Int.zero);

            BuildTile(Vector2Int.zero, CellToAnchoredPos(Vector2Int.zero));
        }

        private void SetScreenSize()
        {
            screenSize = new Vector2(Screen.width, Screen.height);
            defaultPos = screenSize / 2;
        }

        private void Update()
        {
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {
                StartBuildTile();
            }
        }

        public void StartBuildTile()
        {
            Cancel();
            _cts = new CancellationTokenSource();
            curImage = Instantiate(tileImage, tileUIParent);
            _ = StartAwaitable(_cts.Token);
        }

        private void Cancel()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private void BuildTile(Vector2Int cellPos, Vector2 anchoredPos)
        {
            _previewCellPos = cellPos;
            _previewAnchoredPos = anchoredPos;

            BuildTile();
        }

        private void BuildTile()
        {
            if (!IsInGrid(_previewCellPos)) return;
            if (mapData.ContainsCellPos(_previewCellPos)) return;

            TileData tile = new();
            tile.CellPos = _previewCellPos;
            tile.AnchoredPos = _previewAnchoredPos;
            mapData.AddTileData(tile);
        }

        private async Awaitable StartAwaitable(CancellationToken ct)
        {
            while (true)
            {
                Vector2 mouse = Mouse.current.position.ReadValue();

                bool canBuild = TryGetNearestAndBuildableTile(mouse, out _previewCellPos, out _previewAnchoredPos);

                curImage.enabled = canBuild;
                if (canBuild)
                    curImage.rectTransform.anchoredPosition = _previewAnchoredPos;

                if (Mouse.current.leftButton.wasPressedThisFrame && canBuild)
                {
                    Cancel();
                    BuildTile();
                    return;
                }

                await Awaitable.NextFrameAsync(ct);
            }
        }

        private bool TryGetNearestAndBuildableTile(Vector2 mouseScreenPos, out Vector2Int outCellPos, out Vector2 outAnchoredPos)
        {
            Vector2 mouseLocal = GetMouseLocalInTileParent(mouseScreenPos);

            var tiles = mapData.GetTileDatas();
            if (tiles == null || tiles.Count == 0)
            {
                outCellPos = default;
                outAnchoredPos = default;
                return false;
            }

            TileData bestTile = tiles[0];

            if (tiles.Count > 1)
            {
                float bestSqr = float.MaxValue;
                Vector2Int mouseQuad = new Vector2Int(mouseLocal.x >= 0f ? 1 : -1, mouseLocal.y >= 0f ? 1 : -1);
                bool foundInQuad = false;

                for (int i = 0; i < tiles.Count; i++)
                {
                    var t = tiles[i];
                    Vector2Int tileQuad = new Vector2Int(t.AnchoredPos.x >= 0f ? 1 : -1, t.AnchoredPos.y >= 0f ? 1 : -1);
                    if (mouseQuad != tileQuad) continue;

                    foundInQuad = true;
                    float sqr = (mouseLocal - t.AnchoredPos).sqrMagnitude;
                    if (sqr < bestSqr)
                    {
                        bestSqr = sqr;
                        bestTile = t;
                    }
                }

                if (!foundInQuad)
                {
                    bestSqr = float.MaxValue;
                    for (int i = 0; i < tiles.Count; i++)
                    {
                        var t = tiles[i];
                        float sqr = (mouseLocal - t.AnchoredPos).sqrMagnitude;
                        if (sqr < bestSqr)
                        {
                            bestSqr = sqr;
                            bestTile = t;
                        }
                    }
                }
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

            Vector2Int[] directions = new Vector2Int[8]
            {
                primary, secondary,
                -secondary, -primary,
                primary + secondary, primary - secondary,
                -primary + secondary, -primary - secondary
            };

            Vector2Int basePos = bestTile.CellPos;

            for (int distance = 1; distance <= 3; distance++)
            {
                for (int i = 0; i < directions.Length; i++)
                {
                    Vector2Int p = basePos + directions[i] * distance;

                    if (!IsInGrid(p)) continue;
                    if (mapData.ContainsCellPos(p)) continue;

                    outCellPos = p;
                    outAnchoredPos = CellToAnchoredPos(p);
                    return true;
                }
            }

            outCellPos = basePos;
            outAnchoredPos = bestTile.AnchoredPos;
            return false;
        }

        private Vector2 GetMouseLocalInTileParent(Vector2 mouseScreenPos)
        {
            RectTransform rt = (RectTransform)tileUIParent;

            Camera cam = null;
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = canvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, mouseScreenPos, cam, out Vector2 local);
            return local;
        }

        private void CreateTileBackgroundGrid()
        {
            if (tileBackground == null) return;

            // grid 전체 셀에 대해 배경 생성
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    Vector2Int cell = new Vector2Int(x, y);

                    Image bg = Instantiate(tileBackground, tileUIParent);

                    RectTransform rt = bg.rectTransform;
                    rt.anchoredPosition = CellToAnchoredPos(cell);

                    // 배경 칸 크기를 타일과 동일하게 맞추고 싶으면
                    rt.sizeDelta = new Vector2(cellSize, cellSize);

                    // 혹시라도 배경이 타일 위로 올라오면 맨 뒤로 보내기
                    rt.SetAsFirstSibling();
                }
            }
        }

        private bool IsInGrid(Vector2Int cellPos)
        {
            return cellPos.x >= minX && cellPos.x <= maxX &&
                   cellPos.y >= minY && cellPos.y <= maxY;
        }

        private Vector2 CellToAnchoredPos(Vector2Int cellPos)
        {
            return new Vector2(cellPos.x * step, cellPos.y * step);
        }
    }
}
