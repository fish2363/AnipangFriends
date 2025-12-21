using Core.EventBus;
using Public.Core.Events;
using Public.SO;
using Work.SB._01.Scripts.Enemy.Interface;
using UnityEngine;


namespace Work.SB._01.Scripts.Enemy.Script
{
    public class TamableEnemy : Enemy, ICanTame
    {
        [SerializeField] private CharacterSO _myCharacterSO;

        public CharacterSO GetCharacterSO() => _myCharacterSO;

    }
}