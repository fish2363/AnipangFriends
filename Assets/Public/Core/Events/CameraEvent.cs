using Core.EventBus;
using UnityEngine;

namespace Public.Core.Events
{
    public struct CameraEvent : IEvent
    {
        public CameraShakeInfoSO shakeInfo;

        CameraEvent(CameraShakeInfoSO shakeInfo)
        {
            this.shakeInfo = shakeInfo;
        }
    }
}