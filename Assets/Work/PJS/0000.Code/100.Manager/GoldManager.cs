using Code.Events;
using Core.EventBus;
using UnityEngine;

namespace PJS.Managers
{
    public class GoldManager : MonoBehaviour
    {
        [SerializeField]
        private int _gold;
        public int Gold
        {
            get
            {
                return _gold;
            }
        }

        private void Awake()
        {
            Bus<GoldIncreaseEvent>.OnEvent += GoldIncreaseRequest;
            Bus<GoldDecreaseEvent>.OnEvent += GoldDecreaseRequest;
        }

        private void OnDestroy()
        {
            Bus<GoldIncreaseEvent>.OnEvent -= GoldIncreaseRequest;
            Bus<GoldDecreaseEvent>.OnEvent -= GoldDecreaseRequest;
        }

        public void GoldIncreaseRequest(GoldIncreaseEvent evt)
        {
            int amount = evt.amount;
            if (amount <= 0)
            {
#if UNITY_EDITOR
                Debug.Log("왜 0 골드를 더하는거죠?");
#endif
                return;
            }
            _gold += amount;
        }
        public void GoldDecreaseRequest(GoldDecreaseEvent evt)
        {
            int amount = evt.amount;
            if (amount >= Gold)
            {
#if UNITY_EDITOR
                Debug.Log("왜 없는 골드를 빼려는거죠?");
#endif
            }
            _gold -= amount;
        }
    }
}