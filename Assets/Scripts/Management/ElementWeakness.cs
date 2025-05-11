using UnityEngine;

public class ElementWeakness : MonoBehaviour
{
    [Header("This object can be damaged by these elements:")]
    public bool weakToFire = false;
    public bool weakToWater = false;
    public bool weakToEarth = false;
    public bool weakToWind = false;

    // Returns true if the attack element is allowed to damage this object
    public bool CanBeDamagedBy(string attackElement)
    {
        switch (attackElement)
        {
            case "Fire": return weakToFire;
            case "Water": return weakToWater;
            case "Earth": return weakToEarth;
            case "Wind": return weakToWind;
            default: return false;
        }
    }

    public void ClearAllWeaknesses()
    {
        weakToFire = false;
        weakToWater = false;
        weakToEarth = false;
        weakToWind = false;
    }

    // Optional: list all current weaknesses
    public string[] GetAllWeaknesses()
    {
        System.Collections.Generic.List<string> weaknesses = new System.Collections.Generic.List<string>();
        if (weakToFire) weaknesses.Add("Fire");
        if (weakToWater) weaknesses.Add("Water");
        if (weakToEarth) weaknesses.Add("Earth");
        if (weakToWind) weaknesses.Add("Wind");
        return weaknesses.ToArray();
    }

    void Start()
    {
        string[] weaknesses = GetAllWeaknesses();
        Debug.Log($"{gameObject.name} is vulnerable to: {(weaknesses.Length > 0 ? string.Join(", ", weaknesses) : "Nothing")}");
    }
}
