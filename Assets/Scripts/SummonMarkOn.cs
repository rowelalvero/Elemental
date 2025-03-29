using UnityEngine;

public class SummonMarkOn : MonoBehaviour
{
    public MobSpawn mobSpawner;
    public void EnableSpawn()
    {
        mobSpawner.EnableSpawner();
    }
}
