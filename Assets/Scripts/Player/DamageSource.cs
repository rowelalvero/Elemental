using UnityEngine;

public class DamageSource : MonoBehaviour
{
    private int baseDamage;
    private WeaponInfo weaponInfo;

    private void Start()
    {
        MonoBehaviour currentWeapon = ActiveWeapon.Instance.CurrentActiveWeapon;
        weaponInfo = (currentWeapon as IWeapon).GetWeaponInfo();
        baseDamage = weaponInfo.weaponDamage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
        ElementType enemyElement = other.GetComponent<ElementType>();

        // If the enemy doesn't have the GenElementType component, treat it as 'None'
        if (enemyElement == null)
        {
            enemyElement = new ElementType();  // Create a neutral (None) element type
            enemyElement.SetNeutral(); // Set to neutral (None) if no element is found
        }

        if (enemyHealth != null)
        {
            int finalDamage = CalculateElementalDamage(baseDamage, weaponInfo, enemyElement);
            enemyHealth.TakeDamage(finalDamage);
        }
    }

    private int CalculateElementalDamage(int damage, WeaponInfo weapon, ElementType enemy)
    {
        // Advantage relationships
        if ((weapon.isFire && enemy.isEarth) ||
            (weapon.isWater && enemy.isFire) ||
            (weapon.isWind && enemy.isWater) ||
            (weapon.isEarth && enemy.isWind))
        {
            return Mathf.RoundToInt(damage * 1.5f);
        }

        // Resistance (reverse) relationships
        if ((weapon.isEarth && enemy.isFire) ||
            (weapon.isFire && enemy.isWater) ||
            (weapon.isWater && enemy.isWind) ||
            (weapon.isWind && enemy.isEarth))
        {
            return Mathf.RoundToInt(damage * 0.5f);
        }

        // Neutral or matching types
        return damage;
    }
}
