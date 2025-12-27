using Blade.Combat;
using System;
using Unity.Cinemachine;
using UnityEngine;

namespace Blade.Players
{
    public class PlayerAttackCompo : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        [Header("Impulse Settings")]
        [SerializeField] private CinemachineImpulseSource impulseSource;
        [SerializeField] private bool canImpulseOnlyHit;

        [Header("attack data"), SerializeField] private AttackDataSO[] attackDataList;

        [SerializeField] private float comboWindow;
        private Entity _entity;
        private EntityAnimator _entityAnimator;
        private EntityVFX _vfxCompo;
        private EntityAnimatorTrigger _animatorTrigger;
        private EntityStatCompo _statCompo;
        private DamageCompo _damageCompo;

        private readonly int _attackSpeedHash = Animator.StringToHash("ATTACK_SPEED");
        private readonly int _comboCounterHash = Animator.StringToHash("COMBO_COUNTER");

        private float _attackSpeed = 1f;
        private float _lastAttackTime;

        public bool useMouseDirection = false;

        public int ComboCounter { get; set; } = 0;

        [SerializeField] private DamageCaster damageCaster;
        public float AttackSpeed
        {
            get => _attackSpeed;
            set
            {
                _attackSpeed = value;
                _entityAnimator.SetParam(_attackSpeedHash, _attackSpeed);
            }
        }

        public void Initialize(Entity entity)
        {
            _entity = entity;
            _entityAnimator = entity.GetCompo<EntityAnimator>();
            _vfxCompo = entity.GetCompo<EntityVFX>();
            _animatorTrigger = entity.GetCompo<EntityAnimatorTrigger>();
            _statCompo = entity.GetCompo<EntityStatCompo>();
            _damageCompo = entity.GetCompo<DamageCompo>();
        }

        public void AfterInitialize()
        {
            _animatorTrigger.OnAttackVFXTrigger += HandleAttackVFXTrigger;
            _animatorTrigger.OnDamageCastTrigger += HandleDamageCasterTrigger;

            AttackSpeed = 0.5f;
        }

        private void OnDestroy()
        {
            _animatorTrigger.OnAttackVFXTrigger -= HandleAttackVFXTrigger;
            _animatorTrigger.OnDamageCastTrigger -= HandleDamageCasterTrigger;
        }

        private void HandleAttackSpeedChange(StatSO stat, float currentvalue, float previousvalue)
        {
            AttackSpeed = currentvalue;
        }

        private void HandleDamageCasterTrigger()
        {
            AttackDataSO attackData = GetCurrentAttackData();
            //DamageData damageData = _damageCompo.CalculateDamage(physicalDamageStat, attackData);

            Vector3 position = damageCaster.transform.position;
            //bool isSuccess = damageCaster.CastDamage(damageData, position, _entity.transform.forward, attackData);

            //if (canImpulseOnlyHit == false || isSuccess)
            //{
            //    impulseSource.GenerateImpulse(attackData.impulseForce);
            //}
        }

        private void HandleAttackVFXTrigger()
        {
            _vfxCompo.PlayVfx($"Blade{ComboCounter}", Vector3.zero, Quaternion.identity);
        }

        public void Attack()
        {
            bool comboCounterOver = ComboCounter > 2;
            bool comboWindowExhaust = Time.time >= _lastAttackTime + comboWindow;
            if (comboCounterOver || comboWindowExhaust)
            {
                ComboCounter = 0;
            }
            _entityAnimator.SetParam(_comboCounterHash, ComboCounter);
        }

        public void EndAttack()
        {
            ComboCounter++;
            if (ComboCounter > 2) ComboCounter = 0;
            _lastAttackTime = Time.time;
        }

        public AttackDataSO GetCurrentAttackData()
        {
            Debug.Assert(attackDataList.Length > ComboCounter, "Combo counter is out of range");
            return attackDataList[ComboCounter];
        }


    }
}