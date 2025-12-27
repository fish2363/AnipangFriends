using UnityEngine;

public delegate void CooldownInfo(float current, float duration);

public abstract class Skill : MonoBehaviour
{
    [SerializeField] protected float cooldownDuration = 1f;

    protected float _cooldownTimer = 0f;
    protected Entity _owner;
    protected SkillComponent _skillComponent;

    public bool IsCooldown => _cooldownTimer > 0f;
    public event CooldownInfo OnCooldownEvent;

    public virtual void InitializeSkill(Entity owner, SkillComponent skillComponent)
    {
        _owner = owner;
        _skillComponent = skillComponent;
    }

    protected virtual void Update()
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer < 0f)
            {
                _cooldownTimer = 0f;
            }
            OnCooldownEvent?.Invoke(_cooldownTimer, cooldownDuration);
        }
    }

    public virtual void UseSkill()
    {
        _cooldownTimer = cooldownDuration;
    }
}