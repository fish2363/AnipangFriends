using Core.EventBus;
using GondrLib.ObjectPool.RunTime;
using UnityEngine;

public struct SpawnGoldEvt : IEvent
{
    public PoolItemSO goldSO;
    public int amount;
    public SpawnGoldEvt(PoolItemSO goldSO, int amount)
    {
        this.goldSO = goldSO;
        this.amount = amount;
    }
}
