using Code.Events;
using Core.EventBus;
using UnityEngine;

namespace Test
{
    public class GoldIncreaseTest : MonoBehaviour
    {
        [SerializeField] private int _amount;
        [ContextMenu("goldIncrease Test")]
        private void GoldIncrease()
        {
            Bus<GoldIncreaseEvent>.Raise(new GoldIncreaseEvent
            {
                amount = _amount
            });
        }
    }
}