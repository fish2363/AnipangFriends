using Public.Core.Entity;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "SO/CharacterSO")]
public class CharacterSO : ScriptableObject
{
    public string name;
    public string description;

    public string SkillName;

    public RuntimeAnimatorController unitAnimator; 
    public UnitType unitType; 
    public EntityStatCompo unitStatCompo;
    public EntityAttackCompo attackCompo;
    public GameObject visual;
}
