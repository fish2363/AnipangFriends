using NUnit.Framework;
using System;
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
        [SerializeField] private float cellSize = 10f;

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
            Vector2Int mouseQuad = new Vector2Int((mouseScreenPos.x / screenSize.x) > 0.5f ? 1 : -1, (mouseScreenPos.y / screenSize.y) > 0.5f ? 1 : -1);

            Vector2Int nearestTilePos = Vector2Int.zero;
            float bestSqr = float.MaxValue;
            bool found = false;

            foreach (Vector2Int tilePos in mapDataSO.TileDatas.Values)
            {
                Vector2Int tileQuad = new Vector2Int(tilePos.x > 0 ? 1 : -1, tilePos.y > 0 ? 1 : -1);
                if (!IsSameQuadrant(tileQuad, mouseQuad))
                    continue;

                float sqr = (mouseScreenPos - tilePos).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    nearestTilePos = tilePos; // 반환은 셀 좌표로
                    found = true;
                }
            }

            return found ? nearestTilePos : Vector2Int.zero;
        }

        private bool IsSameQuadrant(Vector2Int a, Vector2Int b)
        {
            return a.x == b.x && a.y == b.y;
        }
    }
}
