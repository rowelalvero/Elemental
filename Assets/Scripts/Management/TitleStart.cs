using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleStart : MonoBehaviour
{
    public string sceneToLoad = "Scene 2";

    void Update()
    {
        // Check if Enter or Return key is pressed
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
    //loads scene
        SceneManager.LoadScene(sceneToLoad);
    }
}
