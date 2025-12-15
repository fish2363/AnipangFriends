using Core.EventBus;
using System;
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
        public Action<bool> ResultCallback;

        public GoldDecreaseEvent(int amount, Action<bool> resultCallback)
        {
            this.amount = amount;
            this.ResultCallback = resultCallback;
        }
    }
}