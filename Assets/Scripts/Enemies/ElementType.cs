using UnityEngine;

public class ElementType : MonoBehaviour
{
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
        return "None"; // Default if no element is selected
    }

    // Example of usage to check for an element
    void Start()
    {
        Debug.Log($"{gameObject.name} is of element: {GetElementType()}");
    }
}
