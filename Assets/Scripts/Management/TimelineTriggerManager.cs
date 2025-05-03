using UnityEngine;
using UnityEngine.Playables;

public class TimelineTriggerManager : MonoBehaviour
{
    [Header("Timeline Reference")]
    [Tooltip("PlayableDirector that plays the boss defeat timeline.")]
    public PlayableDirector bossDefeatTimeline;

    /// <summary>
    /// Plays the assigned timeline if one is set.
    /// </summary>
    public void PlayBossTimeline()
    {
        if (bossDefeatTimeline != null)
        {
            Debug.Log("[TimelineTriggerManager] Playing boss defeat timeline.");
            bossDefeatTimeline.Play();
        }
        else
        {
            Debug.LogWarning("[TimelineTriggerManager] No timeline assigned.");
        }
    }

    /// <summary>
    /// Returns whether the timeline is currently playing.
    /// </summary>
    public bool IsTimelinePlaying()
    {
        return bossDefeatTimeline != null && bossDefeatTimeline.state == PlayState.Playing;
    }
}
