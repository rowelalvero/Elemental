using UnityEngine;

public class CursorLock : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Hide the cursor
        Cursor.visible = false;

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        //in case you want to reuse, its the return cursor code
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;
}
