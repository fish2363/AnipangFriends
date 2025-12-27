using UnityEngine;
using DG.Tweening;
using GondrLib.ObjectPool.RunTime;
using Core.EventBus;

namespace Code.Component
{
    public class ItemDropFeedback : MonoBehaviour
    {
        [SerializeField]
        private PoolItemSO coinPrefab;
        [SerializeField] private int _amount;

        [ContextMenu("spawnGold")]
        public void CreateFeedback()
        {
            DropItem(_amount);
        }

        public void StopFeedback()
        {

        }

        private void DropItem(int amount)
        {
            Bus<SpawnGoldEvt>.Raise(new SpawnGoldEvt(coinPrefab, amount));
        }
    }
}