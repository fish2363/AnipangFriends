using Core.EventBus;
using Unity.Cinemachine;
using UnityEngine;

namespace Public.Core.Events
{
    public struct ImpulseEvent : IEvent
    {
        public CameraShakeInfoSO shakeInfo;

        public ImpulseEvent(CameraShakeInfoSO shakeInfo)
        {
            this.shakeInfo = shakeInfo;
        }
    }

    public struct CameraChangeEvent : IEvent
    {
        public CinemachineCamera leftCam;
        public CinemachineCamera rightCam;
        public Vector2 moveDirection;

        public CameraChangeEvent(CinemachineCamera cam, Vector2 moveDirection)
        {
            this.leftCam = cam;
            this.rightCam = cam;
            this.moveDirection = moveDirection;
        }
    }

    public struct PanEvent : IEvent
    {
        public bool isRewindToStart;
        public PanDirection direction;
        public float distance;
        public float panTime;

        public PanEvent(bool isRewindToStart, PanDirection direction, float distance, float panTime)
        {
            this.isRewindToStart = isRewindToStart;
            this.direction = direction;
            this.distance = distance;
            this.panTime = panTime;
        }
    }
}