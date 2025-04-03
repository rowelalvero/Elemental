using UnityEngine;
using System.Reflection;

public class SummonMarkOn : MonoBehaviour
{
    [Tooltip("Script to trigger at this step.")]
    public MonoBehaviour scriptToTrigger; // Any script that has a function

    [Tooltip("Function to call from the assigned script.")]
    public string functionName; // Function name to invoke

    [Tooltip("Optional parameter for the function (e.g., mobSpawner(1)).")]
    public int functionParameter = -1; // Nullable function parameter, if needed.

    // Method to dynamically trigger the function on the assigned script
    public void TriggerFunction()
    {
        if (scriptToTrigger == null || string.IsNullOrEmpty(functionName))
        {
            Debug.LogWarning("Script or function name is missing.");
            return;
        }

        // Use reflection to find the method by name
        MethodInfo method = scriptToTrigger.GetType().GetMethod(functionName);

        // Check if the method exists
        if (method != null)
        {
            // If a parameter is provided, pass it; otherwise, pass null
            if (functionParameter != -1)
            {
                method.Invoke(scriptToTrigger, new object[] { functionParameter != -1 });
            }
            else
            {
                method.Invoke(scriptToTrigger, null); // No parameter passed
            }
        }
        else
        {
            Debug.LogWarning($"Method {functionName} not found on {scriptToTrigger.GetType().Name}.");
        }
    }
}
