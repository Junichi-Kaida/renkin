using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "AlchemyGame/Weapon")]
public class WeaponSO : ScriptableObject
{
    public string weaponName;
    public int damage;
    public float attackInterval;
    public float attackRange;
    public Sprite weaponSprite;
}
