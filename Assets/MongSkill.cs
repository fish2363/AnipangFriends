using UnityEngine;
using Blade.Combat;

public class MongSkill : Skill
{
    [Header("Bomb Settings")]
    [SerializeField] private GameObject bombPrefab;

    [Header("Roll Settings")]
    [SerializeField] private MovementDataSO rollMovementData;

    private CharacterMovement _movement;
    private Transform _transform;

    public override void InitializeSkill(Entity owner, SkillComponent skillComponent)
    {
        base.InitializeSkill(owner, skillComponent);

        _movement = _owner.GetCompo<CharacterMovement>();
        _transform = _owner.transform;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        // 1. 폭탄 설치
        if (bombPrefab != null)
        {
            Instantiate(
                bombPrefab,
                _transform.position,
                Quaternion.identity
            );
        }

        // 2. 구르기처럼 앞으로 밀기
        if (_movement != null && rollMovementData != null)
        {
            _movement.CanManualMovement = false;

            Vector3 rollDirection = _transform.forward;
            _movement.ApplyMovementData(rollDirection, rollMovementData);

            // 일정 시간 후 수동 이동 가능하게 복구
            _owner.StartCoroutine(EnableManualMovementAfter(rollMovementData.duration));
        }
    }

    private System.Collections.IEnumerator EnableManualMovementAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        _movement.CanManualMovement = true;
    }
}