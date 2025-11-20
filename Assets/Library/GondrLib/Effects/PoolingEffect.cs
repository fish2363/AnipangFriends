using UnityEngine;
using GondrLib.ObjectPool.RunTime;

namespace GondrLib.Effects
{
    public class PoolingEffect : MonoBehaviour, IPoolable
    {
        [field: SerializeField] public PoolItemSO PoolItem { get; private set; }
        [field: SerializeField] public float playTime = 1f;
        public GameObject GameObject => gameObject;

        private Pool _myPool;
        
        [SerializeField] private GameObject effectObject;
        private IPlayableVFX _playableVFX;

        private void OnValidate()
        {
            if(effectObject == null) return;
            _playableVFX = effectObject.GetComponent<IPlayableVFX>();
            if (_playableVFX == null)
            {
                effectObject = null;
            }
        }

        public void SetUpPool(Pool pool)
        {
            _myPool = pool;
            _playableVFX = effectObject.GetComponent<IPlayableVFX>(); //수정
        }

        public void ResetItem()
        {
            _playableVFX.StopVFX();
        }

        public async void PlayVFX(Vector3 position, Quaternion rotation)
        {
            _playableVFX.PlayVFX(position, rotation);
            await Awaitable.WaitForSecondsAsync(playTime);
            _myPool.Push(this);
        }
    }
}