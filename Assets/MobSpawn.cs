using UnityEngine;

public class MobSpawn : MonoBehaviour
{
    public GameObject MobTarget;
    public float count = 6; // Number of mobs to spawn before stopping
    public float spawnRate = 2f; // Time between spawns

    private float timer = 0;
    private int spawnedCount = 0; // Track number of spawned mobs

    void Start()
    {
        gameObject.SetActive(false); // Start disabled
    }

    private void OnEnable()
    {
        // Reset values properly whenever the spawner is re-enabled
        spawnedCount = 0;
        timer = 0;
    }

    void Update()
    {
        if (spawnedCount >= count)
        {
            DisableSpawner(); // Disable when max count reached
            return;
        }

        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            SpawnMob();
            timer = 0;
        }
    }

    void SpawnMob()
    {
        if (MobTarget != null)
        {
            Instantiate(MobTarget, new Vector3(transform.position.x, transform.position.y, 0), transform.rotation);
            spawnedCount++; // Increase count of spawned mobs
        }
    }

    public void EnableSpawner()
    {
        gameObject.SetActive(true); // Enable the spawner
    }

    void DisableSpawner()
    {
        gameObject.SetActive(false); // Disable itself after spawning all mobs
    }
}
