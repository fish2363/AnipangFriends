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
    public struct GoldDecreaseEvent : IEvent
    {
        public int amount;

        public GoldDecreaseEvent(int amount)
        {
            this.amount = amount;
        }
    }
}