using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleStart : MonoBehaviour
{
    public string sceneToLoad = "Scene 2";
    public CutsceneTrigger CutsceneTrigger;

    void Start()
    {
        if (CutsceneTrigger != null)
        {
            CutsceneTrigger.OnCutsceneEnd += LoadNextScene;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (CutsceneTrigger != null)
            {
                CutsceneTrigger.PlayTimeline();
                StartCoroutine(WaitForCutscene());
            }
        }
    }

    System.Collections.IEnumerator WaitForCutscene()
    {
        while (CutsceneTrigger.IsTimelinePlaying())
        {
            yield return null;
        }

        // Load the next scene when the cutscene ends
        LoadNextScene();
    }

    void LoadNextScene()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnDestroy()
    {
        // Remove event listener to avoid memory leaks
        if (CutsceneTrigger != null)
        {
            CutsceneTrigger.OnCutsceneEnd -= LoadNextScene;
        }
    }
}
