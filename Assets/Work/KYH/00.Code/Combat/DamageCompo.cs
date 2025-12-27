using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Blade.Combat
{
    public class DamageCompo : MonoBehaviour, IEntityComponent, IAfterInitialize
    {
        [SerializeField] private StatSO criticalStat, criticalDamageStat;


        private float _critical, _criticalDamage;
        public void Initialize(Entity entity)
        {
        }

        public void AfterInitialize()
        {
            if (criticalStat == null)
                _critical = 0;
            else
                _critical = 1f;


            if (criticalDamageStat == null)
                _criticalDamage = 1f;
            else
                _criticalDamage = 1f;

        }

        private void OnDestroy()
        {
            
        }

        private void HandleCriticalDamageChange(StatSO stat, float currentvalue, float previousvalue)
            => _criticalDamage = currentvalue;


        private void HandleCriticalChange(StatSO stat, float currentvalue, float previousvalue)
            => _critical = currentvalue;

        public DamageData CalculateDamage(StatSO majorStat, AttackDataSO attackData, float multiplier = 1f)
        {
            DamageData data = new DamageData();

            data.damage = majorStat.Value * attackData.damageMultiplier
                          + attackData.damageIncrease * multiplier;
            //증뎀 + 추뎀식
            if (Random.value < _critical)
            {
                data.damage *= _criticalDamage; //크리티컬 증뎀률 곱
                data.isCritical = true;
            }
            else
            {
                data.isCritical = false;
            }

            data.damageType = attackData.damageType;

            return data;
        }
    }
}