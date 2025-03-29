using UnityEngine;

public class MoveCloud : MonoBehaviour
{
    public float moveSpeedMin = 1.5f;  
    public float moveSpeedMax = 3f;    
    public float deadZone = -14f;     
    private float moveSpeed;          

    void Start()
    {
        moveSpeed = Random.Range(moveSpeedMin, moveSpeedMax);
    }

    void Update()
    {
        transform.position += Vector3.left * moveSpeed * Time.deltaTime;

        if (transform.position.x < deadZone)
        {
            Destroy(gameObject);
        }
    }
}
