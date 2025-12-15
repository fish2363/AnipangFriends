using Core.EventBus;
using Public.Core.Events;
using Unity.Cinemachine;
using UnityEngine;

public class CamTest : MonoBehaviour
{
    [SerializeField]
    private CameraShakeInfoSO shakeInfo;
    [SerializeField] private CinemachineCamera cam;
    [ContextMenu("TestShake")]
    private void Shake()
    {
        Bus<ImpulseEvent>.Raise(new ImpulseEvent(shakeInfo));
    }

    [ContextMenu("TestChange")]
    private void Change()
    {
        Bus<CameraChangeEvent>.Raise(new CameraChangeEvent(cam, Vector2.left));
    }
    [ContextMenu("Pan")]
    private void Pan()
    {
        Bus<PanEvent>.Raise(new PanEvent(false, PanDirection.Left, 8f, 1.5f));
    }
}
