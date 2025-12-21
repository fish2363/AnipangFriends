using GondrLib.ObjectPool.RunTime;
using Unity.Cinemachine;
using UnityEngine;

public class PoolingImpulse : MonoBehaviour, IPoolable
{
    [field: SerializeField]
    public PoolItemSO PoolItem { get; set; }
    public GameObject GameObject => gameObject;

    private Pool _pool;
    private CinemachineImpulseSource _source;

    public void ResetItem()
    {

    }

    public void SetUpPool(Pool pool)
    {
        _pool = pool;
    }

    private void Awake()
    {
        _source = GetComponent<CinemachineImpulseSource>();
    }

    public void SetImpulse(CameraShakeInfoSO shakeInfo)
    {
        _source.DefaultVelocity = shakeInfo.velocity;
        _source.ImpulseDefinition.ImpulseType = shakeInfo.impulseTypes;
        _source.ImpulseDefinition.DissipationRate = shakeInfo.dissipationRate;
        _source.ImpulseDefinition.PropagationSpeed = shakeInfo.progationSpeed;
        _source.ImpulseDefinition.DissipationDistance = shakeInfo.dissipationDistance;
        _source.ImpulseDefinition.ImpulseShape = shakeInfo.impulseShapes;
    }

    public void Play()
    {
        _source.GenerateImpulse();
    }
}
