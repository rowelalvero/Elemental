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

        [Tooltip("Script to trigger at this step.")]
        public MonoBehaviour scriptToTrigger; // Any script that has a function

        [Tooltip("Function to call from the assigned script.")]
        public string functionName; // Function name to invoke

        [Tooltip("Optional parameter for the function (e.g., mobSpawner(1)).")]
        public int functionParameter; // Function parameter, if needed
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            step = 0;
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
                if (dialogues[step].scriptToTrigger != null && !string.IsNullOrEmpty(dialogues[step].functionName))
                {
                    TriggerScriptByFunction(dialogues[step].scriptToTrigger, dialogues[step].functionName, dialogues[step].functionParameter);
                }
            }
            step += 1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        dialogueActivated = false;
        dialogueCanvas.SetActive(false);
        step = 0;
    }
    void TriggerScriptByFunction(MonoBehaviour scriptToTrigger, string functionName, int parameter)
    {
        if (scriptToTrigger != null)
        {
            // Use reflection to find the method with the name "functionName"
            var method = scriptToTrigger.GetType().GetMethod(functionName);
            if (method != null)
            {
                // Invoke the method, passing the parameter to it
                method.Invoke(scriptToTrigger, new object[] { parameter });
                return; // Exit after invoking the correct function
            }
            else
            {
                Debug.LogError($"Method '{functionName}' not found in script '{scriptToTrigger.GetType()}'");
            }
        }
        else
        {
            Debug.LogWarning("Script to trigger is null.");
        }
    }

}
