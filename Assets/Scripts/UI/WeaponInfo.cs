using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Weapon")]
public class WeaponInfo : ScriptableObject
{
    public GameObject weaponPrefab;
    public float weaponCooldown;
    public int weaponDamage;
    public float weaponRange;

    // Replacing enum with booleans
    public bool isFire;
    public bool isEarth;
    public bool isWater;
    public bool isWind;
}
