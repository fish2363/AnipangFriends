using Public.SO;
using UnityEngine;

namespace Work.SB._01.Scripts.Enemy.Interface
{
    public interface IKnockBackable
    {
        public void KnockBack(Vector3 direction, MovementDataSO knockBackMovement);
    }
    
}