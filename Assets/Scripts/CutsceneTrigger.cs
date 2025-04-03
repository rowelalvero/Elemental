using UnityEngine;
using UnityEngine.Playables;
//using Cinemachine;

//package install timeline and cinemachine
//drag animation

public class CutsceneTrigger : MonoBehaviour
{
    //public PlayableDirector timeline; // Assign the Timeline asset here
    //public GameObject player;
    //public GameObject cutsceneCamera;
    //public GameObject gameplayCamera;

    void Start()
    {
//       gameplayCamera.SetActive(true);
//        cutsceneCamera.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
//        if (other.gameObject == player)
        {
            StartCutscene();
        }
    }

    void StartCutscene()
    {
////        gameplayCamera.SetActive(false);
//        cutsceneCamera.SetActive(true);
//        timeline.Play();
        //StartCoroutine(EndCutscene());
    }

    //System.Collections.IEnumerator EndCutscene()
    {
        //yield return new WaitForSeconds((float)timeline.duration);
        //cutsceneCamera.SetActive(false);
        //gameplayCamera.SetActive(true);
    }
}
