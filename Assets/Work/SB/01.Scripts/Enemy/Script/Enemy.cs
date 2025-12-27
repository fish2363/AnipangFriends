using Public.Core.Entity;
using Public.SO;
using Unity.Behavior;
using UnityEngine;
using Work.SB._01.Scripts.Enemy.Interface;

namespace Work.SB._01.Scripts.Enemy.Script
{
    public abstract class Enemy : Entity,IKnockBackable
    {

        public BehaviorGraphAgent BtAgent => _btAgent;
        protected BehaviorGraphAgent _btAgent;
        
       // private StateChange _stateChangeChannel;

        protected override void Awake()
        {
            base.Awake();
            OnDeathEvent.AddListener(HandleDeathEvent);
        }

        protected override void AddComponents()
        {
            base.AddComponents();
        }

      
        public void KnockBack(Vector3 direction, MovementDataSO knockBackMovement)
        {
            
        }
        
        private void HandleDeathEvent()
        {
            if (IsDead) return;
            IsDead = true;
            // _stateChangeChannel.SendEventMessage(EnemyState.DEAD); //코드에서 BT로 상태변경 이벤트 전송
        }
    }
}