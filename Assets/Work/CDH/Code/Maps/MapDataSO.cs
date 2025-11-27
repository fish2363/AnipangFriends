using System.Collections.Generic;
using UnityEngine;

namespace Assets.Work.CDH.Code.Maps
{
    [CreateAssetMenu(fileName = "MapDataSO", menuName = "CDH/MapData")]
    public class MapDataSO : ScriptableObject
    {
        public List<Vector2Int> tileDatas;
    }
}
