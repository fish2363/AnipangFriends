using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Work.CDH.Code.Maps
{
    public class MapUIManager : MonoBehaviour
    {
        [SerializeField] private MapDataSO mapDataSO;

        private CancellationTokenSource _cts;
        private Vector2 screenSize;

        public void StartBuildTile()
        {
            screenSize = new(Screen.width, Screen.height);
            Cancel();
            _cts = new CancellationTokenSource();
            _ = StartAwaitable(_cts.Token);
        }

        private void Cancel()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async Awaitable StartAwaitable(CancellationToken ct)
        {
            while (true)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Vector2 tilePos = GetTilePos(mousePos);

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Cancel();
                    return;
                }

                await Awaitable.NextFrameAsync(ct);
            }
        }

        private void BuildTile(Vector2 tilePos)
        {

        }

        private Vector2Int GetTilePos(Vector2 mouseScreenPos)
        {
            Vector2 screenSize = new(Screen.width, Screen.height);
            Vector2Int mouseQuad = GetQuadrantSign(mouseScreenPos, screenSize);

            Vector2Int nearestTilePos = Vector2Int.zero;
            float bestSqr = float.MaxValue;
            bool found = false;

            foreach (Vector2Int tilePos in mapDataSO.TileDatas.Values)
            {
                // TODO: tilePos(그리드/셀)를 "스크린 좌표"로 바꿔야 함
                Vector2 tileScreenPos = TileToScreenPos(tilePos);

                Vector2Int tileQuad = GetQuadrantSign(tileScreenPos, screenSize);
                if (!IsSameQuadrant(tileQuad, mouseQuad))
                    continue;

                float sqr = (mouseScreenPos - tileScreenPos).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    nearestTilePos = tilePos; // 반환은 셀 좌표로
                    found = true;
                }
            }

            return found ? nearestTilePos : Vector2Int.zero;
        }


        /// <summary>
        ///  방향을 우상, 우하, 좌상, 좌우 4방향으로 나눠서 방향을 받는 코드
        /// </summary>
        /// <param name="screenPos"></param>
        /// <param name="screenSize"></param>
        /// <returns></returns>
        private Vector2Int GetQuadrantSign(Vector2 screenPos, Vector2 screenSize)
        {
            // 0~1 정규화
            Vector2 n = new Vector2(screenPos.x / screenSize.x, screenPos.y / screenSize.y);

            // 화면 중심(0.5,0.5) 기준으로 우/좌, 상/하 판정
            int sx = n.x >= 0.5f ? 1 : -1; // right / left
            int sy = n.y >= 0.5f ? 1 : -1; // up / down
            return new Vector2Int(sx, sy);
        }

        private bool IsSameQuadrant(Vector2Int a, Vector2Int b)
        {
            return a.x == b.x && a.y == b.y;
        }
    }
}
