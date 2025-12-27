using Blade.Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillComponent : MonoBehaviour, IEntityComponent,IChangableInfo
{
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private int maxCheckTargetCount = 5;

    public Collider[] Colliders { get; private set; }

    private Entity _entity;
    private Dictionary<Type, Skill> _skillDict;
    [field: SerializeField] public Skill CurrentSkill { get; set; }
    public DamageCompo _damageCompo;

    public void Initialize(Entity entity)
    {
        _entity = entity;
        Colliders = new Collider[maxCheckTargetCount];
        _skillDict = new Dictionary<Type, Skill>();

        GetComponentsInChildren<Skill>().ToList()
            .ForEach(skill => _skillDict.Add(skill.GetType(), skill));

        _skillDict.Values.ToList()
            .ForEach(skill => skill.InitializeSkill(_entity, this));
        _damageCompo = entity.GetCompo<DamageCompo>();
    }

    public Skill GetCurrentSkill()
    {
        Skill skill = CurrentSkill;
        return skill;
    }

    public T GetSkill<T>() where T : Skill
    {
        Type skillType = typeof(T);
        Skill skill = _skillDict.GetValueOrDefault(skillType);

        Debug.Assert(skill != default,
            $"Finding skill type is not found {skillType.Name} in {gameObject.name}");

        return skill as T;
    }


    public void ChangeSkill(string skillTypeName)
    {
        // 1. 현재 어셈블리에서 타입 찾기
        Type skillType = Type.GetType(skillTypeName);

        if (skillType == null)
        {
            Debug.LogError($"[SkillComponent] Type not found: {skillTypeName}");
            return;
        }

        if (!typeof(Skill).IsAssignableFrom(skillType))
        {
            Debug.LogError($"[SkillComponent] Type {skillTypeName} is not a Skill");
            return;
        }

        // 2. 사전에 등록된 스킬 중 해당 타입 찾기
        if (_skillDict.TryGetValue(skillType, out Skill skill))
        {
            CurrentSkill = skill;
        }
        else
        {
            Debug.LogError($"[SkillComponent] Skill of type {skillTypeName} not found on {gameObject.name}");
        }
    }

    public Skill GetSkill(Type skillType)
    {
        return _skillDict.GetValueOrDefault(skillType);
    }

    public void AddSkill(Skill skill)
    {
        _skillDict.Add(skill.GetType(), skill);
    }

    public void RemoveSkill(Skill skill)
    {
        _skillDict.Remove(skill.GetType());
    }

    public int GetEnemiesInRange(Vector3 position, float range)
        => Physics.OverlapSphereNonAlloc(position, range, Colliders, whatIsTarget);

    public Entity FindClosestEnemy(Vector3 position, float range)
    {
        Entity findEnemy = null;
        int cnt = GetEnemiesInRange(position, range);

        float closestDistance = float.MaxValue;

        for (int i = 0; i < cnt; i++)
        {
            if (Colliders[i].TryGetComponent(out Entity enemy) == false
                || enemy.IsDead) continue;

            float distanceToEnemy = Vector3.Distance(position, enemy.transform.position);
            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                findEnemy = enemy;
            }
        }

        return findEnemy;
    }

    public void Change(CharacterSO info)
    {
        ChangeSkill(info.SkillName);
    }
}