using UnityEngine;

public class TutorialScreen : MonoBehaviour
{
    [Header("Canvas and Image to Activate")]
    public GameObject canvasToActivate;  // Canvas that holds the Image

    private void Start()
    {
        // Initially deactivate the canvas
        if (canvasToActivate != null)
        {
            canvasToActivate.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the other collider is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            if (canvasToActivate != null)
            {
                canvasToActivate.SetActive(true);  // Activate the Canvas with the Image
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Deactivate the canvas when player exits the trigger zone
        if (other.CompareTag("Player"))
        {
            if (canvasToActivate != null)
            {
                canvasToActivate.SetActive(false);  // Deactivate the Canvas
            }
        }
    }
}
