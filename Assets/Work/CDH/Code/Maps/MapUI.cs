using System;
using UnityEngine;

namespace Assets.Work.CDH.Code.Maps
{
    public class MapUI : MonoBehaviour
    {
        [SerializeField] private MapDataSO mapDataSO;
        [SerializeField] private GameObject tileImagePrefab;
        [SerializeField] private Transform tileImageParent;

        private float tileImageWidth;
        private float tileImageHeight;

        public void Awake()
        {
            RectTransform tileImageRect = tileImagePrefab.GetComponent<RectTransform>();
            tileImageWidth = tileImageRect.sizeDelta.x;
            tileImageHeight = tileImageRect.sizeDelta.y;
        }

        public void UpdateTileImage()
        {
            foreach(Vector2Int data in mapDataSO.tileDatas)
            {
                GameObject tileImage = Instantiate(tileImagePrefab, tileImageParent);
                tileImage.transform.position = GetWorldPos(data);
            }
        }

        private Vector3 GetWorldPos(Vector2Int data)
        {
            return tileImageParent.position + new Vector3(
                data.x * tileImageWidth,
                data.y * tileImageHeight,
                0f
            );
        }

    }
}