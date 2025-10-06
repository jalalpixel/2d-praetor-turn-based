using UnityEngine;

[CreateAssetMenu(fileName = "New Character Data", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public ClassType classType;
    public bool isMeleeTroops = true;
    public float attackRange = 0.015f;
    public float attackSpeed = 1f;
    
    public enum ClassType
    {
        agility,
        critter,
        explosive,
        tank,
        healer,
        miners,
        ranger
    }

    public RuntimeAnimatorController characterAnimator;
    public int maxHealth = 100;
    public int armor = 10;           // Flat damage reduction
    [Range(0.001f, 1f)]
    public float agility = 0.2f;     // Dodge chance (20%)
    public int damage = 20;
    public int attackHitAmount = 1;
}
