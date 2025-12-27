using Code.Item;
using Core.EventBus;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.RunTime;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnManager : MonoSingleton<SpawnManager>
{
    [Inject]
    private PoolManagerMono _poolM;

    [SerializeField] private float _minSpawnForce;
    [SerializeField] private float _maxSpawnForce;
    [SerializeField] private float _minDir;
    [SerializeField] private float _maxDir;
    protected override void Awake()
    {
        base.Awake();
        Bus<SpawnGoldEvt>.OnEvent += HandleGoldEvent;
    }

    private void OnDestroy()
    {
        Bus<SpawnGoldEvt>.OnEvent -= HandleGoldEvent;
    }

    private void HandleGoldEvent(SpawnGoldEvt evt)
    {
        for (int i = 0; i < evt.amount; i++)
        {
            Item spawnItem = _poolM.Pop<Item>(evt.goldSO);
            var (direction, power) = CalculateDirection();
            spawnItem.DropCoin(direction, power);
        }
    }

    private (Vector3, float) CalculateDirection()
    {
        float randomYaw = Random.Range(0f, 360f);
        Quaternion yawRotation = Quaternion.Euler(0, randomYaw, 0);

        float randomPitch = Random.Range(_minDir, _maxDir);
        Quaternion pitchRotation = Quaternion.Euler(-randomPitch, 0, 0);

        Vector3 finalDirection = (yawRotation * pitchRotation) * Vector3.forward;

        float finalPower = Random.Range(_minSpawnForce, _maxSpawnForce);

        return (finalDirection, finalPower);
    }
}
