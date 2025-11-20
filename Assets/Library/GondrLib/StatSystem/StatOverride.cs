using System;
using UnityEngine;

namespace GondrLib.StatSystem
{
    [Serializable]
    public class StatOverride
    {
        [SerializeField] private StatSO stat;
        [SerializeField] private bool isUseOverride;
        [SerializeField] private float overrideValue;
        
        public string StatName => stat.statName;
        public StatOverride(StatSO stat) => this.stat = stat; //생성자

        //기본 에셋인 SO 를 클론해서 오버라이드 하거나 기본값으로 만들어주는 매서드
        public StatSO CreateStat()
        {
            StatSO newStat = stat.Clone() as StatSO; //클론 만들어야 한다.
            Debug.Assert(newStat != null, $"{nameof(newStat)} stat cloning failed");

            if (isUseOverride)
            {
                newStat.BaseValue = overrideValue;
            }

            return newStat;
        }

    }
}