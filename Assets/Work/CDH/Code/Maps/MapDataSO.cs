using System.Collections.Generic;
using UnityEngine;

namespace Assets.Work.CDH.Code.Maps
{
    /// <summary>
    /// 타일들의 정보를 저장하는 SO
    /// </summary>
    [CreateAssetMenu(fileName = "MapDataSO", menuName = "SO/CDH/MapData")]
    public class MapDataSO : ScriptableObject
    {
        public Dictionary<int, Vector2Int> TileDatas = new();
    }
}
