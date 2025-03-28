using UnityEngine;

public class MoveCloud : MonoBehaviour
{
    public float moveSpeed = 3f;  // Speed of movement
    public float deadZone = -18f; // Position where the cloud gets destroyed

    void Update()
    {
        // Move the cloud to the left
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        // Check if the cloud goes past the deadZone and destroy it
        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
}
