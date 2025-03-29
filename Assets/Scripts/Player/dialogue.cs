using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class dialogue : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField]
    private GameObject dialogueCanvas;

    [SerializeField]
    private TMP_Text speakerText;

    [SerializeField]
    private TMP_Text dialogueText;

    [SerializeField]
    private Image portraitImage;

    //[Header("Choice Settings")]
    //[SerializeField] private GameObject choicePanel; // Panel containing buttons
    //[SerializeField] private Transform choiceContainer; // The Vertical Layout Group
    //[SerializeField] private Button choiceButtonPrefab; // Prefab for choice buttons

    //This will be the dialogue choices, the plan will be to use vertical group for button prefab spawning for muti choice scenario

    [Header("Content")]
    [SerializeField]
    private List<DialogueEntry> dialogues = new List<DialogueEntry>();

    private bool dialogueActivated;
    private int step = 0;

    [System.Serializable]
    public class DialogueEntry
    {
        public string speaker;
        public Sprite portrait;
        [TextArea(2, 8)]
        public string dialogueWords;

        [Tooltip("Optional MobSpawner script to trigger at this step.")]
        public SummonMarkOn mobSpawner;  // Can be left empty

        public bool triggerSpawn;  // If true, will call EnableSpawn()
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            dialogueActivated = true;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Interact") && dialogueActivated)
        {
            if (step >= dialogues.Count)
            {
                dialogueCanvas.SetActive(false);
                step = 0;
            }
            else
            {
                dialogueCanvas.SetActive(true);
                speakerText.text = dialogues[step].speaker;
                dialogueText.text = dialogues[step].dialogueWords;
                portraitImage.sprite = dialogues[step].portrait;

                // Check if mobSpawner is assigned and triggerSpawn is enabled
                if (dialogues[step].mobSpawner != null && dialogues[step].triggerSpawn)
                {
                    dialogues[step].mobSpawner.EnableSpawn();
                }

                step += 1;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        dialogueActivated = false;
        dialogueCanvas.SetActive(false);
    }
}
