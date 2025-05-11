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
        ElementWeakness enemyWeakness = other.GetComponent<ElementWeakness>();

        if (enemyElement == null)
        {
            enemyElement = new ElementType();
            enemyElement.SetNeutral();
        }

        if (enemyWeakness != null)
        {
            string weaponElement = GetWeaponElement();
            if (!enemyWeakness.CanBeDamagedBy(weaponElement))
            {
                Debug.Log($"{other.name} is immune to {weaponElement} attacks.");
                return;
            }
        }

        if (enemyHealth != null)
        {
            int finalDamage = CalculateElementalDamage(baseDamage, weaponInfo, enemyElement);
            enemyHealth.TakeDamage(finalDamage);
        }
    }

    // Normal Element Check
    private string GetWeaponElement()
    {
        if (weaponInfo.isFire) return "Fire";
        if (weaponInfo.isWater) return "Water";
        if (weaponInfo.isEarth) return "Earth";
        if (weaponInfo.isWind) return "Wind";
        return "None";
    }

    private int CalculateElementalDamage(int damage, WeaponInfo weapon, ElementType enemy)
    {
        // Increase Damage
        if ((weapon.isFire && enemy.isEarth) ||
            (weapon.isWater && enemy.isFire) ||
            (weapon.isWind && enemy.isWater) ||
            (weapon.isEarth && enemy.isWind))
        {
            return Mathf.RoundToInt(damage * 1.5f);
        }

        // Resist Damage
        if ((weapon.isEarth && enemy.isFire) ||
            (weapon.isFire && enemy.isWater) ||
            (weapon.isWater && enemy.isWind) ||
            (weapon.isWind && enemy.isEarth))
        {
            return Mathf.RoundToInt(damage * 0.5f);
        }

        // Neutral Damage
        return damage;
    }
}
