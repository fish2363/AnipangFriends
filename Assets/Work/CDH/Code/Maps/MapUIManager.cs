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

        private void Awake()
        {
            TileData tile = new();
            tile.CellPos = new Vector2Int(0, 0);
            tile.AnchoredPos = new Vector2(0, 0);
            mapDataSO.TileDatas.Add(0, tile);
            StartBuildTile();
        }

        public void StartBuildTile()
        {
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
                // 1. 가장 가까운 타일의 위치를 구한다.
                Vector2Int tilePos = GetNearestAndBuildableTilePos(Mouse.current.position.ReadValue());
                Debug.Log(tilePos.ToString()); 

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    Cancel();
                    return;
                }

                await Awaitable.NextFrameAsync(ct);
            }
        }

        private void BuildTile(Vector2Int tilePos)
        {

        }

        private Vector2Int GetNearestAndBuildableTilePos(Vector2 mouseScreenPos)
        {
            // 일단 가장 가까운 타일을 찾아야 함
            // 0,0의 타일부터 시작하므로 사분면을 활용해서 타일을 소거하자
            // 일단 마우스의 위치가 있으니 스크린 크기로 나누면 0-1로 정규화
            screenSize = new(Screen.width, Screen.height); // 중간에 화면 크기가 바뀔 수도 있어서 일단 여기서 진행
            Vector2 normalizedMousePos = mouseScreenPos / screenSize;

            // 스크린 좌표(좌하단 0,0) -> 중앙 기준(0,0) 좌표로 변환
            Vector2 mouseLocal = mouseScreenPos - screenSize * 0.5f;

            // 기본값은 실제 존재하는 타일로 (필터에 의해 하나도 못 찾는 경우 대비)
            TileData bestTile = mapDataSO.TileDatas[0];

            // 만약 첫 타일 설치라면 바로 다음으로
            if (mapDataSO.TileDatas.Count > 1)
            {
                float bestSqr = float.MaxValue;

                // 마우스 사분면: 스크린 중앙 기준으로 판정
                Vector2Int mouseQuad = new Vector2Int(mouseLocal.x >= 0f ? 1 : -1, mouseLocal.y >= 0f ? 1 : -1);

                bool foundInQuad = false;

                // 모든 타일 돌면서
                foreach (var tileData in mapDataSO.TileDatas.Values)
                {
                    // 타일 사분면도 anchoredPos(중앙 기준)로 맞춤
                    Vector2Int tileQuad = new Vector2Int(tileData.AnchoredPos.x >= 0f ? 1 : -1, tileData.AnchoredPos.y >= 0f ? 1 : -1);

                    // 같은 방향이 아니면 다음 타일로
                    if (!mouseQuad.Equals(tileQuad))
                        continue;

                    foundInQuad = true;

                    // ✅ "두 점 사이 거리"로 계산해야 함
                    float sqr = (mouseLocal - tileData.AnchoredPos).sqrMagnitude;
                    if (sqr < bestSqr)
                    {
                        bestSqr = sqr;
                        bestTile = tileData;
                    }
                }

                // 같은 사분면에서 못 찾았으면 전체에서 다시 가장 가까운 타일 찾기
                if (!foundInQuad)
                {
                    bestSqr = float.MaxValue;
                    foreach (var tileData in mapDataSO.TileDatas.Values)
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

            // 이제 상하좌우 확인해서 가장 가까운 설치 가능한 타일 위치 반환해줘.

            bool HasTileAt(Vector2Int cellPos)
            {
                foreach (var t in mapDataSO.TileDatas.Values)
                {
                    if (t.CellPos == cellPos)
                        return true;
                }
                return false;
            }

            // delta도 같은 좌표계(mouseLocal vs anchoredPos)로 계산
            Vector2 delta = mouseLocal - bestTile.AnchoredPos;

            // 마우스가 bestTile 기준 어느 방향에 더 가까운지로 우선순위 결정
            Vector2Int primary, secondary, tertiary, quaternary;

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

            tertiary = -secondary;
            quaternary = -primary;

            Vector2Int basePos = bestTile.CellPos;

            Vector2Int p1 = basePos + primary;
            if (!HasTileAt(p1)) return p1;

            Vector2Int p2 = basePos + secondary;
            if (!HasTileAt(p2)) return p2;

            Vector2Int p3 = basePos + tertiary;
            if (!HasTileAt(p3)) return p3;

            Vector2Int p4 = basePos + quaternary;
            if (!HasTileAt(p4)) return p4;

            // 상하좌우가 전부 막혀있으면 현재 타일 반환
            return basePos;
        }
    }
}
