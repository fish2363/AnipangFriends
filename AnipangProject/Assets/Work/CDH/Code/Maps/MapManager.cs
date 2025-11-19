using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Work.CDH.Code.Maps
{
    public class MapManager : MonoBehaviour
    {
        [SerializeField] private GameObject mapPrefab;

        private List<Map> currentMaps;

        // 맵 처음 만들 때 초기화
        public void Initialize()
        {
            
        }
    }
}
