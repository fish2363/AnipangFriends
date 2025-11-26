using Core.EventBus;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.RunTime;
using Public.Core.Events;
using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    //의존을 최대한 없앨것.
    //임펄스 소스를 풀링할까?
    //나쁜 방법은 아닌듯
    //이벤트엔 뭘 넣어줄까?
    //임펄스 파워/타입/쉐이프/벨로시티
    [Inject]
    private PoolManagerMono _poolM;
    [SerializeField]
    private CinemachineImpulseSource _impulseSource;
    private void Awake()
    {
        Bus<CameraEvent>.OnEvent += HandleCameraShakeEvent;
    }

    private void OnDestroy()
    {
        Bus<CameraEvent>.OnEvent -= HandleCameraShakeEvent;
    }

    private void HandleCameraShakeEvent(CameraEvent evt)
    {

    }
}
