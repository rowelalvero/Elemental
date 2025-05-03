using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossDeathCutsceneTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private TimelineTriggerManager timelineManager;
    [SerializeField] private string sceneToLoad = "Scene 2";

    private bool hasStartedCutscene = false;

    private void Start()
    {
        if (enemyHealth == null)
            enemyHealth = GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            Debug.LogError("[BossDeathCutsceneTrigger] No EnemyHealth found.");
            enabled = false;
        }
    }

    private void Update()
    {
        if (!hasStartedCutscene && enemyHealth != null && IsDead())
        {
            hasStartedCutscene = true;
            StartCoroutine(PlayTimelineThenTransition());
        }
    }

    private bool IsDead()
    {
        // Use reflection or public method/property to expose health status
        // If `currentHealth` is private, you’ll need a getter in EnemyHealth
        return enemyHealth.GetCurrentHealth() <= 0;
    }

    private IEnumerator PlayTimelineThenTransition()
    {
        if (timelineManager != null)
        {
            timelineManager.PlayBossTimeline();
            Debug.Log("[BossDeathCutsceneTrigger] Cutscene started.");
        }
        else
        {
            Debug.LogWarning("[BossDeathCutsceneTrigger] Timeline manager not assigned.");
        }

        // Wait while timeline is playing
        while (timelineManager != null && timelineManager.IsTimelinePlaying())
        {
            yield return null;
        }

        Debug.Log("[BossDeathCutsceneTrigger] Cutscene ended. Loading next scene.");
        SceneManager.LoadScene(sceneToLoad);
    }
}
