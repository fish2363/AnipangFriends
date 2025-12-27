using UnityEngine;

public class Player : Entity
{
    [field: SerializeField] public PlayerInputSO PlayerInput { get; private set; }

    [SerializeField] private StateDataSO[] stateDataList;

    private EntityStateMachine _stateMachine;
    private SkillComponent _skillComponent;

    private ActionData _actionData;
    private CharacterMovement _movement;

    protected override void Awake()
    {
        base.Awake();
        _stateMachine = new EntityStateMachine(this, stateDataList);

        _movement = GetCompo<CharacterMovement>();

        _skillComponent = GetCompo<SkillComponent>();

        OnHitEvent.AddListener(HandleHitEvent);
        OnDeathEvent.AddListener(HandleDeadEvent);
        PlayerInput.OnSkillPressed += HandleSkillPressed;
    }

    private void HandleSkillPressed()
    {
        Skill skill = _skillComponent.GetCurrentSkill();

        if (skill.IsCooldown) return;

        skill.UseSkill(); //스킬 사용처리해주고 전환.
    }

    private void OnDestroy()
    {
        PlayerInput.OnSkillPressed -= HandleSkillPressed;

        OnHitEvent.RemoveListener(HandleHitEvent);
        OnDeathEvent.RemoveListener(HandleDeadEvent);
    }

    private void HandleHealthChangeEvent(float current, float max)
    {
        //PlayerChannel.RaiseEvent(PlayerEvents.PlayerHealthEvent.Initializer(current, max));
    }

    private void HandleDeadEvent()
    {
        if (IsDead) return;
        IsDead = true;
        //PlayerChannel?.RaiseEvent(PlayerEvents.PlayerDead);
        ChangeState("DEAD", true);
    }

    private void HandleHitEvent()
    {
        const string hit = "HIT";
        if (_actionData.HitByPowerAttack)
            ChangeState(hit, true);
    }

    private void Start()
    {
        const string idle = "IDLE";
        _stateMachine.ChangeState(idle);
    }

    private void Update()
    {
        _stateMachine.UpdateStateMachine();
    }

    public void ChangeState(string newStateName, bool force = false)
        => _stateMachine.ChangeState(newStateName, force);


    public void KnockBack(Vector3 direction, MovementDataSO knockbackMovement)
    {
        _movement.KnockBack(direction, knockbackMovement);
    }
}
