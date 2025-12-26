using GondrLib.ObjectPool.RunTime;
using Unity.Mathematics;
using UnityEngine;

namespace Code.Item
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Item : MonoBehaviour, IPoolable
    {
        #region Pool
        [field: SerializeField]
        public PoolItemSO PoolItem { get; private set; }

        public GameObject GameObject => gameObject;
        public void SetUpPool(Pool pool)
        {
            _pool = pool;
        }
        private Pool _pool;
        #endregion

        private Rigidbody _rigid;
        private Collider _collider;
        public void ResetItem()
        {
            _collider.enabled = true;
        }

        private void Awake()
        {
            _rigid = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void DropCoin(Vector3 direction, float power)
        {
            _rigid.angularVelocity = Vector3.zero;
            _rigid.linearVelocity = Vector3.zero;
            _rigid.AddForce(direction.normalized * power, ForceMode.Impulse);
        }

        private void OnCollisionEnter(Collision collision)
        {
            //태스크 실행하기
        }

        private void PushPool()
        {
            //Task 죽이기
            _rigid.linearVelocity = Vector3.zero;
            _rigid.angularVelocity = Vector3.zero;
            _pool.Push(this);
        }
    }
}