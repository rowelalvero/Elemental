using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TimelineTriggerManager : MonoBehaviour
{
    [Header("Timeline Reference")]
    public PlayableDirector bossDefeatTimeline;

    [Header("Scene Transition")]
    public string sceneToLoad = "Scene 8";
    public bool loadSceneOnEnd = true;

    [Header("Teleport Settings")]
    [Tooltip("Where the player should be moved when the cutscene starts.")]
    public Transform teleportTarget;

    [Tooltip("Where the player should move after the cutscene ends.")]
    public Transform teleportAfterCutscene;

    private bool timelineFinishedHandled = false;

    private void Awake()
    {
        if (bossDefeatTimeline != null)
        {
            bossDefeatTimeline.stopped += HandleTimelineFinished;
        }
        else
        {
            Debug.LogWarning("[TimelineTriggerManager] No timeline assigned.");
        }
    }

    private void OnDestroy()
    {
        if (bossDefeatTimeline != null)
        {
            bossDefeatTimeline.stopped -= HandleTimelineFinished;
        }
    }

    public void PlayBossTimeline()
    {
        if (bossDefeatTimeline != null)
        {
            Debug.Log("[TimelineTriggerManager] Playing boss defeat timeline.");
            bossDefeatTimeline.Play();

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && teleportTarget != null)
            {
                Debug.Log("[TimelineTriggerManager] Teleporting player at timeline start.");
                player.transform.position = teleportTarget.position;
            }
            else
            {
                Debug.LogWarning("[TimelineTriggerManager] Player or teleport target not found.");
            }

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            timelineFinishedHandled = false;
        }
    }

    private void HandleTimelineFinished(PlayableDirector director)
    {
        if (timelineFinishedHandled) return;
        timelineFinishedHandled = true;

        Debug.Log("[TimelineTriggerManager] Timeline finished.");

        // Restore the cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && teleportAfterCutscene != null)
        {
            Debug.Log("[TimelineTriggerManager] Teleporting player to post-cutscene position.");
            player.transform.position = teleportAfterCutscene.position;
        }

        if (loadSceneOnEnd && !string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("[TimelineTriggerManager] Loading scene: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    public bool IsTimelinePlaying()
    {
        return bossDefeatTimeline != null && bossDefeatTimeline.state == PlayState.Playing;
    }
}
