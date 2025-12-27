using System;
using UnityEngine;

public class EntityAnimatorTrigger : MonoBehaviour, IEntityComponent
{
    public Action OnAnimationEndTrigger;

    //public event Action<bool> OnRollingStatusChange;
    public event Action OnAttackVFXTrigger;
    public event Action<bool> OnManualRotationTrigger;
    public event Action OnDamageCastTrigger;
    public event Action<bool> OnDamageToggleTrigger;
    public event Action OnCastSkillTrigger;

    private Entity _entity;

    public void Initialize(Entity entity)
    {
        _entity = entity;
    }

    private void AnimationEnd()
    {
        OnAnimationEndTrigger?.Invoke();
    }

    // private void RollingStart() => OnRollingStatusChange?.Invoke(true);
    // private void RollingEnd() => OnRollingStatusChange?.Invoke(false);
    private void PlayAttackVFX() => OnAttackVFXTrigger?.Invoke();
    private void StartManualRotation() => OnManualRotationTrigger?.Invoke(true);
    private void StopManualRotation() => OnManualRotationTrigger?.Invoke(false);
    private void DamageCast() => OnDamageCastTrigger?.Invoke();
    private void StartDamageCast() => OnDamageToggleTrigger?.Invoke(true);
    private void StopDamageCast() => OnDamageToggleTrigger?.Invoke(false);
    private void CastSkill() => OnCastSkillTrigger?.Invoke();
}