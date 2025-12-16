using Code.Events;
using Core.EventBus;
using UnityEngine;

namespace Test
{
    public class GoldTest : MonoBehaviour
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

        [ContextMenu("Gold Decrease Test")]
        private void GoldDecrease()
        {
            Bus<GoldDecreaseEvent>.Raise(new GoldDecreaseEvent
            {
                amount = _amount,
                ResultCallback = Decrease
            });
        }

        private void Decrease(bool result)
        {
            Debug.Log(result);
        }
    }
}