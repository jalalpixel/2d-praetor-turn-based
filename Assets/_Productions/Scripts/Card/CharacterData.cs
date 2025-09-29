using UnityEngine;

[CreateAssetMenu(fileName = "New Character Data", menuName = "Game/Character Data")]
public class CharacterData : ScriptableObject
{
    public string CharacterName;
    public ClassType classType;
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

    public Animator characterAnimator;
    public int maxHealth = 100;
    public int armor = 10;           // Flat damage reduction
    [Range(0.001f, 1f)]
    public float agility = 0.2f;     // Dodge chance (20%)
    public int damage = 20;
    public int attackHitAmount = 1;
}
