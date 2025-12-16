using Core.EventBus;
using DG.Tweening;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.RunTime;
using Public.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace Public.Manager
{
    public class CameraManager : MonoBehaviour
    {
        //의존을 최대한 없앨것.
        //임펄스 소스를 풀링할까?
        //나쁜 방법은 아닌듯
        //이벤트엔 뭘 넣어줄까?
        //임펄스 파워/타입/쉐이프/벨로시티
        public CinemachineCamera currentCamera;
        [Inject]
        private PoolManagerMono _poolM;
        [SerializeField] private PoolItemSO _impulseItem;
        [SerializeField] private int activeCameraPriority = 15;
        [SerializeField] private int disableCameraPriority = 10;

        private Vector2 _originalTrackPosition;

        private Dictionary<PanDirection, Vector2> _panDirections;
        private CinemachinePositionComposer _positionComposer;

        private Tween _panningTween;
        private void Awake()
        {
            Bus<ImpulseEvent>.OnEvent += HandleCameraShakeEvent;
            Bus<CameraChangeEvent>.OnEvent += HandleCameraChangeEvent;
            Bus<PanEvent>.OnEvent += HandleCameraPanning;

            _panDirections = new Dictionary<PanDirection, Vector2>
            {
                { PanDirection.Up, Vector2.up },
                { PanDirection.Down, Vector2.down },
                { PanDirection.Left, Vector2.left },
                { PanDirection.Right, Vector2.right },
            };

            currentCamera = FindObjectsByType<CinemachineCamera>(FindObjectsSortMode.None)
                                .FirstOrDefault(cam => cam.Priority == activeCameraPriority);
        }
        private void OnDestroy()
        {
            Bus<ImpulseEvent>.OnEvent -= HandleCameraShakeEvent;
            Bus<CameraChangeEvent>.OnEvent -= HandleCameraChangeEvent;
        }

        private void HandleCameraShakeEvent(ImpulseEvent evt)
        {
            PoolingImpulse impulse = _poolM.Pop<PoolingImpulse>(_impulseItem);
            impulse.SetImpulse(evt.shakeInfo);
            impulse.Play();
        }


        private void HandleCameraChangeEvent(CameraChangeEvent evt)
        {
            if (currentCamera == evt.leftCam && evt.moveDirection.x > 0)
                ChangeCamera(evt.rightCam);
            else if (currentCamera == evt.rightCam && evt.moveDirection.x < 0)
                ChangeCamera(evt.leftCam);
        }
        public void ChangeCamera(CinemachineCamera newCamera)
        {
            currentCamera.Priority = disableCameraPriority; //현재 카메라 꺼주고
            Transform followTarget = currentCamera.Follow;
            currentCamera = newCamera;
            currentCamera.Priority = activeCameraPriority;
            currentCamera.Follow = followTarget;

            _positionComposer = currentCamera.GetComponent<CinemachinePositionComposer>();
            _originalTrackPosition = _positionComposer.TargetOffset;
        }
        private void HandleCameraPanning(PanEvent evt)
        {
            Vector3 endPosition = evt.isRewindToStart ?
                _originalTrackPosition : _panDirections[evt.direction] * evt.distance + _originalTrackPosition;
            //원위치로 리와인드 시켜주는 이벤트면 원위치로 돌리고, 그렇지 않다면 방향대로 이동시켜주고

            KillTweenIfActive();
            _panningTween = DOTween.To(
                () => _positionComposer.TargetOffset,
                value => _positionComposer.TargetOffset = value,
                endPosition, evt.panTime);

        }
        private void KillTweenIfActive()
        {
            if (_panningTween != null && _panningTween.IsActive())
                _panningTween.Kill();
        }
    }
}