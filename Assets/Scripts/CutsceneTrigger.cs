using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class CutsceneTrigger : MonoBehaviour
{
    public PlayableDirector timeline;
    public float cutsceneDuration = 75f; // Duration before the timeline stops, change again whenever edits are made, with the public code
    public System.Action OnCutsceneEnd; // Callback for when the timeline stops

    public void PlayTimeline()
    {
        if (timeline != null)
        {
            timeline.Play();
            StartCoroutine(StopTimelineAfterSeconds(cutsceneDuration));
        }
    }

    private IEnumerator StopTimelineAfterSeconds(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (timeline != null && timeline.state == PlayState.Playing)
        {
            timeline.Stop();
        }

        // Call the event when the cutscene ends
        OnCutsceneEnd?.Invoke();
    }

    public bool IsTimelinePlaying()
    {
        return timeline != null && timeline.state == PlayState.Playing;
    }
}
