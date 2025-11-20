using Core.EventBus;
using UnityEngine;

namespace Code.Events
{
    public struct GoldIncreaseEvent : IEvent
    {
        public int amount;

        public GoldIncreaseEvent(int amount)
        {
            this.amount = amount;
        }
    }
}