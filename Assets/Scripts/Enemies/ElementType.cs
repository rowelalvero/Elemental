using UnityEngine;

public class ElementType : MonoBehaviour
{
    [Header("Element type of Mob")]
    public bool isFire = false;
    public bool isWater = false;
    public bool isEarth = false;
    public bool isWind = false;

    // This will return the element type based on the checked boxes
    public string GetElementType()
    {
        if (isFire) return "Fire";
        if (isWater) return "Water";
        if (isEarth) return "Earth";
        if (isWind) return "Wind";
        return "None";
    }
    public void SetNeutral()
    {
        isFire = false;
        isEarth = false;
        isWater = false;
        isWind = false;
    }

    // Example of usage to check for an element
    void Start()
    {
        Debug.Log($"{gameObject.name} is of element: {GetElementType()}");
    }
}
