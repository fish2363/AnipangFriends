using System;
using System.Collections.Generic;
using Blade.Core.StatSystem;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GondrLib.StatSystem;
using Public.SO;
using UnityEngine;
using UnityEngine.AI;
using Work.SB._01.Scripts.Enemy.Interface;

namespace Blade.Enemies
{
    public class NavMovement : MonoBehaviour, IEntityComponent, IKnockBackable, IAfterInitialize
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private StatSO moveSpeedStat; 
        [SerializeField] private float stopOffset = 0.05f; //거리에 대한 오프셋
        [SerializeField] private float rotateSpeed = 10f;
        
        private Entity _entity;
        private EntityStatCompo _statCompo;
        private Transform _lookAtTrm;
        
        
        public bool IsArrived => !agent.pathPending && agent.remainingDistance < agent.stoppingDistance + stopOffset;
        public float RemainDistance => agent.pathPending ? -1 : agent.remainingDistance;
        public Vector3 Velocity => agent.velocity;

        public bool UpdateRotation
        {
            get => agent.updateRotation;
            set => agent.updateRotation = value;
        }
        private float _speedMultiplier = 1f;
        public float SpeedMultiplier
        {
            get => _speedMultiplier;
            set
            {
                _speedMultiplier = value;
                agent.speed = _statCompo.GetStat(moveSpeedStat).Value * _speedMultiplier;
            }
        }
        
        public void Initialize(Entity entity)
        {
            _entity = entity;
            _statCompo = entity.GetCompo<EntityStatCompo>();
            
        }
        
        public void AfterInitialize()
        {
            agent.speed = _statCompo.SubscribeStat(moveSpeedStat, HandleMoveSpeedChange, 2f);
        }

        private void OnDestroy()
        {
            _entity.transform.DOKill();
            _statCompo.UnSubscribeStat(moveSpeedStat, HandleMoveSpeedChange);
        }

        private void HandleMoveSpeedChange(StatSO stat, float currentvalue, float previousvalue)
        {
            agent.speed = currentvalue * _speedMultiplier;
        }
        
        public void SetLookAtTarget(Transform target)
        {
            _lookAtTrm = target;
            UpdateRotation = target == null; //타겟이 없을 때는 로테이션을 켜준다.
        }

        private void Update()
        {
            if (_lookAtTrm != null)
            {
                LookAtTarget(_lookAtTrm.position, true);
            }
            else if (agent.hasPath && agent.isStopped == false)
            {
                LookAtTarget(agent.steeringTarget);
            }
        }
        
        /// <summary>
        /// 바라봐야할 최종 로테이션을 반환합니다.
        /// </summary>
        /// <param name="target">바라볼 목표지점을 넣습니다. y축은 무시</param>
        /// <param name="isSmooth">부드럽게 돌아갈 것인지 결정합니다.</param>
        /// <returns></returns>
        public Quaternion LookAtTarget(Vector3 target, bool isSmooth = true)
        {
            Vector3 direction = target - _entity.transform.position;
            direction.y = 0;
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            if (isSmooth)
            {
                _entity.transform.rotation = Quaternion.Slerp(_entity.transform.rotation, 
                                                lookRotation, Time.deltaTime * rotateSpeed);
            }
            else
            {
                _entity.transform.rotation = lookRotation;
            }

            return lookRotation;
        }

        public void SetStop(bool isStop) => agent.isStopped = isStop;
        public void SetVelocity(Vector3 velocity) => agent.velocity = velocity; 
        public void SetSpeed(float speed) => agent.speed = speed;
        public void SetDestination(Vector3 destination) => agent.SetDestination(destination);
        
        public async void KnockBack(Vector3 direction, MovementDataSO knockbackMovement)
        {
            SetStop(true); //네비게이션을 정지시키고
            float duration = knockbackMovement.duration;
            float currentTime = 0;
            float maxSpeed = knockbackMovement.maxSpeed;
            AnimationCurve moveCurve = knockbackMovement.moveCurve;

            while (currentTime < duration)
            {
                float normalizedTime = currentTime / duration;
                float currentSpeed = maxSpeed * moveCurve.Evaluate(normalizedTime);
                Vector3 currentMovement = direction * currentSpeed;
                _entity.transform.Translate(currentMovement * Time.fixedDeltaTime, Space.World);
                currentTime += Time.fixedDeltaTime;
                await UniTask.WaitForFixedUpdate();
                //await Awaitable.FixedUpdateAsync(); //사실 근데 이건 좋은 코드는 아니다.
            }
            
            WarpToPosition(transform.position);
            SetStop(false);
        }

       

        public void WarpToPosition(Vector3 position) => agent.Warp(position);
    }
}