using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneSkip : MonoBehaviour
{
    [Header("Scene & Timeline Settings")]
    public string sceneToLoad = "Scene 2";
    public PlayableDirector timelineToStop;

    [Header("Key Settings")]
    public KeyCode skipKey = KeyCode.Escape; // You can change this in the Inspector

    void Update()
    {
        if (Input.GetKeyDown(skipKey))
        {
            SkipCutscene();
        }
    }

    void SkipCutscene()
    {
        if (timelineToStop != null)
        {
            timelineToStop.Stop();
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene(sceneToLoad);
    }
}
